using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IceBlockPuzzle : MonoBehaviour
{
    private int freezeCounter = 0;
    private SpriteRenderer sr;
    private BoxCollider2D box;
    static private GameManager gm;
    static private Transform playerStart;
    public Sprite frozen;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        box = GetComponent<BoxCollider2D>();
        if (gm == null) {
            gm = GameObject.Find("GameManager").GetComponent<GameManager>();
            playerStart = GameObject.Find("PlayerStart").transform;
        }
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.name.StartsWith("Ice"))
        {
            sr.sprite = frozen;
            box.enabled = false;
            StartCoroutine(Melt(++freezeCounter));
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name.StartsWith("Player"))
        {
            var index = SceneManager.GetActiveScene().buildIndex;
            gm.pc.GetComponent<StatsController>().Damage(1, "ice");
            if (!gm.lastPlayerPos.ContainsKey(index))
            {
                gm.pc.transform.position = playerStart.position;
            }
            else
            {
                gm.pc.transform.position = gm.lastPlayerPos[index];
            }
        }
    }

    private IEnumerator Melt(int freezeCount)
    {
        yield return new WaitForSeconds(3f);
        if (freezeCount == freezeCounter)
        {
            box.enabled = true;
            sr.sprite = null;   
        }
    }
}
