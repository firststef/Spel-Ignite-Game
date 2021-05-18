using Spells;
using UnityEngine;
using Utils;

public class ElementSkill : MonoBehaviour
{
    CastElement spell;
    StatsController st;
    float sustainManaCost = 2;
    float lastConsumeTime = 0;
    float timeTillConsume = 3f;

    public void Setup(StatsController stats, CastElement spell)
    {
        this.spell = spell;
        st = stats;
        stats.endSpell.AddListener(Die);
    }

    private void Update()
    {
        var direction = (UtilsClass.GetMousePosition2D() - transform.position).normalized;
        var len = UtilsClass.HypotenuseLength(direction.x, direction.y);
        var factor = 1.0f / (len == 0 ? Mathf.Epsilon : len);
        direction = new Vector3(direction.x * factor, direction.y * factor, direction.z);

        transform.rotation = Quaternion.LookRotation(direction);

        lastConsumeTime += Time.deltaTime;
        if (lastConsumeTime > timeTillConsume)
        {
            lastConsumeTime = 0;
            st.ConsumeMana(sustainManaCost);
            if (st.GetMP() < 0)
            {
                Die();
            }
        }

        //transform.rotation = Quaternion.Euler(direction);
    }

    private void Die()
    {
        st.castCounter--;
        Destroy(gameObject);
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.tag != "Hurtbox")
        {
            return;
        }

        var stats = other.GetComponentInParent<StatsController>();
        if (stats && st.allegiance != stats.allegiance && st.GetInstanceID() != stats.GetInstanceID())
        {
            stats.Damage(spell.damage); // todo handled more correctly by watching skill type 
        }
    }
}
