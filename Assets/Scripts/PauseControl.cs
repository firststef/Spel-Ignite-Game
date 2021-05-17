using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using System;

public class PauseControl : MonoBehaviour
{
    public static bool gameIsPaused = false;

    public Toggle toggle;
    public GameObject pauseOverlay;

    private bool isKeyPressed = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space)) {
            if (!isKeyPressed)
            {
                isKeyPressed = true;
                TogglePause("own", !gameIsPaused);
            }
        }
        else if (isKeyPressed)
        {
            isKeyPressed = false;
        }
    }

    public void TogglePause(string origin, bool paused)
    {
        if (gameIsPaused != paused)
        {
            if (origin != "button")
            {
                toggle.SetIsOnWithoutNotify(paused);
            }

#if !UNITY_EDITOR && UNITY_WEBGL
            if (paused) {
                GamePaused();
            } else {
                GameUnpaused();
            }
#endif

            StopTime(paused);
            pauseOverlay.SetActive(paused);            
            gameIsPaused = paused;
        }
    }

    public void StopTime(bool enabled)
    {
        Time.timeScale = enabled ? 0f : 1;
    }

    public void ToggleFromButton(Toggle change)
    {
        TogglePause("button", change.isOn);
    }

    public void PlayFromEditor()
    {
        TogglePause("editor", false);
    }

    [DllImport("__Internal")]
    private static extern void GamePaused();
    [DllImport("__Internal")]
    private static extern void GameUnpaused();
}
