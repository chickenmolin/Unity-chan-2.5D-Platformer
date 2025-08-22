using UnityEngine;
using System.Collections;
using Unity.TinyCharacterController.Control;
using Unity.TinyCharacterController.Check;

public class PlayerWallJump : MonoBehaviour
{
    public PlayerState playerState;
    public Animator animator;
    public WallCheck wallCheck;
    public JumpControl jumpControl;
    public float wallJumpForce = 8f;     // Lực nhảy tường
    public float wallJumpDuration = 0.2f; // Thời gian nhảy tường
    public float wallJumpCooldown = 0.5f; // Thời gian hồi nhảy tường

    private bool isWallJumping = false;
    private bool isCooldown = false;

    void Awake()
    {
        if (wallCheck == null) wallCheck = GetComponent<WallCheck>();
        if (jumpControl == null) jumpControl = GetComponent<JumpControl>();
    }

    void Update()
    {
        if (playerState == null || animator == null) return;

        if (!isWallJumping && !isCooldown && playerState.IsJumping && playerState.IsHanging)
        {
            StartCoroutine(WallJumpCoroutine());
        }
    }

    IEnumerator WallJumpCoroutine()
    {
        isWallJumping = true;
        isCooldown = true;
        animator.SetTrigger("WallJump");

        // Tính hướng nhảy (ngược với hướng tường)
        Vector3 jumpDirection = -wallCheck.Normal;
        jumpDirection.y = 1f; // Thêm lực nhảy lên
        jumpDirection.Normalize();

        // Thực hiện nhảy tường
        jumpControl.Jump(true);
        
        // Di chuyển theo hướng nhảy tường
        float timer = 0f;
        while (timer < wallJumpDuration)
        {
            transform.position += jumpDirection * wallJumpForce * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }

        isWallJumping = false;
        yield return new WaitForSeconds(wallJumpCooldown);
        isCooldown = false;
    }
} 