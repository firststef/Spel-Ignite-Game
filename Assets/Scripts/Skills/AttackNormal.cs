using UnityEngine;

public class AttackNormal : MonoBehaviour
{
    private StatsController st;
    public float damage = 1f;

    private void Awake()
    {
        st = GetComponentInParent<StatsController>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag != "Hurtbox")
        {
            return;
        }

        var stats = collision.gameObject.GetComponentInParent<StatsController>();
        if (stats && st.allegiance != stats.allegiance && st.GetInstanceID() != stats.GetInstanceID())
        {
            stats.Damage(damage);
        }
    }
}
