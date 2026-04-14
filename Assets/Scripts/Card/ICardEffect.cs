using System.Collections.Generic;
using UnityEngine;

public interface ICardEffect
{
    ITargetSelector TargetSelector { get; }

    void Execute(Unit caster, List<Unit> targets);
}

public class ChangeActionEffect : ICardEffect
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

public class AddAttackEffect : ICardEffect
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

public class AddDefenseEffect : ICardEffect
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
            target.AddDefense(_defense);
        }
    }
}

public class HealByPercentageEffect : ICardEffect
{
    public ITargetSelector TargetSelector { get; set; }
    private int _percentage;

    public HealByPercentageEffect(int percentage, ITargetSelector selector)
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

public class ReduceAttackByPercentageEffect : ICardEffect
{
    public ITargetSelector TargetSelector { get; set; }
    private int _percentage;

    public ReduceAttackByPercentageEffect(int percentage, ITargetSelector selector)
    {
        _percentage = percentage;
        TargetSelector = selector;
    }

    public void Execute(Unit caster, List<Unit> targets)
    {
        foreach (Unit target in targets)
        {
            target.ReduceAttackByPercentage(_percentage);
        }
    }
}

public class ResetActionEffect : ICardEffect
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
