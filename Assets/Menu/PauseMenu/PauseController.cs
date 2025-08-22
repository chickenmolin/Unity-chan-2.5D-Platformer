using UnityEngine;

public static class PauseController
{
    public static bool IsGamePaused
    {
        get
        {
            var pauseMenu = Object.FindObjectOfType<PauseMenu>();
            return pauseMenu != null && pauseMenu.isPaused;
        }
    }

    public static void SetPause(bool pause)
    {
        var pauseMenu = Object.FindObjectOfType<PauseMenu>();
        if (pauseMenu != null)
        {
            if (pause) pauseMenu.PauseGame();
            else pauseMenu.ResumeGame();
        }
    }
}
