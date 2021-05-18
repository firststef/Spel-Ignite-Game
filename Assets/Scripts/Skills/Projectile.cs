using Spells;
using UnityEngine;
using Utils;

public class Projectile : MonoBehaviour
{
    private Vector3 direction;
    private float moveSpeed = 0;
    private CastOrb spell;

    public void Setup(Vector3 direction, float moveSpeed, CastOrb spell)
    {
        this.direction = direction;
        this.moveSpeed = moveSpeed;
        this.spell = spell;
        transform.eulerAngles = new Vector3(0, 0, UtilsClass.GetAngleFromVectorFloat(direction));
        Destroy(gameObject, 5f);
    }

    private void Update()
    {
        transform.position += moveSpeed * direction * Time.deltaTime;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag != "Hurtbox")
        {
            return;
        }

        var stats = collision.gameObject.GetComponentInParent<StatsController>();
        if (stats && spell.caster.allegiance != stats.allegiance && spell.caster.GetInstanceID() != stats.GetInstanceID())
        {
            stats.Damage(spell.damage); // todo handled more correctly by watching skill type 
            Destroy(gameObject);
        }
    }
}
