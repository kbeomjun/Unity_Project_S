using System.Collections.Generic;
using UnityEngine;

public class PlayerCardManager : MonoBehaviour
{
    [SerializeField] private RectTransform _canvasRect;
    [SerializeField] private GameObject _removeButton;
    [SerializeField] private GameObject _prevButton;

    private Vector3 _originScale = new Vector3(1.0f, 1.0f, 0.0f);
    private Vector2 _addPosition = new Vector2(811.0f, 1037.0f);
    private float _hoverScale = 1.2f;
    private float _selectScale = 1.4f;
    private float _speed = 10.0f;

    private Card _selectedCard = null;
    private Vector2 _uiMousePos = Vector2.zero;

    public static PlayerCardManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
    }

}
