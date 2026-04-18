using UnityEngine;
using UnityEngine.InputSystem;

public enum InputState
{
    None,
    BattlePrepare,
    Battle,
    RewardCard,
    Shop,
}

public class InputManager : MonoBehaviour
{
    public InputState State { get; set; }

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
            case InputState.BattlePrepare:
                BattleManager.Instance.OnClick();
                break;

            case InputState.Battle:
                CardManager.Instance.OnClick();
                break;

            case InputState.RewardCard:
                RewardManager.Instance.OnClick();
                break;

            case InputState.Shop:
                TownManager.Instance.OnClick();
                break;
        }
    }

    private void OnRelease(InputAction.CallbackContext ctx)
    {
        switch (State)
        {
            case InputState.BattlePrepare:
                BattleManager.Instance.OnRelease();
                break;

            case InputState.Battle:
                CardManager.Instance.OnRelease(_mousePos);
                break;

            case InputState.RewardCard:
                RewardManager.Instance.OnRelease();
                break;

            case InputState.Shop:
                TownManager.Instance.OnRelease();
                break;
        }
    }

    private void MouseProcess()
    {
        _mousePos = _input.Player.Position.ReadValue<Vector2>();
        _screenPos = new Vector3(_mousePos.x, _mousePos.y, 8.0f);
        _worldPos = Camera.main.ScreenToWorldPoint(_screenPos);
        _worldPos.z = 0.0f;

        switch (State)
        {
            case InputState.BattlePrepare:
                BattleManager.Instance.MouseProcess(_worldPos);
                break;

            case InputState.Battle:
                CardManager.Instance.MouseProcess(_mousePos, _worldPos);
                break;

            case InputState.RewardCard:
                RewardManager.Instance.MouseProcess(_mousePos);
                break;

            case InputState.Shop:
                TownManager.Instance.MouseProcess(_mousePos);
                break;
        }
    }

    private void Update()
    {
        switch (State)
        {
            case InputState.BattlePrepare:
                MouseProcess();
                break;

            case InputState.RewardCard:
                MouseProcess();
                break;

            case InputState.Shop:
                MouseProcess();
                break;
        }
    }

    private void LateUpdate()
    {
        switch (State)
        {
            case InputState.Battle:
                MouseProcess();
                break;
        }
    }

}
