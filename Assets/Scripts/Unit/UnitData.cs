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
    public string SkillDescription { get; set; }
    public int Upgrade { get; set; }
    public int SlotIndex { get; set; }
    public IUnitAction AttackAction;
    public IUnitAction DefenseAction;
    public IUnitAction SkillAction;


    public UnitData(string name, UnitType type, int maxHealth, int currentHealth, int attack, int defense, int upgrade, string skillDescription)
    {
        Name = name;
        Type = type;
        MaxHealth = maxHealth;
        CurrentHealth = currentHealth;
        Attack = attack;
        Defense = defense;
        Upgrade = upgrade;
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
        SkillDescription = data.SkillDescription;
        SlotIndex = -1;
    }

}
