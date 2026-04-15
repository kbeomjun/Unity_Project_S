using System.Collections.Generic;
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
    public int MaxHealth { get; set; }
    public int CurrentHealth { get; set; }
    public int Attack { get; set; }
    public int Defense { get; set; }
    public int SlotIndex { get; set; }
    public ISkill AttackAction;
    public ISkill DefenseAction;
    public ISkill SkillAction;

    public UnitType Type { get; set; }

    public UnitData(string name, int maxHealth, int currentHealth, int attack, int defense, UnitType type)
    {
        Name = name;
        MaxHealth = maxHealth;
        CurrentHealth = currentHealth;
        Attack = attack;
        Defense = defense;
        Type = type;
        SlotIndex = -1;
    }

    public UnitData(UnitData data)
    {
        Name = data.Name;
        MaxHealth = data.MaxHealth;
        CurrentHealth = data.CurrentHealth;
        Attack = data.Attack;
        Defense = data.Defense;
        Type = data.Type;
        SlotIndex = -1;
    }

}
