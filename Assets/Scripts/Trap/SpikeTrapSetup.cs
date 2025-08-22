using UnityEngine;
using UnityEngine.UI;
/// <summary>
/// Script để tự động setup SpikeTrap
/// </summary>
public class SpikeTrapSetup : MonoBehaviour
{
    [Header("Setup Settings")]
    [SerializeField] private bool autoSetupOnStart = true;
    [SerializeField] private float damageAmount = 25f;
    [SerializeField] private Color trapColor = Color.red;
    
    [Header("Trap Position")]
    [SerializeField] private Vector3 trapOffset = Vector3.right * 3f;
    
    private void Start()
    {
        if (autoSetupOnStart)
        {
            CreateSpikeTrap();
        }
    }
    
    /// <summary>
    /// Tạo SpikeTrap
    /// </summary>
    [ContextMenu("Create SpikeTrap")]
    public void CreateSpikeTrap()
    {
        Debug.Log("Bắt đầu tạo SpikeTrap...");
        
        // Tạo GameObject cho bẫy gai
        GameObject spikeTrap = new GameObject("SpikeTrap");
        spikeTrap.transform.position = transform.position + trapOffset;
        
        // Thêm SpriteRenderer
        SpriteRenderer spriteRenderer = spikeTrap.AddComponent<SpriteRenderer>();
        spriteRenderer.color = trapColor;
        
        // Tạo sprite đơn giản
        Texture2D texture = new Texture2D(32, 16);
        Color[] pixels = new Color[32 * 16];
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = trapColor;
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
        spikeTrapScript.SetDamageAmount(damageAmount);
        
        Debug.Log("Đã tạo SpikeTrap hoàn chỉnh!");
    }
    
    /// <summary>
    /// Tạo nhiều SpikeTrap
    /// </summary>
    [ContextMenu("Create Multiple SpikeTraps")]
    public void CreateMultipleSpikeTraps()
    {
        for (int i = 0; i < 3; i++)
        {
            GameObject spikeTrap = new GameObject($"SpikeTrap_{i}");
            spikeTrap.transform.position = transform.position + Vector3.right * (i + 1) * 2f;
            
            SpriteRenderer spriteRenderer = spikeTrap.AddComponent<SpriteRenderer>();
            spriteRenderer.color = trapColor;
            
            // Tạo sprite đơn giản
            Texture2D texture = new Texture2D(32, 16);
            Color[] pixels = new Color[32 * 16];
            for (int j = 0; j < pixels.Length; j++)
            {
                pixels[j] = trapColor;
            }
            texture.SetPixels(pixels);
            texture.Apply();
            spriteRenderer.sprite = Sprite.Create(texture, new Rect(0, 0, 32, 16), new Vector2(0.5f, 0.5f));
            
            BoxCollider collider = spikeTrap.AddComponent<BoxCollider>();
            collider.size = new Vector3(1f, 0.5f, 1f);
            collider.isTrigger = true;
            
            SpikeTrap spikeTrapScript = spikeTrap.AddComponent<SpikeTrap>();
            spikeTrapScript.SetDamageAmount(damageAmount + i * 5f);
        }
        
        Debug.Log("Đã tạo 3 SpikeTrap!");
    }
    
    /// <summary>
    /// Xóa tất cả SpikeTrap
    /// </summary>
    [ContextMenu("Clear All SpikeTraps")]
    public void ClearAllSpikeTraps()
    {
        SpikeTrap[] traps = FindObjectsOfType<SpikeTrap>();
        int count = 0;
        
        foreach (var trap in traps)
        {
            if (trap.name.Contains("SpikeTrap"))
            {
                DestroyImmediate(trap.gameObject);
                count++;
            }
        }
        
        Debug.Log($"Đã xóa {count} SpikeTrap!");
    }
    
    /// <summary>
    /// Test tất cả SpikeTrap
    /// </summary>
    [ContextMenu("Test All SpikeTraps")]
    public void TestAllSpikeTraps()
    {
        SpikeTrap[] traps = FindObjectsOfType<SpikeTrap>();
        
        foreach (var trap in traps)
        {
            trap.TestDamage();
        }
        
        Debug.Log($"Đã test {traps.Length} SpikeTrap!");
    }
    
    /// <summary>
    /// Kiểm tra hệ thống
    /// </summary>
    [ContextMenu("Check SpikeTrap System")]
    public void CheckSpikeTrapSystem()
    {
        SpikeTrap[] traps = FindObjectsOfType<SpikeTrap>();
        if (traps.Length > 0)
        {
            Debug.Log($"SpikeTrap: OK - Tìm thấy {traps.Length} bẫy");
        }
        else
        {
            Debug.LogWarning("SpikeTrap: NOT FOUND!");
        }
        
        HealthBar healthBar = FindObjectOfType<HealthBar>();
        if (healthBar != null)
        {
            Debug.Log("HealthBar: OK");
        }
        else
        {
            Debug.LogWarning("HealthBar: NOT FOUND!");
        }
        
        BoxCollider[] colliders = FindObjectsOfType<BoxCollider>();
        int triggerCount = 0;
        foreach (var collider in colliders)
        {
            if (collider.isTrigger) triggerCount++;
        }
        Debug.Log($"Trigger Colliders: {triggerCount}");
    }
    
    /// <summary>
    /// Tạo HealthBar nếu chưa có
    /// </summary>
    [ContextMenu("Create HealthBar")]
    public void CreateHealthBar()
    {
        HealthBar existingHealthBar = FindObjectOfType<HealthBar>();
        if (existingHealthBar != null)
        {
            Debug.Log("HealthBar đã tồn tại!");
            return;
        }
        
        // Tạo Canvas nếu chưa có
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("Canvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObj.AddComponent<CanvasScaler>();
            canvasObj.AddComponent<GraphicRaycaster>();
            Debug.Log("Đã tạo Canvas!");
        }
        
        // Tạo HealthBar GameObject
        GameObject healthBarObj = new GameObject("HealthBar");
        healthBarObj.transform.SetParent(canvas.transform, false);
        
        // Thêm HealthBar script
        HealthBar healthBar = healthBarObj.AddComponent<HealthBar>();
        
        // Tạo Slider
        Slider slider = healthBarObj.AddComponent<Slider>();
        slider.minValue = 0f;
        slider.maxValue = 100f;
        slider.value = 100f;
        
        // Setup position
        RectTransform healthBarRect = healthBarObj.GetComponent<RectTransform>();
        healthBarRect.anchorMin = new Vector2(0.1f, 0.8f);
        healthBarRect.anchorMax = new Vector2(0.9f, 0.9f);
        healthBarRect.sizeDelta = Vector2.zero;
        
        Debug.Log("Đã tạo HealthBar!");
    }
} 