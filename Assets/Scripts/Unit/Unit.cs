using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Unit : MonoBehaviour
{
    [SerializeField] public AbilitySystem abilitySystem;
    [SerializeField] public AttributeSet attributes;

    public bool IsDead => HP.BaseValue <= 0f;

    public Attribute MaxHP;
    public Attribute HP;
    public Attribute Armor;

    public System.Action OnDeath;
    public System.Action OnTakeDamage;

    public bool IsStunned { get; protected set; } = false;
    public Coroutine stunCoroutine;
    public Coroutine dotCoroutine;

    public VisualEffect dotIndicator;
    public GameObject stunIndicator;

    protected virtual void Start()
    {
        MaxHP = attributes.GetAttribute("Max HP");
        HP = attributes.GetAttribute("HP");
        Armor = attributes.GetAttribute("Armor");

        stunIndicator.SetActive(false);

    }
    //public virtual void TakeDamage(float amount)
    //{
    //    if (IsDead)
    //        return;

    //    HP.ApplyInstantModifier(new(-amount, AttributeModifierType.Add));

    //    //Vector3 location = transform.position;
    //    //location.y += 1.0f;

    //    //DamageNumberManager.instance.SpawnDamageNumber(location, amount);

    //    if (HP.Value <= 0f)
    //    {
    //        //RemoveFromDetectedList();
    //        //animator.SetTrigger(ANIMATOR_DEATH);
    //        //StartCoroutine(Death());
    //    }
    //}
    public virtual void ApplyStun(float duration)
    {
        if (IsDead)
            return;

        if (stunCoroutine != null)
            StopCoroutine(stunCoroutine);

        stunCoroutine = StartCoroutine(Stun(duration));
    }

    protected virtual IEnumerator Stun(float duration)
    {
        IsStunned = true;
        stunIndicator.SetActive(true);
        WaitForSeconds wait = new(duration);
        yield return wait;

        IsStunned = false;
        stunIndicator.SetActive(false);

        stunCoroutine = null;
        yield break;
    }

    public virtual void ApplyDoT(float damage, float duration, float tickRate)
    {
        if (IsDead)
            return;

        if (dotCoroutine != null)
            StopCoroutine(dotCoroutine);

        dotCoroutine = StartCoroutine(DoT(damage, duration, tickRate));
    }

    protected virtual IEnumerator DoT(float damage, float duration, float tickRate)
    {
        if (damage > float.Epsilon)
        {
            float t = 0f;
            float tick = 1f / tickRate;

            dotIndicator.Play();

            while (t < duration)
            {
                //TakeDamage(damage);
                DamageNumberManager.instance.SpawnDoTNumber(transform.position, damage);
                t += tick;
                yield return new WaitForSeconds(tick);
            }
        }

        dotIndicator.Stop();
        dotCoroutine = null;
        yield break;
    }

    public virtual void ApplyKnockback(Vector3 force, ForceMode forceMode)
    {
        if (gameObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.AddForce(force, forceMode);

        }
    }

    public virtual void ApplyMoveSpeedModifier(AttributeModifier mod, float duration, bool isInfinite)
    {
        if (isInfinite)
        {
            var e = attributes.GetAttribute("Move Speed");
            e.AddModifier(mod);
        }
        else
        {
            StartCoroutine(MoveSpeedModifierDurationCoroutine(mod, duration, isInfinite));
        }
    }

    protected virtual IEnumerator MoveSpeedModifierDurationCoroutine(AttributeModifier mod, float duration, bool isInfinite)
    {
        var e = attributes.GetAttribute("Move Speed");

        e.AddModifier(mod);

        yield return new WaitForSeconds(duration);

        e.RemoveModifier(mod);


    }
}
