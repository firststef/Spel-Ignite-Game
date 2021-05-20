using System;
using UnityEngine;
using Newtonsoft.Json.Linq;
using Spells;
using System.Linq;
using System.Collections;
using Utils;
using System.Collections.Generic;

[RequireComponent(typeof(StatsController))]
public class SpelRuntime : MonoBehaviour
{
    private StatsController stats;
    private SpeechController speech;
    public bool vmIsRunning = false;
    public bool cancelling = false;

    private Dictionary<string, object> storage = new Dictionary<string, object>();

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

    private void SayError(string err)
    {
        speech.Speak("<color=red>" + err + "</color>");
    }

    private void Error(string err)
    {
        SayError(err);
        throw new Exception("VM error");
    }

    private void validate(JObject obj, string field, string value=null)
    {
        if (obj == null)
        {
            Error("Null found when searching field " + field + " with value " + value);
        }
        if (obj[field] == null)
        {
            Error("Field not found: " + field + " in "+ obj.ToString());
        }
        if (value != null && (string)obj[field] != value)
        {
            Error("Could not find " + field + " with value " + value + " in " + obj.ToString());
        }
    }

    private void unreachable()
    {
        Error("<color=red>Unreachable state in spel vm</color>");
    }

    private void unimplemented() 
    {
        Error("<color=red>This part has not been implemented yet</color>");
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
            case "PrintStatement":
                yield return VisitPrintStatementAsync(obj);
                yield break;
            case "CreateStatement":
                yield return VisitCreateStatementAsync(obj);
                yield break;
            case "MoveStatement":
                yield return VisitMoveStatementAsync(obj);
                yield break;
            case "ReleaseStatement":
                yield return VisitReleaseStatementAsync(obj);
                yield break;
            //case "Assignment":
            //    yield return VisitAssignStatementAsync(obj);
            //    yield break;
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
        if ((new[] { "playerHealth", "playerMana" }).Any(name.Equals))
        {
            return GetVirtualValue(name);
        }
        if (name == "orb")
        {
            return new MagicItem(name);
        }
        unimplemented();
        return null;
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

    //private IEnumerator VisitAnyStatementAsync(JObject obj)
    //{
    //    validate(obj, "value");
        
    //    if ((string)obj["value"] == "releaseFromHand")
    //    {
    //        foreach(DictionaryEntry de in stats.effects)
    //        {
    //            var ef = (string)de.Key;
    //            if (ef.EndsWith("_charged"))
    //            {
    //                var skill = CreateElementSkill(ef.Substring(0, ef.IndexOf("_charged")));
    //                yield return CallSkill((ICastSpell)skill);

    //                yield return new WaitForSeconds(0.5f);
    //                yield break;
    //            }
    //        }
    //    }
    //    unreachable();
    //}

    private IEnumerator VisitPrintStatementAsync(JObject obj)
    {
        validate(obj, "message");
        validate(obj, "tone");

        speech.Speak((string)obj["message"]);
        yield return new WaitForSeconds(0.05f);
    }

    private IEnumerator VisitCreateStatementAsync(JObject obj)
    {
        validate(obj, "object");
        validate(obj, "holder");

        var objInst = CreateObject((string)obj["object"]);
        if (objInst == null)
        {
            SayError($"Object ${(string)obj["object"]} could not be created");
            yield break;
        }
        StoreObject((string)obj["holder"], objInst);

        yield return new WaitForSeconds(0.15f);
    }

    private object CreateObject(string identifier)
    {
        switch (identifier)
        {
            case "fire":
            case "water":
            case "ice":
            case "earth":
                return new Element(identifier);
            case "orb":
            case "shield":
                return new MagicItem(identifier);
        }
        unimplemented();
        return null;
    }

    private void StoreObject(string whereTo, object obj)
    {
        if (!(new[] { "soul", "left hand", "right hand" }).Any(whereTo.Equals))
        {
            SayError($"{whereTo} does not exist as a place");
            return;
        }

        if (storage.ContainsKey(whereTo))
        {
            storage.Remove(whereTo);
        }
        storage.Add(whereTo, obj);
    }

    private IEnumerator VisitMoveStatementAsync(JObject obj)
    {
        validate(obj, "object");
        validate(obj, "to");

        var to = (string)obj["to"];
        bool found = false;
        var type = (string)obj["object"]["type"];
        if (type  == "NamedExpression")
        {
            foreach (KeyValuePair<string, object> x in storage)
            {
                if ((x.Value is Element && x.Value.ToString() == (string)obj["object"]["name"])||
                     (x.Value is MagicItem && x.Value.ToString() == (string)obj["object"]["name"]))
                {
                    found = true;
                    var val = x.Value;
                    storage.Remove(x.Key);
                    StoreObject(to, val);
                    break;
                }
            }
        }
        else if (type == "Modification")
        {
            var val = VisitModification((JObject)obj["object"]);
            if (val != null)
            {
                found = true;
                StoreObject(to, val);
            }
        }
        if (!found)
        {
            SayError("Could not find object to move");
            yield break;
        }

        yield return new WaitForSeconds(0.05f);
    }

    private IEnumerator VisitReleaseStatementAsync(JObject obj)
    {
        validate(obj, "from");

        var holder = (string)obj["from"];
        if (!storage.ContainsKey(holder))
        {
            SayError($"There is nothing in {holder}");
        }

        var rel = storage[holder];

        if ((new[] {"left hand", "right hand"}).Any(holder.Equals))
        {
            if (rel is Element)
            {
                var skill = CreateElementSkill(rel.ToString());
                yield return CallSkill(skill);
            }
            else if (rel is MagicItem)
            {
                var skill = CreateMagicItemSkill(rel.ToString());
                yield return CallSkill(skill);
            }
            else if (rel is ICastSpell)
            {
                yield return CallSkill(rel as ICastSpell);
            }
            else
            {
                SayError($"unknown skill for {holder}");
            }
        }
        if (holder == "soul")
        {
            if (rel is Element)
            {
                // if water make healing spell
            }
        }

        yield return new WaitForSeconds(0.05f);
    }

    private object VisitModification(JObject obj)
    {
        var toModify = (JObject)obj["expr"];
        var spell = VisitExpression(toModify); // check is of type

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

    private Skill CreateElementSkill(string skillName)
    {
        if (skillName == "water")
        {
            return new ElementSkill(skillName, stats, pfWater);
        }
        else if (skillName == "fire")
        {
            return new ElementSkill(skillName, stats, pfFire);
        }
        else if (skillName == "earth")
        {
            return new ElementSkill(skillName, stats, pfEarth);
        }
        else if (skillName == "ice")
        {
            return new ElementSkill(skillName, stats, pfIce);
        }
        else
        {
            Error("skill not found");
            return null;
        }
    }

    private Skill CreateMagicItemSkill(string skillName)
    {
        if (skillName == "orb")
        {
            return new RangedSkill("orb", stats, pfOrb, UtilsClass.GetMousePosition2D());
        }
        else
        {
            Error("skill not found");
            return null;
        }
    }

    private RangedSkill CreateRangedSkill(string type)
    {
        if (type == "fire")
        {
            return new RangedSkill("flames", stats, pfOrbFire, UtilsClass.GetMousePosition2D());
        }
        if (type == "water")
        {
            return new RangedSkill("splash", stats, pfOrbWater, UtilsClass.GetMousePosition2D());
        }
        if (type == "earth")
        {
            return new RangedSkill("rock", stats, pfOrbEarth, UtilsClass.GetMousePosition2D());
        }
        if (type == "ice")
        {
            return new RangedSkill("icicle", stats, pfOrbIce, UtilsClass.GetMousePosition2D());
        }
        throw new Exception("skill not found");
    }

    private object CreateSkillModified(JObject obj, object spell)
    {
        var skillName = (string)obj["name"];
        if (spell is MagicItem)
        {
            if (spell.ToString() == "orb")
            {
                return CreateRangedSkill(skillName);
            }
            else
            {
                unimplemented();
            }
        }
        else if (spell is RangedSkill)
        {
            var casted = (RangedSkill)spell;
            //if (skillName == "growth")
            //{
            //    return new CastOrbLarger(casted);
            //}
            //else if (skillName == "speed")
            //{
            //    return new CastOrbFaster(casted);
            //}
            unimplemented();
        }
        else
        {
            throw new Exception("skill modifier not found");
        }
        return spell;
    }

    //private IEnumerator VisitAssignStatementAsync(JObject obj)
    //{
    //    yield return new WaitForSeconds(0.05f);
    //}

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
            spell.applyOnCast(stats);
        }
        yield break;
    }

    [Header("Skill prefabs")]

    public GameObject pfWater;
    public GameObject pfFire;
    public GameObject pfEarth;
    public GameObject pfIce;

    public GameObject pfOrb;
    public GameObject pfOrbWater;
    public GameObject pfOrbFire;
    public GameObject pfOrbEarth;
    public GameObject pfOrbIce;

    //todo teoretic pot sa scap de corutine daca fac un Update care efectueaza actiuni puse de async-uri
}