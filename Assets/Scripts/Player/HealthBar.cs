using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Script quản lý HealthBar UI
/// </summary>
public class HealthBar : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider healthSlider; // Thanh máu trên UI
    [SerializeField] private Image healthFillImage; // Màu của thanh máu
[SerializeField] private TextMeshProUGUI healthText; // Text hiển thị số HP
    
    [Header("Health Settings")]
    [SerializeField] private float maxHealth = 100f; // HP tối đa
    [SerializeField] private float currentHealth; // HP hiện tại
    
    [Header("Color Settings")]
    [SerializeField] private Color fullHealthColor = Color.green; // Màu khi HP đầy
    [SerializeField] private Color lowHealthColor = Color.red; // Màu khi HP thấp
    [SerializeField] private Color mediumHealthColor = Color.yellow; // Màu khi HP trung bình
    [SerializeField] private float lowHealthThreshold = 0.25f; // Ngưỡng HP thấp
    [SerializeField] private float mediumHealthThreshold = 0.5f; // Ngưỡng HP trung bình
    
    [Header("Animation")]
    [SerializeField] private bool smoothUpdate = true; // Bật/tắt smooth update
    [SerializeField] private float updateSpeed = 5f; // Tốc độ cập nhật
    
    private float targetHealthValue; // Giá trị mục tiêu
    private float currentHealthValue; // Giá trị hiện tại
    private bool isAnimating = false; // Trạng thái animation   
    
    private void Start()
    {
        // Khởi tạo HP
        currentHealth = maxHealth;
        targetHealthValue = currentHealth;
        currentHealthValue = currentHealth;
        
        // Setup UI
        SetupUI();
        
        Debug.Log($"HealthBar: HP = {currentHealth}/{maxHealth}");
    }
    
    private void Update()
    {
        // Cập nhật UI mượt mà
        if (smoothUpdate && isAnimating)
        {
            UpdateHealthBarSmooth();
        }
    }
    
    /// <summary>
    /// Setup UI
    /// </summary>
    private void SetupUI()
    {
        if (healthSlider != null)
        {
            healthSlider.minValue = 0f;
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
        
        UpdateHealthText();
        UpdateHealthColor();
    }
    
    /// <summary>
    /// Nhận damage
    /// </summary>
    public void TakeDamage(float damage)
    {
        if (currentHealth <= 0) return;
        
        currentHealth = Mathf.Max(0, currentHealth - damage);
        targetHealthValue = currentHealth;
        
        if (smoothUpdate)
        {
            isAnimating = true;
        }
        else
        {
            currentHealthValue = targetHealthValue;
            UpdateHealthDisplay();
        }
        
        UpdateHealthColor();
        
        Debug.Log($"Nhận {damage} damage! HP còn: {currentHealth}");
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    /// <summary>
    /// Hồi máu
    /// </summary>
    public void Heal(float healAmount)
    {
        if (currentHealth <= 0) return;
        
        currentHealth = Mathf.Min(maxHealth, currentHealth + healAmount);
        targetHealthValue = currentHealth;
        
        if (smoothUpdate)
        {
            isAnimating = true;
        }
        else
        {
            currentHealthValue = targetHealthValue;
            UpdateHealthDisplay();
        }
        
        UpdateHealthColor();
        
        Debug.Log($"Hồi {healAmount} HP! HP hiện tại: {currentHealth}");
    }
    
    /// <summary>
    /// Hồi máu hoàn toàn
    /// </summary>
    public void FullHeal()
    {
        Heal(maxHealth);
    }
    
    /// <summary>
    /// Cập nhật health bar mượt mà
    /// </summary>
    private void UpdateHealthBarSmooth()
    {
        currentHealthValue = Mathf.Lerp(currentHealthValue, targetHealthValue, 
            updateSpeed * Time.deltaTime);
        
        if (Mathf.Abs(currentHealthValue - targetHealthValue) < 0.01f)
        {
            currentHealthValue = targetHealthValue;
            isAnimating = false;
        }
        
        UpdateHealthDisplay();
    }
    
    /// <summary>
    /// Cập nhật hiển thị HP
    /// </summary>
    private void UpdateHealthDisplay()
    {
        // Cập nhật slider
        if (healthSlider != null)
        {
            healthSlider.value = currentHealthValue;
        }
        
        // Cập nhật text
        UpdateHealthText();
    }
    
    /// <summary>
    /// Cập nhật text HP
    /// </summary>
    private void UpdateHealthText()
    {
        if (healthText != null)
        {
            healthText.text = $"{currentHealthValue:F0}/{maxHealth:F0}";
        }
    }
    
    /// <summary>
    /// Cập nhật màu sắc theo HP
    /// </summary>
    private void UpdateHealthColor()
    {
        if (healthFillImage == null) return;
        
        float healthPercentage = currentHealth / maxHealth;
        
        if (healthPercentage <= lowHealthThreshold)
        {
            healthFillImage.color = lowHealthColor;
        }
        else if (healthPercentage <= mediumHealthThreshold)
        {
            healthFillImage.color = mediumHealthColor;
        }
        else
        {
            healthFillImage.color = fullHealthColor;
        }
    }
    
    /// <summary>
    /// Xử lý khi chết
    /// </summary>
    private void Die()
    {
        Debug.Log("Player đã chết!");
        // Có thể thêm logic game over ở đây
        Time.timeScale = 1f; // Đảm bảo không bị pause
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Load lại màn chơi hiện tại
    }
    
    /// <summary>
    /// Lấy thông tin HP dạng string
    /// </summary>
    public string GetHealthText()
    {
        return $"{currentHealth:F0}/{maxHealth:F0}";
    }
    
    /// <summary>
    /// Lấy percentage HP
    /// </summary>
    public float GetHealthPercentage()
    {
        return currentHealth / maxHealth;
    }
    
    /// <summary>
    /// Kiểm tra còn sống không
    /// </summary>
    public bool IsAlive()
    {
        return currentHealth > 0;
    }
    
    /// <summary>
    /// Thiết lập HP tối đa
    /// </summary>
    public void SetMaxHealth(float newMaxHealth)
    {
        maxHealth = newMaxHealth;
        currentHealth = Mathf.Min(currentHealth, maxHealth);
        targetHealthValue = currentHealth;
        currentHealthValue = currentHealth;
        
        if (healthSlider != null)
        {
            healthSlider.maxValue = maxHealth;
        }
        
        UpdateHealthDisplay();
        UpdateHealthColor();
    }
    
    /// <summary>
    /// Thiết lập HP hiện tại
    /// </summary>
    public void SetCurrentHealth(float newHealth)
    {
        currentHealth = Mathf.Clamp(newHealth, 0, maxHealth);
        targetHealthValue = currentHealth;
        
        if (smoothUpdate)
        {
            isAnimating = true;
        }
        else
        {
            currentHealthValue = targetHealthValue;
            UpdateHealthDisplay();
        }
        
        UpdateHealthColor();
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }
    
    /// <summary>
    /// Thiết lập màu sắc
    /// </summary>
    public void SetHealthColors(Color full, Color medium, Color low)
    {
        fullHealthColor = full;
        mediumHealthColor = medium;
        lowHealthColor = low;
        UpdateHealthColor();
    }
    
    /// <summary>
    /// Thiết lập threshold
    /// </summary>
    public void SetHealthThresholds(float low, float medium)
    {
        lowHealthThreshold = low;
        mediumHealthThreshold = medium;
        UpdateHealthColor();
    }
    
    /// <summary>
    /// Bật/tắt smooth update
    /// </summary>
    public void SetSmoothUpdate(bool smooth)
    {
        smoothUpdate = smooth;
    }
}       