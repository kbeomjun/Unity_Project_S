using System.Collections.Generic;
using UnityEngine;

public class Map : MonoBehaviour
{
    [SerializeField] private Transform[] _nodesParent;
    [SerializeField] private Node[] _nodePrefabs;

    private int _maxChapter = 1;
    private int[] _maxLayer = { 15 };

    private int _maxRestInChapter = 0; // 챕터 별 최대 휴식 노드 수
    private int _restCountInChapter = 0;

    private int _maxShopInLayer = 1; // 층 별 최대 상점 노드 수
    private int _maxRestInLayer = 1; // 층 별 최대 휴식 노드 수

    // xOffset: 380 + () + (-40 ~ 40) , yOffset: 200 + (-25 ~ 25)
    private float _maxWidth = 1920.0f;
    private float _maxHeight = 1080.0f;
    
    private float _startX = 960.0f;
    private float _startY = 200.0f;

    private float _baseX = 300.0f;
    private float _baseY = 0.0f;

    private Node[][][] nodes;

    private void Awake()
    {
        _maxWidth -= 2 * _baseX;
    }

    private void Start()
    {
        CreateMap();
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

                    nodes[c][l][0] = SpawnNode(4, c, xPos, _startY * (l + 1), l, i);
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

        int nodeCount = Random.Range(2, 5);
        nodes[c][l] = new Node[nodeCount];

        _baseX = 300.0f;
        _baseY = _startY * (l + 1);

        float xOffset = _maxWidth / nodeCount;

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

}
