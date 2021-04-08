using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirePuzzle : MonoBehaviour
{
    private void OnParticleCollision(GameObject other)
    {
        if (other.name.StartsWith("spray"))
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var stats = collision.collider.GetComponent<StatsController>();
        if (stats)
        {
            stats.Damage(3);
        }
    }
}
