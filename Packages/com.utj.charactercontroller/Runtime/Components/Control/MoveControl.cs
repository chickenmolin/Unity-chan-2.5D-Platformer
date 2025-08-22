using Unity.TinyCharacterController.Interfaces.Components;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using Unity.TinyCharacterController.Interfaces.Core;
using Unity.TinyCharacterController.Utility;

namespace Unity.TinyCharacterController.Control
{
    /// <summary>
    /// Component that controls the movement of the character
    /// <see cref="Move"/> to move the character position.
    /// The direction of movement is corrected by the camera position and the tilt of the ground. The character's orientation is also corrected according to the direction of movement.
    /// If <see cref="MovePriority"/> is higher than the other components, the character will move. If <see cref="TurnPriority"/> is higher than the other components, the character will turn in the direction of movement.
    /// If the character changes direction, <see cref="TurnSpeed"/> determines the speed of the turn.
    /// </summary>
    [AddComponentMenu(MenuList.MenuControl + nameof(MoveControl))]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Transform))]
    [RequireComponent(typeof(CharacterSettings))]
    [RenamedFrom("TinyCharacterController.MoveControl")]
    [RenamedFrom("TinyCharacterController.Control.MoveControl")]
    public class MoveControl : MonoBehaviour, 
        ITurn, 
        IMove, 
        IPriorityLifecycle<ITurn>,
        IUpdateComponent
    {
        /// <summary>
        /// Maximum movement speed of the character.
        /// tốc độ di chuyển
        /// </summary>
        [Header("Movement settings")]
        [SerializeField]
        private float _moveSpeed = 4;
        
        /// <summary>
        /// Rotation speed of the model when character priority is high
        /// tốc độ quay nhân vật
        /// </summary>
        [Header("tốc độ quay")]
        [Range(-1, 50)]
        [SerializeField]
        private int _turnSpeed = 15;

        /// <summary>
        /// Brake strength.
        /// If this value is high, the character will stop quickly.
        /// Kháng khi dừng - càng cao dừng càng nhanh.
        /// </summary>
        [Header("tốc độ hãm")]
        [SerializeField]
        private float _brakePower = 12;

        /// <summary>
        /// Acceleration.
        /// If this value is low, the character will accelerate slowly.
        /// </summary>
        [Header("tốc độ tăng tốc - tăng tốc")]
        [SerializeField]
        private float _accelerator = 6;

        /// <summary>
        /// Movable slope.
        /// If the terrain is below this angle, move along the terrain.
        /// </summary>
        [Header("góc di chuyển - góc di chuyển")]
        [SerializeField] 
        private float _angle = 45;
        
        /// <summary>
        /// Direction of movement.
        /// If anything other than Vector3.zero is specified,
        /// the character's direction of movement is limited to the direction of the specified axis.
        /// </summary>
        [Header("giới hạn di chuyển - giới hạn di chuyển")]
        [SerializeField]
        private Vector3 _lockAxis = Vector3.zero;

        /// <summary>
        /// Determines if MoveControl is used to move the character.
        /// If a higher value than other Priority is set, this component is used.
        /// Độ ưu tiên di chuyển    
        /// </summary>
        [Header("độ ưu tiên")]
        [SerializeField]
        private int _movePriority = 1;
        
        /// <summary>
        /// Threshold to determine if the character is in motion.
        /// If the value falls below this threshold, set <see cref="IsMove"/> to False.
        /// Ngưỡng dừng - nếu giá trị này nhỏ hơn, đặt <see cref="IsMove"/> thành False.
        /// </summary>
        [Header("ngưỡng dừng")]
        [FormerlySerializedAs("_threshold")]
        [SerializeField, Range(0, 1)]
        private float _moveStopThreshold = 0.2f;

        /// <summary>
        /// Determines if MoveControl is used for character orientation.
        /// If a higher value is set compared to other priorities, this component is used.
        /// Độ ưu tiên quay - nếu giá trị này cao hơn so với các độ ưu tiên khác, thì thành phần này được sử dụng.
        /// </summary>
        [FormerlySerializedAs("_turnPriority")]
        [SerializeField]
        public int TurnPriority = 1;

        /// <summary>
        /// Threshold to determine if the character's orientation has reached.
        /// If it's 0, orientation changes immediately after stopping the movement.
        /// If it's 1, the orientation is updated until it reaches the target orientation.
        /// Ngưỡng dừng quay - nếu giá trị này nhỏ hơn, đặt <see cref="IsTurning"/> thành False.
        /// </summary>
        [SerializeField, Range(0, 1)]
        private float _turnStopThreshold = 0;

        /// <summary>
        /// Current movement speed. If priority is 0, the movement speed is also 0.
        /// Tốc độ di chuyển hiện tại. Nếu độ ưu tiên là 0, tốc độ di chuyển cũng là 0.
        /// </summary>
        public float CurrentSpeed
        {
            get =>  (_movePriority > 0 || _currentSpeed < _moveStopThreshold) ? _currentSpeed : 0;
            set => _currentSpeed = value;
        }
        
        /// <summary>
        /// Movement vector in world coordinates
        /// Updated after Update
        /// Vectơ di chuyển trong tọa độ thế giới.
        /// Cập nhật sau khi Update.
        /// </summary>
        public Vector3 MoveVelocity { get; private set; }
        
        /// <summary>
        /// Character-based movement vector
        /// Vectơ di chuyển dựa trên nhân vật
        /// </summary>
        public Vector3 LocalVelocity => Quaternion.Inverse(_transform.rotation) * MoveVelocity;
        
        /// <summary>
        /// The direction the character wants to move in world coordinates.
        /// This value multiplied by Speed is the actual amount of movement.
        /// Hướng di chuyển của nhân vật trong tọa độ thế giới.
        /// Giá trị này nhân với tốc độ là số lượng di chuyển thực tế.
        /// </summary>
        public Vector3 Direction { get; private set; }

        /// <summary>
        /// Current Velocity
        /// Vận tốc hiện tại
        /// </summary>
        public Vector3 Velocity
        {
            get => _moveDirection * _currentSpeed;
            set
            {
                _moveDirection = value.normalized;
                _currentSpeed = value.magnitude;
            }
        } 

        /// <summary>
        /// Turn speed
        /// Tốc độ quay
        /// </summary>
        public int TurnSpeed
        {
            get => _turnSpeed; 
            set => _turnSpeed = value;
        }
        
        /// <summary>
        /// Maximum character movement speed
        /// Tốc độ di chuyển tối đa
        /// </summary>
        public float MoveSpeed
        {
            get => _moveSpeed;
            set =>_moveSpeed = value;
        }

        public float DeltaDirectionAngle => Vector3.SignedAngle(_transform.forward, _moveDirection, Vector3.up);

        /// <summary>.
        /// Character movement priority
        /// If priority is 0, stop moving
        /// Độ ưu tiên di chuyển
        /// Nếu độ ưu tiên là 0, dừng di chuyển
        /// </summary>
        public int MovePriority
        {
            get => _movePriority;
            set
            {
                IsMove = value != 0;
                _movePriority = value;
            }
        }

        /// <summary>
        ///  Limit direction of movement.
        /// Giới hạn hướng di chuyển.
        /// </summary>
        /// <param name="axis">axis</param>
        public void StartLockAxis(Vector3 axis)
        {
            _lockAxis = axis.normalized;
        }

        /// <summary>
        /// Stop limit.
        /// Dừng giới hạn.
        /// </summary>
        public void StopLockAxis()
        {
            _lockAxis = Vector3.zero;
        }
        
        
        /// <summary>
        /// True if the character's movement axis is restricted.
        /// True nếu trục di chuyển của nhân vật bị giới hạn.
            /// </summary>
        public bool IsLockAxis => _lockAxis.sqrMagnitude > 0;

        /// <summary>.
        /// True if the character is moving.
        /// Movement is determined based on <see cref="_moveStopThreshold"/>.
        /// True nếu nhân vật đang di chuyển.
        /// Di chuyển được xác định dựa trên <see cref="_moveStopThreshold"/>.
        /// </summary>
        public bool IsMove { get; private set; }

        /// <summary>
        /// The direction of movement from the character's perspective
        /// Hướng di chuyển từ nhân vật
        /// </summary>
        public Vector3 LocalDirection => Quaternion.Inverse(_transform.rotation) * Direction;
        
        /// <summary>
        /// Delta turn angle from previous frame.
        /// This value does not take TurnSpeed into account.
        /// Góc quay từ frame trước.
        /// Giá trị này không tính đến TurnSpeed.
        /// </summary>
        public float DeltaTurnAngle { get; private set; }
        
        /// <summary>
        /// Moves according to the stick input.
        /// Di chuyển theo đầu vào của tay cầm.
        /// </summary>
        /// <param name="leftStick">Direction of movement.</param>
        public void Move(Vector2 leftStick)
        {
            _inputValue = leftStick;
            _hasInput = leftStick.sqrMagnitude > 0;
        }

        private IGroundContact _groundCheck;               
        private bool _hasGroundCheck;                    
        private Transform _transform; 
        private Vector3 _moveDirection = Vector3.forward; 
        private float _currentSpeed; 
        private CharacterSettings _characterSettings;
        private IBrain _brain;
        private Vector2 _inputValue;
        private bool _hasInput;
        private float _yawAngle;
        private bool _isTurning;
        
        float ITurn.YawAngle => _yawAngle;
        // Quay nhân vật theo hướng di chuyển.
        void IPriorityLifecycle<ITurn>.OnUpdateWithHighestPriority(float deltaTime)
        {
            if (_hasInput)
            {
                _yawAngle = Vector3.SignedAngle(Vector3.forward, _moveDirection, Vector3.up);
            }
        }

        // Nhận độ ưu tiên quay.
        void IPriorityLifecycle<ITurn>.OnAcquireHighestPriority()
        {
            if (IsMove == false)
                _yawAngle = _brain.YawAngle;
        }

        // Mất độ ưu tiên quay.
        void IPriorityLifecycle<ITurn>.OnLoseHighestPriority() { }

        // Độ ưu tiên quay.
        int IPriority<ITurn>.Priority => IsMove || _isTurning ? TurnPriority : 0;
        int IPriority<IMove>.Priority => _currentSpeed > _moveStopThreshold ? _movePriority : 0;
        
        // Khởi tạo.
        private void Awake()
        {
            _hasGroundCheck = TryGetComponent(out _groundCheck);
            TryGetComponent(out _characterSettings);
            TryGetComponent(out _transform);
            TryGetComponent(out _brain);
        }

        // Vẽ gizmos.
        private void OnDrawGizmosSelected()
        {
            var offset = new Vector3(0, 0.1f, 0);
            Gizmos.color = Color.green;
            var position = transform.position + offset;
            
            // show lines when use Lock Axis.
            if (IsLockAxis)
            {
                var size = 0.2f;
                var p1 = position + _lockAxis * 5;
                var p2 = position - _lockAxis * 5;

                var green = Color.green;
                green.a = 0.4f;
                Gizmos.color = green;
                Gizmos.DrawCube(p1, new Vector3(size, size, size));
                Gizmos.DrawCube(p2, new Vector3(size, size, size));
                
                Gizmos.color = Color.green;
                Gizmos.DrawLine(p1, p2);
            }
            
            // show line about Move velocities.
            Gizmos.color = Color.green;
            Gizmos.DrawRay(position, MoveVelocity);
        }


        void IUpdateComponent.OnUpdate(float deltaTime)
        {
            using var profiler = new ProfilerScope(nameof(MoveControl));

            if (_hasInput)
            {
                var preDirection = _moveDirection;
                var cameraYawRotation = Quaternion.AngleAxis(_characterSettings.CameraTransform.rotation.eulerAngles.y, Vector3.up);
                var direction = new Vector3(_inputValue.x, 0, _inputValue.y);

                // Determines direction of movement according to camera orientation
                _moveDirection = cameraYawRotation * direction.normalized;

                if (IsLockAxis)
                {
                    var dot = Vector3.Dot(_moveDirection, _lockAxis);
                    _moveDirection = _lockAxis * Mathf.Round( Mathf.Clamp(dot * 100, -1, 1)) ;
                }

                _currentSpeed = Mathf.Lerp(_currentSpeed, _moveSpeed, _accelerator * deltaTime);
                DeltaTurnAngle = Vector3.SignedAngle(preDirection, _moveDirection, Vector3.up);
                
            } else {
                
                DeltaTurnAngle = 0;
                _currentSpeed = Mathf.Lerp(_currentSpeed, 0, _brakePower * deltaTime);
                if (_currentSpeed < _moveStopThreshold)
                    _currentSpeed = 0;
            }

            // Determines direction of movement according to ground information
            var normal = _hasGroundCheck && _groundCheck.IsOnGround ? _groundCheck.GroundSurfaceNormal : Vector3.up;
            normal = Vector3.Angle(Vector3.up, normal) < _angle ? normal : Vector3.up;
            Direction = Vector3.ProjectOnPlane(_moveDirection, normal);

            MoveVelocity = Direction * _currentSpeed;
            IsMove = _currentSpeed > _moveStopThreshold;
            _isTurning = Vector3.Angle(_transform.forward, _moveDirection) > (1 - _turnStopThreshold) * 360;
        }

        int IUpdateComponent.Order => Order.Control;
    }
}
