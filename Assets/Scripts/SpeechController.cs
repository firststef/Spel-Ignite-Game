using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SpeechController : MonoBehaviour
{
    private Queue<string> lines = new Queue<string>();
    private Animator animator;
    private TextMeshPro tm;

    public float timeForEachLine = 3f;
    private float lineTime;

    private void Awake()
    {
        transform.GetChild(0).gameObject.SetActive(true);
        animator = GetComponent<Animator>();
        tm = GetComponentInChildren<TextMeshPro>();
        lineTime = timeForEachLine;
    }

    void Update()
    {
        transform.GetChild(0).localRotation = transform.parent.rotation;

        if (lineTime > 0)
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
            if (lineTime < 0)
            {
                if (lines.Count > 0)
                {
                    ShowNextLine();
                }
                else
                {
                    animator.SetBool("Enabled", false);
                }
            }
        }
    }

    private void ShowNextLine()
    {
        var line = lines.Dequeue();
        tm.text = line;
        lineTime = timeForEachLine;
    }

    public void Speak(string text)
    {
        lines.Enqueue(text);
    }
}
