using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Map _map;

    private int _maxChapter;
    private int _currentChapter;
    private int _maxLayer;
    private int _currentLayer;
    private Node _currentNode;

    [SerializeField] List<Unit> _playerUnits = new List<Unit>();
    private List<UnitData> _playerUnitDatas = new List<UnitData>();

    [SerializeField] List<Card> _playerCards = new List<Card>();
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
                _playerUnitDatas.Add(new UnitData("Knight", 120, 10, 10, 50, UnitType.Knight));
            } 
            else if(unit is Lancer)
            {
                _playerUnitDatas.Add(new UnitData("Lancer", 100, 10, 20, 30, UnitType.Lancer));
            }
            else if (unit is Archer)
            {
                _playerUnitDatas.Add(new UnitData("Archer", 70, 10, 50, 10, UnitType.Archer));
            }
            else if (unit is Monk)
            {
                _playerUnitDatas.Add(new UnitData("Monk", 50, 10, 5, 5, UnitType.Monk));
            }
        }

        foreach (Card card in _playerCards)
        {
            _playerCardDatas.Add(new CardData("Defense", 1, 5, $"Add <color=#FF5555>{5}</color> Defense To Selected Unit", true, CardType.Buff));
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
