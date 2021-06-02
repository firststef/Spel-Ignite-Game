using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class S_Information : MonoBehaviour
{
    public List<string> informations = new List<string>();
    private SpeechController speech;

    private void Awake()
    {
        speech = GetComponentInChildren<SpeechController>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.GetComponent<PlayerController>() && speech.currentLine == "")
        {
            foreach(string s in informations)
            {
                speech.Speak(s);
            }
        }
    }
}
