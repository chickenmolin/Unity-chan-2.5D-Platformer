using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

/// <summary>
/// Script quản lý máu cho Enemy, tự động cập nhật UI HealthBarEnemy
/// </summary>
public class HealthEnemy : MonoBehaviour
{
    public GameObject enemyVisual; // đối tượng hiển thị mesh, model, v.v.

    [Header("UI References")]
    [SerializeField] private Slider healthSliderEnemy;
    [SerializeField] private Image healthFillImageEnemy;
    [SerializeField] private TextMeshProUGUI healthTextEnemy;

    [Header("Health Settings")]
    [SerializeField] private float maxHealthEnemy = 100f;
    [SerializeField] private float currentHealthEnemy;

    [Header("Color Settings")]
    [SerializeField] private Color fullHealthColor = Color.green;
    [SerializeField] private Color lowHealthColor = Color.red;
    [SerializeField] private Color mediumHealthColor = Color.yellow;
    [SerializeField] private float lowHealthThreshold = 0.25f;
    [SerializeField] private float mediumHealthThreshold = 0.5f;

    [Header("Animation")]
    [SerializeField] private bool smoothUpdate = true;
    [SerializeField] private float updateSpeed = 5f;

    private Animator animator;
    private float targetHealthValue;
    private float currentHealthValue;
    private bool isAnimating = false;

    private void Start()
    {
        currentHealthEnemy = maxHealthEnemy;
        targetHealthValue = currentHealthEnemy;
        currentHealthValue = currentHealthEnemy;
        SetupUI();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (smoothUpdate && isAnimating)
        {
            UpdateHealthBarSmooth();
        }
            // Test: Nhấn phím T để trừ 10 máu
        if (Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("Test: Trừ 10 máu enemy");
            TakeDamage(10f);
        }
    }

    private void SetupUI()
    {
        if (healthSliderEnemy != null)
        {
            healthSliderEnemy.minValue = 0f;
            healthSliderEnemy.maxValue = maxHealthEnemy;
            healthSliderEnemy.value = currentHealthEnemy;
        }
        UpdateHealthText();
        UpdateHealthColor();
    }

    public void TakeDamage(float damage)
    {
        if (currentHealthEnemy <= 0) return;
        currentHealthEnemy = Mathf.Max(0, currentHealthEnemy - damage);
        targetHealthValue = currentHealthEnemy;

            // Play animation bị đánh
        if (animator != null)
        {
            animator.SetTrigger("GetHit");
        }

        if (smoothUpdate) isAnimating = true;
        else { currentHealthValue = targetHealthValue; UpdateHealthDisplay(); }

        UpdateHealthColor();

        if (currentHealthEnemy <= 0) Die();
    }

    public void Heal(float healAmount)
    {
        if (currentHealthEnemy <= 0) return;
        currentHealthEnemy = Mathf.Min(maxHealthEnemy, currentHealthEnemy + healAmount);
        targetHealthValue = currentHealthEnemy;

        if (smoothUpdate) isAnimating = true;
        else { currentHealthValue = targetHealthValue; UpdateHealthDisplay(); }

        UpdateHealthColor();
    }

    public void FullHeal() { Heal(maxHealthEnemy); }

    private void UpdateHealthBarSmooth()
    {
        currentHealthValue = Mathf.Lerp(currentHealthValue, targetHealthValue, updateSpeed * Time.deltaTime);
        if (Mathf.Abs(currentHealthValue - targetHealthValue) < 0.01f)
        {
            currentHealthValue = targetHealthValue;
            isAnimating = false;
        }
        UpdateHealthDisplay();
    }

    private void UpdateHealthDisplay()
    {
        if (healthSliderEnemy != null) healthSliderEnemy.value = currentHealthValue;
        UpdateHealthText();
    }

    private void UpdateHealthText()
    {
        if (healthTextEnemy != null)
            healthTextEnemy.text = $"{currentHealthValue:F0}/{maxHealthEnemy:F0}";
    }

    private void UpdateHealthColor()
    {
        if (healthFillImageEnemy == null) return;
        float healthPercentage = currentHealthEnemy / maxHealthEnemy;
        if (healthPercentage <= lowHealthThreshold)
            healthFillImageEnemy.color = lowHealthColor;
        else if (healthPercentage <= mediumHealthThreshold)
            healthFillImageEnemy.color = mediumHealthColor;
        else
            healthFillImageEnemy.color = fullHealthColor;
    }

    private void Die()
    {
        Debug.Log($"Enemy {gameObject.name} đã chết!");

        StartCoroutine(HideAfterDelay(1f));
    }

        private IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (enemyVisual != null)
        {
            enemyVisual.SetActive(false); // Ẩn phần visual (mesh, model,...)
        }
        else
        {
            // Nếu không gán thì ẩn luôn object chứa script này
            gameObject.SetActive(false);
        }
    }

    // Public API
    public string GetHealthText() => $"{currentHealthEnemy:F0}/{maxHealthEnemy:F0}";
    public float GetHealthPercentage() => currentHealthEnemy / maxHealthEnemy;
    public bool IsAlive() => currentHealthEnemy > 0;

    public void SetMaxHealth(float newMaxHealth)
    {
        maxHealthEnemy = newMaxHealth;
        currentHealthEnemy = Mathf.Min(currentHealthEnemy, maxHealthEnemy);
        targetHealthValue = currentHealthEnemy;
        currentHealthValue = currentHealthEnemy;
        if (healthSliderEnemy != null) healthSliderEnemy.maxValue = maxHealthEnemy;
        UpdateHealthDisplay();
        UpdateHealthColor();
    }

    public void SetCurrentHealth(float newHealth)
    {
        currentHealthEnemy = Mathf.Clamp(newHealth, 0, maxHealthEnemy);
        targetHealthValue = currentHealthEnemy;

        if (smoothUpdate) isAnimating = true;
        else { currentHealthValue = targetHealthValue; UpdateHealthDisplay(); }

        UpdateHealthColor();

        if (currentHealthEnemy <= 0) Die();
    }

    public void SetHealthColors(Color full, Color medium, Color low)
    {
        fullHealthColor = full;
        mediumHealthColor = medium;
        lowHealthColor = low;
        UpdateHealthColor();
    }

    public void SetHealthThresholds(float low, float medium)
    {
        lowHealthThreshold = low;
        mediumHealthThreshold = medium;
        UpdateHealthColor();
    }

    public void SetSmoothUpdate(bool smooth) => smoothUpdate = smooth;
}
