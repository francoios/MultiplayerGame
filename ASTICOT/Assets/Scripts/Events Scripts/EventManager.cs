using System.Collections;
using System.Collections.Generic;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class EventMsg : UnityEvent<System.Object>
{
}

[System.Serializable]
public class ClientConnectionMsg : UnityEvent<System.Object>
{
    public NetworkConnection ClientInfo;
}


public class EventManager : MonoBehaviour
{
    public static EventManager Instance;

    private Dictionary<string, EventMsg> _eventDictionary;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            _eventDictionary = new Dictionary<string, EventMsg>();
        }
        else if (Instance != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    public static void StartListening(string eventName, UnityAction<System.Object> listener)
    {
        EventMsg thisEvent = null;
        if (Instance._eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new EventMsg();
            thisEvent.AddListener(listener);
            Instance._eventDictionary.Add(eventName, thisEvent);
        }
    }

    public static void StopListening(string eventName, UnityAction<System.Object> listener)
    {
        if (Instance == null) return;
        EventMsg thisEvent = null;
        if (Instance._eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public static void TriggerEvent(string eventName, System.Object arg = null)
    {
        EventMsg thisEvent = null;
        if (Instance._eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke(arg);
        }
    }
}
