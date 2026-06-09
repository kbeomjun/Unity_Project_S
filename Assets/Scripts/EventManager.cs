using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EventManager : MonoBehaviour
{
    [SerializeField] private Image _eventIcon;
    [SerializeField] private TMP_Text _eventDescription;
    [SerializeField] private RectTransform _eventButtonTr;

    private List<string> _eventDescriptions = new List<string>();
    private List<GameEvent> _events = new List<GameEvent>();
    private List<EventButton> _eventButtons = new List<EventButton>();

    public static EventManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void InitEvents()
    {
        _eventDescriptions = new List<string>()
        {
            //"Encountered a wandering mercenary group...",
            //"Discovered an ancient sanctuary deep in the forest. A faint light is emanating from the altar...",
            "Found a seriously injured man by the side of the forest path. A worn-out bag lay beside him. He appeared to still be alive.",
        };
        _events = new List<GameEvent>()
        {
            //new GameEvent
            //(
            //    new List<EventOption>
            //    {
            //        new EventOption("Sell supplies", "Gain 50 coin", new List<IEventEffect> { new GainCoin(50) }),
            //        new EventOption("Plunder the mercenary group", "Gain 300 coin, Lose 20 health(All Party)", new List<IEventEffect> { new GainCoin(300), new HurtUnit(20, true) }),
            //        CreateOption()
            //    }
            //),
            //new GameEvent
            //(
            //    new List<EventOption>
            //    {
            //        new EventOption("Offer a prayer to God", "Increase defense by 5(All Party)", new List<IEventEffect> { new UpgradeDefense(5) }),
            //        new EventOption("Offer tribute", "Pay 100 coin, Increase maxhealth by 5(All Party)", new List<IEventEffect> { new PayCoin(100), new UpgradeHealth(5) }),
            //        new EventOption("Pledge a blood oath.", "Lose 10 health(All Party), Increase attack by 5(All Party)", new List<IEventEffect> { new HurtUnit(10, true), new UpgradeAttack(5) }),
            //    }
            //),
            new GameEvent
            (
                new List<EventOption>
                {
                    new EventOption("Heal it", "Pay 100 coin, Get 1 random card", new List<IEventEffect> { new PayCoin(100), new GetRandomCard(1) }),
                    new EventOption("Kill and plunder", "Decrease maxhealth by 5(All Party), Get 1 random card", new List<IEventEffect> { new DecreaseHealth(5), new GetRandomCard(1) }),
                    new EventOption("Abandon and leave", "Nothing happens", new List<IEventEffect> { }),
                }
            ),
        };
    }

    private EventOption CreateOption()
    {
        string text = "Pay 100 coin, Add unit to party: ";
        int unitType = Random.Range(0, DataManager.Instance.PlayerUnitPrefabs.Length);

        switch (unitType)
        {
            case 0:
                text += "Knight";
                break;

            case 1:
                text += "Lancer";
                break;

            case 2:
                text += "Archer";
                break;

            case 3:
                text += "Monk";
                break;

            default:
                break;
        }

        return new EventOption("Recruit troops", text, new List<IEventEffect> { new PayCoin(100), new AddUnit(unitType) });
    }

    public void StartEvent(int chapter)
    {
        InitEvents();

        int index = Random.Range(0, _events.Count);
        _eventIcon.sprite = DataManager.Instance.EventSprites[index];
        _eventDescription.text = _eventDescriptions[index];
        GameEvent gameEvent = _events[index];

        foreach (EventOption option in gameEvent.Options)
        {
            EventButton eventButton = Instantiate(DataManager.Instance.EventButtonPrefab, _eventButtonTr, false);
            eventButton.Init(option);
            _eventButtons.Add(eventButton);
        }
    }

    public void EndEvent()
    {
        Clear();
        GameManager.Instance.OnClearNode();
    }

    private void Clear()
    {
        foreach (EventButton button in _eventButtons)
            Destroy(button.gameObject);
        _eventButtons.Clear();
    }

}
