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
    }

    public virtual void ResetAction()
    {
        _currentDefense = 0;
    }

    public virtual void DecideAction()
    {
        _currentAttack = _attack;
        _target = BattleManager.Instance.GetRandomTarget(this);
    }

    public virtual void PerformAction()
    {

    }

    public void Attack()
    {
        Debug.Log($"{gameObject.name} Attack {_target.gameObject.name}");
        _animator.SetTrigger("Attack");
        _target.Hit(_currentAttack);
    }

    public void Hit(int damage)
    {
        Debug.Log($"{gameObject.name} Hit");
        _currentHealth -= damage;

        if(_currentHealth <= 0)
        {
            _isDead = true;
        }
    }

    public void Defense()
    {
        Debug.Log($"{gameObject.name} Defense");
        _currentDefense += _defense;
    }

    public virtual void UseSkill()
    {
        Debug.Log($"{gameObject.name} UseSkill");
    }

    public virtual void UseSkill(Unit target)
    {
        Debug.Log($"{gameObject.name} UseSkill {target.gameObject.name}");
    }

}
