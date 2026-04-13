using UnityEngine;

public class TargetArrow : MonoBehaviour
{
    [SerializeField] private RectTransform _head;
    [SerializeField] private RectTransform _body;

    private RectTransform _rect;

    private void Awake()
    {
        _rect = GetComponent<RectTransform>();
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

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

}
