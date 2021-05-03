using Spells;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

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
}
