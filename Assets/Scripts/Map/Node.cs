using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum NodeType
{
    Start,
    Battle,
    Elite,
    Shop,
    Rest,
    Event,
    Boss
}

public enum NodeState
{
    Available,
    HighLited,
    Selected,
    Locked   
}

public class Node : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private NodeType _type;
    [SerializeField] private NodeState _state;
    [SerializeField] private Sprite[] _images;

    [SerializeField] private GameObject _selectCircle;
    private Image _selectCircleImage;

    private Color[] _colors =
    {
        new Color32(40, 55, 210, 255),
        new Color32(255, 255, 255, 255),
        new Color32(84, 24, 71, 255),
        new Color32(94, 52, 20, 255),
        new Color32(18, 156, 34, 255),
        new Color32(221, 190, 20, 255),
        new Color32(210, 180, 40, 255)
    };

    public NodeType Type
    {
        get => _type;
        set
        {
            _type = value;
            
            _icon.sprite = _images[(int)value];
            _icon.color = _colors[(int)value];
            _iconBaseColor = _icon.color;
        }
    }
    public NodeState State
    {
        get => _state;
        set => _state = value;
    }

    private List<Node> _nextNode = new List<Node>();
    public List<Node> NextNode => _nextNode;

    private Image _icon;
    private Color _iconBaseColor;
    private Vector3 _iconBaseScale;
    private float _scaleSize = 1.2f;
    private float _scaleSpeed = 10.0f;

    private int _layer;
    private int _index;

    public int Layer
    {
        get => _layer;
        set => _layer = value;
    }
    public int Index
    {
        get => _index;
        set => _index = value;
    }

    public void Init()
    {
        _icon = GetComponent<Image>();
        _iconBaseColor = _icon.color;
        _iconBaseScale = GetComponent<RectTransform>().localScale;

        _selectCircleImage = _selectCircle.GetComponent<Image>();
        _selectCircle.SetActive(false);

        _state = NodeState.Locked;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_state != NodeState.Available) return;
        _state = NodeState.HighLited;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_state != NodeState.HighLited) return;
        _state = NodeState.Available;
    }

    public void OnClick()
    {
        if (_state != NodeState.HighLited) return;

        _selectCircle.SetActive(true);
        StartCoroutine(DrawCircle());
        GameManager.Instance.OnClickNode(this);

        Debug.Log($"OnClick - {_type.ToString()}, ({_layer}, {_index})");
    }

    IEnumerator DrawCircle()
    {
        _selectCircleImage.fillAmount = 0.0f;

        float t = 0.0f;
        float duration = 0.2f;

        while (t < duration)
        {
            t += Time.deltaTime;
            _selectCircleImage.fillAmount = t / duration;
            yield return null;
        }

        _selectCircleImage.fillAmount = 1.0f;
        yield return new WaitForSeconds(0.5f);
        GameManager.Instance.AfterClickNode();
    }

    private void Update()
    {
        switch (_state)
        {
            case NodeState.Available:
                _icon.color = _iconBaseColor;
                transform.localScale = Vector3.Lerp(transform.localScale, _iconBaseScale, _scaleSpeed * Time.deltaTime);
                break;

            case NodeState.HighLited:
                _icon.color = Color.Lerp(_iconBaseColor, Color.white, 0.1f);
                transform.localScale = Vector3.Lerp(transform.localScale, _iconBaseScale * _scaleSize, _scaleSpeed * Time.deltaTime);
                break;

            case NodeState.Selected:
                _icon.color = Color.Lerp(_iconBaseColor, Color.white, 0.2f);
                transform.localScale = Vector3.Lerp(transform.localScale, _iconBaseScale, _scaleSpeed * Time.deltaTime);
                break;

            case NodeState.Locked:
                _icon.color = Color.Lerp(_iconBaseColor, Color.black, 0.2f);
                transform.localScale = Vector3.Lerp(transform.localScale, _iconBaseScale, _scaleSpeed * Time.deltaTime);
                break;
        }
    }

}
