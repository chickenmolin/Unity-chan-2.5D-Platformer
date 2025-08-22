using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [Header("UI References")]
    public GameObject pauseCanvas; // Canvas menu pause
    public GameObject pausePanel; // Reference tới PausePanel
    public bool isPaused = false;

    [SerializeField] private string menuSceneName = "Menu"; // Tên scene menu

    void Start()
    {
        if (pauseCanvas != null)
            pauseCanvas.SetActive(false); // Ẩn khi game bắt đầu
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void PauseGame()
    {
        if (pauseCanvas != null) pauseCanvas.SetActive(true);//Hiển thị canvas
        pausePanel.SetActive(true);
        Time.timeScale = 0f; // Dừng game
        isPaused = true;
    }

    public void ResumeGame()
    {
        if (pauseCanvas != null) pauseCanvas.SetActive(false);//
        pausePanel.SetActive(false);
        Time.timeScale = 1f; // Chạy lại game
        isPaused = false;
    }

    public void ResetLevel() // Reset level
    {
        Time.timeScale = 1f; // Đảm bảo game không bị dừng khi reload
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Load lại scene hiện tại
    }

    public void QuitGame()
    {
        Time.timeScale = 1f; // đảm bảo game chạy lại khi đổi scene
        SceneManager.LoadScene("Menu"); // tên scene menu
    }
}
