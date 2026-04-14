using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    // Unit
    [SerializeField] public Unit[] PlayerUnitPrefabs;
    [SerializeField] public Unit[] EnemyUnitPrefabs;
    public UnitData[] UnitData = new UnitData[]
    {
        new UnitData("Knight", 110, 110, 10, 30, UnitType.Knight),
        new UnitData("Lancer", 90, 90, 20, 20, UnitType.Lancer),
        new UnitData("Archer", 70, 70, 30, 10, UnitType.Archer),
        new UnitData("Monk", 50, 50, 5, 5, UnitType.Monk)
    };

    // Card
    [SerializeField] public Card CardPrefab;
    [SerializeField] public Sprite[] CardImages;
    public CardData[] CardDatas = new CardData[]
    {
        new CardData("ChangeAction", 1, CardType.Skill, TargetType.Ally, 0, 
                        $"Change unit's next action: <color=#FF5555>Attack</color>"),
        
        new CardData("ChangeAction", 1, CardType.Skill, TargetType.Ally, 1, 
                        $"Change unit's next action: <color=#FF5555>Defense</color>"),
        
        new CardData("ChangeAction", 1, CardType.Skill, TargetType.Ally, 2, 
                        $"Change unit's next action: <color=#FF5555>Skill</color>"),

        new CardData("Attack", 1, CardType.Buff, TargetType.Ally, 3, 
                        $"Add <color=#FF5555>10</color> attack to my unit this turn"),

        new CardData("Defense", 1, CardType.Buff, TargetType.Ally, 4, 
                        $"Add <color=#FF5555>10</color> defense to my unit this turn"),

        new CardData("FrontDefense", 2, CardType.Buff, TargetType.None, 5, 
                        $"Add <color=#FF5555>10</color> defense to my units in front line this turn"),

        new CardData("BackAttack", 2, CardType.Buff, TargetType.None, 6, 
                        $"Add <color=#FF5555>10</color> attack to my units in back line this turn"),

        new CardData("HealAll", 3, CardType.Skill, TargetType.None, 7, 
                        $"Restore all my units <color=#FF5555>10%</color> of their maximum health"),

        new CardData("ReduceAttack", 2, CardType.Debuff, TargetType.Enemy, 8, 
                        $"Reduce the attack power of enemy unit by <color=#FF5555>50%</color>"),

        new CardData("ResetAllEnemyAction", 2, CardType.Skill, TargetType.None, 9, 
                        $"Reset all next actions of enemy units")
    };

    public List<ICardEffect>[] CardEffects = new List<ICardEffect>[]
    {
        new List<ICardEffect>{ new ChangeActionEffect(UnitAction.Attack, new SingleTargetSelector()) },
        new List<ICardEffect>{ new ChangeActionEffect(UnitAction.Defense, new SingleTargetSelector()) },
        new List<ICardEffect>{ new ChangeActionEffect(UnitAction.Skill, new SingleTargetSelector()) },
        new List<ICardEffect>{ new AddAttackEffect(10, new SingleTargetSelector()) },
        new List<ICardEffect>{ new AddDefenseEffect(10, new SingleTargetSelector()) },
        new List<ICardEffect>{ new AddDefenseEffect(10, new FrontAllySelector()) },
        new List<ICardEffect>{ new AddAttackEffect(10, new BackAllySelector()) },
        new List<ICardEffect>{ new HealByPercentageEffect(10, new AllAllySelector()) },
        new List<ICardEffect>{ new ReduceAttackByPercentageEffect(50, new SingleTargetSelector()) },
        new List<ICardEffect>{ new ResetActionEffect(new AllEnemySelector()) }
    };

    public static DataManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

}
