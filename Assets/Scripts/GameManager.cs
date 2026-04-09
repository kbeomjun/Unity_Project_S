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

    [SerializeField] private List<Unit> _playerUnits = new List<Unit>();

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
        BattleManager.Instance.StartBattle(_playerUnits);
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
                BattleManager.Instance.StartBattle(_playerUnits);
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
