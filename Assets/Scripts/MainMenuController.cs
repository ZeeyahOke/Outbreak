using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {

    // Set these to match your actual scene file names (in Assets/Scenes).
    public string gameSceneName = "Gameplay";
    public string menuSceneName = "MainMenu";


    // Play button -> starts the game.
    public void PlayGame() {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
    }

    // Replay button (on the win/lose panels) -> restarts the game fresh.
    public void ReplayGame() {
        Time.timeScale = 1f;
        SceneManager.LoadScene(gameSceneName);
    }

    // Main Menu button (on the win/lose panels) -> back to the menu.
    public void GoToMainMenu() {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }

} // class
