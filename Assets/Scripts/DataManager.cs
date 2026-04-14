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
        new CardData("ChangeAction", 1, $"Change unit's next action: <color=#FF5555>Attack</color>", 
                        new ChangeActionEffect(UnitAction.Attack), CardType.ChangeAction, TargetType.Ally),

        new CardData("ChangeAction", 1, $"Change unit's next action: <color=#FF5555>Defense</color>",
                        new ChangeActionEffect(UnitAction.Defense), CardType.ChangeAction, TargetType.Ally),

        new CardData("ChangeAction", 1, $"Change unit's next action: <color=#FF5555>Skill</color>",
                        new ChangeActionEffect(UnitAction.Skill), CardType.ChangeAction, TargetType.Ally),

        new CardData("Attack", 1, $"Add <color=#FF5555>10</color> attack to my unit this turn",
                        new AttackEffect(10), CardType.Attack, TargetType.Ally),

        new CardData("Defense", 1, $"Add <color=#FF5555>10</color> defense to my unit this turn",
                        new DefenseEffect(10), CardType.Defense, TargetType.Ally),

        new CardData("FrontDefense", 2, $"Add <color=#FF5555>10</color> defense to my units in front line this turn",
                        new FrontDefenseEffect(10), CardType.FrontDefense, TargetType.None),

        new CardData("BackAttack", 2, $"Add <color=#FF5555>10</color> attack to my units in back line this turn",
                        new BackAttackEffect(10), CardType.BackAttack, TargetType.None),

        new CardData("HealAll", 3, $"Restore all my units <color=#FF5555>10%</color> of their maximum health",
                        new HealAllEffect(10), CardType.HealAll, TargetType.None),

        new CardData("ReduceAttack", 2, $"Reduce the attack power of enemy unit by <color=#FF5555>50%</color>",
                        new ReduceAttackEffect(50), CardType.ReduceAttack, TargetType.Enemy),

        new CardData("ResetAllEnemyAction", 2, $"Reset all next actions of enemy units",
                        new ResetAllEnemyActionEffect(), CardType.ResetAllEnemyAction, TargetType.None)

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
