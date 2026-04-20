using System.Collections.Generic;
using UnityEngine;

public interface IUnitAction
{
    void Execute(Unit caster);
}

public class Attack : IUnitAction
{
    private List<System.Func<IEffect>> _effectFactories;

    public Attack(List<System.Func<IEffect>> effectFactories)
    {
        _effectFactories = effectFactories;
    }

    public void Execute(Unit caster)
    {
        foreach (var factory in _effectFactories)
        {
            IEffect effect = factory(); 
            List<Unit> targets = effect.TargetSelector.SelectTargets(caster);
            effect.Execute(caster, targets);
        }
    }
}

public class Defense : IUnitAction
{
    private List<System.Func<IEffect>> _effectFactories;

    public Defense(List<System.Func<IEffect>> effectFactories)
    {
        _effectFactories = effectFactories;
    }

    public void Execute(Unit caster)
    {
        foreach (var factory in _effectFactories)
        {
            IEffect effect = factory();
            List<Unit> targets = effect.TargetSelector.SelectTargets(caster);
            effect.Execute(caster, targets);
        }
    }
}

public class Skill : IUnitAction
{
    private List<System.Func<IEffect>> _effectFactories;

    public Skill(List<System.Func<IEffect>> effectFactories)
    {
        _effectFactories = effectFactories;
    }

    public void Execute(Unit caster)
    {
        foreach (var factory in _effectFactories)
        {
            IEffect effect = factory();
            List<Unit> targets = effect.TargetSelector.SelectTargets(caster);
            effect.Execute(caster, targets);
        }
    }
}
