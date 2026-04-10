using UnityEngine;

public enum UnitAction
{
    Attack,
    Defense,
    Skill
}

public class Unit : MonoBehaviour
{
    [SerializeField] protected Animator _animator;
    [SerializeField] private HealthBar _healthBar;
    [SerializeField] private Shield _shieldEffect;
    [SerializeField] private NextAction _nextActionScript;
    public NextAction NextActionScript => _nextActionScript;

    protected UnitData _unitData;
    protected int _currentAttack;
    protected int _currentDefense = 0;

    protected UnitAction _nextAction;
    protected Unit _target;

    private bool _isDead = false;
    protected bool _isSkillUsing = false;
    public bool IsSkillUsing => _isSkillUsing;

    public void Init(UnitData unitData)
    {
        _unitData = unitData;
        _currentAttack = _unitData.Attack;

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

    public void Attack()
    {
        _target = BattleManager.Instance.GetRandomEnemyTarget(this);
        Debug.Log($"{gameObject.name} Attack {_target.gameObject.name}");
        _animator.SetTrigger("Attack");
    }
    
    public void TargetHit()
    {
        _target.Hit(_currentAttack);
    }

    public void Hit(int damage)
    {
        Debug.Log($"{gameObject.name} Hit");

        if (_unitData.Type == UnitType.Knight && _isSkillUsing) damage /= 2;

        if(_currentDefense >= damage)
        {
            _currentDefense -= damage;
            _healthBar.SetDefense(_currentDefense);
        }
        else
        {
            damage -= _currentDefense;
            _currentDefense = 0;
            _unitData.CurrentHealth -= damage;
            _healthBar.SetDefense(_currentDefense);
            _healthBar.SetHp(_unitData.CurrentHealth, _unitData.MaxHealth);
            
            if(_unitData.CurrentHealth > 0)
            {
                if(!_isSkillUsing) _animator.SetTrigger("Hit");
            }
            else
            {
                _isDead = true;
                _animator.SetTrigger("Die");
            }
        }
    }

    public void AfterDie()
    {
        BattleManager.Instance.RemoveUnit(this);
        BattleManager.Instance.Check(this);
        Destroy(gameObject);
    }

    public void Defense()
    {
        Debug.Log($"{gameObject.name} Defense");
        _currentDefense += _unitData.Defense;
        _shieldEffect.ShieldEffect();
        _healthBar.SetDefense(_currentDefense);
    }

    public void Defense(int defense)
    {
        Debug.Log($"{gameObject.name} Defense");
        _currentDefense += defense;
        _shieldEffect.ShieldEffect();
        _healthBar.SetDefense(_currentDefense);
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

}
