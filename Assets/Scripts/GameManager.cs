using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Map _map;
    [SerializeField] private TMP_Text _coinText;
    [SerializeField] private TMP_Text _partyNumText;
    [SerializeField] private TMP_Text _cardNumText;
    [SerializeField] private RectTransform _cardPopupContentTr;

    private int _maxChapter = 0;
    private int _currentChapter = 0;
    private int _maxLayer = 0;
    private int _currentLayer = 0;
    private Node _currentNode = null;
    private int _prevCoin = 0;
    private int _currentCoin = 0;
    public int CurrentCoin 
    {
        get => _currentCoin;
        set => _currentCoin = value;
    }

    private List<UnitData> _playerUnitDatas = new List<UnitData>();
    private List<CardData> _playerCardDatas = new List<CardData>();

    public List<UnitData> PlayerUnitDatas => _playerUnitDatas;

    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        _maxChapter = _map.MaxChapter;
        TownManager.Instance.CardDeleteCoin = 100;

        //StartGame();

        _playerUnitDatas.Add(new UnitData(DataManager.Instance.UnitData[0]));
        //_playerUnitDatas.Add(new UnitData(DataManager.Instance.UnitData[1]));
        _playerUnitDatas.Add(new UnitData(DataManager.Instance.UnitData[2]));
        //_playerUnitDatas.Add(new UnitData(DataManager.Instance.UnitData[3]));

        _playerCardDatas.Add(new CardData(DataManager.Instance.CardDatas[0]));
        _playerCardDatas.Add(new CardData(DataManager.Instance.CardDatas[1]));
        _playerCardDatas.Add(new CardData(DataManager.Instance.CardDatas[2]));
        _playerCardDatas.Add(new CardData(DataManager.Instance.CardDatas[3]));
        _playerCardDatas.Add(new CardData(DataManager.Instance.CardDatas[4]));
        _playerCardDatas.Add(new CardData(DataManager.Instance.CardDatas[5]));
        _playerCardDatas.Add(new CardData(DataManager.Instance.CardDatas[6]));
        _playerCardDatas.Add(new CardData(DataManager.Instance.CardDatas[7]));
        _playerCardDatas.Add(new CardData(DataManager.Instance.CardDatas[8]));
        _playerCardDatas.Add(new CardData(DataManager.Instance.CardDatas[9]));

        //StartBattle();
        StartTown();
    }

    private void StartGame()
    {
        _currentChapter = 0;
        _maxLayer = _map.MaxLayer[_currentChapter];
        _currentCoin = 80;
        _prevCoin = _currentCoin;

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
                StartBattle();
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

    private void StartBattle()
    {
        BattleManager.Instance.StartBattle(_playerUnitDatas, _playerCardDatas);
        ViewManager.Instance.ShowBattleView();
    }

    private void StartTown()
    {
        _currentCoin = 101;
        TownManager.Instance.StartTown();
        ViewManager.Instance.ShowTownView();
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
            if(_currentChapter >= _maxChapter)
            {
                Debug.Log("Game Clear");
                return;
            }
            
            Debug.Log("Chapter Clear");
            _currentChapter++;
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
            ViewManager.Instance.ShowGameOverPopup();
        }
    }

    public void OnClickCardButton()
    {
        ViewManager.Instance.ShowRemoveCardPopup();
    }

    public void AddUnit(int unitType)
    {
        _playerUnitDatas.Add(new UnitData(DataManager.Instance.UnitData[unitType]));
    }

    public void AddCard(CardData data)
    {
        _playerCardDatas.Add(new CardData(data));
    }

    public void RemoveCard(CardData data)
    {
        _playerCardDatas.Remove(data);
    }

    private void UpdateTopBar()
    {
        _coinText.text = _currentCoin.ToString();
        _partyNumText.text = _playerUnitDatas.Count.ToString();
        _cardNumText.text = _playerCardDatas.Count.ToString();
    }

    private void Update()
    {
        UpdateTopBar();
    }

}
