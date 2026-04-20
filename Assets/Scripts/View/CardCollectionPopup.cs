using System.Collections;
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
        StartCoroutine(ResetScrollCoroutine());
    }

    private IEnumerator ResetScrollCoroutine()
    {
        yield return null;
        Canvas.ForceUpdateCanvases();
        _scrollRect.verticalNormalizedPosition = 1.0f;
    }

}
