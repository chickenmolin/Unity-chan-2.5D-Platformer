using UnityEngine;
using System.Collections;

public class PlayerDash : MonoBehaviour
{
    public Animator animator;
    public PlayerState playerState;
    public float dashPower = 10f;      // Lực dash (tốc độ dịch chuyển)
    public float dashTime = 0.2f;      // Thời gian dash
    public float dashCooldown = 1f;    // Thời gian hồi dash

    private bool isDashing = false;
    private bool isCooldown = false;

    void Update()
    {
        if (animator == null || playerState == null) return;

        if (!isDashing && !isCooldown && playerState.IsDashing)
        {
            StartCoroutine(DashCoroutine());
        }
    }

    IEnumerator DashCoroutine()
    {
        isDashing = true;
        isCooldown = true;
        animator.Play("DashStart", 0);

        Vector3 dashDirection = playerState.MoveInput.x != 0 ? new Vector3(Mathf.Sign(playerState.MoveInput.x), 0, 0) : Vector3.right;
        float timer = 0f;
        while (timer < dashTime)
        {
            transform.position += dashDirection * dashPower * Time.deltaTime;
            timer += Time.deltaTime;
            yield return null;
        }
        isDashing = false;
        // Bắt đầu cooldown
        yield return new WaitForSeconds(dashCooldown);
        isCooldown = false;
    }
}
