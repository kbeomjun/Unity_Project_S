using System.Collections.Generic;
using UnityEngine;

public enum CardType
{
    // ¾Æ±º Å¸°Ù
    ChangeAction, Attack, Defense,
    // ¾Æ±º ³íÅ¸°Ù
    FrontDefense, BackAttack, HealAll,
    // Àû Å¸°Ù
    ReduceAttack,
    // Àû ³íÅ¸°Ù
    ResetAllEnemyAction,
}

public enum TargetType
{
    None,           
    Enemy,    
    Ally       
}

public class CardData
{
    public string Name { get; set; }
    public int Cost { get; set; }
    public string Description { get; set; }
    public List<ICardEffect> Effects { get; set; }
    public ICardEffect Effect { get; set; }
    public Sprite Image { get; set; }

    public CardType Type { get; set; }
    public TargetType TargetType { get; set; }

    public CardData(string name, int cost, string description, ICardEffect effect, CardType type, TargetType targetType)
    {
        Name = name;
        Cost = cost;
        Description = description;
        Effect = effect;
        Type = type;
        TargetType = targetType;
        Image = null;
    }

    public CardData(CardData data)
    {
        Name = data.Name;
        Cost = data.Cost;
        Description = data.Description;
        Effect = data.Effect;
        Type = data.Type;
        TargetType = data.TargetType;
        Image = null;
    }

}
