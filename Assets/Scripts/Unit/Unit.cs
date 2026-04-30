using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public enum UnitAction
{
    Attack,
    Defense,
    Skill
}

public enum UnitTeam 
{ 
    Player,
    Enemy 
}

public class Unit : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] protected Animator _animator;
    [SerializeField] protected NextAction _nextActionScript;
    [SerializeField] private HealthBar _healthBar;
    [SerializeField] private UnitEffect _unitEffect;
    [SerializeField] private Transform _unitEffectTr;
    [SerializeField] private StatusIconGroup _statusIconGroup;
    [SerializeField] private Transform _tooltipAnchor;

    public NextAction NextActionScript => _nextActionScript;
    public Transform TooltipAnchor => _tooltipAnchor;

    protected UnitData _unitData;
    public UnitData UnitData => _unitData;

    protected int _currentAttack = 0;
    private int _currentDefense = 0;
    private float _hitdamageMultiplier = 1.0f;
    private float _hitdamageReflection = 1.0f;
    private float _attackdamageMultiplier = 1.0f;
    private int _totalAttack = 0;
    private int _totalDefense = 0;

    public float HitDamageMultiplier
    {
        get => _hitdamageMultiplier;
        set => _hitdamageMultiplier = value;
    }
    public float HitDamageReflection
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
    protected UnitTeam _unitTeam;
    protected Unit _target = null;

    public UnitAction CurrentAction => _currentAction;
    public UnitTeam UnitTeam
    {
        get => _unitTeam;
        set => _unitTeam = value;
    }
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

    public void ApplyStatus(StatusType type, int duration)
    {
        switch (type)
        {
            case StatusType.IronWall:
            case StatusType.Brace:
            case StatusType.Focus:
                Instantiate(DataManager.Instance.BuffEffect, _unitEffectTr, false);
                break;

            case StatusType.Weak:
                Instantiate(DataManager.Instance.DebuffEffect, _unitEffectTr, false);
                break;
        }

        for (int i = 0; i < _statuses.Count; i++)
        {
            IStatusEffect s = _statuses[i];
            if (s is StatusEffect se && se.Type == type)
            {
                se.Duration += duration;
                _statusIconGroup.UpdateStatus(se);
                return;
            }
        }

        IStatusEffect newStatus = new StatusEffect(type, duration);
        newStatus.OnApply(this);
        _statuses.Add(newStatus);
        _statusIconGroup.AddStatus(newStatus);
    }

    public void OnTurnStart()
    {
        for (int i = _statuses.Count - 1; i >= 0; i--)
        {
            IStatusEffect status = _statuses[i];
            status.OnTurnStart(this);
            _statusIconGroup.UpdateStatus(status);
            if (status.Duration <= 0)
            {
                status.OnRemove(this);
                _statusIconGroup.RemoveStatus(status);
                _statuses.RemoveAt(i);
            }
        }
    }

    public void OnTurnEnd()
    {
        for (int i = _statuses.Count - 1; i >= 0; i--)
        {
            IStatusEffect status = _statuses[i];
            status.OnTurnEnd(this);
            _statusIconGroup.UpdateStatus(status);
            if (status.Duration <= 0)
            {
                status.OnRemove(this);
                _statusIconGroup.RemoveStatus(status);
                _statuses.RemoveAt(i);
            }
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
        _nextActionScript.gameObject.SetActive(true);
        _currentAttack = _unitData.Attack;
        int random = Random.Range(0, 3);
        _nextAction = (UnitAction)random;
    }

    public virtual void PerformAction()
    {
        _currentAction = _nextAction;
        _nextActionScript.gameObject.SetActive(false);

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
        int reflectionDamage = (int)(_totalAttack * (1.0f - _target.HitDamageReflection));
        Hit(reflectionDamage);
    }

    private void Hit(int damage)
    {
        damage = (int)(damage * _hitdamageMultiplier);

        Debug.Log($"{gameObject.name} Hit {damage}");

        if (_currentDefense >= damage)
        {
            _currentDefense -= damage;
            _healthBar.SetDefense(_currentDefense);
            if (damage != 0)
            {
                Instantiate(DataManager.Instance.BlockEffect, _unitEffectTr, false);
            }
        }
        else
        {
            damage -= _currentDefense;
            _currentDefense = 0;
            _unitData.CurrentHealth -= damage;
            
            if(_unitData.CurrentHealth > 0)
            {
                if(_currentAction == UnitAction.Skill && (_unitData.Type == UnitType.Knight || _unitData.Type == UnitType.Lancer))
                {
                    Instantiate(DataManager.Instance.HitEffect, _unitEffectTr, false);
                }
                else
                {
                    _animator.SetTrigger("Hit");
                    Instantiate(DataManager.Instance.HitEffect, _unitEffectTr, false);
                }
            }
            else
            {
                _unitData.CurrentHealth = 0;
                Instantiate(DataManager.Instance.HitEffect, _unitEffectTr, false);
                Die();
            }

            _healthBar.SetDefense(_currentDefense);
            _healthBar.SetHp(_unitData.CurrentHealth, _unitData.MaxHealth);
        }
    }

    public virtual void Die()
    {
        _animator.SetTrigger("Die");
        Instantiate(DataManager.Instance.DieEffect, _unitEffectTr, false);
    }

    public void AfterDie()
    {
        BattleManager.Instance.RemoveUnit(this);
        Destroy(gameObject);
    }

    public void Defense()
    {
        _currentDefense += _unitData.Defense;
        _unitEffect.DefenseEffect();
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
            _unitData.CurrentHealth = _unitData.MaxHealth;
        _healthBar.SetHp(_unitData.CurrentHealth, _unitData.MaxHealth);
        //_unitEffect.HealEffect();
        Instantiate(DataManager.Instance.HealEffect, _unitEffectTr, false);
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
        _unitEffect.DefenseEffect();
        _healthBar.SetDefense(_currentDefense);
    }

    public virtual void UseSkill()
    {

    }

    public List<TooltipData> GetTooltipDatas()
    {
        List<TooltipData> datas = new List<TooltipData>();

        datas.Add(new TooltipData(TooltipType.NextAction, _nextAction.ToString(), GetNextActionDescription(), _nextActionScript.GetNextActionIcon()));

        foreach (IStatusEffect status in _statuses)
        {
            if (status is StatusEffect se)
            {
                datas.Add(new TooltipData(TooltipType.Status, se.Type.ToString(), GetStatusDescription(se), se.Icon));
            }
        }

        return datas;
    }

    private string GetNextActionDescription()
    {
        switch (_nextAction) 
        {
            case UnitAction.Attack:
                return $"Attacks with {_totalAttack} damage";

            case UnitAction.Defense:
                return $"Gain {_totalDefense} defense";

            case UnitAction.Skill:
                return $"Use special skill";

            default:
                return $"";
        }
    }

    public string GetStatusDescription(StatusEffect se)
    {
        switch (se.Type)
        {
            case StatusType.IronWall:
                return "Reduce damage taken by 50%";

            case StatusType.Brace:
                return "Reflect 50% of damage taken";

            case StatusType.Focus:
                return "Damage increased by 100%";

            case StatusType.Weak:
                return "Damage reduced by 25%";

            default:
                return "";
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!InputManager.Instance.CanTooltip()) return;
        SetHighlight(true);
        List<TooltipData> datas = GetTooltipDatas();
        Vector2 pos = TooltipUtility.GetCanvasPosition(TooltipPanel.Instance.CanvasRect, _tooltipAnchor.position, Camera.main);
        TooltipPanel.Instance.Show(pos, datas);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SetHighlight(false);
        TooltipPanel.Instance.Hide();
    }

    public void SetHighlight(bool onOff)
    {
        _outlineMaterial.SetFloat("_OutlineSize", onOff ? 1.5f : 0.0f);
    }

    private void Update()
    {
        _totalAttack = (int)(_currentAttack * _attackdamageMultiplier);
        _totalDefense = (int)(_unitData.Defense);
        _nextActionScript.UpdateNextActionIcon((int)_nextAction, _totalAttack, _totalDefense);
    }

}
