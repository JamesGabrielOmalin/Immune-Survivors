using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : Unit
{
    private Attribute MaxHP;
    private Attribute HP;
    private Attribute AttackSpeed;

    public bool IsDead => HP.Value <= 0f;

    public System.Action OnDeath;

    private Player targetPlayer;
    private Coroutine attackCoroutine;

    [field: Header("Antigen")]
    [field: SerializeField] public AntigenType Type { get; private set; }
    [SerializeField] private GameObject stunIndicator;

    public bool IsStunned { get; private set; } = false;
    private Coroutine stunCoroutine;
    private Coroutine dotCoroutine;

    private void Awake()
    {
        // Register to minimap
        //Minimap.Get().Register(this.gameObject, MinimapIconType.Enemy);
    }

    // Start is called before the first frame update
    private void Start()
    {
        MaxHP = attributes.GetAttribute("Max HP");
        HP = attributes.GetAttribute("HP");
        AttackSpeed = attributes.GetAttribute("Attack Speed");

        HP.BaseValue = MaxHP.Value;

        // Upon elimination, spawn antigen
        OnDeath += delegate
        {
            AntigenManager.instance.SpawnAntigen(transform.position, Type);
            RecruitManager.instance.AddKillCount();
            //Minimap.Get().Unregister(this.gameObject, MinimapIconType.Enemy);
            this.gameObject.SetActive(false);
        };

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
    }

    public void TakeDamage(float amount)
    {
        HP.BaseValue -= amount;
        //Debug.Log("Damage taken: "+ HP.BaseValue);

        if (HP.Value <= 0f)
        {
            //RemoveFromDetectedList();
            OnDeath?.Invoke();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<Player>(out Player player))
        {
            attackCoroutine = StartCoroutine(Attack());
            targetPlayer = player;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent<Player>(out Player player))
        {
            StopCoroutine(attackCoroutine);
            targetPlayer = null;
        }
    }

    // Attack the target player
    private IEnumerator Attack()
    {
        while (this)
        {
            yield return new WaitForSeconds(1f / AttackSpeed.Value);

            if (IsStunned)
                continue;

            if (targetPlayer)
            {
                Debug.Log(gameObject.name + "attacked player!");
            }
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
