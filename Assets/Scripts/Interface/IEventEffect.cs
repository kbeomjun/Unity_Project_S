using UnityEngine;

public class EffectResult
{
    public bool CanExecute;
    public string Reason;

    public EffectResult(bool canExecute, string reason = "")
    {
        CanExecute = canExecute;
        Reason = reason;
    }
}

public interface IEventEffect
{
    EffectResult Evaluate();
    void Execute();
}

public class GainGold : IEventEffect
{
    private int _amount;

    public GainGold(int amount)
    {
        _amount = amount;
    }

    public EffectResult Evaluate()
    {
        return new EffectResult(true);
    }

    public void Execute()
    {
        GameManager.Instance.CurrentCoin += _amount;
    }
}

public class AddUnit : IEventEffect
{
    private int _unitType;

    public AddUnit(int unitType)
    {
        _unitType = unitType;
    }

    public EffectResult Evaluate()
    {
        if (GameManager.Instance.PlayerUnitDatas.Count >= 4)
            return new EffectResult(false, "Party is Full");

        return new EffectResult(true);
    }

    public void Execute()
    {
        GameManager.Instance.AddUnit(_unitType);
        GameManager.Instance.PlayRecruitAnimation(_unitType);
    }
}

public class HurtUnit : IEventEffect
{
    private int _damage;
    private bool _isAll;

    public HurtUnit(int damage, bool isAll)
    {
        _damage = damage;
        _isAll = isAll;
    }

    public EffectResult Evaluate()
    {
        return new EffectResult(true);
    }

    public void Execute()
    {
        GameManager.Instance.HurtUnit(_damage, _isAll);
    }
}
