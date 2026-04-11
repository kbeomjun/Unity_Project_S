using UnityEngine;

public class SlotGround : MonoBehaviour
{
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private Material _outlineMaterial;
    public int SlotIndex { get; set; }

    private void Awake()
    {
        _outlineMaterial = _spriteRenderer.material;
    }

    public void SetHighlight(bool onOff)
    {
        _outlineMaterial.SetFloat("_OutlineSize", onOff ? 20.0f : 0f);
    }

}
