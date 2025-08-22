using UnityEngine;
using Unity.TinyCharacterController.Control;

public class PlayerAnimation : MonoBehaviour
{
    public Animator animator;
    public PlayerState playerState;
    public MoveControl moveControl;

    void Update()
    {
        if (animator == null) { Debug.LogError("Animator is NULL!"); return; }
        if (playerState == null) { Debug.LogError("PlayerState is NULL!"); return; }
        if (moveControl == null) { Debug.LogError("MoveControl is NULL!"); return; }

        // Đồng bộ trạng thái tiếp đất
        animator.SetBool("IsGround", playerState.IsGrounded);

        

        // Đồng bộ trạng thái chạy
        animator.SetBool("Run", playerState.IsRunning);

        // Đồng bộ trạng thái đứng
        animator.SetBool("IsCrouch", playerState.IsCrouching);

        // Đồng bộ tốc độ di chuyển cho Blend Tree Walk/Run
        float speed = moveControl.CurrentSpeed;
        animator.SetFloat("Speed", speed);
    }

    public void TriggerAnimationEvent()
    {
        // Thực hiện logic đặc biệt, ví dụ: phát hiệu ứng, đổi trạng thái, v.v.
        Debug.Log("TriggerAnimationEvent được gọi!");
    }
} 