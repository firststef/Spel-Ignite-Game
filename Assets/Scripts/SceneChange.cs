using UnityEngine;

public class SceneChange : MonoBehaviour
{
    public int nextScene = 0;
    private GameManager manager;

    private void Awake()
    {
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.name != "Player")
        {
            return;
        }
        if (GameObject.Find("Enemies").transform.childCount != 0)
        {
            var speech = manager.pc.GetComponentInChildren<SpeechController>();
            var line = "I have to kill every enemy";
            if (speech.currentLine != line)
            {
                speech.Speak(line);
            }
            return;
        }

        manager.GoToScene(nextScene, transform.Find("ReturnTo").position);
    }
}
