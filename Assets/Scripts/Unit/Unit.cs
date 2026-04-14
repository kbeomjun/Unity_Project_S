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

    protected UnitAction _nextAction;
    protected Unit _target;

    protected bool _isSkillUsing = false;
    public bool IsSkillUsing => _isSkillUsing;

    private Material _outlineMaterial;

    public void Init(UnitData unitData)
    {
        _unitData = unitData;
        _currentAttack = _unitData.Attack;

        _outlineMaterial = _spriteRenderer.material;
        _healthBar.InitHp(_unitData.CurrentHealth, _unitData.MaxHealth);

        _nextActionScript.gameObject.SetActive(false);
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

        _nextActionScript.ChangeNextActionIcon(random, _currentAttack, _unitData.Defense);
    }

    public virtual void PerformAction()
    {
        switch (_nextAction)
        {
            case UnitAction.Attack:
                Attack();
                break;

            case UnitAction.Defense:
                Defense();
                break;

            case UnitAction.Skill:
                UseSkill();
                break;
        }
    }

    protected void Attack()
    {
        _target = BattleManager.Instance.GetRandomEnemyTarget(this);
        Debug.Log($"{gameObject.name} Attack {_target.gameObject.name}");
        _animator.SetTrigger("Attack");
    }

    public void HitTarget()
    {
        _target.Hit(_currentAttack);

        if (_target._unitData.Type == UnitType.Lancer && _target._isSkillUsing)
        {
            Hit(_currentAttack / 2);
        }
    }

    private void Hit(int damage)
    {
        Debug.Log($"{gameObject.name} Hit {damage}");

        if (_unitData.Type == UnitType.Knight && _isSkillUsing) damage /= 2;

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

    public void Defense(int defense)
    {
        Debug.Log($"{gameObject.name} Defense");
        _currentDefense += defense;
        _unitEffect.ShieldEffect();
        _healthBar.SetDefense(_currentDefense);
    }

    public void HealTarget()
    {
        _target.Heal(10);
    }

    private void Heal(int percentage)
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
        _nextActionScript.ChangeNextActionIcon((int)_nextAction, _currentAttack, _unitData.Defense);
    }

    public void AddAttack(int attack)
    {
        _currentAttack += attack;
        _nextActionScript.SetNextActionNumberText(_currentAttack);
    }

    public virtual void UseSkill()
    {
        Debug.Log($"{gameObject.name} UseSkill");
    }

    public virtual void UseSkill(Unit target)
    {
        _target = BattleManager.Instance.GetRandomTeamTarget(this);
        Debug.Log($"{gameObject.name} UseSkill {_target.gameObject.name}");
        _animator.SetTrigger("Skill");
    }

    public void SetHighlight(bool onOff)
    {
        _outlineMaterial.SetFloat("_OutlineSize", onOff ? 1.5f : 0f);
    }

}
