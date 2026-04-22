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
        new UnitData("Knight", UnitType.Knight, 500, 500, 10, 30, 0, 100, 100,
                        "Reduce 50% damage, Takes all attack"),
        new UnitData("Lancer", UnitType.Lancer, 500, 500, 20, 20, 0, 100, 100,
                        "Reflex 10 damage to attacker"),
        new UnitData("Archer", UnitType.Archer, 500, 500, 30, 10, 0, 100, 100,
                        "Increase attack 100%"),
        new UnitData("Monk", UnitType.Monk, 500, 500, 20, 5, 0, 100, 100,
                        "Heal random ally unit by 10%")
    };
    public IUnitAction UnitAttack = new Attack(
       new List<Func<IEffect>>
       {
            () => new AttackEffect(new RandomAttackSelector())
       });
    public IUnitAction UnitDefense = new Defense(
        new List<Func<IEffect>>
        {
            () => new AddDefenseEffect(-1, new SelfTargetSelector())
        });
    public IUnitAction[] UnitSkills = new IUnitAction[]
    {
        new Skill(new List<Func<IEffect>>{
            () => new AddDefenseEffect(30, new SelfTargetSelector()),
            () => new ApplyStatusEffect(() => new DamageReductionStatus(0.5f, 1), new SelfTargetSelector())
        }),
        new Skill(new List<Func<IEffect>>{
            () => new ApplyStatusEffect(() => new DamageReflectionStatus(10, 1), new SelfTargetSelector())
        }),
        new Skill(new List<Func<IEffect>>{
            () => new ApplyStatusSelfEffect(() => new AttackBuffStatus(1.0f, 1), new RandomAttackSelector())
        }),
        new Skill(new List<Func<IEffect>>{
            () => new HealEffect(new RandomTeamSelector(1))
        })
    };

    // Card
    [SerializeField] public Card CardPrefab;
    [SerializeField] public Sprite[] CardImages;
    public CardData[] CardDatas = new CardData[]
    {
        new CardData("ChangeAction", 1, CardType.Skill, TargetType.Ally, 0, 50,
                        $"Change unit's next action: <color=#FF5555>Attack</color>"),
        
        new CardData("ChangeAction", 1, CardType.Skill, TargetType.Ally, 1, 50,
                        $"Change unit's next action: <color=#FF5555>Defense</color>"),
        
        new CardData("ChangeAction", 1, CardType.Skill, TargetType.Ally, 2, 50,
                        $"Change unit's next action: <color=#FF5555>Skill</color>"),

        new CardData("Attack", 1, CardType.Buff, TargetType.Ally, 3, 50,
                        $"Add <color=#FF5555>10</color> attack to my unit this turn"),

        new CardData("Defense", 1, CardType.Buff, TargetType.Ally, 4, 50,
                        $"Add <color=#FF5555>10</color> defense to my unit this turn"),

        new CardData("FrontDefense", 2, CardType.Buff, TargetType.None, 5, 50,
                        $"Add <color=#FF5555>10</color> defense to my units in front line this turn"),

        new CardData("BackAttack", 2, CardType.Buff, TargetType.None, 6, 50,
                        $"Add <color=#FF5555>10</color> attack to my units in back line this turn"),

        new CardData("HealAll", 3, CardType.Skill, TargetType.None, 7, 50,
                        $"Restore all my units <color=#FF5555>10%</color> of their maximum health"),

        new CardData("ReduceAttack", 2, CardType.Debuff, TargetType.Enemy, 8, 50,
                        $"Reduce the attack power of enemy unit by <color=#FF5555>50%</color>"),

        new CardData("ResetAllEnemyAction", 2, CardType.Skill, TargetType.None, 9, 50,
                        $"Reset all next actions of enemy units")
    };
    public List<Func<IEffect>>[] CardEffects = new List<Func<IEffect>>[]
    {
        new List<Func<IEffect>>{ () => new ChangeActionEffect(UnitAction.Attack, new SingleTargetSelector()) },
        new List<Func<IEffect>>{ () => new ChangeActionEffect(UnitAction.Defense, new SingleTargetSelector()) },
        new List<Func<IEffect>>{ () => new ChangeActionEffect(UnitAction.Skill, new SingleTargetSelector()) },
        new List<Func<IEffect>>{ () => new AddAttackEffect(10, new SingleTargetSelector()) },
        new List<Func<IEffect>>{ () => new AddDefenseEffect(10, new SingleTargetSelector()) },
        new List<Func<IEffect>>{ () => new AddDefenseEffect(10, new FrontAllySelector()) },
        new List<Func<IEffect>>{ () => new AddAttackEffect(10, new BackAllySelector()) },
        new List<Func<IEffect>>{ () => new HealByPercentageEffect(0.1f, new AllAllySelector()) },
        new List<Func<IEffect>>{ () => new ApplyStatusEffect(() => new AttackReductionStatus(0.5f, 1), new SingleTargetSelector()) },
        new List<Func<IEffect>>{ () => new ResetActionEffect(new AllEnemySelector()) }
    };

    //Reward
    [SerializeField] public Sprite[] RewardItemSprites;
    [SerializeField] public Sprite[] UnitSprites;

    //Effect
    [SerializeField] public UnitEffect HitEffect;
    [SerializeField] public UnitEffect BlockEffect;

    public static DataManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

}
