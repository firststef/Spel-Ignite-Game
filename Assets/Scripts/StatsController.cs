using Spells;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections.Specialized;

public class StatsController : MonoBehaviour
{
    public float maxHP = 10f;
    private float currentHP;

    public float recoveryHP = 0f;
    public float recoverHPTime = 60f;
    private float elapsedHP = 0f;

    public float maxMP = 10f;
    private float currentMP;

    public float recoveryMP = 0.5f;
    public float recoverMPTime = 30f;
    private float elapsedMP = 0f;

    public float maxSpeed = 1f;
    private float currentSpeed;

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

    private void Awake()
    {
        currentHP = maxHP;
        currentMP = maxMP;
        currentSpeed = maxSpeed;
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

    public void Damage(float amount)
    {
        if (isInvincible)
        {
            return;
        }

        currentHP -= amount;
        hpChanged.Invoke();
        StartCoroutine(Flash());

        if (!dead && currentHP <= 0)
        {
            onDeath.Invoke();
            dead = true;
        }
    }

    public void Hit(ICastSpell spell)
    {
        Damage(1); // todo replace depending on spell
    }

    public float GetHP()
    {
        return currentHP;
    }

    public IEnumerator Flash()
    {
        isInvincible = true;
        float totalTime = 0;
        while (totalTime < 100f)
        {
            if ((int)(totalTime / 30f) % 2 == 0)
            {
                GetComponentInChildren<Renderer>().material.SetFloat("_FlashAmount", 0.8f);
            }
            else
            {
                GetComponentInChildren<Renderer>().material.SetFloat("_FlashAmount", 0);
            }

            totalTime += 31f;
            yield return new WaitForSeconds(0.31f);
        }
        isInvincible = false;
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
        }
    }

    public void ClearEffect(string effect)
    {
        if (effects.Contains(effect))
        {
            effects.Remove(effect);
            onRemoveEffect.Invoke(effect);
        }
    }
}
