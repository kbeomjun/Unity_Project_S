using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private Transform[] _playerSlots;
    [SerializeField] private Transform[] _enemySlots;
    [SerializeField] private Unit[] _playerUnitPrefabs;
    [SerializeField] private Unit[] _enemyUnitPrefabs;
    [SerializeField] private Button _endTurnButton;

    private UnitData[] _enemyUnitData = new UnitData[4]
    {
        new UnitData("Knight", 10, 10, 10, 10, UnitType.Knight),
        new UnitData("Lancer", 10, 10, 10, 10, UnitType.Lancer),
        new UnitData("Archer", 10, 10, 10, 10, UnitType.Archer),
        new UnitData("Monk", 1000, 1000, 10, 10, UnitType.Monk)
    };

    private Unit[] _playerUnits = new Unit[4];
    private Unit[] _enemyUnits = new Unit[4];

    private int _currentTurn = 0;
    private bool _isBattle = false;

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
    }

    private void Start()
    {

    }

    public void StartBattle(List<UnitData> playerUnitDatas)
    {
        _isBattle = true;

        for (int i = 0; i < playerUnitDatas.Count; i++)
        {
            _playerUnits[i] = Instantiate(_playerUnitPrefabs[(int)playerUnitDatas[i].Type], _playerSlots[i]);
            _playerUnits[i].transform.localPosition = Vector3.zero;
            _playerUnits[i].Init(playerUnitDatas[i]);
        }

        int random = Random.Range(1, 3);

        for (int i = 0; i < random; i++)
        {
            int random2 = Random.Range(1, 2);
            _enemyUnits[i] = Instantiate(_enemyUnitPrefabs[random2], _enemySlots[i]);
            _enemyUnits[i].transform.localPosition = Vector3.zero;
            _enemyUnits[i].Init(new UnitData(_enemyUnitData[random2]));
        }

        Invoke("StartPlayerTurn", 0.2f);
    }

    public void StartPlayerTurn()
    {
        if (!_isBattle) return;

        _currentTurn++;
        Debug.Log($"PlayerTurn{_currentTurn} Start");

        _endTurnButton.enabled = true;

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

    public void EndPlayerTurn()
    {
        if (!_isBattle) return;

        Debug.Log($"PlayerTurn{_currentTurn} End");
        _endTurnButton.enabled = false;

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
        if (!_isBattle) return;

        Debug.Log($"EnemyTurn{_currentTurn} Start");

        if(_currentTurn > 1)
        {
            foreach (Unit unit in _enemyUnits)
            {
                if (unit == null) continue;

                Debug.Log("EnemyUnit ResetDefense");
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
        int index = -1;
        if (_playerUnits.Contains(unit))
        {
            index = System.Array.IndexOf(_playerUnits, unit);
            _playerUnits[index] = null;
        }
        else
        {
            index = System.Array.IndexOf(_enemyUnits, unit);
            _enemyUnits[index] = null;
        }
    }

    public void Check(Unit unit)
    {
        int index = -1;
        if (_playerUnits.Contains(unit))
        {
            if (_playerUnits[0] == null && _playerUnits[1] == null && _playerUnits[2] == null && _playerUnits[3] == null)
            {
                Debug.Log("Game Over");
                _isBattle = false;
                StopAllCoroutines();
                _endTurnButton.enabled = false;
            }
            else if (_playerUnits[0] == null && _playerUnits[1] == null)
            {
                index = GetIndex(_playerUnits, 2, 4);
                _playerUnits[index - 2] = _playerUnits[index];
                StartCoroutine(MoveToSlot(_playerUnits[index - 2], _playerSlots[index - 2]));
                _playerUnits[index] = null;
            }
        }
        else
        {
            if (_enemyUnits[0] == null && _enemyUnits[1] == null && _enemyUnits[2] == null && _enemyUnits[3] == null)
            {
                Debug.Log("Stage Clear");
                _isBattle = false;
                StopAllCoroutines();
                _endTurnButton.enabled = false;
            }
            else if (_enemyUnits[0] == null && _enemyUnits[1] == null)
            {
                index = GetIndex(_enemyUnits, 2, 4);
                _enemyUnits[index - 2] = _enemyUnits[index];
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

}