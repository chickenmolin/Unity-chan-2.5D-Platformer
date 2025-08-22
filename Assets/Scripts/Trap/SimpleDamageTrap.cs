using UnityEngine;

/// <summary>
/// Script damage trap đơn giản
/// </summary>
public class SimpleDamageTrap : MonoBehaviour
{
    [Header("Damage Settings")]
    [SerializeField] private float damageAmount = 20f;
    [SerializeField] private bool isActive = true;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!isActive) return;
        
        // Kiểm tra xem có phải player không
        if (other.CompareTag("Player"))
        {
            // Tìm HealthBar
            HealthBar healthBar = FindObjectOfType<HealthBar>();
            if (healthBar != null)
            {
                healthBar.TakeDamage(damageAmount);
                Debug.Log($"Bẫy gai gây {damageAmount} damage cho player!");
            }
            else
            {
                Debug.LogWarning("Không tìm thấy HealthBar!");
            }
        }
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
} 