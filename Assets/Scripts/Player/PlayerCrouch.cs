using UnityEngine;

public class PlayerCrouch : MonoBehaviour
{
    public Animator animator;         // Kéo thả Animator vào Inspector
    public PlayerState playerState;   // Kéo thả PlayerState vào Inspector

    void Update()
    {
        if (animator == null || playerState == null) return;

        // Đồng bộ trạng thái crouch với Animator
        animator.SetBool("IsCrouch", playerState.IsCrouching);
    }
} 