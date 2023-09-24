using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;

public enum PlayerUnitType
{
    Neutrophil,
    Macrophage,
    Dendritic
}

public class PlayerUnit : Unit, IDamageInterface
{
    [SerializeField] private Player player;

    [field: SerializeField] public AbilitySet AbilitySet { get; private set; }

    [field:SerializeField] public PlayerUnitType UnitType { get; private set; }

    public bool IsDead => HP.BaseValue <= 0f;

    public System.Action OnUnitUpgraded;
    public System.Action OnDeath;
    public System.Action OnTakeDamage;


    private List<Effect> upgrades = new();

    private Attribute level;
    private Attribute maxHP;
    private Attribute HP;
    private Attribute HPRegen;
    private Attribute Armor;

    [SerializeField] private GameObject stunIndicator;
    [SerializeField] private VisualEffect dotIndicator;

    public bool IsStunned { get; private set; } = false;
    private Coroutine stunCoroutine;
    private Coroutine dotCoroutine;

    [Header("UI")]
    [SerializeField] private Slider HPBar;


    private void Start()
    {
        level = attributes.GetAttribute("Level");
        maxHP = attributes.GetAttribute("Max HP");
        HP = attributes.GetAttribute("HP");
        HPRegen = attributes.GetAttribute("HP Regen");
        Armor = attributes.GetAttribute("Armor");

        // Hide stun indicator
        stunIndicator.SetActive(false);

        if (HPBar)
        {
            maxHP.OnAttributeModified += delegate { HPBar.maxValue = maxHP.Value; };
            HP.OnAttributeModified += delegate { HPBar.value = HP.Value; };

            HPBar.maxValue = maxHP.Value;
            HPBar.value = HP.Value;
        }

        StartCoroutine(Attack());
        StartCoroutine(Regen());
    }

    public void TakeDamage(float amount)
    {
        OnTakeDamage?.Invoke();

        HP.ApplyInstantModifier(new(-(amount - Armor.Value), AttributeModifierType.Add));

        if (HP.Value <= 0f)
        {
            //RemoveFromDetectedList();
            OnDeath?.Invoke();
        }
    }

    public void Heal(float amount)
    {
        HP.ApplyInstantModifier(new(amount, AttributeModifierType.Add));
        HP.BaseValue = Mathf.Clamp(HP.BaseValue, 0f, maxHP.Value);
    }
    public void ApplyStun(float duration)
    {
        if (IsDead)
            return;

        if (stunCoroutine != null)
            StopCoroutine(stunCoroutine);

        stunCoroutine = StartCoroutine(Stun(duration));
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

    public void ApplyKnockback(Vector3 force, ForceMode forceMode)
    {
        if (gameObject.TryGetComponent<Rigidbody>(out Rigidbody rb))
        {
            rb.AddForce(force, forceMode);

        }
    }
    public void Upgrade()
    {
        OnUnitUpgraded?.Invoke();

        UpgradeManager.instance.OpenUpgradeScreen(UnitType);

        // Level up
        level.ApplyInstantModifier(new(1, AttributeModifierType.Add));

        if (level.BaseValue == 5)
        {
            AbilitySet.GrantUltimate();
        }
    }

    public void AddUpgrade(Effect upgrade)
    {
        abilitySystem.ApplyEffectToSelf(upgrade);
    }

    public bool CanBeUpgraded()
    {
        return upgrades.Count < 3;
    }

    private IEnumerator Attack()
    {
        yield return null;

        while (this)
        {
            var attacks = abilitySystem.GetAbilitiesOfType(AbilityType.BasicAttack);

            foreach (var basicAttack in attacks)
            {
                //yield return basicAttack.TryActivateAbility();
                StartCoroutine(basicAttack.TryActivateAbility());
            }

            yield return new WaitUntil(() => attacks.TrueForAll((attack) => attack.CanActivateAbility()));
        }
    }

    private IEnumerator Regen()
    {
        WaitForSeconds wait = new(1f);
        yield return null;

        while (HP.Value > 0f)
        {
            yield return new WaitUntil(() => HP.BaseValue < maxHP.Value);
            yield return wait;

            // Restore HP through HP Regen
            HP.ApplyInstantModifier(new(HPRegen.Value, AttributeModifierType.Add));
            // Clamp HP to Max HP
            HP.BaseValue = Mathf.Clamp(HP.BaseValue, 0f, maxHP.Value);
        }
    }
}
