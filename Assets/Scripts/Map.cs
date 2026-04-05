using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] private RectTransform _mapImageRectTr;

    [SerializeField] private Transform[] _nodesParent;
    [SerializeField] private Node[] _nodePrefabs;

    [SerializeField] private Transform[] _linesParent;
    [SerializeField] private GameObject _linePrefab;

    private int _maxChapter = 1;
    private int[] _maxLayer = { 15 };

    private int _maxRestInChapter = 0; // 챕터 별 최대 휴식 노드 수
    private int _restCountInChapter = 0;

    private int _maxShopInLayer = 1; // 층 별 최대 상점 노드 수
    private int _maxRestInLayer = 1; // 층 별 최대 휴식 노드 수

    // xOffset: 380 + () + (-40 ~ 40) , yOffset: 200 + (-25 ~ 25)
    private float _maxWidth;
    private float _maxHeight;

    private float _mapWidth;
    
    private float _startX;
    private float _startY = 250.0f;

    private float _baseX = 300.0f;
    private float _baseY = 0.0f;

    private Node[][][] nodes;

    private float _nodeDistance = 0.0f;

    private void Awake()
    {
        _maxWidth = GetComponent<RectTransform>().rect.width;
        _maxHeight = GetComponent<RectTransform>().rect.height;

        _startX = _maxWidth / 2.0f;

        _baseX = _maxWidth / 6.0f;

        _mapWidth = _maxWidth - 2 * _baseX;
        _mapImageRectTr.sizeDelta = new Vector2(_mapWidth, _mapImageRectTr.sizeDelta.y);

        _nodeDistance = _maxWidth / 4.26f;
    }

    private void Start()
    {
        CreateMap();
        MakeConnectionBetweenNodes();
        CreateLine();
    }

    private void CreateMap()
    {
        nodes = new Node[_maxChapter][][];

        for (int c = 0; c < nodes.Length; c++)
        {
            CreateLayer(c);
        }
    }

    private void CreateLayer(int c)
    {
        _maxRestInChapter = (_maxLayer[c] - 3) / 2;
        _restCountInChapter = 0;

        nodes[c] = new Node[_maxLayer[c]][];

        for (int l = 0; l < nodes[c].Length; l++)
        {
            if (l == 0)
            {
                nodes[c][l] = new Node[1];

                nodes[c][l][0] = SpawnNode(0, c, _startX, _startY, l, 0);
            }            
            else if (l == _maxLayer[c] - 2)
            {
                int length = nodes[c][l - 1].Length;
                nodes[c][l] = new Node[length];

                for (int i = 0; i < length; i++)
                {
                    float xPos = nodes[c][l - 1][i].gameObject.transform.position.x;

                    nodes[c][l][i] = SpawnNode(4, c, xPos, _startY * (l + 1), l, i);
                }
            }
            else if (l == _maxLayer[c] - 1)
            {
                nodes[c][l] = new Node[1];

                nodes[c][l][0] = SpawnNode(6, c, _startX, _startY * (l + 1), l, 0);
            }
            else
            {
                CreateNode(c, l);
            }
        }
    }

    private void CreateNode(int c, int l)
    {
        int shopCount = 0;
        int restCount = 0;

        int nodeCount = Random.Range(2, 6);
        nodes[c][l] = new Node[nodeCount];

        _baseX = _maxWidth / 6.0f;
        _baseY = _startY * (l + 1);

        float xOffset = _mapWidth / nodeCount;

        for (int i = 0; i < nodeCount; i++)
        {
            _baseX += (i == 0) ? (xOffset / 2) : xOffset;

            float randomXOffset = Random.Range(-40.0f, 40.0f);
            float randomYOffset = Random.Range(-25.0f, 25.0f);

            List<int> allowedTypes = new List<int> { 1, 2, 3, 4, 5 };

            if (l == 1) allowedTypes = new List<int> { 1 }; // 첫 층은 전투 노드만
            if (l <= 4) allowedTypes.Remove(2);
            if (shopCount >= _maxShopInLayer) allowedTypes.Remove(3);
            if (restCount >= _maxRestInLayer || l <= 4 || _restCountInChapter >= _maxRestInChapter) allowedTypes.Remove(4);

            int nodeType = allowedTypes[Random.Range(0, allowedTypes.Count)];

            nodes[c][l][i] = SpawnNode(nodeType, c, _baseX + randomXOffset, _baseY + randomYOffset, l, i);

            if (nodeType == 3) shopCount++;
            if (nodeType == 4) { 
                restCount++;
                _restCountInChapter++;
            }
        }
    }

    private Node SpawnNode(int nodeType, int chapter, float x, float y, int layer, int index)
    {
        Node node = Instantiate(_nodePrefabs[nodeType], _nodesParent[chapter], false);
        RectTransform rect = node.GetComponent<RectTransform>();
        rect.position = new Vector3(x, y, 0.0f);
        node.Layer = layer;
        node.Index = index;

        return node;
    }

    private void MakeConnectionBetweenNodes()
    {
        for (int c = 0; c < nodes.Length; c++)
        {
            List<(Vector2, Vector2)> edges = new List<(Vector2, Vector2)>();

            for(int l = 0; l < nodes[c].Length - 1; l++)
            {
                if(l == 0)
                {
                    for(int i = 0; i < nodes[c][l + 1].Length; i++)
                    {
                        nodes[c][l][0].NextNode.Add(nodes[c][l + 1][i]);
                    }
                }
                else if (l == _maxLayer[c] - 3)
                {
                    for (int i = 0; i < nodes[c][l].Length; i++)
                    {
                        nodes[c][l][i].NextNode.Add(nodes[c][l + 1][i]);
                    }
                }
                else if (l == _maxLayer[c] - 2)
                {
                    for(int i = 0; i < nodes[c][l].Length; i++)
                    {
                        nodes[c][l][i].NextNode.Add(nodes[c][l + 1][0]);
                    }
                }
                else
                {
                    for(int i = 0; i < nodes[c][l].Length; i++)
                    {
                        Node currentNode = nodes[c][l][i];

                        List<Node> sortedNextNodes = nodes[c][l + 1]
                            .Where(n => Vector3.Distance(currentNode.transform.position, n.transform.position) <= _nodeDistance)
                            .OrderBy(n => Vector3.Distance(currentNode.transform.position, n.transform.position))
                            .ToList();

                        currentNode.NextNode.Add(sortedNextNodes[0]);

                        if (Random.value < 0.5f)
                        {
                            for (int j = 1; j < sortedNextNodes.Count; j++)
                            {
                                TryConnect(currentNode, sortedNextNodes[j], edges);
                            }
                        }
                    }

                    // 모든 다음 노드가 최소 1개는 연결되도록 보장
                    for (int j = 0; j < nodes[c][l + 1].Length; j++)
                    {
                        Node nextNode = nodes[c][l + 1][j];

                        bool hasConnection = false;

                        for (int i = 0; i < nodes[c][l].Length; i++)
                        {
                            if (nodes[c][l][i].NextNode.Contains(nextNode))
                            {
                                hasConnection = true;
                                break;
                            }
                        }

                        if (!hasConnection)
                        {
                            List<Node> sortedCurrentNodes = nodes[c][l]
                                .OrderBy(n => Vector3.Distance(nextNode.transform.position, n.transform.position))
                                .ToList();

                            sortedCurrentNodes[0].NextNode.Add(nextNode);
                        }
                    }
                }
            }
        }
    }

    private void TryConnect(Node a, Node b, List<(Vector2, Vector2)> edges)
    {
        Vector2 aPos = a.transform.position;
        Vector2 bPos = b.transform.position;

        foreach (var edge in edges)
        {
            if (IsCross(aPos, bPos, edge.Item1, edge.Item2))
            {
                return; // 교차하면 연결 안함
            }
        }

        a.NextNode.Add(b);
        edges.Add((aPos, bPos));
    }

    private bool IsCross(Vector2 a1, Vector2 a2, Vector2 b1, Vector2 b2)
    {
        float ccw(Vector2 A, Vector2 B, Vector2 C)
        {
            return (B.x - A.x) * (C.y - A.y) - (B.y - A.y) * (C.x - A.x);
        }

        return ccw(a1, a2, b1) * ccw(a1, a2, b2) < 0 && ccw(b1, b2, a1) * ccw(b1, b2, a2) < 0;
    }

    private void CreateLine()
    {
        for (int c = 0; c < nodes.Length; c++)
        {
            for (int l = 0; l < nodes[c].Length - 1; l++)
            {
                for (int i = 0; i < nodes[c][l].Length; i++)
                {
                    DrawLine(nodes[c][l][i], c);
                }
            }
        }
    }

    private void DrawLine(Node node, int chapter)
    {
        List<Node> nextNodes = node.NextNode;

        Vector3 startPos = node.transform.position;
        Vector3 endPos = Vector3.zero;

        for(int i = 0; i < nextNodes.Count; i++)
        {
            endPos = nextNodes[i].transform.position;

            Vector3 direction = endPos - startPos;
            direction = direction.normalized;

            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            angle -= 90.0f;

            float step = 60.0f;
            Vector3 start = startPos + direction * step;
            Vector3 end = endPos - direction * step;

            float distance = Vector3.Distance(start, end);

            GameObject line = Instantiate(_linePrefab, _linesParent[chapter], false);
            
            RectTransform rect = line.GetComponent<RectTransform>();
            rect.position = (startPos + endPos) / 2.0f;
            rect.localRotation = Quaternion.Euler(0, 0, angle);
            rect.sizeDelta = new Vector2(rect.sizeDelta.x, distance);
        }
    }

}
