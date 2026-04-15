using System.Collections.Generic;
using UnityEngine;

public enum CardType
{
    Buff,
    Debuff,
    Skill
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
    public List<IEffect> Effects { get; set; }
    public CardType Type { get; set; }
    public TargetType TargetType { get; set; }
    public int Index { get; set; }
    public Sprite Image { get; set; }

    public CardData(string name, int cost, CardType type, TargetType targetType, int index, string description)
    {
        Name = name;
        Cost = cost;
        Description = description;
        Type = type;
        TargetType = targetType;
        Index = index;
        Image = null;
    }

    public CardData(CardData data)
    {
        Name = data.Name;
        Cost = data.Cost;
        Description = data.Description;
        Type = data.Type;
        TargetType = data.TargetType;
        Index = data.Index;
        Image = null;
    }

}
