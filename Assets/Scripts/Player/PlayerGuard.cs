using UnityEngine;
using System.Collections;

public class PlayerGuard : MonoBehaviour
{
    public PlayerState playerState;
    public Animator animator;
    public float guardDuration = 0.5f;  // Thời gian phòng thủ
    public float guardCooldown = 1f;    // Thời gian hồi phòng thủ

    private bool isGuarding = false;
    private bool isCooldown = false;

    void Update()
    {
        if (playerState == null || animator == null) return;

        if (!isGuarding && !isCooldown && playerState.IsGuarding)
        {
            StartCoroutine(GuardCoroutine());
        }
    }

    IEnumerator GuardCoroutine()
    {
        isGuarding = true;
        isCooldown = true;
        playerState.IsGuarding = true;
        
        if (animator != null)
        {
            animator.Play("Guard", 0);
            animator.SetBool("Guard", true); // Dùng tên "Guard"
            Debug.Log("Guard animation triggered!");
        }

        yield return new WaitForSeconds(guardDuration);

        isGuarding = false;
        playerState.IsGuarding = false;
        
        if (animator != null)
        {
            animator.SetBool("Guard", false); // Dùng tên "Guard"
            Debug.Log("Guard animation ended!");
        }

        yield return new WaitForSeconds(guardCooldown);
        isCooldown = false;
    }
} 