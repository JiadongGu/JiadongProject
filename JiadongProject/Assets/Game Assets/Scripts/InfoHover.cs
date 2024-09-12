using UnityEngine;
using UnityEngine.EventSystems;

public class InfoHover : MonoBehaviour
{
    public GameObject infoPanel;
    EventTrigger eventTrigger;

    void Start()
    {
        OnPointerExit();
        
        eventTrigger = gameObject.GetComponent<EventTrigger>();
        if (eventTrigger == null)
        {
            eventTrigger = gameObject.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry pointerEnterEntry = new EventTrigger.Entry();
        pointerEnterEntry.eventID = EventTriggerType.PointerEnter;
        pointerEnterEntry.callback.AddListener((eventData) => { OnPointerEnter(); });

        // Create a new entry for OnPointerExit
        EventTrigger.Entry pointerExitEntry = new EventTrigger.Entry();
        pointerExitEntry.eventID = EventTriggerType.PointerExit;
        pointerExitEntry.callback.AddListener((eventData) => { OnPointerExit(); });

        // Add the entries to the EventTrigger
        eventTrigger.triggers.Add(pointerEnterEntry);
        eventTrigger.triggers.Add(pointerExitEntry);

        
    }

    void OnPointerEnter()
    {
        infoPanel.SetActive(true);
    }

    void OnPointerExit()
    {
        infoPanel.SetActive(false);
    }
}
