using UnityEngine;
using UnityEngine.UI;

public class MapView : UIView
{
    [SerializeField] private ScrollRect _scrollRect;
    [SerializeField] private RectTransform _content; // Map ºÎ¸ð
    [SerializeField] private RectTransform _viewport;

    public override void Show()
    {
        base.Show();
        if(GameManager.Instance.CurrentNode != null)
            FocusNode(GameManager.Instance.CurrentNode);
    }

    public void FocusNode(Node node)
    {
        Canvas.ForceUpdateCanvases();
        RectTransform nodeRect = node.GetComponent<RectTransform>();

        float contentHeight = _content.rect.height;
        float viewportHeight = _viewport.rect.height;
        float scrollableHeight = contentHeight - viewportHeight;

        if (scrollableHeight <= 0)
        {
            _scrollRect.verticalNormalizedPosition = 0f;
            return;
        }

        Vector3 worldPos = nodeRect.position;
        Vector3 localPos = _content.InverseTransformPoint(worldPos);

        float targetY = localPos.y + (contentHeight * 0.5f) - (viewportHeight * 0.5f);
        float normalized = targetY / scrollableHeight;
        _scrollRect.verticalNormalizedPosition = Mathf.Clamp01(normalized);
    }

}
