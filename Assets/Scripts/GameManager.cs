using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Map _map;
    [SerializeField] private List<Unit> _playerUnits = new List<Unit>();

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

        _playerCardDatas.Add(DataManager.Instance.CardDatas[0]);
        _playerCardDatas.Add(DataManager.Instance.CardDatas[1]);
        _playerCardDatas.Add(DataManager.Instance.CardDatas[2]);
        _playerCardDatas.Add(DataManager.Instance.CardDatas[3]);
        _playerCardDatas.Add(DataManager.Instance.CardDatas[4]);
        _playerCardDatas.Add(DataManager.Instance.CardDatas[5]);
        _playerCardDatas.Add(DataManager.Instance.CardDatas[6]);
        _playerCardDatas.Add(DataManager.Instance.CardDatas[7]);
        _playerCardDatas.Add(DataManager.Instance.CardDatas[8]);
        _playerCardDatas.Add(DataManager.Instance.CardDatas[9]);

        //BattleManager.Instance.StartBattle(_playerUnitDatas, _playerCardDatas);

        OnBattleEnd(true);
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

    public void OnBattleEnd(bool isWin)
    {
        if (isWin)
        {
            RewardManager.Instance.ShowRewards(NodeType.Battle);
        }
        else
        {

        }
    }

}
