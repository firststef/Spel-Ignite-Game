using UnityEngine;
using Utils;

namespace Spells
{
    public interface ICastSpell
    {
        public bool isWorthy(StatsController stats);
        public void cast();
        public void applyConsequences(StatsController stats);
    }

    public class Cast
    {
        public string name;
        public int manaCost = 1;

        public Cast(string name)
        {
            this.name = name;
        }

        public bool isWorthy(StatsController stats)
        {
            return stats.GetMP() >= manaCost;
        }

        public void applyConsequences(StatsController stats)
        {
            stats.ConsumeMana(manaCost);
        }
    }

    public class CastOrb : Cast, ICastSpell
    {

        public StatsController caster;
        public GameObject prefab;

        public float moveSpeed = 3f;
        public float damage = 1f;
        public Vector3 scale;
        public Vector3 whereTo;

        public CastOrb(string name)
            : base(name)
        { 
        }

        public CastOrb(string name, StatsController caster, GameObject prefab, Vector3 whereTo, float damage)
            :base(name)
        {
            this.caster = caster;
            this.prefab = prefab;
            this.whereTo = whereTo;
            this.damage = damage;
            scale = prefab.transform.localScale;
        }

        public void cast()
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
    }

    public class CastOrbDecorator : ICastSpell
    {
        public CastOrb castOrb;

        public CastOrbDecorator(CastOrb cast)
        {
            castOrb = cast;
        }

        public bool isWorthy(StatsController stats)
        {
            return castOrb.isWorthy(stats);
        }

        public void cast()
        {
            castOrb.cast();
        }

        public void applyConsequences(StatsController stats)
        {
            castOrb.applyConsequences(stats);
        }
    }

    public class CastOrbLarger : CastOrbDecorator
    {
        public CastOrbLarger(CastOrb cast)
            : base(cast)
        {
            castOrb.scale *= 2;
            castOrb.manaCost *= 1;
        }
    }

    public class CastOrbFaster : CastOrbDecorator
    {
        public CastOrbFaster(CastOrb cast)
            : base(cast)
        {
            castOrb.moveSpeed *= 2;
            castOrb.manaCost *= 1;
        }
    }

    public class CastElement : Cast, ICastSpell
    {
        private GameObject prefab;
        private StatsController stats;
        public float damage = 1f;
        public CastElement(string name) 
            :base(name)
        {
        }

        public CastElement(string name, StatsController stats, GameObject prefab)
            :this(name)
        {
            this.prefab = prefab;
            this.stats = stats;
        }

        public void cast()
        {
            var obj = Object.Instantiate(prefab, stats.transform);
            obj.GetComponent<ElementSkill>().Setup(stats, this);
        }
    }
}
