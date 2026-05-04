using System.Collections.Generic;
using UnityEngine;

public class StartManager : MonoBehaviour
{
    [SerializeField] private EventButton[] _eventButton;
    [SerializeField] private GameObject _nextButton;

    private List<EventOption> _startEvents = new List<EventOption>();

    public static StartManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        InitEvents();
    }

    private void InitEvents()
    {
        _startEvents = new List<EventOption>()
        {
            new EventOption("Gain 100 Coin", new List<IEventEffect> { new GainGold(100) }),
            new EventOption("Recruit Unit: Knight", new List<IEventEffect> { new AddUnit(0) }),
            new EventOption("Recruit Unit: Lancer", new List<IEventEffect> { new AddUnit(1) }),
            new EventOption("Recruit Unit: Archer", new List<IEventEffect> { new AddUnit(2) }),
            new EventOption("Recruit Unit: Monk", new List<IEventEffect> { new AddUnit(3) }),
        };
    }

    public void StartStart(int chapter)
    {
        RecruitRandomUnit();

        for (int i = 0; i < GameManager.Instance.PlayerUnitDatas.Count; i++)
            GameManager.Instance.CureUnit(i);

        GameEvent gameEvent = CreateRandomStartEvent(chapter);

        for (int i = 0; i < _eventButton.Length; i++)
        {
            _eventButton[i].gameObject.SetActive(true);
            _eventButton[i].Init(gameEvent.Options[i]);
        }
    }

    private void RecruitRandomUnit()
    {
        if (GameManager.Instance.PlayerUnitDatas.Count >= 4) return;
        int unitType = Random.Range(0, DataManager.Instance.PlayerUnitPrefabs.Length);
        GameManager.Instance.AddUnit(unitType);
        GameManager.Instance.PlayRecruitAnimation(unitType);
    }

    private GameEvent CreateRandomStartEvent(int chapter)
    {
        HashSet<int> set = new HashSet<int>();
        GameEvent gameEvent = new GameEvent(new List<EventOption>());

        while(gameEvent.Options.Count < 3)
        {
            int random = Random.Range(0, _startEvents.Count);

            if (set.Contains(random)) continue;

            set.Add(random);
            gameEvent.Options.Add(_startEvents[random]);
        }

        return gameEvent;
    }

    public void EndEvent()
    {
        for (int i = 0; i < _eventButton.Length; i++)
        {
            _eventButton[i].gameObject.SetActive(false);
        }
    }

    public void OnClickNextButton()
    {
        GameManager.Instance.OnClearNode();
        SoundManager.Instance.PlayButtonClickSound();
    }

}
