using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    [SerializeField] private Map _map;
    [SerializeField] private GameObject _topBar;
    [SerializeField] private TMP_Text _coinText;
    [SerializeField] private TMP_Text _layerText;
    [SerializeField] private TMP_Text _playTimeText;
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
    public Node CurrentNode => _currentNode;
    private int _prevCoin = 0;
    private int _currentCoin = 10000;
    private float _playTime = 0.0f;
    private bool _isGameStart = false;

    public int CurrentCoin 
    {
        get => _currentCoin;
        set
        {
            if (_currentCoin == value) return;
            _prevCoin = _currentCoin;
            _currentCoin = value;
            StopAllCoroutines();
            StartCoroutine(UpdateCoinText());
        }
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
        TownRestManager.Instance.CardDeleteCoin = 100;

        //_playerUnitDatas.Add(new UnitData(DataManager.Instance.UnitDatas[0]));
        //_playerUnitDatas.Add(new UnitData(DataManager.Instance.UnitDatas[1]));
        _playerUnitDatas.Add(new UnitData(DataManager.Instance.UnitDatas[2]));
        _playerUnitDatas.Add(new UnitData(DataManager.Instance.UnitDatas[2]));
        _playerUnitDatas.Add(new UnitData(DataManager.Instance.UnitDatas[2]));
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
        //StartStart();
        StartBattle();
        //StartTown();
        //StartRest();
        //StartEvent();
    }

    private void StartGame()
    {
        _currentChapter = 0;
        _maxLayer = _map.MaxLayer[_currentChapter];
        _currentLayer = 0;
        CurrentCoin = 100;
        _prevCoin = _currentCoin;
        _playTime = 0.0f;
        _isGameStart = true;
        
        _map.Init();
        _topBar.SetActive(true);
        ViewManager.Instance.ShowMapView();

        StartChapter();
    }

    private void StartChapter()
    {
        _map.CreateMap(_currentChapter);
        _map.Nodes[_currentChapter][_currentLayer][0].State = NodeState.Idle;
    }

    public void OnClickNode(Node node)
    {
        _currentNode = node;
        _currentNode.State = NodeState.Selected;

        for(int i = 0; i < _map.Nodes[_currentChapter][_currentLayer].Length; i++)
        {
            if (i == _currentNode.Index) continue;
            _map.Nodes[_currentChapter][_currentLayer][i].State = NodeState.Locked;
        }
    }

    public void AfterClickNode()
    {
        switch (_currentNode.Type)
        {
            case NodeType.Start:
                StartStart();
                break;

            case NodeType.Battle:
                StartBattle();
                break;

            case NodeType.Elite:
                OnClearNode();
                break;

            case NodeType.Town:
                StartTown();
                break;

            case NodeType.Rest:
                StartRest();
                break;

            case NodeType.Event:
                StartEvent();
                break;

            case NodeType.Boss:
                OnClearNode();
                break;
        }
    }

    private void StartStart()
    {
        StartManager.Instance.StartStart(_currentChapter);
        ViewManager.Instance.ShowStartView();
        SoundManager.Instance.PlayBGM(BGMType.Start);
    }

    private void StartBattle()
    {
        BattleManager.Instance.StartBattle(_playerUnitDatas, _playerCardDatas);
        ViewManager.Instance.ShowBattleView();
    }

    private void StartTown()
    {
        TownRestManager.Instance.StartTown();
        ViewManager.Instance.ShowTownView();
        SoundManager.Instance.PlayBGM(BGMType.Town);
    }

    private void StartRest()
    {
        ViewManager.Instance.ShowRestView();
    }

    private void StartEvent()
    {
        EventManager.Instance.StartEvent(_currentChapter);
        ViewManager.Instance.ShowEventView();
    }

    public void OnClearNode()
    {
        foreach(Node node in _currentNode.NextNode)
            node.State = NodeState.Idle;

        _currentLayer++;

        if (_currentLayer >= _maxLayer)
        {
            _currentChapter++;

            if(_currentChapter >= _maxChapter)
            {
                Debug.Log("Game Clear");
                return;
            }
            
            Debug.Log("Chapter Clear");
            StartChapter();
        }

        ViewManager.Instance.ShowMapView();
        SoundManager.Instance.StopBGM();
    }

    public void OnBattleEnd(bool isWin)
    {
        if (isWin)
        {
            BattleManager.Instance.ShowRewards(NodeType.Battle);
        }
        else
        {
            _isGameStart = false;
            ViewManager.Instance.ShowGameOverPopup();
        }
    }

    public void AddUnit(int unitType)
    {
        if (_playerUnitDatas.Count >= 4) return;
        _playerUnitDatas.Add(new UnitData(DataManager.Instance.UnitDatas[unitType]));
    }

    public void RemoveUnit(int index)
    {
        _playerUnitDatas.RemoveAt(index);
    }

    public void CureUnit(int index)
    {
        UnitData unit = _playerUnitDatas[index];
        unit.CurrentHealth += (int)(unit.MaxHealth * 0.5f);
        if (unit.CurrentHealth > unit.MaxHealth) unit.CurrentHealth = unit.MaxHealth;
    }

    public void HurtUnit(int damage, bool isAll)
    {
        if (isAll)
        {
            for (int i = _playerUnitDatas.Count - 1; i >= 0; i--)
            {
                _playerUnitDatas[i].CurrentHealth -= damage;
                if (_playerUnitDatas[i].CurrentHealth < 0) RemoveUnit(i);
            }
        }
        else
        {
            int index = Random.Range(0, _playerUnitDatas.Count);
            _playerUnitDatas[index].CurrentHealth -= damage;
            if (_playerUnitDatas[index].CurrentHealth < 0) RemoveUnit(index);
        }

        CheckGameOver();
    }

    public void AddCard(CardData data)
    {
        _playerCardDatas.Add(new CardData(data));
    }

    public void RemoveCard(int index)
    {
        _playerCardDatas.RemoveAt(index);
    }

    private void CheckGameOver()
    {
        if (_playerUnitDatas.Count <= 0)
        {
            _isGameStart = false;
            ViewManager.Instance.ShowGameOverPopup();
        }
    }

    public void PlayRecruitAnimation(int unitType)
    {
        ItemSprite unitSprite = Instantiate(DataManager.Instance.ItemSpritePrefab, DataManager.Instance.CanvasRect, false);
        unitSprite.Init(1, unitType);
        unitSprite.PlayRecruitAnimation(1);
    }

    public void OnClickMapButton()
    {
        if (!ViewManager.Instance.ShowMapPopup()) return;
        InputManager.Instance.Push(InputState.None);
        SoundManager.Instance.PlayMapButtonClickSound();
    }

    public void OnClickMapPrevButton()
    {
        InputManager.Instance.Pop();
        ViewManager.Instance.Pop();
        SoundManager.Instance.PlayMapButtonClickSound();
    }

    public void OnClickUnitCollectionButton(int type)
    {
        if (!ViewManager.Instance.ShowUnitCollectionPopup()) return;
        _unitCollectionUI.Init(_playerUnitDatas, type);
        InputManager.Instance.Push(InputState.None);
        SoundManager.Instance.PlayButtonClickSound();
    }

    public void OnClickCardCollectionButton(bool isRemove)
    {
        if (!ViewManager.Instance.ShowCardCollectionPopup()) return;
        _cardCollectionUI.Init(_playerCardDatas, isRemove);
        InputManager.Instance.Push(InputState.CardCollection);
        SoundManager.Instance.PlayButtonClickSound();
    }

    private void UpdateUnitIndex()
    {
        for (int i = 0; i < _playerUnitDatas.Count; i++) _playerUnitDatas[i].Index = i;
    }

    private IEnumerator UpdateCoinText()
    {
        SoundManager.Instance.PlayCoinSound();
        float time = 0.0f;
        float duration = 1.0f;

        int start = _prevCoin;
        int end = _currentCoin;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            int value = Mathf.RoundToInt(Mathf.Lerp(start, end, t));
            _coinText.text = value.ToString();
            yield return null;
        }

        _coinText.text = end.ToString();
    }

    private void UpdateTopBar()
    {
        _playTime += Time.deltaTime;

        int hour = (int)(_playTime / 3600);
        int minute = (int)((_playTime % 3600) / 60);
        int second = (int)(_playTime % 60);

        _layerText.text = $"{_currentLayer + 1}";
        _playTimeText.text = $"{hour:D2}:{minute:D2}:{second:D2}";
        _partyNumText.text = _playerUnitDatas.Count.ToString();
        _cardNumText.text = _playerCardDatas.Count.ToString();
    }

    private void Update()
    {
        if (_isGameStart)
        {
            UpdateUnitIndex();
            UpdateTopBar();
        }
    }

}
