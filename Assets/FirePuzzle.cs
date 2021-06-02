using UnityEngine;

public class FirePuzzle : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        CheckDeath(collision.gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        CheckDeath(collision.gameObject);
    }

    private void OnParticleCollision(GameObject other)
    {
        CheckDeath(other);
    }

    private void CheckDeath(GameObject collider)
    {
        if (collider.name.StartsWith("Water") || collider.name.StartsWith("Splash"))
        {
            Destroy(transform.parent.gameObject, 1.3f);
        }
    }
}
