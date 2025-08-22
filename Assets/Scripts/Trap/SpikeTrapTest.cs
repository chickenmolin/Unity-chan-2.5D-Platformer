using UnityEngine;

/// <summary>
/// Script test đơn giản để test SpikeTrap
/// </summary>
public class SpikeTrapTest : MonoBehaviour
{
    [Header("Test Settings")]
    [SerializeField] private bool enableTestInputs = true;
    [SerializeField] private KeyCode testDamageKey = KeyCode.T;
    [SerializeField] private KeyCode createTrapKey = KeyCode.Y;
    
    [Header("Test Values")]
    [SerializeField] private float testDamageAmount = 25f;
    
    private void Update()
    {
        if (!enableTestInputs) return;
        
        // Test damage
        if (Input.GetKeyDown(testDamageKey))
        {
            TestDamage();
        }
        
        // Create trap
        if (Input.GetKeyDown(createTrapKey))
        {
            CreateTestTrap();
        }
    }
    
    /// <summary>
    /// Test gây damage
    /// </summary>
    [ContextMenu("Test Damage")]
    public void TestDamage()
    {
        HealthBar healthBar = FindObjectOfType<HealthBar>();
        if (healthBar != null)
        {
            healthBar.TakeDamage(testDamageAmount);
            Debug.Log($"Test: Gây {testDamageAmount} damage cho player");
        }
        else
        {
            Debug.LogWarning("Không tìm thấy HealthBar!");
        }
    }
    
    /// <summary>
    /// Tạo bẫy gai test
    /// </summary>
    [ContextMenu("Create Test Trap")]
    public void CreateTestTrap()
    {
        // Tạo GameObject cho bẫy gai
        GameObject spikeTrap = new GameObject("Test_SpikeTrap");
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
        
        // Thêm Collider (3D cho CharacterController)
        BoxCollider collider = spikeTrap.AddComponent<BoxCollider>();
        collider.size = new Vector3(1f, 0.5f, 1f);
        collider.isTrigger = true;
        
        // Thêm SpikeTrap script
        SpikeTrap spikeTrapScript = spikeTrap.AddComponent<SpikeTrap>();
        spikeTrapScript.SetDamageAmount(testDamageAmount);
        
        Debug.Log("Test: Đã tạo bẫy gai test");
    }
    
    /// <summary>
    /// Hiển thị thông tin test
    /// </summary>
    // private void OnGUI()
    // {
    //     GUILayout.BeginArea(new Rect(10, 10, 300, 100));
    //     GUILayout.Label("=== SpikeTrap Test ===");
    //     GUILayout.Label("Controls:");
    //     GUILayout.Label($"{testDamageKey}: Test Damage");
    //     GUILayout.Label($"{createTrapKey}: Create Trap");
    //     GUILayout.EndArea();
    // }
} 