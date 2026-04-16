using UnityEngine;

public interface IStatusEffect
{
    void OnApply(Unit target);     // 처음 적용될 때
    void OnRemove(Unit target);    // 제거될 때

    void OnTurnStart(Unit target); // 턴 시작
    void OnTurnEnd(Unit target);   // 턴 종료

    int Duration { get; set; }     // 남은 턴
}

public class DamageReductionStatus : IStatusEffect
{
    private float _reduction;
    public int Duration { get; set; }

    public DamageReductionStatus(float reduction, int duration)
    {
        _reduction = reduction;
        Duration = duration;
    }

    public void OnApply(Unit target)
    {
        target.HitDamageMultiplier *= (1 - _reduction);
    }

    public void OnRemove(Unit target)
    {
        target.HitDamageMultiplier /= (1 - _reduction);
    }

    public void OnTurnStart(Unit target)
    {
        Duration--;
    }

    public void OnTurnEnd(Unit target) { }
}

public class DamageReflectionStatus : IStatusEffect
{
    private int _reflection;
    public int Duration { get; set; }

    public DamageReflectionStatus(int reflection, int duration)
    {
        _reflection = reflection;
        Duration = duration;
    }

    public void OnApply(Unit target)
    {
        target.HitDamageReflection += _reflection;
    }

    public void OnRemove(Unit target)
    {
        target.HitDamageReflection -= _reflection;
    }

    public void OnTurnStart(Unit target) 
    {
        Duration--;
    }

    public void OnTurnEnd(Unit target) { }
}

public class AttackBuffStatus : IStatusEffect
{
    private float _increase;
    public int Duration { get; set; }

    public AttackBuffStatus(float increase, int duration)
    {
        _increase = increase;
        Duration = duration;
    }

    public void OnApply(Unit target)
    {
        target.AttackDamageMultiplier *= (1 + _increase);
    }

    public void OnRemove(Unit target)
    {
        target.AttackDamageMultiplier /= (1 + _increase);
    }

    public void OnTurnStart(Unit target)
    {
        Duration--;
    }

    public void OnTurnEnd(Unit target) { }
}

public class AttackReductionStatus : IStatusEffect
{
    private float _reduction;
    public int Duration { get; set; }

    public AttackReductionStatus(float reduction, int duration)
    {
        _reduction = reduction;
        Duration = duration;
    }

    public void OnApply(Unit target)
    {
        target.AttackDamageMultiplier *= (1 - _reduction);
    }

    public void OnRemove(Unit target)
    {
        target.AttackDamageMultiplier /= (1 - _reduction);
    }

    public void OnTurnStart(Unit target)
    {
        Duration--;
    }

    public void OnTurnEnd(Unit target) { }
}
