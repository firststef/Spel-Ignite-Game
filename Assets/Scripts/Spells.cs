using UnityEngine;
using Utils;

namespace Spells
{
    public interface ICastSpell
    {
        public bool isWorthy(StatsController stats);
        public void cast(Vector3 shootTo, float skillOffset);
        public void applyConsequences(StatsController stats);
    }

    public class CastElement : ICastSpell
    {
        public Transform caster;
        public Transform prefab;
        public float moveSpeed = 3f;
        public int manaCost = 1;
        public Vector3 scale;

        public CastElement() {}

        public CastElement(Transform caster, Transform prefab)
        {
            this.caster = caster;
            this.prefab = prefab;
            scale = prefab.localScale;
        }

        public bool isWorthy(StatsController stats)
        {
            return stats.GetMP() >= manaCost;
        }

        public void cast(Vector3 shootTo, float skillOffset)
        {
            var startPosition = caster.position;

            var direction = (shootTo - startPosition).normalized;
            var len = UtilsClass.HypotenuseLength(direction.x, direction.y);
            var factor = 1.0f / (len == 0 ? Mathf.Epsilon : len);
            direction = new Vector3(direction.x * factor, direction.y * factor, direction.z);

            var newRoot = new Vector3(startPosition.x + direction.x * skillOffset, startPosition.y + direction.y * skillOffset, startPosition.z);
            var obj = Object.Instantiate(prefab, newRoot, Quaternion.identity);
            obj.localScale = scale;
            obj.GetComponent<OrbSkill>().Setup(direction, moveSpeed, this);
        }

        public void applyConsequences(StatsController stats)
        {
            stats.ConsumeMana(manaCost);
        }
    }

    public class CastElementDecorator: ICastSpell
    {
        public CastElement castElement;

        public CastElementDecorator(CastElement cast)
        {
            castElement = cast;
        }

        public bool isWorthy(StatsController stats)
        {
            return castElement.isWorthy(stats);
        }

        public void cast(Vector3 shootTo, float skillOffset)
        {
            castElement.cast(shootTo, skillOffset);
        }

        public void applyConsequences(StatsController stats)
        {
            castElement.applyConsequences(stats);
        }
    }

    public class CastElementLarger: CastElementDecorator
    {
        public CastElementLarger(CastElement cast)
            :base(cast)
        {
            castElement.scale *= 2;
            castElement.manaCost *= 1;
        }
    }

    public class CastElementFaster : CastElementDecorator
    {
        public CastElementFaster(CastElement cast)
            : base(cast)
        {
            castElement.moveSpeed *= 2;
            castElement.manaCost *= 1;
        }
    }
}
