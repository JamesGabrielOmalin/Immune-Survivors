using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Enemy : Unit, IDamageInterface
{
    private Attribute MaxHP;
    private Attribute HP;
    private Attribute AttackDamage;
    private Attribute AttackSpeed;
    private Attribute MoveSpeed;
    private Attribute Armor;
    private Attribute AntigenSpawnChance;

    public bool IsDead => HP.BaseValue <= 0f;

    public System.Action OnDeath;

    private Player targetPlayer;
    private Coroutine attackCoroutine;
    private const float INITIAL_ATTACK_DELAY = 0.25f;

    [field: Header("Antigen")]
    [field: SerializeField] public AntigenType Type { get; private set; }
    [SerializeField] private GameObject stunIndicator;
    [SerializeField] private VisualEffect dotIndicator;

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
        MoveSpeed = attributes.GetAttribute("Move Speed");
        Armor = attributes.GetAttribute("Armor");
        AntigenSpawnChance = attributes.GetAttribute("Antigen Spawn Chance");

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
        if (MoveSpeed == null)
            MoveSpeed = attributes.GetAttribute("Move Speed");

        MaxHP.RemoveAllModifiers();
        MoveSpeed.RemoveAllModifiers();

        // Increase HP and Move Speed by 10% for every minute that has passed
        if (GameManager.instance)
        {
            MaxHP.AddModifier(new(GameManager.instance.GameTime.Minutes * 0.1f, AttributeModifierType.Multiply));
            MoveSpeed.AddModifier(new(GameManager.instance.GameTime.Minutes * 0.1f, AttributeModifierType.Multiply));
        }

        HP.BaseValue = MaxHP.Value;

        // Upon elimination, spawn antigen
        OnDeath += delegate
        {
            if (Random.value < AntigenSpawnChance.Value)
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

        //Vector3 location = transform.position;
        //location.y += 1.0f;

        //DamageNumberManager.instance.SpawnDamageNumber(location, amount);

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
            if (attackCoroutine == null)
                attackCoroutine = StartCoroutine(Attack());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(PLAYER_TAG))
        {
            //StopCoroutine(attackCoroutine);
            targetPlayer = null;
        }
    }

    // Attack the target player
    private IEnumerator Attack()
    {
        // Short delay before actually attacking
        yield return new WaitForSeconds(INITIAL_ATTACK_DELAY);
        while (targetPlayer)
        {
            yield return new WaitUntil(() => !this.IsStunned);

            if (!targetPlayer)
                break;

            var activeUnit = targetPlayer.GetActiveUnit();
            float armor = activeUnit.attributes.GetAttribute("Armor").Value;
            DamageCalculator.ApplyDamage(AttackDamage.Value,  armor, activeUnit);
            targetPlayer.GetActiveUnit().TakeDamage(AttackDamage.Value);

            yield return new WaitForSeconds(1f / AttackSpeed.Value);
        }

        attackCoroutine = null;
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
        if (damage > float.Epsilon)
        {
            float t = 0f;
            float tick = 1f / tickRate;

            dotIndicator.Play();

            while (t < duration)
            {
                TakeDamage(damage);
                DamageNumberManager.instance.SpawnDoTNumber(transform.position, damage);
                t += tick;
                yield return new WaitForSeconds(tick);
            }
        }

        dotIndicator.Stop();
        dotCoroutine = null;
        yield break;
    }
}
