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


    private static readonly WaitForSeconds attackInterval = new(0.25f);

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
        float knockBack = knockbackPower.Value / 10f;

        //vfxInstance = GameObject.Instantiate(ult.ultimateVFX, owner.transform);
        //vfxInstance.GetComponent<VisualEffect>().Play();

        //float damage = DamageCalculator.CalcDamage(ad * (1f + aS), cr, cd);
        //float kbChance = 0.25f;

        AudioManager.instance.Play("NeutrophilUltimate", owner.transform.position);

        for (int i = 0; i < 10; i++)
        {
            var hits = Physics.OverlapSphere(owner.transform.position, 5f, ult.LayerMask);

            foreach (var hit in hits)
            {
                if (hit.TryGetComponent<Enemy>(out Enemy enemy))
                {
                    Vector3 dir = (enemy.transform.position - owner.transform.position).normalized;

                    //enemy.TakeDamage(damage);
                    float armor = enemy.Armor.Value;
                    DamageCalculator.ApplyDamage(AD * (AS), CRIT_RATE, CRIT_DMG, armor, enemy);

                    //enemy.GetComponent<ImpactReceiver>().AddImpact(dir, knockBack);
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

        CurrentCD = ability.Cooldown;
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
        attackDamage = owner.GetComponent<AttributeSet>().GetAttribute("Attack Damage");
        attackSpeed = owner.GetComponent<AttributeSet>().GetAttribute("Attack Speed");
        critRate = owner.GetComponent<AttributeSet>().GetAttribute("Critical Rate");
        critDMG = owner.GetComponent<AttributeSet>().GetAttribute("Critical Damage");
        knockbackPower = owner.GetComponent<AttributeSet>().GetAttribute("Knockback Power");

        ult = ability as Neutrophil_Ultimate;
    }
}

