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
    Town,
    Rest,
    Event,
    Boss
}

public enum NodeState
{
    Idle,
    Hover,
    Selected,
    Locked   
}

public class Node : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private NodeType _type;
    [SerializeField] private NodeState _state;
    [SerializeField] private Sprite[] _images;
    [SerializeField] private GameObject _selectCircle;

    private RectTransform _rect;
    private Image _selectCircleImage;

    public NodeType Type
    {
        get => _type;
        set
        {
            _type = value;
            _icon.sprite = _images[(int)value];
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
    private float _pulseTime = 0.0f;
    private float _pulseAmount = 0.2f;
    private float _pulseSpeed = 3.0f;
    private float _idleScale = 0.0f;
    private float _hoverScale = 1.4f;
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
        _rect = GetComponent<RectTransform>();
        _icon = GetComponent<Image>();
        _iconBaseColor = _icon.color;
        _iconBaseScale = _rect.localScale;
        _selectCircleImage = _selectCircle.GetComponent<Image>();
        _selectCircle.SetActive(false);
        _state = NodeState.Locked;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_state != NodeState.Idle) return;
        _state = NodeState.Hover;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_state != NodeState.Hover) return;
        _state = NodeState.Idle;
    }

    public void OnClick()
    {
        if (_state != NodeState.Hover) return;

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
            case NodeState.Idle:
                _icon.color = _iconBaseColor;
                _pulseTime += Time.deltaTime;
                _idleScale = 1.0f + Mathf.Abs(Mathf.Sin(_pulseTime * _pulseSpeed)) * _pulseAmount;
                _rect.localScale = Vector3.Lerp(_rect.localScale, _iconBaseScale * _idleScale, _scaleSpeed * Time.deltaTime);
                break;

            case NodeState.Hover:
                _icon.color = Color.Lerp(_iconBaseColor, Color.white, 0.1f);
                _rect.localScale = Vector3.Lerp(_rect.localScale, _iconBaseScale * _hoverScale, _scaleSpeed * Time.deltaTime);
                break;

            case NodeState.Selected:
                _icon.color = Color.Lerp(_iconBaseColor, Color.white, 0.2f);
                _rect.localScale = Vector3.Lerp(_rect.localScale, _iconBaseScale, _scaleSpeed * Time.deltaTime);
                break;

            case NodeState.Locked:
                _icon.color = Color.Lerp(_iconBaseColor, Color.black, 0.2f);
                _rect.localScale = Vector3.Lerp(_rect.localScale, _iconBaseScale, _scaleSpeed * Time.deltaTime);
                break;
        }
    }

}
