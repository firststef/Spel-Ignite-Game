using UnityEngine;
using UnityEngine.UI;

public class PauseControl : MonoBehaviour
{
    public static bool gameIsPaused = false;
    private static bool changed = false;

    public Toggle toggle;
    public GameObject pauseOverlay;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            toggle.isOn = !toggle.isOn;
        }

        if (changed)
        {
            Time.timeScale = gameIsPaused ? 0f : 1;
            pauseOverlay.SetActive(!pauseOverlay.activeSelf);
            changed = false;
        }
    }

    public void TogglePaused()
    {
        gameIsPaused = !gameIsPaused;
        changed = true;
    }
}
