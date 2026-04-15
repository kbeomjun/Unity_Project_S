using System.Collections.Generic;
using UnityEngine;

public interface ISkill
{
    void Execute(Unit caster);
}

public class Attack : ISkill
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

public class Defense : ISkill
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

public class Skill : ISkill
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
