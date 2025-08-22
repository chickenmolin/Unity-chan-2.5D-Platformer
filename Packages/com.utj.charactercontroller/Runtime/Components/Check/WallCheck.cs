using Unity.TinyCharacterController.Interfaces.Components;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Unity.TinyCharacterController.Attributes;
using Unity.TinyCharacterController.Interfaces.Core;
using Unity.TinyCharacterController.Utility;

namespace Unity.TinyCharacterController.Check
{
    /// <summary>
    /// A component that performs collision detection with walls is implemented.
    /// It detects walls in the direction of the character's movement.
    /// When a collision with a wall occurs, callbacks are triggered during the collision,
    /// while in contact with the wall, and when the character moves away from the wall.
    /// kiểm tra va chạm với tường
    /// kiểm tra va chạm với tường trong hướng di chuyển của nhân vật
    /// khi va chạm với tường, sẽ gọi các sự kiện trong khi va chạm, trong khi tiếp xúc với tường, và khi nhân vật di chuyển ra khỏi tường.
    /// </summary>
    [AddComponentMenu(MenuList.MenuCheck + nameof(WallCheck))]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterSettings))]
    [Unity.VisualScripting.RenamedFrom("TinyCharacterController.WallCheck")]
    [Unity.VisualScripting.RenamedFrom("TinyCharacterController.Check.WallCheck")]
    public class WallCheck : MonoBehaviour, 
        IEarlyUpdateComponent, IWallCheck
    {
        [Header("cài đặt")]
        [FormerlySerializedAs("_angleMinMax")] 
        [SerializeField, MinMax(15, 165)] 
        private Vector2 _wallAngleRange = new Vector2(75, 115);

        [FormerlySerializedAs("_maxDistance")]
        [Range(0.01f, 1f)]
        [SerializeField] private float _wallDetectionDistance　= 0.1f;
        
        // sự kiện
        
        /// <summary>
        /// Invoke when touch with a wall.
        /// </summary>
        [Header("sự kiện")]
        [FormerlySerializedAs("OnTouchWall")] 
        public UnityEvent OnWallContacted;
        
        /// <summary>
        /// Invoke when leave from a wall.
        /// </summary>
        [FormerlySerializedAs("OnLeaveWall")] 
        public UnityEvent OnWallLeft;
        
        /// <summary>
        /// Invoke when contact with a wall.
        /// </summary>
        [FormerlySerializedAs("OnStickWall")] 
        public UnityEvent OnWallStuck;

        // thành phần
        private int _order;
        private IBrain _brain;
        private ITransform _transform;
        private CharacterSettings _settings;
        
        // dữ liệu
        private Vector3 _normal;
        private Vector3 _hitPoint;
        private Collider _contactObject ;
        private static readonly RaycastHit[] Hits = new RaycastHit[5];
        
        /// <summary>
        /// If there is contact, it returns True.
        /// </summary>
        public bool IsContact { get; private set; }

        /// <summary>
        /// Returns normal vector of the contact surface. If there is no contact, it returns Vector3.Zero.
        /// </summary>
        public Vector3 Normal => _normal;

        public GameObject ContactObject => _contactObject.gameObject;

        public Vector3 HitPoint => _hitPoint;

        // khởi tạo
        private void Awake()
        {
            TryGetComponent(out _brain);
            TryGetComponent(out _transform);
            TryGetComponent(out _settings);
        }

        // cập nhật
        void IEarlyUpdateComponent.OnUpdate(float deltaTime)
        {
            // If the component is invalid, do not update the process.
            if (enabled == false)
                return;

            // kiểm tra va chạm với tường
            var preContact = IsContact;
            var direction = _brain.ControlVelocity.normalized;

            IsContact = HitCheck(direction, out _normal, out _hitPoint, out _contactObject);
            
            // gọi sự kiện khi va chạm với tường
            if (IsContact && !preContact )
            {
                OnWallContacted?.Invoke();
            }
            
            // gọi sự kiện khi tiếp xúc với tường
            if (IsContact)
            {
                OnWallStuck?.Invoke();
            }
            
            // gọi sự kiện khi nhân vật di chuyển ra khỏi tường
            if (!IsContact && preContact)
            {
                OnWallLeft?.Invoke();
            }
        }

        // kiểm tra va chạm với tường
        /// <summary>
        /// Immediately performs wall determination.
        /// The results of this calculation are processed independently of the component calculations. This means that the calculation results are not saved.
        /// The calculation ignores colliders in the same component.
        /// kiểm tra va chạm với tường
        /// kết quả của phép tính này được xử lý độc lập với các phép tính của thành phần. Điều này có nghĩa là kết quả của phép tính không được lưu trữ.
        /// phép tính bỏ qua các collider trong cùng một thành phần.
        /// </summary>
        /// <param name="direction">direction</param>
        /// <param name="normal">result normal</param>
        /// <param name="point">result hit point</param>
        /// <param name="contactCollider">return contact object.  if no contact return null.</param>
        /// <returns>is contact any collider</returns>
        /// <param name="direction">hướng di chuyển của nhân vật</param>
        /// <param name="normal">vectơ pháp tuyến của bề mặt tiếp xúc</param>
        /// <param name="point">điểm va chạm</param>
        /// <param name="contactCollider">đối tượng tiếp xúc. nếu không có va chạm, trả về null.</param>
        /// <returns>có va chạm với bất kỳ collider nào không</returns>
        public bool HitCheck(Vector3 direction, out Vector3 normal, out Vector3 point, out Collider contactCollider)
        {
            // tính toán khoảng cách
            var distance = _settings.Radius + _wallDetectionDistance;
            var halfHeight = _settings.Height * 0.5f;
            var centerPosition = _transform.Position + Vector3.up * halfHeight;
            // tạo ray
            var ray = new Ray(centerPosition, direction);
            var count = Physics.SphereCastNonAlloc(ray, _settings.Radius, Hits, distance, _settings.EnvironmentLayer,
                QueryTriggerInteraction.Ignore);

            // tìm đối tượng gần nhất.
            var hasClosestHit = _settings.ClosestHit(Hits, count, distance, out var hit);
            if (hasClosestHit)
            {
                // áp dụng giới hạn góc.
                var angle = Vector3.Angle(Vector3.up, hit.normal);
                if (angle > _wallAngleRange.x && angle < _wallAngleRange.y &&
                    Vector3.Distance(hit.point, centerPosition) < distance)
                {
                    normal = hit.normal;
                    point = hit.point;
                    contactCollider = hit.collider;
                    return true;
                }
            }

            // trả về kết quả
            normal = Vector3.zero;
            point = Vector3.zero;
            contactCollider = null;
            return false;
        }

        // vẽ gizmos
        private void OnDrawGizmosSelected()
        {
            if (Application.isPlaying == false)
                return;
            
            // vẽ collider của đối tượng tiếp xúc
            if( IsContact)
                GizmoDrawUtility.DrawCollider(_contactObject, Color.yellow, 0);
            
            // tính toán vị trí của nhân vật
            var distance = _settings.Radius + 0.1f;
            var halfHeight = _settings.Height * 0.5f;
            var centerPosition = _transform.Position + Vector3.up * halfHeight;
            // tính toán hướng di chuyển của nhân vật
            var direction = _brain.ControlVelocity.normalized;
            var position = centerPosition + direction * distance;
            
            // vẽ hình cầu
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(position,  _settings.Radius);
            
            // vẽ hình cầu
            Gizmos.color = IsContact ? Color.red : Color.white;
            Gizmos.DrawWireSphere(position, _settings.Radius);
        }

        // độ ưu tiên
        int IEarlyUpdateComponent.Order => Order.Check;
    }
}
