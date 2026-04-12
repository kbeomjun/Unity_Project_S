using UnityEngine;

public enum CardType
{
    Buff,
    Debuff
}

public class CardData
{
    public string Name { get; set; }
    public int Cost { get; set; }
    public int Value { get; set; }
    public string Description { get; set; }
    public bool NeedTarget { get; set; }

    public CardType Type { get; set; }

    public CardData(string name, int cost, int value, string description, bool needTarget, CardType type)
    {
        Name = name;
        Cost = cost;
        Value = value;
        Description = description;
        Type = type;
    }

    public CardData(CardData data)
    {
        Name = data.Name;
        Cost = data.Cost;
        Value = data.Value;
        Description = data.Description;
        Type = data.Type;
    }

}
