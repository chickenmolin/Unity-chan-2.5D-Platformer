using UnityEngine;

public class FPSDebugger : MonoBehaviour
{
    [Header("Tùy chọn hiển thị")]
    public bool showFPS = true;
    public KeyCode toggleKey = KeyCode.F2;

    private float deltaTime = 0.0f;

    void Start()
    {
        // Tắt VSync và giới hạn FPS
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = -1;

        Debug.Log("✅ FPS Debugger started. Press " + toggleKey + " to toggle display.");
    }

    void Update()
    {
        // Toggle hiển thị FPS
        if (Input.GetKeyDown(toggleKey))
        {
            showFPS = !showFPS;
            Debug.Log("🔁 Toggled FPS display: " + (showFPS ? "ON" : "OFF"));
        }

        // Cập nhật thời gian
        deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;

        if (showFPS)
        {
            float fps = 1.0f / deltaTime;
            Debug.Log("📈 FPS: " + Mathf.RoundToInt(fps));
        }
    }
}
