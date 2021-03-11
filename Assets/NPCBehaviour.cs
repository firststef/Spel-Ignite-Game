using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCBehaviour : MonoBehaviour
{
    public GameObject target;
    public GameObject panel;

    // Update is called once per frame
    void Update()
    {
        var le = Vector3.Distance(target.transform.position, transform.position);
        if (le < 2f)
        {
            panel.SetActive(true);
        }
        else
        {
            panel.SetActive(false);
        }
    }
}
