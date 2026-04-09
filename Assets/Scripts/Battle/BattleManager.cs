using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private Transform[] _playerSlots;
    [SerializeField] private Transform[] _enemySlots;
    [SerializeField] private Unit[] _enemyPrefabs;
    [SerializeField] private Button _endTurnButton;

    private Unit[] _playerUnits = new Unit[4];
    private Unit[] _enemyUnits = new Unit[4];

    private int _currentTurn = 0;

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
        //Invoke("StartPlayerTurn", 0.5f);
    }

    public void StartBattle(List<Unit> playerUnits)
    {
        for (int i = 0; i < playerUnits.Count; i++)
        {
            _playerUnits[i] = Instantiate(playerUnits[i], _playerSlots[i]);
            _playerUnits[i].transform.localPosition = Vector3.zero;
        }

        int random = Random.Range(1, 3);

        for (int i = 0; i < random; i++)
        {
            int random2 = Random.Range(0, 4);
            _enemyUnits[i] = Instantiate(_enemyPrefabs[random2], _enemySlots[i]);
            _enemyUnits[i].transform.localPosition = Vector3.zero;
        }

        Invoke("StartPlayerTurn", 0.5f);
    }

    public void StartPlayerTurn()
    {
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

    IEnumerator UnitTurnProcess(Unit[] units)
    {
        foreach (Unit unit in units)
        {
            if (unit == null) continue;
            
            unit.PerformAction();
            yield return new WaitForSeconds(1.0f);
        }
    }

    public Unit GetRandomEnemyTarget(Unit unit)
    {
        Unit[] targets = _playerUnits.Contains(unit) ? _enemyUnits : _playerUnits;
        int index = -1;

        while (true)
        {
            index = Random.Range(0, 2);

            if (targets[index] == null) continue;

            break;
        }

        return targets[index];
    }

    public Unit GetRandomTeamTarget(Unit unit)
    {
        Unit[] targets = _playerUnits.Contains(unit) ? _playerUnits : _enemyUnits;
        int index = -1;

        while (true)
        {
            index = Random.Range(0, 4);

            if (targets[index] == null) continue;

            break;
        }

        return targets[index];
    }

}