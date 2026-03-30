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

    private Image _icon;
    private Color _iconBaseColor;
    private Vector3 _iconBaseScale;
    private float _scaleSize = 1.15f;
    private float _scaleSpeed = 10.0f;

    private int _layer;
    private int _index;

    public int Layer
    {
        set => _layer = value;
    }
    public int Index
    {
        set => _index = value;
    }

    private void Awake()
    {
        _icon = GetComponent<Image>();

        _iconBaseColor = _icon.color;
        _iconBaseScale = GetComponent<RectTransform>().localScale;
    }

    private void Start()
    {

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_state != NodeState.Available) return;

        SetState(NodeState.HighLited);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (_state != NodeState.HighLited) return;

        SetState(NodeState.Available);
    }

    public void OnClick()
    {
        if (_state != NodeState.HighLited) return;

        SetState(NodeState.Selected);

        Debug.Log($"OnClick - {_type.ToString()}, ({_layer}, {_index})");
    }

    private void SetState(NodeState state)
    {
        _state = state;

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

    private void Update()
    {
        SetState(_state);
    }

}
