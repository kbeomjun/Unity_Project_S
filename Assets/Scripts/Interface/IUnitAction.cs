using System.Collections.Generic;
using UnityEngine;

public interface IUnitAction
{
    void Execute(Unit caster);
}

public class Attack : IUnitAction
{
    private List<IEffect> _effects;

    public Attack(List<IEffect> effects)
    {
        _effects = effects;
    }

    public void Execute(Unit caster)
    {
        foreach (IEffect effect in _effects)
        {
            List<Unit> targets = effect.TargetSelector.SelectTargets(caster);
            effect.Execute(caster, targets);
        }
    }
}

public class Defense : IUnitAction
{
    private List<IEffect> _effects;

    public Defense(List<IEffect> effects)
    {
        _effects = effects;
    }

    public void Execute(Unit caster)
    {
        foreach (IEffect effect in _effects)
        {
            List<Unit> targets = effect.TargetSelector.SelectTargets(caster);
            effect.Execute(caster, targets);
        }
    }
}

public class Skill : IUnitAction
{
    private List<IEffect> _effects;

    public Skill(List<IEffect> effects)
    {
        _effects = effects;
    }

    public void Execute(Unit caster)
    {

        foreach (IEffect effect in _effects)
        {
            List<Unit> targets = effect.TargetSelector.SelectTargets(caster);
            effect.Execute(caster, targets);
        }
    }
}
