using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Enemy : Unit, IDamageInterface
{
    //private Attribute MaxHP;
    //private Attribute HP;
    private Attribute AttackDamage;
    private Attribute AttackSpeed;
    private Attribute MoveSpeed;
    //private Attribute Armor;
    private Attribute AntigenSpawnChance;

    //public bool IsDead => HP.BaseValue <= 0f;

    //public System.Action OnDeath;


    private Player targetPlayer;

    private Coroutine attackCoroutine;
    private const float INITIAL_ATTACK_DELAY = 0.25f;

    [field: Header("Antigen")]
    [field: SerializeField] public AntigenType Type { get; private set; }

    [SerializeField] private SpriteRenderer sprite;
    //[SerializeField] private GameObject stunIndicator;
    //[SerializeField] private VisualEffect dotIndicator;
    [SerializeField] private VisualEffect armorShredIndicator;

    //[SerializeField] private GameObject stunIndicator;
    //[SerializeField] private VisualEffect dotIndicator;


    //public bool IsStunned { get; private set; } = false;
    private Coroutine armorShredCoroutine;
    //private Coroutine stunCoroutine;
    //private Coroutine dotCoroutine;

    private const string PLAYER_TAG = "Player";

    [SerializeField] private Animator animator;
    private readonly int ANIMATOR_DEATH = Animator.StringToHash("Death");

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        //MaxHP = attributes.GetAttribute("Max HP");
        //HP = attributes.GetAttribute("HP");
        AttackDamage = attributes.GetAttribute("Attack Damage");
        AttackSpeed = attributes.GetAttribute("Attack Speed");
        MoveSpeed = attributes.GetAttribute("Move Speed");
        //Armor = attributes.GetAttribute("Armor");
        AntigenSpawnChance = attributes.GetAttribute("Antigen Spawn Chance");

        HP.BaseValue = MaxHP.Value;

        // Hide stun indicator
        //stunIndicator.SetActive(false);
        EnemyManager.instance.allEnemies.Add(this.gameObject);

        attackCoroutine = null;
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

        // Increase HP and Move Speed by 20% for every minute that has passed
        if (GameManager.instance)
        {
            MaxHP.AddModifier(new(GameManager.instance.GameTime.Minutes * 1f, AttributeModifierType.Multiply));

            //MaxHP.AddModifier(new(GameManager.instance.GameTime.Minutes * 0.1f, AttributeModifierType.Multiply));
            MoveSpeed.AddModifier(new(GameManager.instance.GameTime.Minutes * 0.2f, AttributeModifierType.Multiply));
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

        //dotCoroutine = null;
        //stunCoroutine = null;
        //attackCoroutine = null;
        //armorShredCoroutine = null;

        StopAllCoroutines();

        
    }

    public void TakeDamage(float amount)
    {
        if (IsDead)
            return;

        if (TutorialManager.isFirstTime)
        {
            TutorialManager.instance.OnEnemyVisible?.Invoke();
        }

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

        IsStunned = false;
        stunIndicator.SetActive(false);

        dotIndicator.Stop();

        this.gameObject.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Detect Player");

        if (other.CompareTag(PLAYER_TAG))
        {
            targetPlayer = other.GetComponent<Player>();
            if (attackCoroutine == null)
            {
                Debug.Log("Start Attack Coroutine");

                attackCoroutine = StartCoroutine(Attack());

            }
            else
            {
                Debug.Log("Coroutine not null");
            }
        }
        else
        {
            //Debug.Log("tag: " + other.tag + "Name: " + other.name);
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
            Debug.Log("Attack Player");

            DamageCalculator.ApplyDamage(AttackDamage.Value,  armor, activeUnit);
            targetPlayer.GetActiveUnit().TakeDamage(AttackDamage.Value);

            yield return new WaitForSeconds(1f / AttackSpeed.Value);
        }

        attackCoroutine = null;
    }

    public override void ApplyStun(float duration)
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

    private static readonly WaitForSeconds armorShredDuration = new(3f);

    private IEnumerator ArmorShred(float amount)
    {
        AttributeModifier mod = new(-amount, AttributeModifierType.Multiply);

        Armor.AddModifier(mod);
        armorShredIndicator.Play();
        sprite.color = Color.gray;

        yield return armorShredDuration;

        Armor.RemoveModifier(mod);
        armorShredIndicator.Stop();
        sprite.color = Color.white;

        armorShredCoroutine = null;
    }

    protected override IEnumerator Stun(float duration)
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

    public override void ApplyDoT(float damage, float duration, float tickRate)
    {
        if (IsDead)
            return;

        if (dotCoroutine != null)
            StopCoroutine(dotCoroutine);

        dotCoroutine = StartCoroutine(DoT(damage, duration, tickRate));
    }

    protected override IEnumerator DoT(float damage, float duration, float tickRate)
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

    public override void ApplyKnockback(Vector3 force, ForceMode forceMode)
    {
        if (gameObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.AddForce(force, forceMode);
            ApplyStun(0.6f);
        }
    }

    public override void ApplyMoveSpeedModifier(AttributeModifier mod, float duration, bool isInfinite)
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

    protected override IEnumerator MoveSpeedModifierDurationCoroutine(AttributeModifier mod, float duration, bool isInfinite)
    {
        var e = attributes.GetAttribute("Move Speed");

        e.AddModifier(mod);

        yield return new WaitForSeconds(duration);

        e.RemoveModifier(mod);


    }
}
