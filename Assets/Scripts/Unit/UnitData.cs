using UnityEngine;

public enum UnitType
{
    Knight,
    Lancer,
    Archer,
    Monk
}

public class UnitData
{
    public string Name { get; set; }
    public UnitType Type { get; set; }
    public int MaxHealth { get; set; }
    public int CurrentHealth { get; set; }
    public int Attack { get; set; }
    public int Defense { get; set; }
    public int SlotIndex { get; set; }
    public string SkillDescription { get; set; }
    public int Upgrade { get; set; }
    public int UpgradeCoin { get; set; }
    public int SellCoin { get; set; }
    public int Index { get; set; }
    public IUnitAction AttackAction;
    public IUnitAction DefenseAction;
    public IUnitAction SkillAction;

    public UnitData(string name, UnitType type, int maxHealth, int currentHealth, int attack, int defense, 
                    int upgrade, int upgradeCoin, int sellCoin, string skillDescription)
    {
        Name = name;
        Type = type;
        MaxHealth = maxHealth;
        CurrentHealth = currentHealth;
        Attack = attack;
        Defense = defense;
        Upgrade = upgrade;
        UpgradeCoin = upgradeCoin;
        SellCoin = sellCoin;
        SkillDescription = skillDescription;
        SlotIndex = -1;
    }

    public UnitData(UnitData data)
    {
        Name = data.Name;
        Type = data.Type;
        MaxHealth = data.MaxHealth;
        CurrentHealth = data.CurrentHealth;
        Attack = data.Attack;
        Defense = data.Defense;
        Upgrade = data.Upgrade;
        UpgradeCoin = data.UpgradeCoin;
        SellCoin = data.SellCoin;
        SkillDescription = data.SkillDescription;
        SlotIndex = -1;
    }

    public void UpgradeUnit()
    {
        Upgrade++;
        CurrentHealth += (int)(MaxHealth * 0.1f);
        MaxHealth = (int)(MaxHealth * (1 + 0.1f));
        Attack = (Attack < 10) ? (Attack + 1) : (int)(Attack * (1 + 0.1f));
        Defense = (Defense < 10) ? (Defense + 1) : (int)(Defense * (1 + 0.1f));
        UpgradeCoin = (int)(UpgradeCoin * (1 + 0.2f));
        SellCoin = (int)(SellCoin * (1 + 0.05f));
    }

}
