using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.VFX;

[CreateAssetMenu(fileName = "Neutrophil_Ultimate", menuName = "Ability System/Abilities/Neutrophil Ultimate")]
public class Neutrophil_Ultimate : Ability
{
    //[SerializeField] public GameObject ultimateVFX;
    [field: SerializeField] public LayerMask LayerMask { get; private set; }

    public override AbilitySpec CreateSpec(AbilitySystem owner)
    {
        AbilitySpec spec = new Neutrophil_UltimateSpec(this, owner);
        return spec;
    }
}

public class Neutrophil_UltimateSpec : AbilitySpec
{
    public AttributeSet attributes;
    public Attribute level; 
    public Attribute attackDamage;
    public Attribute attackSpeed;
    public Attribute critRate;
    public Attribute critDMG;
    public Attribute knockbackPower;
    public Attribute CDReduction; 
    public Attribute Type_1_DMG_Bonus;
    public Attribute Type_2_DMG_Bonus;
    public Attribute Type_3_DMG_Bonus;

    private Neutrophil_Ultimate ult;

    public Neutrophil_UltimateSpec(Neutrophil_Ultimate ability, AbilitySystem owner) : base(ability, owner)
    {
        Init();
    }

    //private GameObject vfxInstance = null;

    // TODO: Make required level visible on ScriptableObject
    public override bool CanActivateAbility()
    {
        return level.Value >= 5f && base.CanActivateAbility();
    }

    private const int MAX_HITS = 25;
    private const int MAX_TARGETS = 20;
    private static readonly WaitForSeconds attackInterval = new(1f / MAX_HITS);

    public override IEnumerator ActivateAbility()
    {
        var player = GameManager.instance.Player.GetComponent<Player>();
        Transform sprite = player.GetUnit(PlayerUnitType.Neutrophil).transform.GetChild(0);

        foreach (Transform spr in sprite)
        {
            spr.gameObject.SetActive(false);
        }

        var abilitySet = owner.GetComponent<AbilitySet>();
        abilitySet.EnableBasicAttack(false);

        var playable = owner.GetComponent<PlayableDirector>();
        playable.Play();

        float AD = attackDamage.Value;
        float AS = attackSpeed.Value;
        float CRIT_RATE = critRate.Value;
        float CRIT_DMG = critDMG.Value;
        float knockBack = knockbackPower.Value;

        float Type_1 = Type_1_DMG_Bonus.Value;
        float Type_2 = Type_2_DMG_Bonus.Value;
        float Type_3 = Type_3_DMG_Bonus.Value;

        //vfxInstance = GameObject.Instantiate(ult.ultimateVFX, owner.transform);
        //vfxInstance.GetComponent<VisualEffect>().Play();

        //float damage = DamageCalculator.CalcDamage(ad * (1f + aS), cr, cd);
        //float kbChance = 0.25f;

        AudioManager.instance.Play("NeutrophilUltimate", owner.transform.position);

        for (int i = 0; i < MAX_HITS; i++)
        {
            var hits = Physics.OverlapSphere(owner.transform.position, 5f, ult.LayerMask);

            for (int j = 0; j < Mathf.Min(MAX_TARGETS, hits.Length); j++)
            {
                if (hits[j].TryGetComponent(out Enemy enemy))
                {
                    Vector3 dir = (enemy.transform.position - owner.transform.position).normalized;

                    //enemy.TakeDamage(damage);
                    float DMGBonus = Type_1;

                    switch (enemy.Type)
                    {
                        case AntigenType.Type_1:
                            DMGBonus = Type_1;
                            break;
                        case AntigenType.Type_2:
                            DMGBonus = Type_2;
                            break;
                        case AntigenType.Type_3:
                            DMGBonus = Type_3;
                            break;
                    }

                    float damage = AD * AS * DMGBonus;
                    float armor = enemy.Armor.Value;
                    DamageCalculator.ApplyDamage(damage, CRIT_RATE, CRIT_DMG, armor, enemy);

                    if (enemy.TryGetComponent(out ImpactReceiver impact))
                        impact.AddImpact(dir, knockBack);
                }
            }

            yield return attackInterval;
        }

        playable.Stop();
        abilitySet.EnableBasicAttack(true);

        foreach (Transform spr in sprite)
        {
            spr.gameObject.SetActive(true);
        }

        CurrentCD = ability.Cooldown * (100f / (100f + CDReduction.Value));
        owner.StartCoroutine(UpdateCD());

        yield return new WaitForSeconds(1f);
    }

    public override void EndAbility()
    {
        //GameObject.Destroy(vfxInstance);
        //vfxInstance = null;
        base.EndAbility();
    }

    public void Init()
    {
        attributes = owner.GetComponent<AttributeSet>();

        level = attributes.GetAttribute("Level");
        attackDamage = attributes.GetAttribute("Attack Damage");
        attackSpeed = attributes.GetAttribute("Attack Speed");
        critRate = attributes.GetAttribute("Critical Rate");
        critDMG = attributes.GetAttribute("Critical Damage");
        knockbackPower = attributes.GetAttribute("Knockback Power");
        CDReduction = attributes.GetAttribute("CD Reduction");

        Type_1_DMG_Bonus = attributes.GetAttribute("Type_1 DMG Bonus");
        Type_2_DMG_Bonus = attributes.GetAttribute("Type_2 DMG Bonus");
        Type_3_DMG_Bonus = attributes.GetAttribute("Type_3 DMG Bonus");

        ult = ability as Neutrophil_Ultimate;
    }
}

