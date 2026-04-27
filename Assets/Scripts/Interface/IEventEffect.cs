using UnityEngine;

public interface IEventEffect
{
    void Execute();
}

public class GainGold : IEventEffect
{
    private int _amount;

    public GainGold(int amount)
    {
        _amount = amount;
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

    public void Execute()
    {
        GameManager.Instance.AddUnit(_unitType);
        StartManager.Instance.PlayRecruitAnimation(_unitType);
    }
}
