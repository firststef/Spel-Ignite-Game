using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCRotate : MonoBehaviour
{
    private Transform player;

    void Awake()
    {
        player = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void Update()
    {
        var we = player.position.x - transform.position.x;
        bool sign = we == 0 ? transform.eulerAngles.y == 180 || (player.position.x < transform.position.x) : we < 0;
        transform.rotation = Quaternion.Euler(new Vector3(transform.eulerAngles.x, sign ? 180 : 0, transform.eulerAngles.z));
    }
}
