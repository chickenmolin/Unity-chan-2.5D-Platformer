using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// SpikeTrap cho game 2.5D với CharacterController
/// </summary>
public class SpikeTrap : MonoBehaviour
{
    [Header("Trap Settings")]
    [SerializeField] private Animator animator;//animation
    [SerializeField] private float damageAmount = 25f;//sát thương
    [SerializeField] private bool isActive = true;//trạng thái
    
    [Header("Effects")]
    [SerializeField] private bool flashOnActivation = true;//flash
    [SerializeField] private Color flashColor = Color.white;//màu
    [SerializeField] private float flashDuration = 0.2f;//thời gian
    
    private SpriteRenderer spriteRenderer;//sprite
    private Color originalColor;//màu
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();//sprite
        if (spriteRenderer != null) //nếu có sprite     
        {
            originalColor = spriteRenderer.color;//màu
        }
    }
    
    /// <summary>
    /// Trigger khi player chạm vào (3D cho CharacterController)
    /// </summary>
    private void OnTriggerEnter(Collider other) // 3D: dùng Collider cho CharacterController
    {
        if (!isActive) return;
        
        if (other.CompareTag("Player"))
        {
            // Kích hoạt animation nếu có
            if (animator != null)
            {
                animator.SetTrigger("Attack");
            }
            
            // Tìm HealthBar và gây damage
            HealthBar healthBar = FindObjectOfType<HealthBar>();
            if (healthBar != null)
            {
                healthBar.TakeDamage(damageAmount);
                Debug.Log($"SpikeTrap gây {damageAmount} damage cho player!");
            }
            else
            {
                Debug.LogWarning("Không tìm thấy HealthBar!");
            }
            
            // Flash effect
            if (flashOnActivation && spriteRenderer != null)
            {
                StartCoroutine(FlashCoroutine());
            }
        }
    }
    
    /// <summary>
    /// Flash effect khi kích hoạt
    /// </summary>
    private System.Collections.IEnumerator FlashCoroutine()//flash
    {
        if (spriteRenderer == null) yield break;
        
        spriteRenderer.color = flashColor;
        yield return new WaitForSeconds(flashDuration);
        spriteRenderer.color = originalColor;
    }
    
    /// <summary>
    /// Bật/tắt bẫy
    /// </summary>
    public void SetActive(bool active)
    {
        isActive = active;
    }
    
    /// <summary>
    /// Thiết lập damage
    /// </summary>
    public void SetDamageAmount(float damage)
    {
        damageAmount = damage;
    }
    
    /// <summary>
    /// Kích hoạt bẫy thủ công
    /// </summary>
    [ContextMenu("Activate Trap")]
    public void ActivateTrap()
    {
        if (!isActive) return;
        
        HealthBar healthBar = FindObjectOfType<HealthBar>();
        if (healthBar != null)
        {
            healthBar.TakeDamage(damageAmount);
            Debug.Log($"SpikeTrap thủ công gây {damageAmount} damage!");
        }
    }
    
    /// <summary>
    /// Test damage
    /// </summary>
    [ContextMenu("Test Damage")]
    public void TestDamage()
    {
        HealthBar healthBar = FindObjectOfType<HealthBar>();
        if (healthBar != null)
        {
            healthBar.TakeDamage(damageAmount);
            Debug.Log($"Test: SpikeTrap gây {damageAmount} damage!");
        }
        else
        {
            Debug.LogWarning("Không tìm thấy HealthBar để test!");
        }
    }
}
