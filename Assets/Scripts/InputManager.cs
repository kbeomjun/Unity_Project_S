using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public enum InputState
{
    None,
    CardCollection,
    BattlePrepare,
    Battle,
    RewardCard,
    Shop,
}

public class InputManager : MonoBehaviour
{
    public InputState State => _stateStack.Count > 0 ? _stateStack.Peek() : InputState.None;
    private Stack<InputState> _stateStack = new Stack<InputState>();

    private PlayerInputActions _input;
    private Vector2 _mousePos = Vector2.zero;
    private Vector3 _screenPos = Vector3.zero;
    private Vector3 _worldPos = Vector3.zero;

    public static InputManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        _input = new PlayerInputActions();
    }

    public void Push(InputState newState)
    {
        _stateStack.Push(newState);
    }

    public void Pop()
    {
        if (_stateStack.Count > 0)
            _stateStack.Pop();
    }

    public void PopUntil(InputState targetState)
    {
        while (_stateStack.Count > 0)
        {
            if (_stateStack.Peek() == targetState)
            {
                _stateStack.Pop();
                break;
            }
            
            InputState state = _stateStack.Pop();
            if (state == InputState.CardCollection || state == InputState.None)
                ViewManager.Instance.Pop();
        }
    }

    private void OnEnable()
    {
        _input.Enable();
        _input.Player.Click.started += OnClick;
        _input.Player.Click.canceled += OnRelease;
    }

    private void OnDisable()
    {
        _input.Player.Click.started -= OnClick;
        _input.Player.Click.canceled -= OnRelease;
        _input.Disable();
    }

    private void OnClick(InputAction.CallbackContext ctx)
    {
        switch (State)
        {
            case InputState.CardCollection:
                GameManager.Instance.CardCollectionUI.OnClick();
                break;

            case InputState.BattlePrepare:
                BattleManager.Instance.OnClick();
                break;

            case InputState.Battle:
                BattleManager.Instance.DeckUI.OnClick();
                break;

            case InputState.RewardCard:
                RewardManager.Instance.OnClick();
                break;

            case InputState.Shop:
                TownRestManager.Instance.OnClick();
                break;
        }
    }

    private void OnRelease(InputAction.CallbackContext ctx)
    {
        switch (State)
        {
            case InputState.CardCollection:
                GameManager.Instance.CardCollectionUI.OnRelease();
                break;

            case InputState.BattlePrepare:
                BattleManager.Instance.OnRelease();
                break;

            case InputState.Battle:
                BattleManager.Instance.DeckUI.OnRelease(_mousePos);
                break;

            case InputState.RewardCard:
                RewardManager.Instance.OnRelease();
                break;

            case InputState.Shop:
                TownRestManager.Instance.OnRelease();
                break;
        }
    }

    private void MouseProcess()
    {
        _mousePos = _input.Player.Position.ReadValue<Vector2>();
        _screenPos = new Vector3(_mousePos.x, _mousePos.y, 8.0f);
        _worldPos = Camera.main.ScreenToWorldPoint(_screenPos);
        _worldPos.z = 0.0f;
    }

    private void Update()
    {
        MouseProcess();

        switch (State)
        {
            case InputState.CardCollection:
                GameManager.Instance.CardCollectionUI.MouseProcess(_mousePos);
                break;

            case InputState.BattlePrepare:
                BattleManager.Instance.MouseProcess(_worldPos);
                break;

            case InputState.RewardCard:
                RewardManager.Instance.MouseProcess(_mousePos);
                break;

            case InputState.Shop:
                TownRestManager.Instance.MouseProcess(_mousePos);
                break;
        }
    }

    private void LateUpdate()
    {
        switch (State)
        {
            case InputState.Battle:
                BattleManager.Instance.DeckUI.MouseProcess(_mousePos, _worldPos);
                break;
        }
    }

}
