using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public interface ITargetSelector
{
    List<Unit> SelectTargets(Unit caster);
}

public class RandomAttackSelector : ITargetSelector
{
    public List<Unit> SelectTargets(Unit caster)
    {
        List<Unit> units = BattleManager.Instance.PlayerUnits.Contains(caster) ? 
                            BattleManager.Instance.EnemyUnits.ToList() : BattleManager.Instance.PlayerUnits.ToList();
        List<Unit> frontUnits = new List<Unit>();

        foreach (Unit unit in units)
        {
            if (unit != null && unit.UnitData.SlotIndex < 2) frontUnits.Add(unit);
        }

        foreach (Unit unit in frontUnits)
        {
            if (unit.UnitData.Type == UnitType.Knight && unit.CurrentAction == UnitAction.Skill) return new List<Unit> { unit };
        }

        int index = Random.Range(0, frontUnits.Count);

        return new List<Unit> { frontUnits[index] };
    }
}

public class RandomTeamSelector : ITargetSelector
{
    private int _count;

    public RandomTeamSelector(int count)
    {
        _count = count;
    }

    public List<Unit> SelectTargets(Unit caster)
    {
        List<Unit> units = BattleManager.Instance.PlayerUnits.Contains(caster) ?
                            BattleManager.Instance.PlayerUnits.ToList() : BattleManager.Instance.EnemyUnits.ToList();
        List<Unit> result = new List<Unit>();

        for (int i = 0; i < _count && units.Count > 0; i++)
        {
            int index = Random.Range(0, units.Count);
            if (units[index] == null) continue;
            result.Add(units[index]);
            units.RemoveAt(index);
        }

        return result;
    }
}

public class SelfTargetSelector : ITargetSelector
{
    public List<Unit> SelectTargets(Unit caster)
    {
        return new List<Unit> { caster };
    }
}

public class SingleTargetSelector : ITargetSelector
{
    public List<Unit> SelectTargets(Unit caster)
    {
        return new List<Unit> { CardManager.Instance.SelectedUnit };
    }
}

public class AllAllySelector : ITargetSelector
{
    public List<Unit> SelectTargets(Unit caster)
    {
        Unit[] units = BattleManager.Instance.PlayerUnits;
        List<Unit> allies = new List<Unit>();

        for (int i = 0; i < units.Length; i++)
        {
            if (units[i] != null) allies.Add(units[i]);
        }

        return allies;
    }
}

public class FrontAllySelector : ITargetSelector
{
    public List<Unit> SelectTargets(Unit caster)
    {
        Unit[] units = BattleManager.Instance.PlayerUnits;
        List<Unit> allies = new List<Unit>();

        for (int i = 0; i < units.Length; i++)
        {
            if (units[i] != null && i < 2) allies.Add(units[i]);
        }

        return allies;
    }
}

public class BackAllySelector : ITargetSelector
{
    public List<Unit> SelectTargets(Unit caster)
    {
        Unit[] units = BattleManager.Instance.PlayerUnits;
        List<Unit> allies = new List<Unit>();

        for (int i = 0; i < units.Length; i++)
        {
            if (units[i] != null && i >= 2) allies.Add(units[i]);
        }

        return allies;
    }
}

public class RandomAllySelector : ITargetSelector
{
    private int _count;

    public RandomAllySelector(int count)
    {
        _count = count;
    }

    public List<Unit> SelectTargets(Unit caster)
    {
        List<Unit> allies = BattleManager.Instance.PlayerUnits.ToList();
        List<Unit> result = new List<Unit>();

        for (int i = 0; i < _count && allies.Count > 0; i++)
        {
            int index = Random.Range(0, allies.Count);
            if (allies[index] == null) continue;
            result.Add(allies[index]);
            allies.RemoveAt(index);
        }

        return result;
    }
}

public class AllEnemySelector : ITargetSelector
{
    public List<Unit> SelectTargets(Unit caster)
    {
        Unit[] units = BattleManager.Instance.EnemyUnits;
        List<Unit> enemies = new List<Unit>();

        for (int i = 0; i < units.Length; i++)
        {
            if (units[i] != null) enemies.Add(units[i]);
        }

        return enemies;
    }
}

public class FrontEnemySelector : ITargetSelector
{
    public List<Unit> SelectTargets(Unit caster)
    {
        Unit[] units = BattleManager.Instance.EnemyUnits;
        List<Unit> enemies = new List<Unit>();

        for (int i = 0; i < units.Length; i++)
        {
            if (units[i] != null && i < 2) enemies.Add(units[i]);
        }

        return enemies;
    }
}

public class BackEnemySelector : ITargetSelector
{
    public List<Unit> SelectTargets(Unit caster)
    {
        Unit[] units = BattleManager.Instance.EnemyUnits;
        List<Unit> enemies = new List<Unit>();

        for (int i = 0; i < units.Length; i++)
        {
            if (units[i] != null && i >= 2) enemies.Add(units[i]);
        }

        return enemies;
    }
}

public class RandomEnemySelector : ITargetSelector
{
    private int _count;

    public RandomEnemySelector(int count)
    {
        _count = count;
    }

    public List<Unit> SelectTargets(Unit caster)
    {
        List<Unit> enemies = BattleManager.Instance.EnemyUnits.ToList();
        List<Unit> result = new List<Unit>();

        for (int i = 0; i < _count && enemies.Count > 0; i++)
        {
            int index = Random.Range(0, enemies.Count);
            if (enemies[index] == null) continue;
            result.Add(enemies[index]);
            enemies.RemoveAt(index);
        }

        return result;
    }
}

public class LowestHpEnemySelector : ITargetSelector
{
    public List<Unit> SelectTargets(Unit caster)
    {
        Unit target = null;
        float minHp = float.MaxValue;

        foreach (Unit enemy in BattleManager.Instance.EnemyUnits)
        {
            if (enemy == null) continue;

            if (enemy.UnitData.CurrentHealth < minHp)
            {
                minHp = enemy.UnitData.CurrentHealth;
                target = enemy;
            }
        }

        return new List<Unit> { target };
    }
}
