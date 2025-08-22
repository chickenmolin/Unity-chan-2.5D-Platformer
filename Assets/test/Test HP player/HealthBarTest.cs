using UnityEngine;

/// <summary>
/// Script test đơn giản để test HealthBar
/// Gắn script SimpleHealthTest vào bất kỳ GameObject nào
// Play game và nhấn:
// H: Gây 10 damage
// J: Hồi 20 HP
/// </summary>
public class HealthBarTest : MonoBehaviour
{
    [Header("Test Settings")]
    [SerializeField] private bool enableTestInputs = true; // Bật/tắt test inputs
    [SerializeField] private KeyCode damageKey = KeyCode.H; // Phím gây damage
    [SerializeField] private KeyCode healKey = KeyCode.J; // Phím hồi máu
    [SerializeField] private KeyCode fullHealKey = KeyCode.K; // Phím hồi máu hoàn toàn
    
    [Header("Test Values")]
    [SerializeField] private float testDamageAmount = 20f; // Số lượng damage test
    [SerializeField] private float testHealAmount = 30f; // Số lượng heal test
    
    private HealthBar healthBar; // Reference to HealthBar component
    
    private void Start()
    {
        // Tìm HealthBar trong scene
        healthBar = FindObjectOfType<HealthBar>();
        
        if (healthBar == null)
        {
            Debug.LogWarning("HealthBarTest: Không tìm thấy HealthBar component!");
        }
        else
        {
            Debug.Log("HealthBarTest: Đã tìm thấy HealthBar!");
        }
    }
    
    private void Update()
    {
        if (!enableTestInputs || healthBar == null) return;
        
        // Test damage
        if (Input.GetKeyDown(damageKey))
        {
            TestDamage();
        }
        
        // Test heal
        if (Input.GetKeyDown(healKey))
        {
            TestHeal();
        }
        
        // Test full heal
        if (Input.GetKeyDown(fullHealKey))
        {
            TestFullHeal();
        }
    }
    
    /// <summary>
    /// Test gây damage
    /// </summary>
    [ContextMenu("Test Damage")]
    public void TestDamage()
    {
        if (healthBar == null) return;
        
        healthBar.TakeDamage(testDamageAmount);
        Debug.Log($"Test: Gây {testDamageAmount} damage cho player");
    }
    
    /// <summary>
    /// Test heal
    /// </summary>
    [ContextMenu("Test Heal")]
    public void TestHeal()
    {
        if (healthBar == null) return;
        
        healthBar.Heal(testHealAmount);
        Debug.Log($"Test: Hồi {testHealAmount} HP cho player");
    }
    
    /// <summary>
    /// Test full heal
    /// </summary>
    [ContextMenu("Test Full Heal")]
    public void TestFullHeal()
    {
        if (healthBar == null) return;
        
        healthBar.FullHeal();
        Debug.Log("Test: Hồi máu hoàn toàn cho player");
    }
    
    /// <summary>
    /// Tạo bẫy gai test
    /// </summary>
    [ContextMenu("Create Test Damage Trap")]
    public void CreateTestDamageTrap()
    {
        // Tạo GameObject cho bẫy gai
        GameObject spikeTrap = new GameObject("Test_DamageTrap");
        spikeTrap.transform.position = Vector3.right * 3f;
        
        // Thêm SpriteRenderer
        SpriteRenderer spriteRenderer = spikeTrap.AddComponent<SpriteRenderer>();
        spriteRenderer.color = Color.red;
        
        // Tạo sprite đơn giản
        Texture2D texture = new Texture2D(32, 16);
        Color[] pixels = new Color[32 * 16];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.red;
        }
        texture.SetPixels(pixels);
        texture.Apply();
        spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, 32, 16), new Vector2(0.5f, 0.5f));
        
        // Thêm Collider2D
        BoxCollider2D collider = spikeTrap.AddComponent<BoxCollider2D>();
        collider.size = new Vector2(1f, 0.5f);
        collider.isTrigger = true;
        
        // Thêm SimpleDamageTrap script
        SimpleDamageTrap damageTrap = spikeTrap.AddComponent<SimpleDamageTrap>();
        
        Debug.Log("Test: Đã tạo bẫy gai test");
    }
    
    /// <summary>
    /// Hiển thị thông tin HP
    /// </summary>
    // private void OnGUI()
    // {
    //     if (healthBar == null) return;
        
    //     GUILayout.BeginArea(new Rect(10, 10, 300, 150));
    //     GUILayout.Label("=== HealthBar Test ===");
    //     GUILayout.Label($"HP: {healthBar.GetHealthText()}");
    //     GUILayout.Label($"Percentage: {healthBar.GetHealthPercentage():P0}");
    //     GUILayout.Label($"Is Alive: {healthBar.IsAlive()}");
    //     GUILayout.Space(10);
    //     GUILayout.Label("Controls:");
    //     GUILayout.Label($"{damageKey}: Damage");
    //     GUILayout.Label($"{healKey}: Heal");
    //     GUILayout.Label($"{fullHealKey}: Full Heal");
    //     GUILayout.EndArea();
    // }
} 