using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class NamePanel : MonoBehaviour
{
    [SerializeField] private TMP_Text _nameText;

    private RectTransform _rect;
    private CanvasGroup _canvasGroup;

    public static NamePanel Instance { get; private set; }

    private Tween _fadeTween;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        _rect = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void Init()
    {
        _canvasGroup.alpha = 0f;
    }

    public void Show(Vector2 localPosition, string text)
    {
        _fadeTween?.Kill();

        _rect.anchoredPosition = localPosition;
        _nameText.text = text;

        LayoutRebuilder.ForceRebuildLayoutImmediate(_rect);

        _canvasGroup.alpha = 0.0f;
        _fadeTween = _canvasGroup.DOFade(1.0f, 0.5f);
    }

    public void Hide()
    {
        _fadeTween?.Kill();
        _fadeTween = _canvasGroup.DOFade(0.0f, 0.5f);
    }

}
