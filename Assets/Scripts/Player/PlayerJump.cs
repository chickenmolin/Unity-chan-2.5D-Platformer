using UnityEngine;
using Unity.TinyCharacterController.Control;
using Unity.TinyCharacterController.Effect;

public class PlayerJump : MonoBehaviour
{
    public PlayerState playerState;   // Kéo thả PlayerState vào Inspector
    public JumpControl jumpControl;   // Kéo thả JumpControl vào Inspector
    public Gravity gravity;           // Kéo thả Gravity vào Inspector
    public int maxAerialJumpCount = 1; // Số lần nhảy trên không, chỉnh được ngoài Inspector

    // Tham số vẽ đường nhảy
    [Header("Tham số vẽ đường nhảy")]
    public int trajectoryPoints = 30;
    public float timeStep = 0.05f;
    public Color trajectoryColor = Color.cyan;

    public Animator animator; // Kéo thả Animator vào Inspector

    private bool wasGrounded = true;

    void Awake()
    {
        if (jumpControl == null) jumpControl = GetComponent<JumpControl>();
        if (gravity == null) gravity = GetComponent<Gravity>();
    }

    void Start()
    {
        if (jumpControl != null)
            jumpControl.MaxAerialJumpCount = maxAerialJumpCount;
    }

    void Update()
    {
        if (playerState == null || jumpControl == null) return;

        // Xử lý nhảy: nhảy lần 1 khi trên mặt đất, double jump khi trên không
        if (playerState.IsJumping)
        {
            if (playerState.IsGrounded)
            {
                jumpControl.Jump(true);
                if (animator != null)
                    animator.SetBool("BonusJump", false); // Nhảy lần đầu, không phải double jump
                    // Đồng bộ trạng thái nhảy
                animator.SetTrigger("Jump");
            }
            else
            {
                jumpControl.Jump(true);
                if (animator != null)
                    animator.SetBool("BonusJump", true); // Double jump!
            }
        }

        // Reset BonusJump khi tiếp đất
        if (!wasGrounded && playerState.IsGrounded && animator != null)
        {
            animator.SetBool("BonusJump", false);
        }
        wasGrounded = playerState.IsGrounded;
    }

    void OnDrawGizmosSelected()
    {
        if (jumpControl == null || playerState == null || gravity == null) return;

        Vector3 startPos = transform.position;
        float jumpPower = jumpControl.JumpHeight;
        float gravityValue = Physics.gravity.y * gravity.GravityScale;

        // Lấy vận tốc ngang từ MoveControl nếu có
        float horizontalSpeed = 0f;
        var moveControl = GetComponent<Unity.TinyCharacterController.Control.MoveControl>();
        if (moveControl != null)
            horizontalSpeed = moveControl.CurrentSpeed * Mathf.Sign(playerState.MoveInput.x);

        Vector3 velocity = new Vector3(horizontalSpeed, Mathf.Sqrt(jumpPower * -2.0f * gravityValue), 0);

        Gizmos.color = trajectoryColor;
        Vector3 prevPoint = startPos;
        for (int i = 1; i <= trajectoryPoints; i++)
        {
            float t = i * timeStep;
            Vector3 nextPoint = startPos + velocity * t + 0.5f * new Vector3(0, gravityValue, 0) * t * t;
            Gizmos.DrawLine(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }
    }
} 