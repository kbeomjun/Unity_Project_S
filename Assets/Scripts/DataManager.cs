using System;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    // Unit
    [SerializeField] public Unit[] PlayerUnitPrefabs;
    [SerializeField] public Unit[] EnemyUnitPrefabs;
    public UnitData[] UnitDatas = new UnitData[]
    {
        new UnitData("Knight", UnitType.Knight, 90, 90, 1, 30, 0, 100, 100,
                        "Reduce 50% damage, Takes all attack"),
        new UnitData("Lancer", UnitType.Lancer, 70, 70, 2, 20, 0, 100, 100,
                        "Reflex 10 damage to attacker"),
        new UnitData("Archer", UnitType.Archer, 50, 50, 3, 10, 0, 100, 100,
                        "Increase attack 100%"),
        new UnitData("Monk", UnitType.Monk, 30, 30, 5, 5, 0, 100, 100,
                        "Heal random ally unit by 10%")
    };
    public IUnitAction UnitAttack = new Attack(new List<Func<IEffect>> { () => new AttackEffect(new RandomAttackSelector()) });
    public IUnitAction UnitDefense = new Defense(new List<Func<IEffect>> { () => new AddDefenseEffect(-1, new SelfTargetSelector()) });
    public IUnitAction[] UnitSkills = new IUnitAction[]
    {
        new Skill(new List<Func<IEffect>>{
            () => new AddDefenseEffect(30, new SelfTargetSelector()),
            () => new ApplyStatusEffect(StatusType.IronWall, 1, new SelfTargetSelector())
        }),
        new Skill(new List<Func<IEffect>>{
            () => new ApplyStatusEffect(StatusType.Brace, 1, new SelfTargetSelector())
        }),
        new Skill(new List<Func<IEffect>>{
            () => new ApplyStatusSelfEffect(StatusType.Focus, 1, new RandomAttackSelector())
        }),
        new Skill(new List<Func<IEffect>>{
            () => new HealEffect(new RandomTeamSelector(1))
        })
    };
    [SerializeField] public Sprite[] NextActionSprites;
    [SerializeField] public Sprite[] StatusSprites;
    [SerializeField] public Sprite[] UnitSprites;
    [SerializeField] public UnitSprite UnitSpritePrefab;

    // Card
    [SerializeField] public Card CardPrefab;
    [SerializeField] public Sprite[] CardImages;
    public CardData[] CardDatas = new CardData[]
    {
        new CardData("ChangeAction", 1, CardType.Skill, TargetType.Ally, 0, 50,
                        $"Change unit's next action: <color=#008844>Attack</color>"),
        
        new CardData("ChangeAction", 1, CardType.Skill, TargetType.Ally, 1, 50,
                        $"Change unit's next action: <color=#008844>Defense</color>"),
        
        new CardData("ChangeAction", 1, CardType.Skill, TargetType.Ally, 2, 50,
                        $"Change unit's next action: <color=#008844>Skill</color>"),

        new CardData("Focus", 1, CardType.Buff, TargetType.Ally, 3, 50,
                        $"Inflicts a <color=#008844>Focus</color> status on the selected unit"),

        new CardData("Defense", 1, CardType.Buff, TargetType.Ally, 4, 50,
                        $"Add <color=#008844>10</color> defense to my unit this turn"),

        new CardData("FrontDefense", 2, CardType.Buff, TargetType.None, 5, 50,
                        $"Add <color=#008844>10</color> defense to my units in front line this turn"),

        new CardData("Load Aim Fire", 2, CardType.Buff, TargetType.None, 6, 50,
                        $"Inflicts a <color=#008844>Focus</color> status to my units in back line"),

        new CardData("HealAll", 3, CardType.Skill, TargetType.None, 7, 50,
                        $"Restore all my units <color=#008844>10%</color> of their maximum health"),

        new CardData("Weakness", 2, CardType.Debuff, TargetType.Enemy, 8, 50,
                        $"Inflicts a <color=#008844>Weak</color> status on the selected enemy"),

        new CardData("ResetAllEnemyAction", 2, CardType.Skill, TargetType.None, 9, 50,
                        $"Reset all next actions of enemy units")
    };
    public List<Func<IEffect>>[] CardEffects = new List<Func<IEffect>>[]
    {
        new List<Func<IEffect>>{ () => new ChangeActionEffect(UnitAction.Attack, new SingleTargetSelector()) },
        new List<Func<IEffect>>{ () => new ChangeActionEffect(UnitAction.Defense, new SingleTargetSelector()) },
        new List<Func<IEffect>>{ () => new ChangeActionEffect(UnitAction.Skill, new SingleTargetSelector()) },
        new List<Func<IEffect>>{ () => new ApplyStatusEffect(StatusType.Focus, 1, new SingleTargetSelector()) },
        new List<Func<IEffect>>{ () => new AddDefenseEffect(10, new SingleTargetSelector()) },
        new List<Func<IEffect>>{ () => new AddDefenseEffect(10, new FrontAllySelector()) },
        new List<Func<IEffect>>{ () => new ApplyStatusEffect(StatusType.Focus, 1, new BackAllySelector()) },
        new List<Func<IEffect>>{ () => new HealByPercentageEffect(0.1f, new AllAllySelector()) },
        new List<Func<IEffect>>{ () => new ApplyStatusEffect(StatusType.Weak, 1, new SingleTargetSelector()) },
        new List<Func<IEffect>>{ () => new ResetActionEffect(new AllEnemySelector()) }
    };

    //Reward
    [SerializeField] public Sprite[] RewardItemSprites;

    //Effect
    [SerializeField] public UnitEffect HitEffect;
    [SerializeField] public UnitEffect BlockEffect;
    [SerializeField] public UnitEffect DieEffect;
    [SerializeField] public UnitEffect HealEffect;
    [SerializeField] public UnitEffect BuffEffect;
    [SerializeField] public UnitEffect DebuffEffect;

    //EventButton
    [SerializeField] public EventButton EventButtonPrefab;

    public static DataManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

}
