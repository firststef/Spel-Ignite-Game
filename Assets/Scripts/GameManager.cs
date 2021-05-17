using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    private GameObject ui;
    private PlayerController pc;
    private Animator transition;
    private PauseControl pause;
    private CinemachineConfiner cinema;

    private void Awake()
    {
        pc = GameObject.Find("Player").GetComponent<PlayerController>();
        ui = GameObject.Find("UI");
        cinema = GameObject.Find("Cinemachine").GetComponent<CinemachineConfiner>();
        transition = GameObject.Find("Transition").GetComponent<Animator>();
        pause = GetComponent<PauseControl>();

        DontDestroyOnLoad(transform.parent.gameObject);
    }

    public void GoToScene(int sceneIndex)
    {
        StartCoroutine(SceneCoroutine(sceneIndex));
    }

    public IEnumerator SceneCoroutine(int sceneIndex)
    {
        pause.StopTime(true);
        transition.SetTrigger("Exit");
        yield return new WaitForSecondsRealtime(1f);
        SceneManager.LoadScene(sceneIndex);
        yield return new WaitForSecondsRealtime(1f);
        pc.transform.position = GameObject.Find("PlayerStart").transform.position;
        cinema.m_BoundingShape2D = GameObject.Find("Map").GetComponent<PolygonCollider2D>();
        transition.SetTrigger("Enter");
        pause.StopTime(false);
    }
}
