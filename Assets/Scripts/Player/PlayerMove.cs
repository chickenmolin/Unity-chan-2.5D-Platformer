using UnityEngine;
using Unity.TinyCharacterController.Control;
using Unity.TinyCharacterController.Effect;

[RequireComponent(typeof(MoveControl))]

public class PlayerMove : MonoBehaviour
{
    public PlayerState playerState;
    public MoveControl moveControl;
    
    private float defaultWalkSpeed;
    public float runSpeedBonus = 2f;

    void Awake()
    {
        moveControl = GetComponent<MoveControl>();
       
        if (moveControl == null) moveControl = GetComponent<MoveControl>();
        defaultWalkSpeed = moveControl.MoveSpeed;
    }

    void Update()
    {
        // Nếu đang chạy, tăng tốc độ lên thêm 2, nếu không thì giữ nguyên tốc độ mặc định
        if (playerState.IsRunning)
            moveControl.MoveSpeed = defaultWalkSpeed + runSpeedBonus;
        else
            moveControl.MoveSpeed = defaultWalkSpeed;

        // Debug input
        //Debug.Log($"MoveInput: {playerState.MoveInput}, IsJumping: {playerState.IsJumping}");

        // Không di chuyển khi đang guard hoặc attack
        if (playerState.IsGuarding || playerState.IsAttacking)
        {
            moveControl.Move(Vector2.zero); // Đứng yên
            return;
        }

        // Di chuyển ngang
        moveControl.Move(playerState.MoveInput);
    }
}