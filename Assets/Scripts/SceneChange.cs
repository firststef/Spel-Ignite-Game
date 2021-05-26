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
            manager.pc.GetComponent<SpeechController>().Speak("I have to kill every enemy");
            return;
        }

        manager.GoToScene(nextScene, transform.Find("ReturnTo").position);
    }
}
