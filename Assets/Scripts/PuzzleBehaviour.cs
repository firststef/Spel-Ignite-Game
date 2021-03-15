using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PuzzleBehaviour : MonoBehaviour
{
    public GameObject npc;
    private List<string> last = new List<string>();

    // Update is called once per frame
    private void OnCollisionEnter2D(Collision2D collision)
    {
        int i = 0;
        last.Add(collision.gameObject.name);
        while (i < last.Count)
        {
            switch (i)
            {
                case 0:
                    if (!last[i].StartsWith("Earth"))
                    {
                        last.Clear();
                    }
                    break;
                case 1:
                    if (!last[i].StartsWith("Fire"))
                    {
                        last.Clear();
                    }
                    break;
                case 2:
                    if (!last[i].StartsWith("Water"))
                    {
                        last.Clear();
                    }
                    else
                    {
                        Destroy(gameObject);
                        Destroy(npc);
                    }
                    break;
            }
            i++;
        }
    }
}
