using System;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Spells;
using System.Linq;
using System.Collections;
using Utils;

[RequireComponent(typeof(StatsController))]
public class SpelRuntime : MonoBehaviour
{
    private StatsController stats;
    private SpeechController speech;
    public bool vmIsRunning = false;
    public bool cancelling = false;

    private void Awake()
    {
        stats = GetComponent<StatsController>();
        speech = stats.GetComponentInChildren<SpeechController>();
        stats.endSpell.AddListener(OnCancel);

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
        validate(obj, "type", "Document");
        vmIsRunning = true;
        cancelling = false;
        yield return VisitBlockAsync((JObject)obj["block"]);
        cancelling = false;
        vmIsRunning = false;
    }

    private IEnumerator VisitBlockAsync(JObject obj)
    {
        validate(obj, "type", "Block");
        foreach (var blockItem in (JArray)obj["items"])
        {
            yield return VisitBlockItemAsync((JObject)blockItem);
        }
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
        switch ((string)obj["type"])
        {
            case "Call":
                yield return VisitCall(obj);
                yield break;
            case "WhileStatement":
                yield return VisitWhileStatementAsync(obj);
                yield break;
            case "NoneStatement":
                yield break;
            case "AnyStatement":
                yield return VisitAnyStatementAsync(obj);
                yield break;
            case "ChargeStatement":
                yield return VisitChargeStatementAsync(obj);
                yield break;
            case "PrintStatement":
                yield return VisitPrintStatementAsync(obj);
                yield break;
            case "CreateStatement":
                yield return VisitCreateStatementAsync(obj);
                yield break;
            case "Assignment":
                yield return VisitAssignStatementAsync(obj);
                yield break;
            default:
                unimplemented();
                yield break;
        }
    }

    private IEnumerator VisitCall(JObject obj)
    {
        validate(obj, "type", "Call");
        
        var callable = (JObject)obj["expr"];
        // var prms = (object)obj["params"];
        ICastSpell spell = (ICastSpell)VisitExpression(callable); //check is of type

        // Player casts
        yield return CallSkill(spell);
    }

    private object VisitExpression(JObject obj)
    {
        switch ((string)obj["type"])
        {
            case "NamedExpression":
                return VisitNamedExpression(obj);
            case "Modification":
                return VisitModification(obj);
            default:
                unreachable();
                break;
        }
        return null;
    }

    private object VisitNamedExpression(JObject obj)
    {
        var name = (string)obj["name"];
        if ((new[] { "playerHealth", "playerMana" }).Any(name.Contains))
        {
            return GetVirtualValue(name);
        }
        //if (name == "orb")
        //{
        //    return Cre
        //}
        unimplemented();
        return null;
    }

    private object VisitModification(JObject obj)
    {
        var callable = (JObject)obj["expr"];
        ICastSpell spell = (ICastSpell)VisitExpression(callable); // check is of type

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

    private Cast CreateElementSkill(string skillName)
    {
        if (skillName == "water")
        {
            return new CastElement(skillName, stats, pfWater);
        }
        else if (skillName == "fire")
        {
            return new CastElement(skillName, stats, pfFire);
        }
        else if (skillName == "earth")
        {
            return new CastElement(skillName, stats, pfEarth);
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
            return new CastOrb("flames", stats, pfOrbFire, UtilsClass.GetMousePosition2D());
        }
        if (spell.name == "water")
        {
            return new CastOrb("splash", stats, pfOrbWater, UtilsClass.GetMousePosition2D());
        }
        if (spell.name == "earth")
        {
            return new CastOrb("rock", stats, pfOrbEarth, UtilsClass.GetMousePosition2D());
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
        else if (spell is CastOrb)
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
        else
        {
            throw new Exception("skill modifier not found");
        }
        return spell;
    }

    private IEnumerator VisitWhileStatementAsync(JObject obj)
    {
        validate(obj, "expr");
        validate(obj, "stmts");

        while (true)
        {
            var expr = VisitExpression((JObject)obj["expr"]);
            if (!(expr is int || expr is float || expr is bool))
            {
                unimplemented();
            }

            if ((expr is float && (float)expr == 0f) ||
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
                yield return new WaitForSeconds(0.2f);
            }

            yield return new WaitForSeconds(0.3f);
        }
    }

    private IEnumerator VisitAnyStatementAsync(JObject obj)
    {
        validate(obj, "value");
        
        if ((string)obj["value"] == "releaseFromHand")
        {
            foreach(DictionaryEntry de in stats.effects)
            {
                var ef = (string)de.Key;
                if (ef.EndsWith("_charged"))
                {
                    var skill = CreateElementSkill(ef.Substring(0, ef.IndexOf("_charged")));
                    yield return CallSkill((ICastSpell)skill);

                    yield return new WaitForSeconds(0.5f);
                    yield break;
                }
            }
        }
        unreachable();
    }

    private IEnumerator VisitChargeStatementAsync(JObject obj)
    {
        validate(obj, "element");

        string effect = (string)obj["element"] + "_charged";
        stats.AddEffect(effect, 0, 5f, () => stats.ClearEffect(effect));
        yield return new WaitForSeconds(1f);
    }

    private IEnumerator VisitPrintStatementAsync(JObject obj)
    {
        validate(obj, "message");
        validate(obj, "tone");

        speech.Speak((string)obj["message"]);
        yield return new WaitForSeconds(0.05f);
    }

    private IEnumerator VisitCreateStatementAsync(JObject obj)
    {
        
        yield return new WaitForSeconds(0.05f);
    }

    private IEnumerator VisitAssignStatementAsync(JObject obj)
    {

        yield return new WaitForSeconds(0.05f);
    }

    private object GetVirtualValue(string name)
    {
        if (name == "playerMana")
        {
            return stats.GetMP(); // todo playerMana => mana
        }
        if (name == "playerHealth")
        {
            return stats.GetComponent<StatsController>().GetHP();
        }
        unimplemented();
        return null;
    }

    /* Skills */

    private IEnumerator CallSkill(ICastSpell spell)
    {
        if (spell.isWorthy(stats))
        {
            stats.castCounter++;
            yield return new WaitForSeconds(0.5f);
            spell.cast();
            spell.applyConsequences(stats);
        }
        yield break;
    }

    [Header("Skill prefabs")]

    public GameObject pfWater;
    public GameObject pfFire;
    public GameObject pfEarth;

    public GameObject pfOrbWater;
    public GameObject pfOrbFire;
    public GameObject pfOrbEarth;

    //todo teoretic pot sa scap de corutine daca fac un Update care efectueaza actiuni puse de async-uri
}