using Spells;
using UnityEngine;
using Utils;

public class Projectile : MonoBehaviour
{
    private Vector3 direction;
    private RangedSkill skill;

    public void Setup(Vector3 direction, float moveSpeed, RangedSkill skill)
    {
        this.direction = direction;
        this.skill = skill;
        transform.eulerAngles = new Vector3(0, 0, UtilsClass.GetAngleFromVectorFloat(direction));
        Destroy(gameObject, 5f);
    }

    void Update()
    {
        transform.position += skill.moveSpeed * direction * Time.deltaTime;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag != "Hurtbox")
        {
            return;
        }

        var stats = collision.gameObject.GetComponentInParent<StatsController>();
        if (stats && skill.caster.allegiance != stats.allegiance && skill.caster.GetInstanceID() != stats.GetInstanceID())
        {
            skill.applyOnAttack(stats);
            Destroy(gameObject);
        }
    }
}
