using System.Collections.Generic;
using UnityEngine;

public class S_WantGold : MonoBehaviour
{
    private SpeechController speech;
    private GameManager gm;
    private bool alreadyGiven = false;

    private void Awake()
    {
        speech = GetComponentInChildren<SpeechController>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.GetComponent<PlayerController>() && speech.currentLine == "")
        {
            speech.Speak("Want some gold?");
        }
        var otherSpeech = collider.GetComponentInParent<SpeechController>();
        if (otherSpeech)
        {
            if (otherSpeech.currentLine == "yes")
            {
                if (!alreadyGiven)
                {
                    gm.pc.ChangeGold(2);
                    alreadyGiven = true;
                }
                else
                {
                    speech.Speak("Wait but i just gave you");
                    speech.Speak("Thief!");
                }
            }
        }
    }
}
