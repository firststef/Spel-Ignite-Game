using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackNormal : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag != "Hurtbox")
        {
            return;
        }

        var stats = collision.gameObject.GetComponentInParent<StatsController>();
        stats.Damage(1);
    }
}
