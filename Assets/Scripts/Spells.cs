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

        public PlayerController caster;
        public Transform prefab;

        public float moveSpeed = 3f;
        public Vector3 scale;

        public CastOrb(string name)
            : base(name)
        { 
        }

        public CastOrb(string name, PlayerController caster, Transform prefab)
            :base(name)
        {
            this.caster = caster;
            this.prefab = prefab;
            scale = prefab.localScale;
        }

        public void cast()
        {
            var startPosition = caster.transform.position;

            var direction = (UtilsClass.GetMousePosition2D() - startPosition).normalized;
            var len = UtilsClass.HypotenuseLength(direction.x, direction.y);
            var factor = 1.0f / (len == 0 ? Mathf.Epsilon : len);
            direction = new Vector3(direction.x * factor, direction.y * factor, direction.z);

            var newRoot = new Vector3(startPosition.x + direction.x * caster.spellOffset, startPosition.y + direction.y * caster.spellOffset, startPosition.z);
            var obj = Object.Instantiate(prefab, newRoot, Quaternion.identity);
            obj.localScale = scale;
            obj.GetComponent<OrbSkill>().Setup(direction, moveSpeed, this);

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
        private Transform prefab;
        private PlayerController pc;

        public CastElement(string name) 
            :base(name)
        {
        }

        public CastElement(string name, PlayerController pc, Transform prefab)
            :this(name)
        {
            this.prefab = prefab;
            this.pc = pc;
        }

        public void cast()
        {
            var obj = Object.Instantiate(prefab, pc.transform);
            obj.GetComponent<ElementSkill>().Setup(pc, this);
        }
    }
}
