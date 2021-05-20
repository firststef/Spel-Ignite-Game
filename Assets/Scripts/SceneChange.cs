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

        manager.GoToScene(nextScene, transform.Find("ReturnTo").position);
    }
}
