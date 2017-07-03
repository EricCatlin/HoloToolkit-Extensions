using HoloToolkit.Unity.InputModule;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class OnAirTap : MonoBehaviour, IInputClickHandler
{

    //Maintains a list of UnityEvents to be triggered by AirTapping
    public List<UnityEvent> OnClickEvents = new List<UnityEvent>();
    public void OnInputClicked(InputClickedEventData eventData)
    {
        //Validate
        if (OnClickEvents == null || OnClickEvents.Count == 0) { Debug.Log("No Events in " + gameObject.name); return; }
        Debug.Log(string.Format("{0} Airtapped by {1} : Invoking {2} events", gameObject.name, eventData.SourceId, OnClickEvents.Count));

        //Execution Loop
        for (var i = OnClickEvents.Count-1; i >= 0; i--)
        {
            var ev = OnClickEvents[i];
            if (ev == null) { Debug.LogError("Null event skipped while airtapping " + gameObject.name); continue; }
            ev.Invoke();
        }
    }
}
