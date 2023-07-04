using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using UnityEngine;
using UnityEngine.VFX;
using UnityEngine.Timeline;
using UnityEngine.Playables;

[CreateAssetMenu(fileName = "Dendritic_Ultimate", menuName = "Ability System/Abilities/Dendritic Ultimate")]
public class Dendritic_Ultimate : Ability
{
    //[SerializeField] public GameObject ultimateVFX;

    public override AbilitySpec CreateSpec(AbilitySystem owner)
    {
        AbilitySpec spec = new Dendritic_UltimateSpec(this, owner);
        return spec;
    }
}

public class Dendritic_UltimateSpec : AbilitySpec
{
    public AttributeSet attributes;
    public Attribute level;

    Dendritic_Ultimate ult;

    public Dendritic_UltimateSpec(Dendritic_Ultimate ability, AbilitySystem owner) : base(ability, owner)
    {
        Init();
    }

    // TODO: Make required level visible on ScriptableObject
    public override bool CanActivateAbility()
    {
        return level.Value >= 5f && base.CanActivateAbility();
    }

    public override IEnumerator ActivateAbility()
    {
        GameManager.instance.HUD.SetActive(false);
        var player = GameManager.instance.Player.GetComponent<Player>();

        player.EnableHUD(false);

        var playable = owner.GetComponent<PlayableDirector>();
        playable.Play();

        WaitForSecondsRealtime wait = new(5f);

        // Vergil: You shall die!
        GameManager.instance.PauseGameTime();
        // I AM THE STORM THAT IS APPROACHING
        yield return wait;
        GameManager.instance.ResumeGameTime();
        // PROVOKING
        // BLACK CLOUDS IN ISOLATION

        var hits = Physics.OverlapSphere(owner.transform.position, 10f);

        foreach (var hit in hits)
        {
            Enemy enemy = hit.GetComponent<Enemy>();
            if (!enemy)
                continue;

            enemy.TakeDamage(999999);
        }

        // I AM RECLAIMER OF MY NAME
        // BORN IN FLAMES
        // I HAVE BEEN BLESSED
        // MY FAMILY CREST IS A DEMON OF DEATH

        yield return new WaitForSeconds(1f);
        playable.Stop();
        CurrentCD = ability.Cooldown;
        owner.StartCoroutine(UpdateCD());

        GameManager.instance.HUD.SetActive(true);
        player.EnableHUD(true);
    }

    public override void EndAbility()
    {
        base.EndAbility();
    }

    public void Init()
    {
        attributes = owner.GetComponent<AttributeSet>();

        level = attributes.GetAttribute("Level");

        ult = ability as Dendritic_Ultimate;
    }
}

