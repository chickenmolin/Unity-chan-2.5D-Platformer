using UnityEngine;

public class FinishTrigger : MonoBehaviour
{
    public GameObject finishCanvas; // Giao diện khi hoàn thành màn chơi
    public string nextSceneName = "Level2"; // Tên scene kế tiếp
    [SerializeField] private LoadingScreen loadingScreen; // ← kéo vào từ Inspector

    private bool levelFinished = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) // Kiểm tra va chạm với người chơi
        {
            if (finishCanvas != null)
            {
                finishCanvas.SetActive(true);   // Hiện canvas
                Time.timeScale = 0f;            // Dừng game nếu cần
            }
        }
    }
    public void OnNextLevelButtonPressed()// Nút tiếp theo  
    {
        Time.timeScale = 1f; // Tiếp tục game
        if (loadingScreen != null)
        {
            loadingScreen.StartLoading(nextSceneName);
        }
        else
        {
            Debug.LogError("LoadingScreen chưa được gán!");
        }
    }
}