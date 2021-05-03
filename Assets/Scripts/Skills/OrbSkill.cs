using Spells;
using UnityEngine;
using Utils;

public class OrbSkill : MonoBehaviour
{
    private Vector3 direction;
    private float moveSpeed = 0;
    private ICastSpell spell;

    public void Setup(Vector3 direction, float moveSpeed, ICastSpell spell)
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
        if (collision.gameObject.tag != "Hitbox")
        {
            return;
        }

        var stats = collision.gameObject.GetComponent<StatsController>();
        if (stats)
        {
            stats.Damage(1); // todo handled more correctly by watching skill type 
        }

        Destroy(gameObject);
    }
}
