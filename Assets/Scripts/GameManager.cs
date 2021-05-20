using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    private GameObject ui;
    private PlayerController pc;
    private Animator transition;
    private PauseControl pause;
    private GameObject persist;
    public GameObject shop;
    private int shopIndex = 0;
    private Dictionary<int, Vector3> lastPlayerPos = new Dictionary<int, Vector3>();

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

    public void GoToScene(int sceneIndex, Vector3 returnTo)
    {
        StartCoroutine(SceneCoroutine(sceneIndex, returnTo));
    }

    public IEnumerator SceneCoroutine(int sceneIndex, Vector3 returnTo)
    {
        lastPlayerPos[SceneManager.GetActiveScene().buildIndex] = returnTo;

        pause.StopTime(true);
        transition.SetTrigger("Exit");
        yield return new WaitForSecondsRealtime(1f);

        DontDestroyOnLoad(persist);
        SceneManager.LoadScene(sceneIndex);
        yield return new WaitForSecondsRealtime(0.5f);
        SceneManager.MoveGameObjectToScene(persist, SceneManager.GetActiveScene());

        if (!lastPlayerPos.ContainsKey(sceneIndex))
        {
            pc.transform.position = GameObject.Find("PlayerStart").transform.position;
        }
        else
        {
            pc.transform.position = lastPlayerPos[sceneIndex];
        }
        var cinema = GameObject.Find("Cinemachine").GetComponent<CinemachineVirtualCamera>();
        cinema.Follow = pc.transform;

        transition.SetTrigger("Enter");
        pause.StopTime(false);
    }

    public void OpenShop()
    {
        shop.SetActive(true);
    }

    public void ShopPrev()
    {
        var items = shop.transform.Find("Items");
        var count = items.childCount;
        shopIndex--;
        if (count != 0)
        {
            shopIndex = shopIndex % count;
            for (int i = 0; i < count; i++)
            {
                items.GetChild(i).gameObject.SetActive(i == shopIndex);
            }
        }
        else
        {
            shopIndex = 0;
        }
    }

    public void ShopNext()
    {
        var items = shop.transform.Find("Items");
        var count = items.childCount;
        shopIndex++;
        if (count != 0)
        {
            shopIndex = shopIndex % count;
            for (int i = 0; i < count; i++)
            {
                items.GetChild(i).gameObject.SetActive(i == shopIndex);
            }
        }
        else
        {
            shopIndex = 0;
        }
    }

    public void ShopExit()
    {
        shop.SetActive(false);
    }

    public void Buy(ShopItem item)
    {
        if (pc.GetGold() >= item.price)
        {
            pc.ChangeGold(-item.price);
            pc.inventory.Remove(item.consume);
            pc.inventory.AddRange(item.receive);
            Destroy(item.gameObject);
            ShopPrev();
        }
    }
}
