using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Map _map;
    [SerializeField] List<Unit> _playerUnits = new List<Unit>();
    [SerializeField] List<Sprite> _cardImages = new List<Sprite>();

    private int _maxChapter;
    private int _currentChapter;
    private int _maxLayer;
    private int _currentLayer;
    private Node _currentNode;

    private List<UnitData> _playerUnitDatas = new List<UnitData>();
    private List<CardData> _playerCardDatas = new List<CardData>();

    public static GameManager Instance { get; private set; }
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

    private void Start()
    {
        _maxChapter = _map.MaxChapter;

        //StartGame();

        foreach(Unit unit in _playerUnits)
        {
            if(unit is Knight)
            {
                _playerUnitDatas.Add(new UnitData("Knight", 110, 60, 10, 30, UnitType.Knight));
            } 
            else if(unit is Lancer)
            {
                _playerUnitDatas.Add(new UnitData("Lancer", 90, 45, 20, 20, UnitType.Lancer));
            }
            else if (unit is Archer)
            {
                _playerUnitDatas.Add(new UnitData("Archer", 70, 60, 30, 10, UnitType.Archer));
            }
            else if (unit is Monk)
            {
                _playerUnitDatas.Add(new UnitData("Monk", 50, 40, 5, 5, UnitType.Monk));
            }
        }

        {
            _playerCardDatas.Add(
                new CardData("ChangeAction", 1, $"Change unit's next action: <color=#FF5555>Attack</color>",
                                true, true, new ChangeActionEffect(UnitAction.Attack),
                                _cardImages[(int)CardType.ChangeAction], CardType.ChangeAction)
            );

            _playerCardDatas.Add(
                new CardData("ChangeAction", 1, $"Change unit's next action: <color=#FF5555>Defense</color>",
                                true, true, new ChangeActionEffect(UnitAction.Defense),
                                _cardImages[(int)CardType.ChangeAction], CardType.ChangeAction)
            );

            _playerCardDatas.Add(
                new CardData("ChangeAction", 1, $"Change unit's next action: <color=#FF5555>Skill</color>",
                                true, true, new ChangeActionEffect(UnitAction.Skill),
                                _cardImages[(int)CardType.ChangeAction], CardType.ChangeAction)
            );

            _playerCardDatas.Add(
                new CardData("Attack", 1, $"Add <color=#FF5555>10</color> attack to my unit this turn",
                                true, true, new AttackEffect(10),
                                _cardImages[(int)CardType.Attack], CardType.Attack)
            );

            _playerCardDatas.Add(
                new CardData("Defense", 1, $"Add <color=#FF5555>10</color> defense to my unit this turn",
                                true, true, new DefenseEffect(10),
                                _cardImages[(int)CardType.Defense], CardType.Defense)
            );

            _playerCardDatas.Add(
                new CardData("FrontDefense", 2, $"Add <color=#FF5555>10</color> defense to my units in front line this turn",
                                false, false, new FrontDefenseEffect(10),
                                _cardImages[(int)CardType.FrontDefense], CardType.FrontDefense)
            );

            _playerCardDatas.Add(
                new CardData("BackAttack", 2, $"Add <color=#FF5555>10</color> attack to my units in back line this turn",
                                false, false, new BackAttackEffect(10),
                                _cardImages[(int)CardType.BackAttack], CardType.BackAttack)
            );

            _playerCardDatas.Add(
                new CardData("HealAll", 3, $"Restore all my units <color=#FF5555>10</color>% of their maximum health",
                                false, false, new HealAllEffect(10),
                                _cardImages[(int)CardType.HealAll], CardType.HealAll)
            );

            _playerCardDatas.Add(
                new CardData("ReduceAttack", 2, $"Reduce the attack power of enemy unit by <color=#FF5555>50</color>%",
                                true, false, new ReduceAttackEffect(50),
                                _cardImages[(int)CardType.ReduceAttack], CardType.ReduceAttack)
            );

            _playerCardDatas.Add(
                new CardData("ResetAllEnemyAction", 2, $"Reset all next actions of enemy units",
                                false, false, new ResetAllEnemyActionEffect(),
                                _cardImages[(int)CardType.ResetAllEnemyAction], CardType.ResetAllEnemyAction)
            );
        }

        BattleManager.Instance.StartBattle(_playerUnitDatas, _playerCardDatas);
    }

    private void StartGame()
    {
        _currentChapter = 0;
        _maxLayer = _map.MaxLayer[_currentChapter];

        StartChapter();
    }

    private void StartChapter()
    {
        _currentLayer = 0;
        _map.CreateMap(_currentChapter);
        _map.Nodes[_currentChapter][_currentLayer][0].SetState(NodeState.Available);
    }

    public void OnClickNode(Node node)
    {
        _currentNode = node;
        _currentNode.SetState(NodeState.Selected);

        for(int i = 0; i < _map.Nodes[_currentChapter][_currentLayer].Length; i++)
        {
            if (i == _currentNode.Index) continue;

            _map.Nodes[_currentChapter][_currentLayer][i].SetState(NodeState.Locked);
        }

        switch (_currentNode.Type)
        {
            case NodeType.Start:
                break;

            case NodeType.Battle:
                BattleManager.Instance.StartBattle(_playerUnitDatas, _playerCardDatas);
                break;

            case NodeType.Elite:
                break;

            case NodeType.Shop:
                break;

            case NodeType.Rest:
                break;

            case NodeType.Event:
                break;

            case NodeType.Boss:
                break;
        }

        OnClearNode();
    }

    private void OnClearNode()
    {
        foreach(Node node in _currentNode.NextNode)
        {
            node.SetState(NodeState.Available);
        }

        _currentLayer++;

        if (_currentLayer >= _maxLayer)
        {
            Debug.Log("Chapter Clear");
            _currentChapter++;

            if(_currentChapter >= _maxChapter)
            {
                Debug.Log("Game Clear");
                return;
            }

            StartChapter();
        }
    }

}
