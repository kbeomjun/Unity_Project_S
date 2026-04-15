using System.Collections.Generic;
using UnityEngine;

public interface IEffect
{
    ITargetSelector TargetSelector { get; }

    void Execute(Unit caster, List<Unit> targets);
}

public class ChangeActionEffect : IEffect
{
    public ITargetSelector TargetSelector { get; set; }
    private UnitAction _action;

    public ChangeActionEffect(UnitAction action, ITargetSelector selector)
    {
        _action = action;
        TargetSelector = selector;
    }

    public void Execute(Unit caster, List<Unit> targets)
    {
        foreach (Unit target in targets)
        {
            target.SetNextAction(_action);
        }
    }
}

public class AttackEffect : IEffect
{
    public ITargetSelector TargetSelector { get; set; }

    public AttackEffect(ITargetSelector selector)
    {
        TargetSelector = selector;
    }

    public void Execute(Unit caster, List<Unit> targets)
    {
        foreach (Unit target in targets)
        {
            caster.Attack(target);
        }
    }
}

public class AddAttackEffect : IEffect
{
    public ITargetSelector TargetSelector { get; set; }
    private int _attack;

    public AddAttackEffect(int attack, ITargetSelector selector)
    {
        _attack = attack;
        TargetSelector = selector;
    }

    public void Execute(Unit caster, List<Unit> targets)
    {
        foreach (Unit target in targets)
        {
            target.AddAttack(_attack);
        }
    }
}

public class AddDefenseEffect : IEffect
{
    public ITargetSelector TargetSelector { get; set; }
    private int _defense;

    public AddDefenseEffect(int defense, ITargetSelector selector)
    {
        _defense = defense;
        TargetSelector = selector;
    }

    public void Execute(Unit caster, List<Unit> targets)
    {
        foreach (Unit target in targets)
        {
            if (_defense == -1) target.Defense();
            else target.AddDefense(_defense);
        }
    }
}

public class HealEffect : IEffect
{
    public ITargetSelector TargetSelector { get; set; }

    public HealEffect(ITargetSelector selector)
    {
        TargetSelector = selector;
    }

    public void Execute(Unit caster, List<Unit> targets)
    {
        foreach (Unit target in targets)
        {
            caster.Heal(target);
        }
    }
}

public class HealByPercentageEffect : IEffect
{
    public ITargetSelector TargetSelector { get; set; }
    private float _percentage;

    public HealByPercentageEffect(float percentage, ITargetSelector selector)
    {
        _percentage = percentage;
        TargetSelector = selector;
    }

    public void Execute(Unit caster, List<Unit> targets)
    {
        foreach (Unit target in targets)
        {
            target.HealByPercentage(_percentage);
        }
    }
}

public class ResetActionEffect : IEffect
{
    public ITargetSelector TargetSelector { get; set; }

    public ResetActionEffect(ITargetSelector selector)
    {
        TargetSelector = selector;
    }

    public void Execute(Unit caster, List<Unit> targets)
    {
        foreach (Unit target in targets)
        {
            target.DecideAction();
        }
    }
}

public class ApplyStatusSelfEffect : IEffect
{
    public ITargetSelector TargetSelector { get; set; }

    private IStatusEffect _status;

    public ApplyStatusSelfEffect(IStatusEffect status, ITargetSelector selector)
    {
        _status = status;
        TargetSelector = selector;
    }

    public void Execute(Unit caster, List<Unit> targets)
    {
        foreach (Unit target in targets)
        {
            caster.Target = target;
            caster.ApplyStatus(_status);
        }
    }
}

public class ApplyStatusEffect : IEffect
{
    public ITargetSelector TargetSelector { get; set; }

    private IStatusEffect _status;

    public ApplyStatusEffect(IStatusEffect status, ITargetSelector selector)
    {
        _status = status;
        TargetSelector = selector;
    }

    public void Execute(Unit caster, List<Unit> targets)
    {
        foreach (Unit target in targets)
        {
            target.ApplyStatus(_status);
        }
    }
}
