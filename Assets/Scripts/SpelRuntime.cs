using System;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Spells;
using System.Linq;
using System.Collections;
using Utils;

public class SpelRuntime : MonoBehaviour
{
    private PlayerController pc;
    private Animator anim;

    private void Awake()
    {
        pc = GetComponent<PlayerController>();
        anim = GetComponent<Animator>();

#if !UNITY_EDITOR && UNITY_WEBGL
        WebGLInput.captureAllKeyboardInput = false;
#endif
    }

    public void TriggerAction(string skillJson)
    {
        var json = JObject.Parse(skillJson);
        try
        {
            StartCoroutine("VisitDocumentAsync", json);
        }
        catch (SpelVMError e)
        {
            Debug.LogError(e);
        }
    }

    /* Virtual machine */

    class SpelVMError: Exception
    {
        public SpelVMError(string msg)
            :base(msg)
        {}
    }

    private void validate(JObject obj, string field, string value=null)
    {
        if (obj == null)
        {
            throw new SpelVMError("Null found when searching field " + field + " with value " + value);
        }
        if (obj[field] == null)
        {
            throw new SpelVMError("Field not found: " + field + " in "+ obj.ToString());
        }
        if (value != null && (string)obj[field] != value)
        {
            throw new SpelVMError("Could not find " + field + " with value " + value + " in " + obj.ToString());
        }
    }

    private void unreachable()
    {
        throw new Exception("Unreachable state in spel vm");
    }

    private void unimplemented()
    {
        throw new Exception("This part has not been implemented yet");
    }

    private IEnumerator VisitDocumentAsync(JObject obj)
    {
        validate(obj, "type", "Document");
        anim.SetBool("Attacking", true);
        yield return VisitBlockAsync((JObject)obj["block"]);
        anim.SetBool("Attacking", false);
        yield break;
    }

    private IEnumerator VisitBlockAsync(JObject obj)
    {
        validate(obj, "type", "Block");
        foreach (var blockItem in (JArray)obj["items"])
        {
            yield return VisitBlockItemAsync((JObject)blockItem);
        }
        yield break;
    }

    private IEnumerator VisitBlockItemAsync(JObject obj)
    {
        validate(obj, "type", "BlockItem");
        validate(obj, "which");
        if ((string)obj["which"] == "statement")
        {
            validate(obj, "statement");
            yield return VisitStatementAsync((JObject)obj["statement"]);
            yield break;
        }
        if ((string)obj["which"] == "declaration")
        {
            validate(obj, "declaration");
            unimplemented();
        }
        unreachable();
    }

    private IEnumerator VisitStatementAsync(JObject obj)
    {
        validate(obj, "type");
        if ((string)obj["type"] == "Call")
        {
            VisitCall(obj);
            yield break;
        }
        if ((string)obj["type"] == "WhileStatement")
        {
            yield return VisitWhileStatementAsync(obj);
            yield break;
        }
        if ((string)obj["type"] == "NoneStatement")
        {
            yield break;
        }
        unimplemented();
    }

    private IEnumerator VisitWhileStatementAsync(JObject obj)
    {
        validate(obj, "expr");
        validate(obj, "stmts");

        while (true)
        {
            var expr = VisitNamedExpression((JObject)obj["expr"]);
            if (!(expr is int || expr is float || expr is bool))
            {
                unimplemented();
            }

            Debug.Log((float)expr);

            if ((expr is float && (float)expr == 0f) ||
                (expr is bool && !(bool)expr))
            {
                Debug.Log("stopped");
                break;
            }

            foreach (var stmt in (JArray)obj["stmts"])
            {
                yield return VisitStatementAsync((JObject)stmt);
                yield return new WaitForSeconds(1f);
            }
        }
    }

    private void VisitCall(JObject obj)
    {
        validate(obj, "type", "Call");
        
        var callable = (JObject)obj["expr"];
        // var prms = (object)obj["params"];
        ICastSpell spell;
        if ((string)callable["type"] == "NamedExpression")
        {
            spell = (ICastSpell)VisitNamedExpression(callable);
        }
        else if ((string)callable["type"] == "Modification")
        {
            spell = (ICastSpell)VisitModification(callable);
        }
        else
        {
            throw new Exception("Call type not found");
        }

        // Player casts
        CallSkill(spell);
    }

    private object VisitNamedExpression(JObject obj)
    {
        var name = (string)obj["name"];
        if ((new[] { "fire", "water", "earth" }).Any(name.Contains))
        {
            return CreateSkill(obj);
        }
        if ((new[] { "playerHealth", "playerMana" }).Any(name.Contains))
        {
            return GetVirtualValue(name);
        }
        unimplemented();
        return null;
    }

    private object VisitModification(JObject obj)
    {
        var callable = (JObject)obj["expr"];
        ICastSpell spell;
        if ((string)callable["type"] == "NamedExpression")
        {
            spell = (ICastSpell)VisitNamedExpression(callable);
        }
        else if ((string)callable["type"] == "Modification")
        {
            spell = (ICastSpell)VisitModification(callable);
        }
        else
        {
            unimplemented();
            return null;
        }

        var cmodify = (JObject)obj["value"];
        if ((string)cmodify["type"] == "NamedExpression")
        {
            return CreateSkillModified(cmodify, spell);
        }
        else
        {
            unimplemented();
            return null;
        }
    }

    private CastElement CreateSkill(JObject obj)
    {
        Transform inst = null;
        var skillName = (string)obj["name"];
        if (skillName == "water")
        {
            inst = pfWater;
        }
        else if (skillName == "fire")
        {
            inst = pfFire;
        }
        else if (skillName == "earth")
        {
            inst = pfEarth;
        }
        else
        {
            throw new Exception("skill not found");
        }
        return new CastElement(pc.transform, inst);
    }

    private CastElementDecorator CreateSkillModified(JObject obj, ICastSpell spell)
    {
        var skillName = (string)obj["name"];
        if (spell is CastElement)
        {
            var casted = (CastElement)spell;
            if (skillName == "growth")
            {
                return new CastElementLarger(casted);
            }
            else if (skillName == "speed")
            {
                return new CastElementFaster(casted);
            }
        }
        throw new Exception("skill modifier not found");
    }

    private object GetVirtualValue(string name)
    {
        if (name == "playerMana")
        {
            return pc.GetComponent<StatsController>().GetMP();
        }
        if (name == "playerHealth")
        {
            return pc.GetComponent<StatsController>().GetHP();
        }
        unimplemented();
        return null;
    }

    /* Skills */

    private void CallSkill(ICastSpell spell)
    {
        var stats = pc.GetComponent<StatsController>();
        if (spell.isWorthy(stats))
        {
            pc.Cast(spell);
            spell.applyConsequences(stats);
        }
    }

    [Header("Skill prefabs")]

    public Transform pfWater;
    public Transform pfFire;
    public Transform pfEarth;
}