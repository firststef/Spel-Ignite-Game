using UnityEngine;

public class S_OpenShop : MonoBehaviour
{
    private SpeechController speech;
    private GameManager gm;

    private void Awake()
    {
        speech = GetComponentInChildren<SpeechController>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.GetComponent<PlayerController>() && speech.currentLine == "")
        {
            speech.Speak("Hi!");
            speech.Speak("You should check out my shop!");
            speech.Speak("Just let me know, like saying `shop`");
        }
        var otherSpeech = collider.GetComponentInParent<SpeechController>();
        if (otherSpeech)
        {
            if (otherSpeech.currentLine == "shop")
            {
                gm.OpenShop();
            }
        }
    }
}
