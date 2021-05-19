using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
public class SpeechController : MonoBehaviour
{
    public Queue<string> lines = new Queue<string>();
    private Animator animator;
    private TextMeshPro tm;
    private GameObject eventCollider;

    public string currentLine;
    public float timeForEachLine = 3f;
    private float lineTime;

    private void Awake()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        animator = GetComponent<Animator>();
        tm = GetComponentInChildren<TextMeshPro>();
        eventCollider = transform.Find("EventCollider").gameObject;
        eventCollider.SetActive(false);
        lineTime = timeForEachLine;
    }

    void Update()
    {
        transform.GetChild(0).localRotation = transform.parent.rotation;

        if (lineTime >= 0)
        {
            lineTime -= Time.deltaTime;
        }

        if (!animator.GetBool("Enabled"))
        {
            if (lines.Count > 0)
            {
                ShowNextLine();
                animator.SetBool("Enabled", true);
            }
        }
        else
        {
            if (lineTime <= 0)
            {
                if (lines.Count > 0)
                {
                    StartCoroutine(ResetCollider());
                    ShowNextLine();
                }
                else
                {
                    animator.SetBool("Enabled", false);
                    currentLine = "";
                }
            }
        }
    }

    private void ShowNextLine()
    {
        var line = lines.Dequeue();
        currentLine = line;
        tm.text = line;
        lineTime = timeForEachLine;
    }

    private IEnumerator ResetCollider()
    {
        eventCollider.SetActive(false);
        yield return new WaitForSeconds(0.1f);
        eventCollider.SetActive(true);
    }

    public void Speak(string text)
    {
        lines.Enqueue(text);
    }
}
