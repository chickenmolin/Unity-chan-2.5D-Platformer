// Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// InputManager
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.TinyCharacterController.Control;

// Quản lý input cho player, sử dụng InputPlayer (Input System mới)
public class InputManager : MonoBehaviour
{
    public static InputPlayer InputPlayerInstance;

    public static Vector2 Movement => InputPlayerInstance?.Player.Move.ReadValue<Vector2>() ?? Vector2.zero;
    public static bool JumpWasPressed => InputPlayerInstance?.Player.Jump.WasPressedThisFrame() ?? false;
    public static bool RunIsHeld => InputPlayerInstance?.Player.Sprint.IsPressed() ?? false;
    public static bool RunWasPressed => InputPlayerInstance?.Player.Sprint.WasPressedThisFrame() ?? false;
    public static bool CrouchIsHeld => InputPlayerInstance?.Player.Crouch.IsPressed() ?? false;
    // Nếu muốn thêm các action khác như Dash, Attack, Guard, MousePosition, có thể bổ sung tương tự:
    public static bool DashWasPressed => InputPlayerInstance?.Player.Dash.WasPressedThisFrame() ?? false;
    public static bool AttackWasPressed => InputPlayerInstance?.Player.Attack.WasPressedThisFrame() ?? false;
    public static bool GuardIsHeld => InputPlayerInstance?.Player.Guard.IsPressed() ?? false;
    public static Vector2 MousePosition => InputPlayerInstance?.Player.MousePosition.ReadValue<Vector2>() ?? Vector2.zero;

    private void Awake()
    {
        InputPlayerInstance = new InputPlayer();
        InputPlayerInstance.Enable();
    }

    private void OnEnable()
    {
        if (InputPlayerInstance == null)
        {
            InputPlayerInstance = new InputPlayer();
        }
        InputPlayerInstance.Enable();
    }

    private void OnDisable()
    {
        InputPlayerInstance?.Disable();
    }

    // Xoá hàm Update vì không cần lưu giá trị mỗi frame nữa
}

public class PlayerInputToTCC : MonoBehaviour
{
    private MoveControl moveControl;

    void Awake()
    {
        moveControl = GetComponent<MoveControl>();
    }

    void Update()
    {
        // Truyền input từ InputManager cho MoveControl của TCC
        moveControl.Move(InputManager.Movement);
    }
}


