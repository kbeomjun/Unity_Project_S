using UnityEngine;

public enum StatusType
{
    IronWall,       // 받는 피해 -50%
    Brace,          // 받는 피해 50% 반사
    Focus,          // 공격 +X
    Weak,           // 공격 -25%
}

public interface IStatusEffect
{
    StatusType Type { get; }
    int Duration { get; set; }     // 남은 턴
    Sprite Icon { get; }

    void OnApply(Unit target);     // 처음 적용될 때
    void OnRemove(Unit target);    // 제거될 때
    void OnTurnStart(Unit target); // 턴 시작
    void OnTurnEnd(Unit target);   // 턴 종료
}

public class StatusEffect : IStatusEffect
{
    public StatusType Type { get; }
    public int Duration { get; set; }
    public Sprite Icon { get; }

    public StatusEffect(StatusType type, int duration)
    {
        Type = type;
        Duration = duration;
        Icon = DataManager.Instance.StatusSprites[(int)type];
    }

    public void OnApply(Unit target)
    {
        switch (Type)
        {
            case StatusType.IronWall:
                target.HitDamageMultiplier *= 0.5f;
                break;

            case StatusType.Brace:
                target.HitDamageReflection *= 0.5f;
                break;

            case StatusType.Focus:
                target.AttackDamageMultiplier *= 2.0f;
                break;

            case StatusType.Weak:
                target.AttackDamageMultiplier *= 0.75f;
                break;
        }
    }

    public void OnRemove(Unit target)
    {
        switch (Type)
        {
            case StatusType.IronWall:
                target.HitDamageMultiplier /= 0.5f;
                break;

            case StatusType.Brace:
                target.HitDamageReflection -= 50;
                break;

            case StatusType.Focus:
                target.AttackDamageMultiplier /= 2.0f;
                break;

            case StatusType.Weak:
                target.AttackDamageMultiplier /= 0.75f;
                break;
        }
    }

    public void OnTurnStart(Unit target)
    {
        switch (Type)
        {
            case StatusType.IronWall:
                Duration--;
                break;

            case StatusType.Brace:
                Duration--;
                break;
        }
    }

    public void OnTurnEnd(Unit target)
    {
        switch (Type)
        {
            case StatusType.Focus:
                Duration--;
                break;

            case StatusType.Weak:
                Duration--;
                break;
        }
    }

}
