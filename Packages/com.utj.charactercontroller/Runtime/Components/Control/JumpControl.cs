using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;
using Unity.TinyCharacterController.Attributes;
using Unity.TinyCharacterController.Interfaces.Components;
using Unity.TinyCharacterController.Interfaces.Core;
using Unity.TinyCharacterController.Utility;
using Unity.VisualScripting;

namespace Unity.TinyCharacterController.Control
{
    /// <summary>
    /// This component controls the behavior of jumping.
    /// When the Jump method is executed, it controls the Gravity and moves upwards.
    /// The Priority only works during the jump operation.
    /// The TurnPriority is only effective when the movement is set to the horizontal direction.
    /// </summary>
    [AddComponentMenu(MenuList.MenuControl + nameof(JumpControl))]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(CharacterSettings))]
    [RequireInterface(typeof(IGravity))]
    [RequireInterface(typeof(IGroundContact))]
    [RequireInterface(typeof(IOverheadDetection))]
    [RenamedFrom("TinyCharacterController.JumpControl")]
    [RenamedFrom("TinyCharacterController.Control.JumpControl")]
    public class JumpControl : MonoBehaviour, 
        IUpdateComponent, 
        IMove, 
        ITurn
    {
        /// <summary>
        /// Jump height.
        /// </summary>
        [FormerlySerializedAs("_jumpHeight")]
        [Unity.VisualScripting.RenamedFrom("_jumpHeight")]
        [Header("Settings")]
        [Header("chiều cao nhảy")]
        [Tooltip("JumpHeight"), SerializeField]
        public float JumpHeight = 3;

        /// <summary>
        /// Number of jumps in the air
        /// </summary>
        [FormerlySerializedAs("_maxAerialJumpCount")] 
        [Header("số lần nhảy trong không khí")]
        [Tooltip("Areal Jump Count."), SerializeField]
        public int MaxAerialJumpCount = 0;

        /// <summary>
        /// Aerodynamic drag
        /// </summary>
        [Header("lực cản không khí")]
        public float Drag = 0;
        
        /// <summary>
        /// The speed at which the character will change direction.
        /// If this value is -1, the character will change direction immediately.
        /// </summary>
        [Header("tốc độ quay")]
        [SerializeField]
        [Range(-1, 50)]
        private int _turnSpeed;

        /// <summary>
        /// The time that <see cref="Jump"/> can be entered ahead of time.
        /// If a jump is possible within the time, it will be automatically jumped.
        /// </summary>
        [Header("Input Settings")]
        [Header("thời gian chờ")]
        [SerializeField, Range(0, 1)]
        private float _standbyTime = 0.05f;

        /// <summary>
        /// Move Priority
        /// </summary>
        [Header("Priority Settings")]
        [Header("độ ưu tiên di chuyển")]
        public int MovePriority;

        /// <summary>
        /// Turn Priority
        /// </summary>
        [Header("độ ưu tiên quay")]
        public int TurnPriority;
        
        /// <summary>
        /// Callback called when a jump is requested.
        /// Called if the jump is feasible, regardless of <see cref="IsAllowJump"/>.
        /// </summary>
        [Header("Callbacks")]
        [Header("sự kiện khi nhảy")]
        public UnityEvent OnReadyToJump;
        
        /// <summary>
        /// Callback called just before jumping.
        /// </summary>
        [Header("sự kiện khi nhảy")]
        public UnityEvent OnJump;
        
        /// <summary>
        /// Callback called just before jumping.
        /// </summary>
        [Header("số lần nhảy trong không khí")]
        public int AerialJumpCount { get; private set; } = 0;
        
        /// <summary>
        /// True if a jump starts on this frame
        /// </summary>
        [Header("trạng thái nhảy")]
        public bool IsJumpStart { get; private set; } = false;

        /// <summary>
        /// True if a jump starts on this frame
        /// </summary>
        [Header("trạng thái nhảy")]
        public bool IsReadyToJumpStart { get; private set; }

        /// <summary>
        /// True if ready to jump
        /// </summary>
        [Header("trạng thái nhảy")]
        public bool IsReadyToJump { get; private set; } = false;

        /// <summary>
        /// True if jumping
        /// </summary>
        [Header("trạng thái nhảy")]
        public bool IsJumping { get; private set; } = false;

        /// <summary>
        /// The elapsed time between when the jump is ready and when Allow becomes True.
        /// Use this when you want to change the intensity of the jump with the delay time.
        /// </summary>
        [Header("thời gian nhảy")]
        public float TimeSinceReady => IsReadyToJump ? _readyTime : 0;
        

        /// <summary>
        /// The elapsed time between when the jump is ready and when Allow becomes True.
        /// Use this when you want to change the intensity of the jump with the delay time.
        /// </summary>
        [Header("trạng thái nhảy")]
        public bool IsAllowJump { get; set; } = true;

             
        /// <summary>
        /// Direction of jump.
        /// Usually use up (0,1,0).
        /// If you are doing a wall jump or dash jump, set a vector in that direction.
        /// This setting does not take the character's direction into account.
        /// This value is normalized when used.
        /// </summary>
        [Header("hướng nhảy")]
        public Vector3 JumpDirection { get; set; } = Vector3.up;

        // components
        private IGroundContact _groundCheck;
        private IGravity _gravity;
        private IOverheadDetection _head;
        private CharacterSettings _settings;
        
        private float _requestJump = -1; // Time at which the jump was requested. Request invalid state at -1
        private bool _requestJumpIncrement = false; // Determines if the number of jumps should be increased.
        private float _leaveTime = 0; // Time away from the ground. Used to determine that the ground is still ground even if it leaves the _standbyTime time.
        private float _readyTime = 0; // Time after which a jump is possible.
        private Vector3 _velocity;
        private float _yawAngle;

        private void Awake()
        {
            TryGetComponent(out _gravity);
            TryGetComponent(out _groundCheck);
            TryGetComponent(out _head);
            TryGetComponent(out _settings);
        }
        
        /// <summary>
        /// Requests a jump and jumps at the timing when a jump becomes possible.
        /// If a request comes in at a timing when jumping is not possible, maintain the jump request for the time of _standbyTime. (In other words, the jump is processed at the moment it becomes possible to jump.)
        /// Note that it does not jump immediately.
        /// Nếu có yêu cầu nhảy, nhảy khi thời gian nhảy trở thành khả dụng.
        /// Nếu có yêu cầu nhảy khi không thể nhảy, giữ yêu cầu nhảy trong thời gian _standbyTime. (Nói cách khác, nhảy được xử lý khi thời gian nhảy trở thành khả dụng.)
        /// Lưu ý rằng nó không nhảy ngay lập tức.
        /// </summary>
        /// <param name="incrementJumpCount">Count the number of jumps</param>.
        [RenamedFrom("RequestJump")]
        public void Jump(bool incrementJumpCount = true)
        {
            _requestJumpIncrement = incrementJumpCount;
            _requestJump = Time.time + _standbyTime;
        }

        /// <summary>
        /// Forces a jump, ignoring AllowJump and JumpCount.
        /// This process is executed immediately.
        /// Quyền nhảy, bỏ qua AllowJump và JumpCount.
        /// Quyền nhảy, bỏ qua AllowJump và JumpCount.
        /// </summary>
        /// <param name="incrementJumpCount">The number of jumps is +1. </param>
        [RenamedFrom("Jump")]
        public void ForceJump(bool incrementJumpCount = true)
        {
            // +1 to the number of jumps if already in the air; if not off the ground,
            // the number of jumps in the air is not counted.
            // Nếu có yêu cầu nhảy, nhảy khi thời gian nhảy trở thành khả dụng.
            // Nếu có yêu cầu nhảy khi không thể nhảy, giữ yêu cầu nhảy trong thời gian _standbyTime. (Nói cách khác, nhảy được xử lý khi thời gian nhảy trở thành khả dụng.)
            // Lưu ý rằng nó không nhảy ngay lập tức.
            if ( incrementJumpCount && _leaveTime > 0)
                AerialJumpCount += 1;
            
            IsJumpStart = true;
            OnJump?.Invoke();
            
            // Initialize
            // Khởi tạo yêu cầu nhảy.
            _requestJump = -1;
            IsReadyToJump = false;
            _readyTime = 0;

            // Tính toán độ mạnh của nhảy dựa trên hướng nhảy và lực nhảy.
            // Vectơ là cho trục XZ; sử dụng giá trị Gravity cho trục Y.
            // Calculate jump strength based on jump direction and jump force.
            // Vectors are for XZ axis only; use Gravity values for Y axis.
            var direction = JumpDirection.normalized;
            _velocity = new Vector3(direction.x, 0, direction.z) * HeightToJumpPower;
            _gravity.SetVelocity( new Vector3(0, direction.y, 0) * HeightToJumpPower );

            // Nếu vận tốc không bằng không, tính toán góc quay
            if (_velocity != Vector3.zero)
                _yawAngle = Vector3.SignedAngle(Vector3.forward, _velocity, Vector3.up);
            
            // Đặt trạng thái nhảy thành True.
            IsJumping = true;
        }

        /// <summary>
        /// Reset the number of jumps, the vector, and the decision during a jump
        /// Đặt lại số lần nhảy, vectơ, và quyết định trong khi nhảy.
        /// </summary>
        public void ResetJump()
        {
            // Đặt lại số lần nhảy, vectơ, và quyết định trong khi nhảy.
            AerialJumpCount = 0;
            IsJumping = false;
            _velocity = Vector3.zero;
            IsReadyToJump = false;
            IsReadyToJumpStart = false;
        }

        private void ReadyJump()
        {
            // Đặt trạng thái nhảy thành True.
            IsReadyToJump = true;
            IsReadyToJumpStart = true;
            OnReadyToJump?.Invoke();
        }

        private void CalculateContactEnvironment()
        {
            // Nếu bất kỳ thứ gì tiếp xúc với đầu, tốc độ giảm xuống không.
            if ( _head.IsHeadContact && _gravity.FallSpeed > 0)
            {
                _gravity.SetVelocity(Vector3.zero);
            }

            // Đặt số lần nhảy thành 0 khi đạp đất.
            if (_groundCheck.IsFirmlyOnGround && _gravity.FallSpeed <= 0)
            {
                ResetJump();
            }
        }
        
        // Kiểm tra xem có thể nhảy không.
        private bool IsCanJump => _groundCheck.IsFirmlyOnGround || AerialJumpCount < MaxAerialJumpCount;

        // Tính toán độ mạnh của nhảy dựa trên hướng nhảy và lực nhảy.
        private float HeightToJumpPower => Mathf.Sqrt(JumpHeight * -2.0f * _gravity.GravityScale * Physics.gravity.y);   

        void IUpdateComponent.OnUpdate(float deltaTime)
        {
            // Khởi tạo.
            IsJumpStart = false;
            IsReadyToJumpStart = false;
            
            // Tính toán thời gian ngoài đất.　Dùng để đánh giá nhảy trong không khí.
            _leaveTime = (_groundCheck.IsFirmlyOnGround && _gravity.FallSpeed <= 0) ? 0 : _leaveTime + deltaTime;
            if( IsReadyToJump )
                _readyTime += deltaTime;

            // Dập tắt vectơ nhảy.
            _velocity = _leaveTime == 0 ? Vector3.zero : Vector3.Lerp(_velocity, Vector3.zero, Drag * Time.deltaTime);
            
            CalculateContactEnvironment();
            
        // Chuẩn bị nhảy nếu có thể.
            if (Time.time < _requestJump && IsCanJump && IsReadyToJump == false)
            {
                ReadyJump();
                _readyTime = 0;
            }

            // Nhảy nếu nhảy được phép.
            if (IsReadyToJump && IsAllowJump)
            {
                ForceJump(_requestJumpIncrement);
            }
        }

        // Độ ưu tiên.
        int IUpdateComponent.Order => Order.Control;

        // Độ ưu tiên di chuyển.
        int IPriority<IMove>.Priority => IsJumping ?  MovePriority : 0;

        // Vận tốc di chuyển.
        Vector3 IMove.MoveVelocity => _velocity ;

        // Độ ưu tiên quay.
        int IPriority<ITurn>.Priority => IsJumping && _velocity.sqrMagnitude > 0  ? TurnPriority : 0;

        // Góc quay.
        int ITurn.TurnSpeed => _turnSpeed;

        float ITurn.YawAngle => _yawAngle;

        // Vận tốc.
        public Vector3 Velocity => _velocity;
        
        // Vẽ gizmos.
        private void OnDrawGizmosSelected()
        {
            const float cursorRadius = 0.1f;
            // Lấy thành phần CharacterSettings.
            if( _settings == null)
                TryGetComponent(out _settings);

            // Lấy vị trí của nhân vật.
            var position = transform.position;
            var width = _settings.Radius;

            // Nếu nhân vật đang nhảy.
            if (_leaveTime > 0)
            {
                // Tính toán vị trí của nhân vật.
                var characterCenter = position + new Vector3(0, _settings.Height * 0.5f, 0);
                // Tính toán vận tốc của nhân vật.
                var velocityOffset = _velocity + new Vector3(0, _gravity.FallSpeed, 0);
                // Tính toán vị trí của nhân vật.
                var velocityPosition = characterCenter + velocityOffset * 0.3f;
                // Vẽ đường thẳng.
                Gizmos.DrawLine(characterCenter, velocityPosition);
                GizmoDrawUtility.DrawSphere(velocityPosition, cursorRadius, Color.blue, 1);
            }
            else
            {
                var top = position + new Vector3(0, JumpHeight, 0);
                var size = new Vector3(_settings.Radius, 0, _settings.Radius);
                GizmoDrawUtility.DrawCube(top, size, Color.blue);
                Gizmos.DrawLine(position, top);
            }
        }
    }
}