using UnityEngine;
using UnityEngine.UI;

public class CardCollectionPopup : UIPopup
{
    private ScrollRect _scrollRect;

    private void Awake()
    {
        _scrollRect = GetComponent<ScrollRect>();
    }

    public override void Show()
    {
        base.Show();
        ResetScroll();
    }

    private void ResetScroll()
    {
        Canvas.ForceUpdateCanvases();
        _scrollRect.verticalNormalizedPosition = 1f;
    }

}
