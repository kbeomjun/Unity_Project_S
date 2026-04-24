using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public enum BattleState
{
    Prepare,
    Battle,
    End
}    

public class BattleManager : MonoBehaviour
{
    [SerializeField] private Transform[] _playerSlots;
    [SerializeField] private SlotGround[] _playerSlotGrounds;
    [SerializeField] private Transform[] _enemySlots;
    [SerializeField] private GameObject _cardView;
    [SerializeField] private TMP_Text _costText;
    [SerializeField] private Button _endPrepareButton;
    [SerializeField] private Button _endTurnButton;

    private DeckUI _deckUI;
    public DeckUI DeckUI => _deckUI;

    private Unit[] _playerUnits = new Unit[4];
    private Unit[] _enemyUnits = new Unit[4];

    public Unit[] PlayerUnits => _playerUnits;
    public Unit[] EnemyUnits => _enemyUnits;

    private int _currentTurn = 0;
    private int _drawCardNum = 5;
    private int _maxCost = 10;
    private int _currentCost = 0;
    private BattleState _state;
    
    private Unit _selectedUnit = null;
    private SlotGround _selectedSlotGround = null;
    private bool _isDrag = false;

    public static BattleManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        _deckUI = GetComponent<DeckUI>();
        for (int i = 0; i < _playerSlotGrounds.Length; i++)
            _playerSlotGrounds[i].SlotIndex = i;
    }

    public void StartBattle(List<UnitData> playerUnitDatas, List<CardData> playerCardDatas)
    {
        ClearUnits();

        _currentTurn = 0;
        _drawCardNum = 10;
        _maxCost = 10;
        _state = BattleState.Prepare;
        _endPrepareButton.gameObject.SetActive(true);
        _cardView.SetActive(false);

        foreach (SlotGround slotGround in _playerSlotGrounds)
            slotGround.gameObject.SetActive(true);

        for (int i = 0; i < playerUnitDatas.Count; i++)
        {
            _playerUnits[i] = Instantiate(DataManager.Instance.PlayerUnitPrefabs[(int)playerUnitDatas[i].Type], _playerSlots[i]);
            _playerUnits[i].Init(playerUnitDatas[i]);
            _playerUnits[i].UnitData.SlotIndex = i;
            _playerUnits[i].transform.localPosition = Vector3.zero;
            _playerUnits[i].UnitTeam = UnitTeam.Player;
        }

        int random = Random.Range(4, 5);
        for (int i = 0; i < random; i++)
        {
            int random2 = Random.Range(0, 4);
            _enemyUnits[i] = Instantiate(DataManager.Instance.EnemyUnitPrefabs[random2], _enemySlots[i]);
            _enemyUnits[i].Init(new UnitData(DataManager.Instance.UnitDatas[random2]));
            _enemyUnits[i].UnitData.SlotIndex = i;
            _enemyUnits[i].transform.localPosition = Vector3.zero;
            _enemyUnits[i].UnitTeam = UnitTeam.Enemy;
        }

        InputManager.Instance.Push(InputState.BattlePrepare);
        _deckUI.Init(playerCardDatas);
    }

    private void StartPlayerTurn()
    {
        if (_state != BattleState.Battle) return;
        _currentTurn++;
        Debug.Log($"PlayerTurn{_currentTurn} Start");
        _currentCost = _maxCost;
        _endTurnButton.enabled = true;
        
        StartCoroutine(_deckUI.DrawCards(_drawCardNum));

        foreach (Unit unit in _playerUnits)
        {
            if (unit == null) continue;
            unit.ResetAction();
            unit.OnTurnStart();
            unit.DecideAction();
        }
        foreach(Unit unit in _enemyUnits)
        {
            if (unit == null) continue;
            unit.DecideAction();
        }
    }

    public void EndPlayerTurn()
    {
        if (_state != BattleState.Battle) return;
        Debug.Log($"PlayerTurn{_currentTurn} End");
        _endTurnButton.enabled = false;

        _deckUI.DiscardHandCards();
        StartCoroutine(PlayerToEnemyFlow());
    }

    private void StartEnemyTurn()
    {
        if (_state != BattleState.Battle) return;
        Debug.Log($"EnemyTurn{_currentTurn} Start");

        foreach (Unit unit in _enemyUnits)
        {
            if (unit == null) continue;
            unit.ResetAction();
            unit.OnTurnStart();
        }
    }

    private void EndEnemyTurn()
    {
        if (_state != BattleState.Battle) return;
        Debug.Log($"EnemyTurn{_currentTurn} End");

        foreach (Unit unit in _enemyUnits)
        {
            if (unit == null) continue;
            unit.OnTurnEnd();
        }
    }

    private IEnumerator PlayerToEnemyFlow()
    {
        yield return StartCoroutine(UnitTurnProcess(_playerUnits));
        foreach (Unit unit in _playerUnits)
        {
            if (unit == null) continue;
            unit.OnTurnEnd();
        }
        yield return null;

        StartEnemyTurn();
        yield return null;

        yield return StartCoroutine(UnitTurnProcess(_enemyUnits));
        yield return null;

        EndEnemyTurn();
        yield return null;
        
        StartPlayerTurn();
    }

    private IEnumerator UnitTurnProcess(Unit[] units)
    {
        List<Unit> turnList = units.Where(u => u != null).ToList();
        foreach (Unit unit in turnList)
        {
            if (unit == null) continue;
            unit.PerformAction();
            yield return new WaitForSeconds(1.0f);
        }
    }

    public void RemoveUnit(Unit unit)
    {
        if (_playerUnits.Contains(unit))
        {
            GameManager.Instance.RemoveUnit(_playerUnits[unit.UnitData.SlotIndex].UnitData.Index);
            _playerUnits[unit.UnitData.SlotIndex] = null;
            Check(true);
        }
        else
        {
            _enemyUnits[unit.UnitData.SlotIndex] = null;
            Check(false);
        }
    }

    public void Check(bool flag)
    {
        int index = -1;
        if (flag)
        {
            if (_playerUnits[0] == null && _playerUnits[1] == null && _playerUnits[2] == null && _playerUnits[3] == null)
            {
                Debug.Log("Game Over");
                BattleEnd(false);
            }
            else if (_playerUnits[0] == null && _playerUnits[1] == null)
            {
                index = GetIndex(_playerUnits, 2, 4);
                _playerUnits[index - 2] = _playerUnits[index];
                _playerUnits[index - 2].UnitData.SlotIndex = index - 2;
                StartCoroutine(MoveToSlot(_playerUnits[index - 2], _playerSlots[index - 2]));
                _playerUnits[index] = null;
            }
        }
        else
        {
            if (_enemyUnits[0] == null && _enemyUnits[1] == null && _enemyUnits[2] == null && _enemyUnits[3] == null)
            {
                Debug.Log("Stage Clear");
                BattleEnd(true);
            }
            else if (_enemyUnits[0] == null && _enemyUnits[1] == null)
            {
                index = GetIndex(_enemyUnits, 2, 4);
                _enemyUnits[index - 2] = _enemyUnits[index];
                _enemyUnits[index - 2].UnitData.SlotIndex = index - 2;
                StartCoroutine(MoveToSlot(_enemyUnits[index - 2], _enemySlots[index - 2]));
                _enemyUnits[index] = null;
            }
        }
    }

    private void BattleEnd(bool isWin)
    {
        _state = BattleState.End;
        _endTurnButton.enabled = false;
        _cardView.SetActive(false);
        StopAllCoroutines();
        InputManager.Instance.PopUntil(InputState.BattlePrepare);
        _deckUI.StopAllCoroutines();
        GameManager.Instance.OnBattleEnd(isWin);
    }

    private IEnumerator MoveToSlot(Unit unit, Transform targetSlot)
    {
        Transform t = unit.transform;

        Vector3 startPos = t.position;
        Vector3 endPos = targetSlot.position;

        float duration = 0.3f;
        float time = 0f;

        while (time < duration)
        {
            time += Time.deltaTime;
            float tValue = time / duration;
            t.position = Vector3.Lerp(startPos, endPos, tValue);
            yield return null;
        }

        t.SetParent(targetSlot);
        t.localPosition = Vector3.zero;
    }

    private int GetIndex(Unit[] targets, int start, int end)
    {
        List<int> validIndexes = new List<int>();
        
        for (int i = start; i < end; i++)
            if (targets[i] != null) validIndexes.Add(i);

        if (validIndexes.Count == 0) return -1;
        
        return validIndexes[Random.Range(0, validIndexes.Count)];
    }

    public void OnClickEndPrepareButton()
    {
        if (_playerUnits[0] == null && _playerUnits[1] == null)
        {
            Debug.Log($"앞열 유닛 없음 전투 시작 불가");
            return;
        }

        _state = BattleState.Battle;
        _endPrepareButton.gameObject.SetActive(false);
        _endTurnButton.gameObject.SetActive(true);
        _cardView.SetActive(true);

        foreach (SlotGround slotGround in _playerSlotGrounds)
            slotGround.gameObject.SetActive(false);

        InputManager.Instance.Push(InputState.Battle);
        Invoke("StartPlayerTurn", 0.1f);
    }

    public void OnClickEndTurnButton()
    {
        if (_deckUI.IsDrawing)
        {
            _deckUI.RequestEndTurn();
            return;
        }

        EndPlayerTurn();
    }

    private void ClearUnits()
    {
        foreach(Unit unit in _playerUnits)
        {
            if (unit == null) continue;
            Destroy(unit.gameObject);
        }
        foreach (Unit unit in _enemyUnits)
        {
            if (unit == null) continue;
            Destroy(unit.gameObject);
        }
    }
    
    public void OnClick()
    {
        if (_selectedUnit == null) return;
        _isDrag = true;
    }

    public void OnRelease()
    {
        if (_selectedUnit == null) return;

        if(_selectedUnit != null && _selectedSlotGround != null)
        {
            int index1 = _selectedUnit.UnitData.SlotIndex;
            int index2 = _selectedSlotGround.SlotIndex;

            if (_playerUnits[index2] == null)
            {
                _playerUnits[index2] = _playerUnits[index1];
                _playerUnits[index1] = null;

                _playerUnits[index2].UnitData.SlotIndex = index2;
                _playerUnits[index2].transform.position = _playerSlots[index2].position;
            }
            else
            {
                Unit tempUnit = _playerUnits[index1];
                _playerUnits[index1] = _playerUnits[index2];
                _playerUnits[index2] = tempUnit;

                _playerUnits[index1].UnitData.SlotIndex = index1;
                _playerUnits[index1].transform.position = _playerSlots[index1].position;

                _playerUnits[index2].UnitData.SlotIndex = index2;
                _playerUnits[index2].transform.position = _playerSlots[index2].position;
            }
        }
        else
        {
            _selectedUnit.transform.position = _playerSlots[_selectedUnit.UnitData.SlotIndex].position;
        }

        _endPrepareButton.enabled = true;
        _isDrag = false;
        _selectedUnit = null;
        if(_selectedSlotGround != null)
        {
            _selectedSlotGround.SetHighlight(false);
            _selectedSlotGround = null;
        }
    }

    public void MouseProcess(Vector3 worldPos)
    {
        if (!_isDrag)
        {
            int count = 0;

            foreach(Unit unit in _playerUnits)
            {
                if (unit == null) continue;

                if(Vector3.Distance(unit.transform.position, worldPos) <= 0.6f)
                {
                    unit.SetHighlight(true);
                    _selectedUnit = unit;
                    count++;
                }
                else
                {
                    unit.SetHighlight(false);
                }
            }

            if (count == 0) _selectedUnit = null;
        }

        if (_isDrag && _selectedUnit != null)
        {
            _endPrepareButton.enabled = false;
            _selectedUnit.transform.position = worldPos;

            int count = 0;

            foreach (SlotGround slotGround in _playerSlotGrounds)
            {
                if(Vector3.Distance(slotGround.transform.position, worldPos) <= 0.6f)
                {
                    if (slotGround.SlotIndex != _selectedUnit.UnitData.SlotIndex)
                    {
                        slotGround.SetHighlight(true);
                        _selectedSlotGround = slotGround;
                        count++;
                    }
                }
                else
                {
                    slotGround.SetHighlight(false);
                }
            }

            if (count == 0) _selectedSlotGround = null;
        }
    }

    public bool UseCard(int cost)
    {
        if (cost > _currentCost) return false;
        _currentCost -= cost;
        return true;
    }

    private void UpdateCostText()
    {
        string costText = $"{_currentCost}/{_maxCost}";
        _costText.text = costText;
    }

    private void Update()
    {
        switch (_state)
        {
            case BattleState.Battle:
                UpdateCostText();
                break;
        }
    }

}