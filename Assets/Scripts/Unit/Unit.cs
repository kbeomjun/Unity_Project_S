using System.Collections.Generic;
using UnityEngine;

public enum UnitAction
{
    Attack,
    Defense,
    Skill
}

public class Unit : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] protected Animator _animator;
    [SerializeField] private HealthBar _healthBar;
    [SerializeField] private UnitEffect _unitEffect;
    [SerializeField] private NextAction _nextActionScript;
    public NextAction NextActionScript => _nextActionScript;

    protected UnitData _unitData;
    public UnitData UnitData => _unitData;

    protected int _currentAttack;
    private int _currentDefense = 0;
    private float _damageMultiplier = 1.0f;
    private int _damageReflection = 0;

    public float DamageMultiplier
    {
        get => _damageMultiplier;
        set => _damageMultiplier = value;
    }
    public int DamageReflection
    {
        get => _damageReflection;
        set => _damageReflection = value;
    }

    protected UnitAction _currentAction;
    protected UnitAction _nextAction;
    protected Unit _target;

    protected bool _isSkillUsing = false;
    public bool IsSkillUsing => _isSkillUsing;

    private List<IStatusEffect> _statuses = new List<IStatusEffect>();

    private Material _outlineMaterial;

    public void Init(UnitData unitData)
    {
        _unitData = unitData;
        _unitData.AttackAction = DataManager.Instance.UnitAttack;
        _unitData.DefenseAction = DataManager.Instance.UnitDefense;
        _unitData.SkillAction = DataManager.Instance.UnitSkills[(int)_unitData.Type];
        _currentAttack = _unitData.Attack;

        _outlineMaterial = _spriteRenderer.material;
        _healthBar.InitHp(_unitData.CurrentHealth, _unitData.MaxHealth);
        _nextActionScript.gameObject.SetActive(false);
    }

    public void ApplyStatus(IStatusEffect status)
    {
        status.OnApply(this);
        _statuses.Add(status);
    }

    public void OnTurnStart()
    {
        for (int i = _statuses.Count - 1; i >= 0; i--)
        {
            IStatusEffect status = _statuses[i];

            status.OnTurnStart(this);

            if (status.Duration <= 0)
            {
                status.OnRemove(this);
                _statuses.RemoveAt(i);
            }
        }
    }

    public void OnTurnEnd()
    {
        foreach (IStatusEffect status in _statuses)
        {
            status.OnTurnEnd(this);
        }
    }

    public virtual void ResetAction()
    {
        _currentDefense = 0;
        _target = null;
        _healthBar.SetDefense(_currentDefense);
    }

    public virtual void DecideAction()
    {
        _currentAttack = _unitData.Attack;
        int random = Random.Range(0, 3);
        _nextAction = (UnitAction)random;
    }

    public virtual void PerformAction()
    {
        _currentAction = _nextAction;

        switch (_nextAction)
        {
            case UnitAction.Attack:
                PerformAttackAction();
                break;

            case UnitAction.Defense:
                PerformDefenseAction();
                break;

            case UnitAction.Skill:
                PerformSkillAction();
                break;
        }
    }

    private void PerformAttackAction()
    {
        _unitData.AttackAction.Execute(this);
    }

    private void PerformDefenseAction()
    {
        _unitData.DefenseAction.Execute(this);
    }

    private void PerformSkillAction()
    {
        _unitData.SkillAction.Execute(this);
        UseSkill();
    }

    public void Attack(Unit target)
    {
        _target = target;
        Debug.Log($"{gameObject.name} Attack {_target.gameObject.name}");
        _animator.SetTrigger("Attack");
    }

    public void HitTarget()
    {
        _target.Hit(_currentAttack);
        Hit(_target.DamageReflection);
    }

    private void Hit(int damage)
    {
        damage = (int)(damage * _damageMultiplier);

        Debug.Log($"{gameObject.name} Hit {damage}");

        if (_currentDefense >= damage)
        {
            _currentDefense -= damage;
            _healthBar.SetDefense(_currentDefense);
        }
        else
        {
            damage -= _currentDefense;
            _currentDefense = 0;
            _unitData.CurrentHealth -= damage;
            
            if(_unitData.CurrentHealth > 0)
            {
                if(!_isSkillUsing) _animator.SetTrigger("Hit");
            }
            else
            {
                _unitData.CurrentHealth = 0;
                _animator.SetTrigger("Die");
            }

            _healthBar.SetDefense(_currentDefense);
            _healthBar.SetHp(_unitData.CurrentHealth, _unitData.MaxHealth);
        }
    }

    public void AfterDie()
    {
        BattleManager.Instance.RemoveUnit(this);
        Destroy(gameObject);
    }

    public void Defense()
    {
        Debug.Log($"{gameObject.name} Defense");
        _currentDefense += _unitData.Defense;
        _unitEffect.ShieldEffect();
        _healthBar.SetDefense(_currentDefense);
    }

    public void HealTarget()
    {
        _target.HealByPercentage(10);
    }

    public void HealByPercentage(int percentage)
    {
        Debug.Log($"{gameObject.name} Heal");
        _unitData.CurrentHealth += (_unitData.MaxHealth * percentage) / 100;

        if (_unitData.CurrentHealth > _unitData.MaxHealth)
        {
            _unitData.CurrentHealth = _unitData.MaxHealth;
        }
        
        _healthBar.SetHp(_unitData.CurrentHealth, _unitData.MaxHealth);
        _unitEffect.HealEffect();
    }

    public void SetNextAction(UnitAction action)
    {
        _nextAction = action;
    }

    public void AddAttack(int attack)
    {
        _currentAttack += attack;
    }

    public void AddDefense(int defense)
    {
        Debug.Log($"{gameObject.name} Defense");
        _currentDefense += defense;
        _unitEffect.ShieldEffect();
        _healthBar.SetDefense(_currentDefense);
    }

    public void ReduceAttackByPercentage(int percentage)
    {
        _currentAttack = (_currentAttack * percentage) / 100;
    }

    public virtual void UseSkill()
    {

    }

    public void SetHighlight(bool onOff)
    {
        _outlineMaterial.SetFloat("_OutlineSize", onOff ? 1.5f : 0f);
    }

    private void Update()
    {
        _nextActionScript.ChangeNextActionIcon((int)_nextAction, _currentAttack, _unitData.Defense);
    }

}
