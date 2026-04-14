using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
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

    private UnitData[] _enemyUnitData = new UnitData[4]
    {
        new UnitData("Knight", 110, 110, 10, 30, UnitType.Knight),
        new UnitData("Lancer", 90, 90, 20, 20, UnitType.Lancer),
        new UnitData("Archer", 70, 70, 30, 10, UnitType.Archer),
        new UnitData("Monk", 50, 50, 5, 5, UnitType.Monk)
    };

    private Unit[] _playerUnits = new Unit[4];
    private Unit[] _enemyUnits = new Unit[4];

    public Unit[] PlayerUnits => _playerUnits;
    public Unit[] EnemyUnits => _enemyUnits;

    private int _currentTurn = 0;
    private int _drawCardNum = 5;
    private int _maxCost = 10;
    private int _currentCost = 0;
    private BattleState _state;
    
    private PlayerInputActions _input;
    private Unit _selectedUnit = null;
    private SlotGround _selectedSlotGround = null;
    private Vector3 _mousePos = Vector3.zero;
    private Vector3 _screenPos = Vector3.zero;
    private Vector3 _worldPos = Vector3.zero;
    private bool _isDrag = false;

    public static BattleManager Instance { get; private set; }
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

        _input = new PlayerInputActions();

        for (int i = 0; i < _playerSlotGrounds.Length; i++)
            _playerSlotGrounds[i].SlotIndex = i;
    }

    public void StartBattle(List<UnitData> playerUnitDatas, List<CardData> playerCardDatas)
    {
        _currentTurn = 0;
        _drawCardNum = 5;
        _maxCost = 10;
        _state = BattleState.Prepare;
        _endPrepareButton.gameObject.SetActive(true);
        _cardView.SetActive(false);

        foreach (SlotGround slotGround in _playerSlotGrounds)
            slotGround.gameObject.SetActive(true);

        for (int i = 0; i < playerUnitDatas.Count; i++)
        {
            if (playerUnitDatas[i].SlotIndex == -1)
            {
                _playerUnits[i] = Instantiate(DataManager.Instance.PlayerUnitPrefabs[(int)playerUnitDatas[i].Type], _playerSlots[i]);
                _playerUnits[i].Init(playerUnitDatas[i]);
                _playerUnits[i].UnitData.SlotIndex = i;
                _playerUnits[i].transform.localPosition = Vector3.zero;
            }
            else
            {
                _playerUnits[playerUnitDatas[i].SlotIndex] 
                    = Instantiate(DataManager.Instance.PlayerUnitPrefabs[(int)playerUnitDatas[i].Type], _playerSlots[playerUnitDatas[i].SlotIndex]);
                _playerUnits[playerUnitDatas[i].SlotIndex].Init(playerUnitDatas[i]);
                _playerUnits[playerUnitDatas[i].SlotIndex].transform.localPosition = Vector3.zero;
            }
        }

        int random = Random.Range(1, 5);

        for (int i = 0; i < random; i++)
        {
            int random2 = Random.Range(0, 4);
            _enemyUnits[i] = Instantiate(DataManager.Instance.EnemyUnitPrefabs[random2], _enemySlots[i]);
            _enemyUnits[i].Init(new UnitData(_enemyUnitData[random2]));
            _enemyUnits[i].UnitData.SlotIndex = i;
            _enemyUnits[i].transform.localPosition = Vector3.zero;
        }

        CardManager.Instance.Init(playerCardDatas);
    }

    public void EndPrepare()
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

        Invoke("StartPlayerTurn", 0.1f);
    }

    public void StartPlayerTurn()
    {
        if (_state != BattleState.Battle) return;

        _currentTurn++;
        Debug.Log($"PlayerTurn{_currentTurn} Start");
        _currentCost = _maxCost;
        _endTurnButton.enabled = true;

        SetCostText();
        StartCoroutine(DrawCard(_drawCardNum));

        foreach (Unit unit in _playerUnits)
        {
            if (unit == null) continue;

            if (_currentTurn <= 1) unit.NextActionScript.gameObject.SetActive(true);
            if (_currentTurn > 1) unit.ResetAction();
            unit.DecideAction();
        }

        if(_currentTurn <= 1)
        {
            foreach (Unit unit in _enemyUnits)
            {
                if (unit == null) continue;

                unit.NextActionScript.gameObject.SetActive(true);
                unit.DecideAction();
            }
        }
    }
    
    private void SetCostText()
    {
        string costText = $"{_currentCost}/{_maxCost}";
        _costText.text = costText;
    }

    public bool UseCard(int cost)
    {
        if(cost > _currentCost) return false;

        _currentCost -= cost;
        SetCostText();
        return true;
    }

    private IEnumerator DrawCard(int num)
    {
        for (int i = 0; i < num; i++)
        {
            yield return StartCoroutine(CardManager.Instance.DrawCardRoutine());
            yield return new WaitForSeconds(0.5f);
        }
    }

    public void EndPlayerTurn()
    {
        if (_state != BattleState.Battle) return;

        Debug.Log($"PlayerTurn{_currentTurn} End");
        _endTurnButton.enabled = false;

        CardManager.Instance.DiscardHandCards();

        StartCoroutine(PlayerToEnemyFlow());
    }

    private IEnumerator PlayerToEnemyFlow()
    {
        yield return StartCoroutine(UnitTurnProcess(_playerUnits));

        StartEnemyTurn();

        yield return StartCoroutine(UnitTurnProcess(_enemyUnits));

        Debug.Log($"EnemyTurn{_currentTurn} End");

        foreach (Unit unit in _enemyUnits)
        {
            if (unit == null) continue;
            unit.DecideAction();
        }

        StartPlayerTurn();
    }

    public void StartEnemyTurn()
    {
        if (_state != BattleState.Battle) return;

        Debug.Log($"EnemyTurn{_currentTurn} Start");

        if(_currentTurn > 1)
        {
            foreach (Unit unit in _enemyUnits)
            {
                if (unit == null) continue;
                unit.ResetAction();
            }
        }
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
                _state = BattleState.End;
                StopAllCoroutines();
                _endTurnButton.enabled = false;
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
                _state = BattleState.End;
                StopAllCoroutines();
                _endTurnButton.enabled = false;
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

    public Unit GetRandomEnemyTarget(Unit unit)
    {
        Unit[] targets = _playerUnits.Contains(unit) ? _enemyUnits : _playerUnits;

        int index = GetIndex(targets, 0, 2);
        if ((targets[0] != null && targets[0].UnitData.Type == UnitType.Knight && targets[0].IsSkillUsing) 
            && (targets[1] != null && targets[1].UnitData.Type == UnitType.Knight && targets[1].IsSkillUsing))
        {

        }
        else if (targets[0] != null && targets[0].UnitData.Type == UnitType.Knight && targets[0].IsSkillUsing)
        {
            index = 0;
        }
        else if (targets[1] != null && targets[1].UnitData.Type == UnitType.Knight && targets[1].IsSkillUsing)
        {
            index = 1;
        }

        return index != -1 ? targets[index] : null;
    }

    public Unit GetRandomTeamTarget(Unit unit)
    {
        Unit[] targets = _playerUnits.Contains(unit) ? _playerUnits : _enemyUnits;
        int index = GetIndex(targets, 0, 4);
        return index != -1 ? targets[index] : null;
    }

    public int GetIndex(Unit[] targets, int start, int end)
    {
        List<int> validIndexes = new List<int>();
        
        for (int i = start; i < end; i++)
        {
            if (targets[i] != null) validIndexes.Add(i);
        }

        if (validIndexes.Count == 0) return -1;
        
        return validIndexes[Random.Range(0, validIndexes.Count)];
    }

    private void OnEnable()
    {
        _input.Enable();
        _input.Player.Click.started += OnClick;
        _input.Player.Click.canceled += OnRelease;
    }

    private void OnDisable()
    {
        _input.Player.Click.started -= OnClick;
        _input.Player.Click.canceled -= OnRelease;
        _input.Disable();
    }

    private void OnClick(InputAction.CallbackContext ctx)
    {
        if (_selectedUnit == null) return;
        Debug.Log($"OnClickUnit");
        _isDrag = true;
    }

    private void OnRelease(InputAction.CallbackContext ctx)
    {
        if (_selectedUnit == null) return;
        Debug.Log($"OnReleaseUnit");

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

    private void MouseProcess()
    {
        _mousePos = _input.Player.Position.ReadValue<Vector2>();
        _screenPos = new Vector3(_mousePos.x, _mousePos.y, 8.0f);
        _worldPos = Camera.main.ScreenToWorldPoint(_screenPos);
        _worldPos.z = 0.0f;

        if (!_isDrag)
        {
            int count = 0;

            foreach(Unit unit in _playerUnits)
            {
                if (unit == null) continue;

                if(Vector3.Distance(unit.transform.position, _worldPos) <= 0.6f)
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
            _selectedUnit.transform.position = _worldPos;

            foreach(SlotGround slotGround in _playerSlotGrounds)
            {
                if(Vector3.Distance(slotGround.transform.position, _worldPos) <= 0.6f)
                {
                    if (slotGround.SlotIndex != _selectedUnit.UnitData.SlotIndex)
                    {
                        slotGround.SetHighlight(true);
                        _selectedSlotGround = slotGround;
                    }
                }
                else
                {
                    slotGround.SetHighlight(false);
                }
            }
        }
    }

    private void Update()
    {
        switch (_state)
        {
            case BattleState.Prepare:
                MouseProcess();
                break;

            case BattleState.Battle:
                break;

            case BattleState.End:
                break;
        }
    }

}