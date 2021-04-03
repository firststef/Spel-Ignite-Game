using UnityEngine;
using Utils;

namespace Spells
{
    public interface ICastSpell
    {
        public bool isPlayerWorthy(PlayerController pc);
        public void cast();
        public void applyConsequences(PlayerController pc);
    }

    public interface ICastElement: ICastSpell
    {
        // to fill with other required functions for elemental skills
    }

    // todo remove all states from this and the classes should be functional => start from a filled struct or pass it etc
    public class CastElement: ICastElement
    {
        public Transform prefab;
        public Vector3 endPointPosition;
        public float skillOffset;
        public float moveSpeed = 3f;
        public int manaCost = 1;
        public Vector3 scale;

        public CastElement(Transform prefab, Vector3 endPointPosition, float skillOffset)
        {
            this.prefab = prefab;
            this.endPointPosition = endPointPosition;
            this.skillOffset = skillOffset;
            scale = prefab.localScale;
        }

        public bool isPlayerWorthy(PlayerController pc)
        {
            return pc.playerMana >= manaCost;
        }

        public void cast()
        {
            var shootPosition = UtilsClass.GetMousePosition2D();

            var direction = (shootPosition - endPointPosition).normalized;
            var len = UtilsClass.HypotenuseLength(direction.x, direction.y);
            var factor = 1.0f / (len == 0 ? Mathf.Epsilon : len);
            direction = new Vector3(direction.x * factor, direction.y * factor, direction.z);

            var newRoot = new Vector3(endPointPosition.x + direction.x * skillOffset, endPointPosition.y + direction.y * skillOffset, endPointPosition.z);
            var obj = Object.Instantiate(prefab, newRoot, Quaternion.identity);
            obj.localScale = scale;
            obj.GetComponent<OrbSkill>().Setup(direction, moveSpeed);
        }

        public void applyConsequences(PlayerController pc)
        {
            pc.ConsumeMana(manaCost);
        }
    }

    public class CastElementDecorator: ICastElement
    {
        public ICastSpell castElement;

        public CastElementDecorator(ICastElement cast)
        {
            castElement = cast;
        }

        public bool isPlayerWorthy(PlayerController pc)
        {
            return castElement.isPlayerWorthy(pc);
        }

        public virtual void cast()
        {
            castElement.cast();
        }

        public void applyConsequences(PlayerController pc)
        {
            castElement.applyConsequences(pc);
        }

        protected CastElement searchForOriginal()
        {
            var ce = castElement;
            while (!(ce is CastElement))
            {
                ce = ((CastElementDecorator)ce).castElement;
            }
            return (CastElement)ce;
        }
    }

    public class CastElementLarger: CastElementDecorator
    {
        public CastElementLarger(ICastElement cast)
            :base(cast)
        {
            var ce = searchForOriginal();
            ce.scale *= 2;
            ce.manaCost *= 1;
        }
    }

    public class CastElementFaster : CastElementDecorator
    {
        public CastElementFaster(ICastElement cast)
            : base(cast)
        {
            var ce = searchForOriginal();
            ce.moveSpeed *= 2;
            ce.manaCost *= 1;
        }
    }
}
