using System;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Spells;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;

public class SpelRuntime : MonoBehaviour
{
    private PlayerController pc;

    private void Awake()
    {
        pc = GetComponent<PlayerController>();

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
        StartCoroutine(VisitBlockAsync((JObject)obj["block"]));
        yield break;
    }

    private IEnumerator VisitBlockAsync(JObject obj)
    {
        validate(obj, "type", "Block");
        foreach (var blockItem in (JArray)obj["items"])
        {
            StartCoroutine(VisitBlockItemAsync((JObject)blockItem));
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
            StartCoroutine(VisitStatementAsync((JObject)obj["statement"]));
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
            StartCoroutine(VisitWhileStatementAsync(obj));
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

        var expr = VisitNamedExpression((JObject)obj["expr"]);
        if (!(expr is int || expr is bool))
        {
            unimplemented();
        }

        while (true)
        {
            if ((expr is int && (int)expr == 0) ||
                (expr is bool && !(bool)expr))
            {
                break;
            }

            foreach (var stmt in (JArray)obj["stmts"])
            {
                yield return StartCoroutine(VisitStatementAsync((JObject)stmt));
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
        pc.CastSpell(spell);
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
        ICastElement spell;
        if ((string)callable["type"] == "NamedExpression")
        {
            spell = (ICastElement)VisitNamedExpression(callable);
        }
        else if ((string)callable["type"] == "Modification")
        {
            spell = (ICastElement)VisitModification(callable);
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
        return new CastElement(inst, pc.transform.position, pc.skillOffset);
    }

    private CastElementDecorator CreateSkillModified(JObject obj, ICastElement spell)
    {
        var skillName = (string)obj["name"];
        if (skillName == "growth")
        {
            return new CastElementLarger(spell);
        }
        else if (skillName == "speed")
        {
            return new CastElementFaster(spell);
        }
        else
        {
            throw new Exception("skill modifier not found");
        }
    }

    private object GetVirtualValue(string name)
    {
        if (name == "playerMana")
        {
            return pc.playerMana;
        }
        if (name == "playerHealth")
        {
            return pc.playerHealth;
        }
        unimplemented();
        return null;
    }

    /* Skills */

    [Header("Skill prefabs")]

    public Transform pfWater;
    public Transform pfFire;
    public Transform pfEarth;
}