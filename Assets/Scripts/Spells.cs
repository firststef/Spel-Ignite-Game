using UnityEngine;
using Utils;

namespace Spells
{
    public interface ICastSpell
    {
        public bool isWorthy(StatsController stats);
        public void cast();
        public void applyOnCast(StatsController stats);
        public void applyOnAttack(StatsController stats);
    }

    public class Skill: ICastSpell
    {
        public string name;
        public int manaCost = 1;

        public Skill(string name)
        {
            this.name = name;
        }

        public bool isWorthy(StatsController stats)
        {
            return stats.GetMP() >= manaCost;
        }

        public void cast()
        {}

        public void applyOnCast(StatsController stats)
        {
            if ((name == "fire" || name == "flames") && stats.effects.Contains("fire_charged"))
            {
                return;
            }
            stats.ConsumeMana(manaCost);
        }

        public void applyOnAttack(StatsController stats)
        {}
    }

    public class RangedSkill : Skill, ICastSpell
    {
        public StatsController caster;
        public GameObject prefab;

        public float moveSpeed = 3f;
        public float damage = 1f;
        public Vector3 scale;
        public Vector3 whereTo;

        public RangedSkill(string name)
            : base(name)
        { 
        }

        public RangedSkill(string name, StatsController caster, GameObject prefab, Vector3 whereTo)
            :base(name)
        {
            this.caster = caster;
            this.prefab = prefab;
            this.whereTo = whereTo;
            scale = prefab.transform.localScale;
        }

        public new void cast()
        {
            var startPosition = caster.transform.position;

            var direction = (whereTo - startPosition).normalized;
            var len = UtilsClass.HypotenuseLength(direction.x, direction.y);
            var factor = 1.0f / (len == 0 ? Mathf.Epsilon : len);
            direction = new Vector3(direction.x * factor, direction.y * factor, direction.z);

            var newRoot = new Vector3(startPosition.x + direction.x * caster.spellOffset, startPosition.y + direction.y * caster.spellOffset, startPosition.z);
            var obj = Object.Instantiate(prefab, newRoot, Quaternion.identity);
            obj.transform.localScale = scale;
            obj.GetComponent<Projectile>().Setup(direction, moveSpeed, this);

            caster.castCounter--;
        }

        public new virtual void applyOnAttack(StatsController stats)
        {
            stats.Damage(damage, name);
        }
    }

    public class Element
    {
        public string type;

        public Element(string el)
        {
            type = el;
        }

        public override string ToString()
        {
            return type;
        }
    }

    public class MagicItem
    {
        public string type;

        public MagicItem(string el)
        {
            type = el;
        }

        public override string ToString()
        {
            return type;
        }
    }

    public class Item
    {
        public string type;

        public Item(string el)
        {
            type = el;
        }

        public override string ToString()
        {
            return type;
        }
    }

    public class ElementSkill : Skill, ICastSpell
    {
        private GameObject prefab;
        private StatsController stats;
        public float damage = 1f;
        public ElementSkill(string name) 
            :base(name)
        {
        }

        public ElementSkill(string name, StatsController stats, GameObject prefab)
            :this(name)
        {
            this.prefab = prefab;
            this.stats = stats;
        }

        public new void cast()
        {
            var obj = Object.Instantiate(prefab, stats.transform);
            obj.GetComponent<ElementRelease>().Setup(stats, this);
        }

        public new void applyOnAttack(StatsController stats)
        {
            stats.Damage(damage, name);
        }
    }

    public class MorphSkill : RangedSkill
    {
        public GameObject morph;

        public MorphSkill(string name, StatsController caster, GameObject projectile, Vector3 whereTo, GameObject morph)
            : base(name, caster, projectile, whereTo)
        {
            this.morph = morph;
        }

        public override void applyOnAttack(StatsController stats)
        {
            var anim = stats.GetComponentInChildren<Animator>();
            anim.runtimeAnimatorController = this.morph.GetComponentInChildren<Animator>().runtimeAnimatorController;
            stats.AddEffect("morphed", null, 20f, () => stats.ClearEffect("morphed"));
            stats.Damage(damage, name);
        }
    }
}
