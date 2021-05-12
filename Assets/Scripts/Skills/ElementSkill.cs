using Spells;
using UnityEngine;
using Utils;

public class ElementSkill : MonoBehaviour
{
    CastElement spell;
    StatsController stats;
    float sustainManaCost = 2;
    float lastConsumeTime = 0;
    float timeTillConsume = 3f;

    public void Setup(StatsController stats, CastElement spell)
    {
        this.spell = spell;
        this.stats = stats;
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
            stats.ConsumeMana(sustainManaCost);
            if (stats.GetMP() < 0)
            {
                Die();
            }
        }

        //transform.rotation = Quaternion.Euler(direction);
    }

    private void Die()
    {
        stats.castCounter--;
        Destroy(gameObject);
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.tag != "Hurtbox")
        {
            return;
        }

        var stats = other.GetComponentInParent<StatsController>();
        if (stats)
        {
            stats.Damage(1); // todo handled more correctly by watching skill type 
        }
    }
}
