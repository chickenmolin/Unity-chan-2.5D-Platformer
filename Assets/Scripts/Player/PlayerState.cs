using UnityEngine;
using Unity.TinyCharacterController;
using Unity.TinyCharacterController.Check;
using Unity.TinyCharacterController.Control;
using Unity.TinyCharacterController.Core;
using Unity.TinyCharacterController.Effect;
using Unity.TinyCharacterController.Interfaces.Components;
using Unity.TinyCharacterController.Interfaces.Core;
using Unity.TinyCharacterController.Utility;

public class PlayerState : MonoBehaviour
{
    public bool IsGrounded { get; private set; }
    public bool IsJumping { get; set; }
    public bool IsRunning { get; set; }
    public bool IsCrouching { get; set; }
    public bool IsDashing { get; set; }
    public bool IsAttacking { get; set; }
    public bool IsHanging { get; set; } // Thêm trạng thái bám tường
    public bool IsGuarding { get; set; } // Thêm trạng thái phòng thủ
    public Vector2 MoveInput { get; set; }
    public Vector2 MousePosition { get; set; }

    private GroundCheck groundCheck;
    private bool isRunning = false;

    void Awake()
    {
        groundCheck = GetComponent<GroundCheck>();
        if (groundCheck == null)
            Debug.LogError("GroundCheck component is missing!");
    }

    void Update()
    {
        // Cập nhật trạng thái dựa trên input
        MoveInput = InputManager.Movement;
        IsJumping = InputManager.JumpWasPressed;
        if (InputManager.RunWasPressed)
        {
            isRunning = !isRunning;
        }
        IsRunning = isRunning;
        IsCrouching = InputManager.CrouchIsHeld; // hoặc toggle nếu bạn muốn nhấn 1 lần đổi trạng thái
        IsDashing = InputManager.DashWasPressed;
        IsAttacking = InputManager.AttackWasPressed;
        IsGuarding = InputManager.GuardIsHeld; // Thêm cập nhật trạng thái Guard
        MousePosition = InputManager.MousePosition;
        // Không cập nhật IsGrounded ở đây nữa
    }

    void LateUpdate()
    {
        // Lấy trạng thái grounded từ GroundCheck 
        if (groundCheck != null)
        {
            IsGrounded = groundCheck.IsOnGround;
            //Debug.Log($"[PlayerState] IsGrounded: {IsGrounded}");
        }
    }
} 