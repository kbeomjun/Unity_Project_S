using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BattleManager : MonoBehaviour
{
    [SerializeField] private Transform[] _playerSlots;
    [SerializeField] private Transform[] _enemySlots;
    [SerializeField] private Button _endTurnButton;

    [SerializeField] private Unit[] _playerUnits;
    [SerializeField] private Unit[] _enemyUnits;
    
    public static BattleManager Instance { get; private set; }

    private int _currentTurn = 0;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        StartPlayerTurn();
    }

    public void StartPlayerTurn()
    {
        _currentTurn++;
        Debug.Log($"PlayerTurn{_currentTurn} Start");

        _endTurnButton.enabled = true;

        foreach (Unit unit in _playerUnits)
        {
            if(_currentTurn > 1) unit.ResetAction();

            unit.DecideAction();
        }

        if(_currentTurn <= 1)
        {
            foreach (Unit unit in _enemyUnits)
            {
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
            yield return new WaitForSeconds(0.5f);
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