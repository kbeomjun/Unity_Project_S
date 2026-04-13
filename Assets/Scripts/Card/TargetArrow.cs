using UnityEngine;
using UnityEngine.UI;

public class TargetArrow : MonoBehaviour
{
    [SerializeField] private RectTransform _head;
    [SerializeField] private RectTransform _body;

    private RectTransform _rect;

    private Image _headImage;
    private Image _bodyImage;

    private Color[] _colors = new Color[]
    {
        new Color32(210, 40, 40, 255),
        new Color32(18, 156, 34, 255)
    };

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
        _headImage = _head.GetComponent<Image>();
        _bodyImage = _body.GetComponent<Image>();
        SetArrowColor(false);
    }

    public void UpdateArrow(Vector2 start, Vector2 end)
    {
        Vector2 dir = end - start;
        float dist = dir.magnitude;

        if (dist <= 0.001f) return;

        Vector2 direction = dir / dist;

        _rect.anchoredPosition = start;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        _rect.localRotation = Quaternion.Euler(0.0f, 0.0f, angle);

        float headSize = _head.rect.width;

        _body.anchoredPosition = new Vector2(dist * 0.5f, 0f);
        _body.sizeDelta = new Vector2(dist, _body.sizeDelta.y);

        _head.anchoredPosition = new Vector2(dist, 0f);
    }

    public void SetArrowColor(bool onOff)
    {
        if (onOff)
        {
            _headImage.color = _colors[1];
            _bodyImage.color = _colors[1];
        }
        else
        {
            _headImage.color = _colors[0];
            _bodyImage.color = _colors[0];
        }
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

}
