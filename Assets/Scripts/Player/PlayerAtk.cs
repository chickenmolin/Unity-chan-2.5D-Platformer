using UnityEngine;

public class PlayerAtk : MonoBehaviour
{
    public PlayerState playerState;
    public Animator animator;
    public float attackDuration = 0.5f; // Thời gian attack
    public float comboWindow = 0.3f; // Thời gian để combo tiếp

    private bool isAttacking = false;
    private float attackTimer = 0f;
    private float lastAttackTime = 0f;

    void Update()
    {
        if (playerState == null || animator == null) return;

        // Reset combo nếu quá lâu không attack
        if (Time.time - lastAttackTime > comboWindow)
        {
            animator.SetBool("Hit1", false);
            animator.SetBool("Hit2", false);
            animator.SetBool("Hit3", false);
            Debug.Log("Combo timeout, reset all hits");
        }

        // Xử lý attack timer
        if (isAttacking)
        {
            attackTimer += Time.deltaTime;
            if (attackTimer >= attackDuration)
            {
                // Chỉ reset isAttacking, không reset bool
                isAttacking = false;
                attackTimer = 0f;
                Debug.Log("Attack finished, ready for next attack");
            }
        }

        // Cho phép attack ngay cả khi đang attack (để combo)
        if (playerState.IsAttacking)
        {
            Debug.Log("IsAttacking = true, calling OnClick");
            OnClick();
        }
    }

    void OnClick()
    {
        if (animator == null) return;
        
        var hit1 = animator.GetBool("Hit1");
        var hit2 = animator.GetBool("Hit2");

        Debug.Log($"Current state: hit1={hit1}, hit2={hit2}");

        if (!hit1 && !hit2)
        {
            animator.SetBool("Hit1", true);
            animator.Play("Slash1", 0);
            isAttacking = true;
            attackTimer = 0f;
            lastAttackTime = Time.time;
            Debug.Log("Attack: hit1 - Playing Slash1");
        }
        else if (hit1 && !hit2)
        {
            animator.SetBool("Hit2", true);
            animator.Play("Slash2", 0);
            isAttacking = true;
            attackTimer = 0f;
            lastAttackTime = Time.time;
            Debug.Log("Attack: hit2 - Playing Slash2");
        }
        else if (hit2)
        {
            animator.SetBool("Hit3", true);
            animator.Play("Slash3", 0);
            isAttacking = true;
            attackTimer = 0f;
            lastAttackTime = Time.time;
            Debug.Log("Attack: hit3 - Playing Slash3");
        }
    }

    // Gọi trong Animation Events
    public void ResetHit1() => animator.SetBool("Hit1", false);
    public void ResetHit2() => animator.SetBool("Hit2", false);
    public void ResetHit3() => animator.SetBool("Hit3", false);
} 