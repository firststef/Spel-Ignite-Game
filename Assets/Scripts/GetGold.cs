using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetGold : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        var pc = collision.gameObject.GetComponent<PlayerController>();
        if (pc)
        {
            pc.ChangeGold(1);
            Destroy(gameObject);
        }
    }
}
