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

public class GainCoin : IEventEffect
{
    private int _amount;

    public GainCoin(int amount)
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

public class PayCoin : IEventEffect
{
    private int _amount;

    public PayCoin(int amount)
    {
        _amount = amount;
    }

    public EffectResult Evaluate()
    {
        if (GameManager.Instance.CurrentCoin < _amount) 
            return new EffectResult(false, "Not Enough Coin");
        return new EffectResult(true);
    }

    public void Execute()
    {
        GameManager.Instance.CurrentCoin -= _amount;
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

public class UpgradeDefense : IEventEffect
{
    private int _amount;

    public UpgradeDefense(int amount)
    {
        _amount = amount;
     }

    public EffectResult Evaluate()
    {
        return new EffectResult(true);
    }

    public void Execute()
    {
        foreach(UnitData unit in GameManager.Instance.PlayerUnitDatas)
        {
            unit.UpgradeDefense(_amount);
        }
    }
}

public class UpgradeHealth : IEventEffect
{
    private int _amount;

    public UpgradeHealth(int amount)
    {
        _amount = amount;
    }

    public EffectResult Evaluate()
    {
        return new EffectResult(true);
    }

    public void Execute()
    {
        foreach (UnitData unit in GameManager.Instance.PlayerUnitDatas)
        {
            unit.UpgradeHealth(_amount);
        }
    }
}

public class UpgradeAttack : IEventEffect
{
    private int _amount;

    public UpgradeAttack(int amount)
    {
        _amount = amount;
    }

    public EffectResult Evaluate()
    {
        return new EffectResult(true);
    }

    public void Execute()
    {
        foreach (UnitData unit in GameManager.Instance.PlayerUnitDatas)
        {
            unit.UpgradeAttack(_amount);
        }
    }
}

public class DecreaseHealth : IEventEffect
{
    private int _amount;

    public DecreaseHealth(int amount)
    {
        _amount = amount;
    }

    public EffectResult Evaluate()
    {
        return new EffectResult(true);
    }

    public void Execute()
    {
        foreach (UnitData unit in GameManager.Instance.PlayerUnitDatas)
        {
            unit.DecreaseHealth(_amount);
        }
    }
}

public class GetRandomCard : IEventEffect
{
    private int _count;

    public GetRandomCard(int count)
    {
        _count = count;
    }

    public EffectResult Evaluate()
    {
        return new EffectResult(true);
    }

    public void Execute()
    {
        for(int i = 0; i < _count; i++)
        {
            int index = Random.Range(0, DataManager.Instance.CardDatas.Length);
            CardData data = DataManager.Instance.CardDatas[index];
            GameManager.Instance.AddCard(data);
            GameManager.Instance.PlayAddCardAnimation(data);
        }
    }
}
