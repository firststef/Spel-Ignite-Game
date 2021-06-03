using Spells;
using UnityEngine;
using Utils;

public class ElementRelease : MonoBehaviour
{
    ElementSkill skill;
    StatsController st;
    float sustainManaCost = 3;
    float lastConsumeTime = 0;
    float timeTillConsume = 2f;

    public void Setup(StatsController stats, Spells.ElementSkill skill)
    {
        this.skill = skill;
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
            lastConsumeTime = 0f;
            float cost = sustainManaCost;
            if ((name == "fire" || name == "flames") && st.effects.Contains("fire_charged"))
            {
                cost = 0.5f;
            }

            st.ConsumeMana(cost);
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
            skill.applyOnAttack(stats);
        }
    }
}
