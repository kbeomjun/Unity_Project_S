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

public class CardData
{
    public string Name { get; set; }
    public int Cost { get; set; }
    public string Description { get; set; }
    public bool NeedTarget { get; set; }
    public bool TargetType { get; set; }
    public ICardEffect Effect { get; set; }
    public Sprite Image { get; set; }

    public CardType Type { get; set; }

    public CardData(string name, int cost, string description, bool needTarget, bool targetType, 
                    ICardEffect effect, Sprite image, CardType type)
    {
        Name = name;
        Cost = cost;
        Description = description;
        NeedTarget = needTarget;
        TargetType = targetType;
        Effect = effect;
        Image = image;
        Type = type;
    }

    public CardData(CardData data)
    {
        Name = data.Name;
        Cost = data.Cost;
        Description = data.Description;
        NeedTarget = data.NeedTarget;
        TargetType = data.TargetType;
        Effect = data.Effect;
        Image = data.Image;
        Type = data.Type;
    }

}
