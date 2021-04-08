using System;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Spells;
using System.Linq;
using System.Collections;
using Utils;
using System.Collections.Generic;

public class SpelRuntime : MonoBehaviour
{
    private PlayerController pc;
    public bool vmIsRunning = false;
    public bool cancelling = false;

    private void Awake()
    {
        pc = GetComponent<PlayerController>();
        pc.endSpell.AddListener(OnCancel);

#if !UNITY_EDITOR && UNITY_WEBGL
        WebGLInput.captureAllKeyboardInput = false;
#endif
    }

    public void Execute(string skillJson)
    {
        var json = JObject.Parse(skillJson);
        try
        {
            StartCoroutine(VisitDocumentAsync(json));
        }
        catch (SpelVMError e)
        {
            Debug.LogError(e);
        }
    }

    void OnCancel()
    {
        if (vmIsRunning)
        {
            cancelling = true;
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
        vmIsRunning = true;
        validate(obj, "type", "Document");
        cancelling = false;
        yield return VisitBlockAsync((JObject)obj["block"]);
        cancelling = false;
        vmIsRunning = false;
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
            yield return VisitCall(obj);
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

            if ( (expr is float && (float)expr == 0f) ||
                 (expr is bool && !(bool)expr) ||
                 cancelling
                )
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

    private IEnumerator VisitCall(JObject obj)
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
        yield return CallSkill(spell);
    }

    private object VisitNamedExpression(JObject obj)
    {
        var name = (string)obj["name"];
        if ((new[] { "fire", "water", "earth", "orb" }).Any(name.Contains))
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

    private Cast CreateSkill(JObject obj)
    {
        var skillName = (string)obj["name"];
        if (skillName == "water")
        {
            return new CastElement(skillName, pc, pfWater);
        }
        else if (skillName == "fire")
        {
            return new CastElement(skillName, pc, pfFire);
        }
        else if (skillName == "earth")
        {
            return new CastOrb("rock", pc, pfOrbEarth);
        }
        else
        {
            throw new Exception("skill not found");
        }
    }

    private CastOrb CreateOrbSkill(CastElement spell)
    {
        if (spell.name == "fire")
        {
            return new CastOrb("flames", pc, pfOrbFire);
        }
        if (spell.name == "water")
        {
            return new CastOrb("splash", pc, pfOrbWater);
        }
        if (spell.name == "earth")
        {
            return new CastOrb("rock", pc, pfOrbEarth);
        }
        throw new Exception("skill not found");
    }

    private ICastSpell CreateSkillModified(JObject obj, ICastSpell spell)
    {
        var skillName = (string)obj["name"];
        if (spell is CastElement)
        {
            if (skillName == "orb")
            {
                return CreateOrbSkill((CastElement)spell);
            }
        }
        if (spell is CastOrb)
        {
            var casted = (CastOrb)spell;
            if (skillName == "growth")
            {
                return new CastOrbLarger(casted);
            }
            else if (skillName == "speed")
            {
                return new CastOrbFaster(casted);
            }
        }
        //throw new Exception("skill modifier not found");
        return spell;
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

    private IEnumerator CallSkill(ICastSpell spell)
    {
        var stats = pc.GetComponent<StatsController>();
        if (spell.isWorthy(stats))
        {
            pc.castCounter++;
            yield return new WaitForSeconds(0.5f);
            spell.cast();
            spell.applyConsequences(stats);
        }
        yield break;
    }

    [Header("Skill prefabs")]

    public Transform pfWater;
    public Transform pfFire;
    public Transform pfEarth;

    public Transform pfOrbWater;
    public Transform pfOrbFire;
    public Transform pfOrbEarth;
}