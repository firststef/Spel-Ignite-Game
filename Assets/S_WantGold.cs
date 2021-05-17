using UnityEngine;

public class S_WantGold : MonoBehaviour
{
    private SpeechController speech;

    private void Awake()
    {
        speech = GetComponentInChildren <SpeechController>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        speech.Speak("wow");
    }
}
