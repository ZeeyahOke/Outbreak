using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {

    public string gameSceneName = "Gameplay";
    public string menuSceneName = "MainMenu";

  [Header("Fade")]
    public CanvasGroup fadeGroup;     
    public float fadeDuration = 0.5f;
 
    // Play button -> fade to black, then start the game.
    public void PlayGame() {
        StartCoroutine(FadeOutAndLoad(gameSceneName));
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

        // Fade the screen to black, then load the game scene.
    IEnumerator FadeOutAndLoad(string sceneName) {
        Time.timeScale = 1f;
        if (fadeGroup != null) {
            float t = 0f;
            while (t < fadeDuration) {
                t += Time.unscaledDeltaTime;
                fadeGroup.alpha = t / fadeDuration;
                yield return null;
            }
            fadeGroup.alpha = 1f;
        }
        SceneManager.LoadScene(sceneName);
    }

} // class
