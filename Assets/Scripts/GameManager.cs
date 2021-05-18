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
    private GameObject persist;

    private void Awake()
    {
        if (GameObject.Find("GlobalPersistence"))
        {
            Destroy(GameObject.Find("Persistence"));
            return;
        }

        pc = GameObject.Find("Player").GetComponent<PlayerController>();
        ui = GameObject.Find("UI");
        transition = GameObject.Find("Transition").GetComponent<Animator>();
        pause = GetComponent<PauseControl>();
        persist = GameObject.Find("Persistence");

        persist.name = "GlobalPersistence";
        pc.transform.position = GameObject.Find("PlayerStart").transform.position;
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

        DontDestroyOnLoad(persist);
        SceneManager.LoadScene(sceneIndex);
        yield return new WaitForSecondsRealtime(0.5f);
        SceneManager.MoveGameObjectToScene(persist, SceneManager.GetActiveScene());

        pc.transform.position = GameObject.Find("PlayerStart").transform.position;
        var cinema = GameObject.Find("Cinemachine").GetComponent<CinemachineVirtualCamera>();
        cinema.Follow = pc.transform;

        transition.SetTrigger("Enter");
        pause.StopTime(false);
    }
}
