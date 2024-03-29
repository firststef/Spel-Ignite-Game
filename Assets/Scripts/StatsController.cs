using Spells;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Specialized;
using System.Collections.Generic;

public class StatsController : MonoBehaviour
{
    public float maxHP = 10f;
    private float currentHP;

    public float recoveryHP = 0f;
    public float recoverHPTime = 30f;
    private float elapsedHP = 0f;

    public float maxMP = 10f;
    private float currentMP;

    public float recoveryMP = 0.5f;
    public float recoverMPTime = 30f;
    private float elapsedMP = 0f;

    public float maxDamage = 1f;
    private float damage;

    public float maxSpeed = 1f;
    private float currentSpeed;

    public float spellOffset = 0.8f;
    [HideInInspector]
    public int castCounter = 0;
    [HideInInspector]
    public UnityEvent endSpell = new UnityEvent();

    private bool dead = false;
    private bool isInvincible = false;

    [HideInInspector]
    public UnityEvent mpChanged = new UnityEvent();
    [HideInInspector]
    public UnityEvent hpChanged = new UnityEvent();
    [HideInInspector]
    public UnityEvent onDeath = new UnityEvent();

    public struct EffectData
    {
        public float time;
        public object data;
    }
    public OrderedDictionary effects = new OrderedDictionary();

    [HideInInspector]
    public UnityEvent<string> onAddEffect;
    [HideInInspector]
    public UnityEvent<string> onRemoveEffect;

    public enum Allegiance
    {
        Good,
        Evil
    }
    public Allegiance allegiance = Allegiance.Evil;

    private Animator anim;
    private RuntimeAnimatorController originalController;

    public List<string> weakAgainst = new List<string>();

    private void Awake()
    {
        currentHP = maxHP;
        currentMP = maxMP;
        damage = maxDamage;
        currentSpeed = maxSpeed;

        anim = GetComponentInChildren<Animator>();
        originalController = anim.runtimeAnimatorController;
    }

    private void Update()
    {
        elapsedHP += Time.deltaTime;
        elapsedMP += Time.deltaTime;
        if (elapsedHP > recoverHPTime)
        {
            AddLife(recoveryHP);
            elapsedHP = 0;
        }
        if (elapsedMP > recoverMPTime)
        {
            AddMana(recoveryMP);
            mpChanged.Invoke();
            elapsedMP = 0;
        }

        string[] keys = new string[effects.Keys.Count];
        effects.Keys.CopyTo(keys, 0);
        foreach (string key in keys)
        { 
            var da = (EffectData)effects[key];
            da.time -= Time.deltaTime;
            if (da.time <= 0f)
            {
                ClearEffect(key);
            }
            else
            {
                effects.Remove(key);
                effects.Add(key, da);
            }
        }
    }

    public void AddLife(float amount)
    {
        currentHP = Mathf.Min(currentHP + amount, maxHP);
        hpChanged.Invoke();
    }

    public void Damage(float amount, string skillName)
    {
        if (isInvincible)
        {
            return;
        }

        var effective = weakAgainst.Contains(skillName) || amount >= 0.2f * maxHP;
        var factor = !effective ? 1 : (currentHP / 6 > amount ? 2.5f : 3.5f);
        currentHP -= amount * factor;

        hpChanged.Invoke();
        StartCoroutine(Flash(effective));

        if (!dead && currentHP <= 0)
        {
            onDeath.Invoke();
            dead = true;
        }
    }

    public float GetHP()
    {
        return currentHP;
    }

    public IEnumerator Flash(bool effective)
    {
        isInvincible = true;
        float totalTime = 0;
        var material = GetComponentInChildren<Renderer>().material;

        if (effective)
        {
            material.SetColor("_FlashColor", Color.red);
        }
        else
        {
            material.SetColor("_FlashColor", Color.white);
        }

        while (totalTime < 63f)
        {
            if ((int)(totalTime / 20f) % 2 == 0)
            {
                material.SetFloat("_FlashAmount", 0.8f);
            }
            else
            {
                material.SetFloat("_FlashAmount", 0);
            }

            totalTime += 21f;
            yield return new WaitForSeconds(0.21f);
        }
        isInvincible = false;
        material.SetFloat("_FlashAmount", 0);
    }

    public void AddMana(float mana)
    {
        currentMP = Mathf.Min(currentMP + mana, maxMP);
        mpChanged.Invoke();
    }

    public void ConsumeMana(float mana)
    {
        currentMP -= mana;
        mpChanged.Invoke();
    }

    public float GetMP()
    {
        return currentMP;
    }

    public float GetDamage()
    {
        return damage;
    }

    public float GetSpeed()
    {
        return currentSpeed;
    }

    public void AddEffect(string effect, object data, float after, Action action)
    {
        if (after <= 0 || action == null) return;

        var wasPresent = effects.Contains(effect);
        EffectData oldData;
        if (wasPresent)
        {
            oldData = (EffectData)effects[effect];
            oldData.time = after;
            effects.Remove(effect);
        }
        else
        {
            oldData = new EffectData { time = after, data = data };
        }
        effects.Add(effect, oldData);

        if (!wasPresent)
        {
            onAddEffect.Invoke(effect);

            if (effect == "morphed")
            {
                damage = 1;
            }
        }
    }

    public void ClearEffect(string effect)
    {
        if (effects.Contains(effect))
        {
            effects.Remove(effect);

            if (effect == "morphed")
            {
                anim.runtimeAnimatorController = originalController;
                damage = maxDamage;
            }

            onRemoveEffect.Invoke(effect);
        }
    }

    public void EndSpell()
    {
        endSpell.Invoke();
    }
}
