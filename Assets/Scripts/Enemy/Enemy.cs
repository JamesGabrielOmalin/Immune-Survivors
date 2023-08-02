using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Unit, IDamageInterface
{
    private Attribute MaxHP;
    private Attribute HP;
    private Attribute AttackDamage;
    private Attribute AttackSpeed;
    private Attribute Armor;

    public bool IsDead => HP.BaseValue <= 0f;

    public System.Action OnDeath;

    private Player targetPlayer;
    private Coroutine attackCoroutine;

    [field: Header("Antigen")]
    [field: SerializeField] public AntigenType Type { get; private set; }
    [SerializeField] private GameObject stunIndicator;

    public bool IsStunned { get; private set; } = false;
    private Coroutine armorShredCoroutine;
    private Coroutine stunCoroutine;
    private Coroutine dotCoroutine;

    private const string PLAYER_TAG = "Player";

    [SerializeField] private Animator animator;
    private readonly int ANIMATOR_DEATH = Animator.StringToHash("Death");

    // Start is called before the first frame update
    private void Start()
    {
        MaxHP = attributes.GetAttribute("Max HP");
        HP = attributes.GetAttribute("HP");
        AttackDamage = attributes.GetAttribute("Attack Damage");
        AttackSpeed = attributes.GetAttribute("Attack Speed");
        Armor = attributes.GetAttribute("Armor");

        HP.BaseValue = MaxHP.Value;

        // Hide stun indicator
        stunIndicator.SetActive(false);
    }

    private void OnEnable()
    {
        if (MaxHP == null)
            MaxHP = attributes.GetAttribute("Max HP");
        if (HP == null)
            HP = attributes.GetAttribute("HP");

        HP.BaseValue = MaxHP.Value;

        // Upon elimination, spawn antigen
        OnDeath += delegate
        {
            AntigenManager.instance.SpawnAntigen(transform.position, Type);
            RecruitManager.instance.AddKillCount();
        };
    }

    private void OnDisable()
    {
        OnDeath = null;
    }

    public void TakeDamage(float amount)
    {
        if (IsDead)
            return;

        HP.ApplyInstantModifier(new(-amount, AttributeModifierType.Add));

        if (HP.Value <= 0f)
        {
            //RemoveFromDetectedList();
            animator.SetTrigger(ANIMATOR_DEATH);
            StartCoroutine(Death());
        }
    }

    private IEnumerator Death()
    {
        yield return new WaitForSeconds(0.5f);
        OnDeath?.Invoke();
        this.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(PLAYER_TAG))
        {
            targetPlayer = other.GetComponent<Player>();
            attackCoroutine = StartCoroutine(Attack());
            Debug.Log("Player Entered");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(PLAYER_TAG))
        {
            StopCoroutine(attackCoroutine);
            targetPlayer = null;
            Debug.Log("Player Exited");
        }
    }

    // Attack the target player
    private IEnumerator Attack()
    {
        while (targetPlayer)
        {
            yield return new WaitForSeconds(1f / AttackSpeed.Value);

            if (IsStunned)
                continue;

            DamageCalculator.ApplyDamage(AttackDamage.Value, 0f, 1f, 0f, targetPlayer.GetActiveUnit());
            targetPlayer.GetActiveUnit().TakeDamage(AttackDamage.Value);
            Debug.Log(gameObject.name + "attacked player!");
        }
    }

    public void ApplyStun(float duration)
    {
        if (IsDead)
            return;

        if (stunCoroutine != null)
            StopCoroutine(stunCoroutine);

        stunCoroutine = StartCoroutine(Stun(duration));
    }

    public void ApplyArmorShred(float amount)
    {
        if (IsDead)
            return;

        if (armorShredCoroutine != null)
            StopCoroutine(armorShredCoroutine);

        armorShredCoroutine = StartCoroutine(ArmorShred(amount));
    }

    private IEnumerator ArmorShred(float amount)
    {
        AttributeModifier mod = new(-amount, AttributeModifierType.Multiply);

        Armor.AddModifier(mod);

        yield return new WaitForSeconds(3f);

        Armor.RemoveModifier(mod);
        armorShredCoroutine = null;
    }

    private IEnumerator Stun(float duration)
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

    public void ApplyDoT(float damage, float duration, float tickRate)
    {
        if (IsDead)
            return;

        if (dotCoroutine != null)
            StopCoroutine(dotCoroutine);

        dotCoroutine = StartCoroutine(DoT(damage, duration, tickRate));
    }

    private IEnumerator DoT(float damage, float duration, float tickRate)
    {
        float t = 0f;
        float tick = 1f / tickRate;

        while (t < duration)
        {
            TakeDamage(damage);
            t += tick;
            yield return new WaitForSeconds(tick);
        }

        dotCoroutine = null;
        yield break;
    }
}
