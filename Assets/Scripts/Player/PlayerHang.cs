using UnityEngine;
using Unity.TinyCharacterController.Check;
using Unity.TinyCharacterController.Effect;

public class PlayerHang : MonoBehaviour
{
    public WallCheck wallCheck;      // Kéo thả WallCheck vào Inspector
    public PlayerState playerState;  // Kéo thả PlayerState vào Inspector
    public Animator animator;        // Kéo thả Animator vào Inspector
    public float hangGravity = 0f;   // Gravity khi bám tường (0 = đứng yên, <0 = tụt chậm)
    public bool isHanging = false;
    public PlayerJump playerJump;    // Kéo thả PlayerJump vào Inspector (hoặc tự động lấy)

    private CharacterController characterController;
    private Gravity gravity;

    void Awake()
    {
        if (wallCheck == null) wallCheck = GetComponent<WallCheck>();
        if (playerState == null) playerState = GetComponent<PlayerState>();
        if (animator == null) animator = GetComponent<Animator>();
        if (playerJump == null) playerJump = GetComponent<PlayerJump>();
        characterController = GetComponent<CharacterController>();
        gravity = GetComponent<Gravity>();
    }

    void Update()
    {
        if (wallCheck == null || playerState == null || animator == null || characterController == null) return;

        if (wallCheck.IsContact)
        {
            if (!isHanging)
            {
                isHanging = true;
                playerState.IsHanging = true;
                animator.SetBool("IsHang", true);
                animator.Play("Hang", 0);
                // Reset jump count khi bắt đầu hang
                if (playerJump != null && playerJump.jumpControl != null)
                {
                    playerJump.jumpControl.ResetJump();
                    Debug.Log("Reset jump count khi bám tường");
                }
                // Disable gravity khi bám tường
                if (gravity != null)
                {
                    gravity.enabled = false;
                }
                Debug.Log("Player bắt đầu bám tường");
            }
            // Giữ player đứng yên hoàn toàn
            characterController.Move(Vector3.zero);
        }
        else
        {
            if (isHanging)
            {
                isHanging = false;
                playerState.IsHanging = false;
                animator.SetBool("IsHang", false);
                // Enable gravity khi rời tường
                if (gravity != null)
                {
                    gravity.enabled = true;
                }
                Debug.Log("Player rời tường");
            }
        }
    }
} 