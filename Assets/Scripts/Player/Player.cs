using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerInput input;
    [SerializeField] private PlayerMovement playerMovement;

    [Header("Units")]
    [SerializeField] private PlayerUnit neutrophil;
    [SerializeField] private PlayerUnit macrophage;
    [SerializeField] private PlayerUnit dendritic;
    [SerializeField] private GameObject neutrophilHUD;
    [SerializeField] private GameObject macrophageHUD;
    [SerializeField] private GameObject dendriticHUD;

    [Header("Slots")]
    [SerializeField] Transform activeSlot;
    [SerializeField] private Transform slot1;
    [SerializeField] private Transform slot2;

    private PlayerUnit activeUnit;
    [HideInInspector] public GameObject activeHUD;
    private int numRecruit;
    [SerializeField] private GameObject hudParent;

    [SerializeField] private List<Image> buffIcons = new();

    public static PlayerUnitType toSpawn = PlayerUnitType.Neutrophil;

    private readonly Dictionary<PlayerUnitType, bool> unitRecruited = new()
    {
        { PlayerUnitType.Neutrophil, false },
        { PlayerUnitType.Macrophage, false },
        { PlayerUnitType.Dendritic, false },
    };

    private readonly Dictionary<AntigenType, Coroutine> buffCoroutines = new()
    {
        { AntigenType.Type_1, null },
        { AntigenType.Type_2, null },
        { AntigenType.Type_3, null },
    };

    private const string DMG_BONUS_STRING = " DMG Bonus";

    private void Awake()
    {
        switch (toSpawn)
        {
            case PlayerUnitType.Neutrophil:
                RecruitUnit(neutrophil);
                activeUnit = neutrophil;
                neutrophilHUD.SetActive(true);
                activeHUD = neutrophilHUD;
                break;
            case PlayerUnitType.Macrophage:
                RecruitUnit(macrophage);
                activeUnit = macrophage;
                macrophageHUD.SetActive(true);
                activeHUD = macrophageHUD;
                break;
            case PlayerUnitType.Dendritic:
                RecruitUnit(dendritic);
                activeUnit = dendritic;
                dendriticHUD.SetActive(true);
                activeHUD = dendriticHUD;       
                break;
        }

        activeUnit.outline.SetActive(true);
    }

    private System.Action<InputAction.CallbackContext> mobilityHandler;
    private System.Action<InputAction.CallbackContext> ultimateHandler;

    private void Start()
    {
        mobilityHandler = (ctx) => activeUnit.AbilitySet.ActivateMobility();
        ultimateHandler = (ctx) => activeUnit.AbilitySet.ActivateUltimate();

        input.Controls.Abilities.Mobility.started += mobilityHandler;
        input.Controls.Abilities.Ultimate.started += ultimateHandler;  

        activeUnit.OnDeath += delegate { this.gameObject.SetActive(false); GameManager.instance.HUD.SetActive(false); };
        activeUnit.OnDeath += delegate { GameManager.instance.OnGameLose?.Invoke(); };
    }

    private void OnDestroy()
    {
        input.Controls.Abilities.Mobility.started -= mobilityHandler;
        input.Controls.Abilities.Ultimate.started -= ultimateHandler;
    }

    public void RecruitUnit(PlayerUnit recruit)
    {
        if (activeUnit)
        {
            activeUnit.Heal(recruit.attributes.GetAttribute("Max HP").BaseValue * 0.10f);
        }

        // If already recruit, upgrade instead
        if (unitRecruited[recruit.UnitType])
        {
            UpgradeUnit(recruit.UnitType);
            return;
        }

        Transform slot = null;

        switch (numRecruit)
        {
            case 0:
                slot = activeSlot;
                break;
            case 1:
                slot = slot1;
                break;
            case 2:
                slot = slot2;
                break;
        }

        PlayerUnit unit = GetUnit(recruit.UnitType);
        unit.gameObject.SetActive(true);
        unit.transform.parent = slot;
        unit.transform.localPosition = Vector3.zero;

        if (numRecruit > 0)
        {
            unit.GetComponent<Collider>().enabled = false;
            unit.GetComponent<Rigidbody>().detectCollisions = false;
        }

        //switch (recruit.UnitType)
        //{
        //    case PlayerUnitType.Neutrophil:
        //        neutrophil.gameObject.SetActive(true);
        //        neutrophil.transform.parent = slot;
        //        neutrophil.transform.localPosition = Vector3.zero;
        //        break;
        //    case PlayerUnitType.Macrophage:
        //        macrophage.gameObject.SetActive(true);
        //        macrophage.transform.parent = slot;
        //        macrophage.transform.localPosition = Vector3.zero;
        //        break;
        //    case PlayerUnitType.Dendritic:
        //        dendritic.gameObject.SetActive(true);
        //        dendritic.transform.parent = slot;
        //        dendritic.transform.localPosition = Vector3.zero;
        //        break;
        //}

        unitRecruited[recruit.UnitType] = true;

        numRecruit++;
    }

    private void UpgradeUnit(PlayerUnitType unitType)
    {
        switch (unitType)
        {
            case PlayerUnitType.Neutrophil:
                neutrophil.Upgrade();
                break;
            case PlayerUnitType.Macrophage:
                macrophage.Upgrade();
                break;
            case PlayerUnitType.Dendritic:
                dendritic.Upgrade();
                break;
        }
    }

    public PlayerUnit GetActiveUnit()
    {
        return activeUnit;
    }

    public void ApplyAntigenBuffs(AntigenType type, AttributeModifier mod, float duration)
    {
        if (buffCoroutines[type] != null)
        {
            StopCoroutine(buffCoroutines[type]);
        }
        buffCoroutines[type] = StartCoroutine(AntigenBuffCoroutine(type, mod, duration));
    }

    private IEnumerator AntigenBuffCoroutine(AntigenType type, AttributeModifier mod, float duration)
    {
        var n = neutrophil.attributes.GetAttribute(type.ToString() + DMG_BONUS_STRING);
        var m = macrophage.attributes.GetAttribute(type.ToString() + DMG_BONUS_STRING);
        var d = dendritic.attributes.GetAttribute(type.ToString() + DMG_BONUS_STRING);
        n.AddModifier(mod);
        m.AddModifier(mod);
        d.AddModifier(mod);

        neutrophil.buffVFX.Play();
        macrophage.buffVFX.Play();
        dendritic.buffVFX.Play();

        float t = 0f;

        // Update buff icon
        buffIcons[(int)type].transform.parent.gameObject.SetActive(true);
        while (t < duration)
        {
            buffIcons[(int)type].fillAmount = 1f - (t / duration);

            t += Time.deltaTime;
            yield return null;
        }
        buffIcons[(int)type].transform.parent.gameObject.SetActive(false);

        neutrophil.buffVFX.Stop();
        macrophage.buffVFX.Stop();
        dendritic.buffVFX.Stop();

        n.RemoveModifier(mod);
        m.RemoveModifier(mod);
        d.RemoveModifier(mod);
    }

    public void ApplyMoveSpeedModifier(AttributeModifier mod, float duration, bool isInfinite)
    {
       StartCoroutine(MoveSpeedModifierDurationCoroutine(mod, duration, isInfinite));
    }

    private IEnumerator MoveSpeedModifierDurationCoroutine(AttributeModifier mod, float duration, bool isInfinite)
    {
        var n = neutrophil.attributes.GetAttribute("Move Speed");
        var m = macrophage.attributes.GetAttribute("Move Speed");
        var d = dendritic.attributes.GetAttribute("Move Speed");

        n.AddModifier(mod);
        m.AddModifier(mod);
        d.AddModifier(mod);

        playerMovement.UpdateMoveSpeed();

        if (!isInfinite)
        {
            yield return new WaitForSeconds(duration);

            n.RemoveModifier(mod);
            m.RemoveModifier(mod);
            d.RemoveModifier(mod);

            playerMovement.UpdateMoveSpeed();
        }
    }

    public PlayerUnit GetUnit(PlayerUnitType type)
    {
        return type switch
        {
            PlayerUnitType.Neutrophil => neutrophil,
            PlayerUnitType.Macrophage => macrophage,
            PlayerUnitType.Dendritic => dendritic,
            _ => null,
        };
    }

    public void EnableHUD(bool enabled)
    {
        hudParent.SetActive(enabled);
        //activeHUD.SetActive(enabled);
    }

    public void OnEnableBuffHUD(bool enabled)
    {
        for (int i = 0; i < buffIcons.Count; i++)
        {
            buffIcons[i].gameObject.SetActive(enabled);
        }
    }

}
