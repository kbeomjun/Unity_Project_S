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

    protected int _currentAttack = 0;
    private int _currentDefense = 0;
    private float _hitdamageMultiplier = 1.0f;
    private int _hitdamageReflection = 0;
    private float _attackdamageMultiplier = 1.0f;
    private int _totalAttack = 0;
    private int _totalDefense = 0;

    public float HitDamageMultiplier
    {
        get => _hitdamageMultiplier;
        set => _hitdamageMultiplier = value;
    }
    public int HitDamageReflection
    {
        get => _hitdamageReflection;
        set => _hitdamageReflection = value;
    }
    public float AttackDamageMultiplier
    {
        get => _attackdamageMultiplier;
        set => _attackdamageMultiplier = value;
    }

    protected UnitAction _currentAction;
    protected UnitAction _nextAction;
    protected Unit _target = null;

    public UnitAction CurrentAction => _currentAction;
    public Unit Target
    {
        get => _target;
        set => _target = value;
    }

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
        int random = Random.Range(2, 3);
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
        _animator.SetTrigger("Attack");
    }

    public void HitTarget()
    {
        _target.Hit(_totalAttack);
        Hit(_target.HitDamageReflection);
    }

    private void Hit(int damage)
    {
        damage = (int)(damage * _hitdamageMultiplier);

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
                if(_currentAction != UnitAction.Skill) _animator.SetTrigger("Hit");
            }
            else
            {
                _unitData.CurrentHealth = 0;
                Die();
            }

            _healthBar.SetDefense(_currentDefense);
            _healthBar.SetHp(_unitData.CurrentHealth, _unitData.MaxHealth);
        }
    }

    public virtual void Die()
    {
        _animator.SetTrigger("Die");
    }

    public void AfterDie()
    {
        BattleManager.Instance.RemoveUnit(this);
        Destroy(gameObject);
    }

    public void Defense()
    {
        _currentDefense += _unitData.Defense;
        _unitEffect.ShieldEffect();
        _healthBar.SetDefense(_currentDefense);
    }

    public void Heal(Unit target)
    {
        _target = target;
        _animator.SetTrigger("Skill");
    }

    public void HealTarget()
    {
        _target.HealByPercentage(0.1f);
    }

    public void HealByPercentage(float percentage)
    {
        _unitData.CurrentHealth += (int)(_unitData.MaxHealth * percentage);

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
        _currentDefense += defense;
        _unitEffect.ShieldEffect();
        _healthBar.SetDefense(_currentDefense);
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
        _totalAttack = (int)(_currentAttack * _attackdamageMultiplier);
        _totalDefense = (int)(_unitData.Defense);
        _nextActionScript.UpdateNextActionIcon((int)_nextAction, _totalAttack, _totalDefense);
    }

}
