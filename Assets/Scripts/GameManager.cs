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

    private UnitCollectionUI _unitCollectionUI;
    private CardCollectionUI _cardCollectionUI;
    public UnitCollectionUI UnitCollectionUI => _unitCollectionUI;
    public CardCollectionUI CardCollectionUI => _cardCollectionUI;

    private int _maxChapter = 0;
    private int _currentChapter = 0;
    private int _maxLayer = 0;
    private int _currentLayer = 0;
    private Node _currentNode = null;
    private int _prevCoin = 0;
    private int _currentCoin = 10000;
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

        _unitCollectionUI = GetComponent<UnitCollectionUI>();
        _cardCollectionUI = GetComponent<CardCollectionUI>();
    }

    private void Start()
    {
        _maxChapter = _map.MaxChapter;
        TownManager.Instance.CardDeleteCoin = 100;

        _playerUnitDatas.Add(new UnitData(DataManager.Instance.UnitDatas[0]));
        //_playerUnitDatas.Add(new UnitData(DataManager.Instance.UnitDatas[1]));
        _playerUnitDatas.Add(new UnitData(DataManager.Instance.UnitDatas[2]));
        //_playerUnitDatas.Add(new UnitData(DataManager.Instance.UnitDatas[3]));

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

        //StartGame();
        StartBattle();
        //StartTown();
    }

    private void StartGame()
    {
        _currentChapter = 0;
        _maxLayer = _map.MaxLayer[_currentChapter];
        _currentCoin = 100;
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

    public void StartBattle()
    {
        BattleManager.Instance.StartBattle(_playerUnitDatas, _playerCardDatas);
        ViewManager.Instance.ShowBattleView();
    }

    public void StartTown()
    {
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

    public void OnClickUnitCollectionButton(bool isBarrack)
    {
        if (ViewManager.Instance.ShowUnitCollectionPopup())
        {
            _unitCollectionUI.Init(_playerUnitDatas, isBarrack);
            InputManager.Instance.Push(InputState.None);
        }
    }

    public void OnClickCardCollectionButton(bool isRemove)
    {
        if (ViewManager.Instance.ShowCardCollectionPopup())
        {
            _cardCollectionUI.Init(_playerCardDatas, isRemove);
            InputManager.Instance.Push(InputState.CardCollection);
        }
    }

    public void AddUnit(int unitType)
    {
        _playerUnitDatas.Add(new UnitData(DataManager.Instance.UnitDatas[unitType]));
    }

    public void RemoveUnit(int index)
    {
        _playerUnitDatas.RemoveAt(index);
    }

    public void AddCard(CardData data)
    {
        _playerCardDatas.Add(new CardData(data));
    }

    public void RemoveCard(int index)
    {
        _playerCardDatas.RemoveAt(index);
    }

    private void UpdateUnitIndex()
    {
        for (int i = 0; i < _playerUnitDatas.Count; i++)
            _playerUnitDatas[i].Index = i;
    }

    private void UpdateTopBar()
    {
        _coinText.text = _currentCoin.ToString();
        _partyNumText.text = _playerUnitDatas.Count.ToString();
        _cardNumText.text = _playerCardDatas.Count.ToString();
    }

    private void Update()
    {
        UpdateUnitIndex();
        UpdateTopBar();
    }

}
