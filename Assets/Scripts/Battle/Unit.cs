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
    [SerializeField] protected SpriteRenderer[] _actionSprites;
    [SerializeField] protected Transform _healthBarTr;
    [SerializeField] protected HealthBar _healthBarPrefab;
    [SerializeField] protected Shield _shieldEffect;

    private HealthBar _healthBar;

    protected string _name;

    protected int _maxHealth;
    protected int _currentHealth;

    protected int _attack;
    protected int _currentAttack;

    protected int _defense;
    protected int _currentDefense = 0;

    protected UnitAction _nextAction;

    protected Unit _target;

    protected bool _isDead = false;

    protected void Init(string name, int maxHealth, int attack, int defense)
    {
        _name = name;
        _maxHealth = maxHealth;
        _currentHealth = maxHealth;
        _attack = attack;
        _currentAttack = attack;
        _defense = defense;

        _healthBar = Instantiate(_healthBarPrefab, _healthBarTr);
        _healthBar.transform.localPosition = Vector3.zero;
        _healthBar.InitHp(_currentHealth, _maxHealth);
    }

    public virtual void ResetAction()
    {
        _currentDefense = 0;
        _target = null;
        _healthBar.SetDefense(_currentDefense);
    }

    public virtual void DecideAction()
    {
        _currentAttack = _attack;
        _nextAction = (UnitAction)Random.Range(0, 3);
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
        _target.Hit(_currentAttack);
    }

    public void Hit(int damage)
    {
        Debug.Log($"{gameObject.name} Hit");

        if(_currentDefense >= damage)
        {
            _currentDefense -= damage;
            _healthBar.SetDefense(_currentDefense);
        }
        else
        {
            damage -= _currentDefense;
            _currentDefense = 0;
            _currentHealth -= damage;
            _healthBar.SetDefense(_currentDefense);
            _healthBar.SetHp(_currentHealth, _maxHealth);
            
            if(_currentHealth > 0)
            {
                _animator.SetTrigger("Hit");
            }
            else
            {
                _isDead = true;
                _animator.SetTrigger("Die");
                //Destroy(gameObject);
            }
        }
    }

    public void Defense()
    {
        Debug.Log($"{gameObject.name} Defense");
        _currentDefense += _defense;
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
