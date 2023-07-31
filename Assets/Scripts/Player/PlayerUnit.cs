using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerUnitType
{
    Neutrophil,
    Macrophage,
    Dendritic
}

public class PlayerUnit : Unit
{
    [SerializeField] private Player player;

    [field: SerializeField] public AbilitySet AbilitySet { get; private set; }

    [field:SerializeField] public PlayerUnitType UnitType { get; private set; }

    public System.Action OnUnitUpgraded;
    public System.Action OnDeath;

    private List<Effect> upgrades = new();

    private Attribute level;
    private Attribute maxHP;
    private Attribute HP;
    private Attribute HPRegen;
    private Attribute Armor;

    [Header("UI")]
    [SerializeField] private Slider HPBar;

    private void Start()
    {
        level = attributes.GetAttribute("Level");
        maxHP = attributes.GetAttribute("Max HP");
        HP = attributes.GetAttribute("HP");
        HPRegen = attributes.GetAttribute("HP Regen");
        Armor = attributes.GetAttribute("Armor");

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

    public void Upgrade()
    {
        OnUnitUpgraded?.Invoke();

        UpgradeManager.instance.OpenUpgradeScreen(UnitType);

        // Level up
        level.ApplyInstantModifier(new(1, AttributeModifierType.Add));
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
