using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class GlobalEventSystem : MonoBehaviour
{
    [Serializable]
    public class EventContainer
    {
        public EventContainer()
        {
            _noParameterEvent = new UnityEvent();
            _singleParameterEvent = new UnityEvent<object>();
            _doubleParameterEvent = new UnityEvent<object, GameObject>();
        }

        // Serialized fields

        [SerializeField]
        private string _eventName;
        [SerializeField]
        private UnityEvent _noParameterEvent = null;
        [SerializeField]
        private UnityEvent<object> _singleParameterEvent = null;
        [SerializeField]
        private UnityEvent<object, GameObject> _doubleParameterEvent = null;

        // Public properties

        public string EventName => _eventName;

        // Public method
        public void AddListener(UnityAction listener) => _noParameterEvent.AddListener(listener);
        public void AddListener(UnityAction<object> listener) => _singleParameterEvent.AddListener(listener);
        public void AddListener(UnityAction<object, GameObject> listener) => _doubleParameterEvent.AddListener(listener);

        public void RemoveListener(UnityAction listener) => _noParameterEvent.RemoveListener(listener);
        public void RemoveListener(UnityAction<object> listener) => _singleParameterEvent.RemoveListener(listener);
        public void RemoveListener(UnityAction<object, GameObject> listener) => _doubleParameterEvent.RemoveListener(listener);

        public void Invoke(object data, GameObject instigator)
        {
            _noParameterEvent?.Invoke();
            _singleParameterEvent?.Invoke(data);
            _doubleParameterEvent?.Invoke(data, instigator);
        }
    }

    // Serialized fields

    [SerializeField]
    private bool _doNotDestroyOnLoad = false;
    [SerializeField]
    private List<EventContainer> _eventContainer = new List<EventContainer>();

    // Private fields

    private static GlobalEventSystem s_instance = null;
    private Dictionary<string, EventContainer> _eventLookup = new Dictionary<string, EventContainer>();

    // Public properties

    public static GlobalEventSystem Instance => s_instance;

    // Private methods

    private void Awake()
    {
        if (Instance == null)
        {
            s_instance = this;
            if (_doNotDestroyOnLoad)
                DontDestroyOnLoad(this);
        }
        else
            Destroy(gameObject);

        prepareLookup();
    }

    private void prepareLookup()
    {
        foreach (var e in _eventContainer)
        {
            if (!isValidEvent(e.EventName))
                _eventLookup.Add(e.EventName, e);
        }
    }

    private bool isValidEvent(string eventName)
    {
        if (_eventLookup.ContainsKey(eventName))
            return true;
        return false;
    }

    // Public methods

    public void AddListener(string eventName, UnityAction listener)
    {
        if (isValidEvent(eventName))
            _eventLookup[eventName].AddListener(listener);
    }

    public void AddListener(string eventName, UnityAction<object> listener)
    {
        if (isValidEvent(eventName))
            _eventLookup[eventName].AddListener(listener);
    }

    public void AddListener(string eventName, UnityAction<object, GameObject> listener)
    {
        if (isValidEvent(eventName))
            _eventLookup[eventName].AddListener(listener);
    }

    public void RemoveListener(string eventName, UnityAction listener)
    {
        if (isValidEvent(eventName))
            _eventLookup[eventName].RemoveListener(listener);
    }

    public void RemoveListener(string eventName, UnityAction<object> listener)
    {
        if (isValidEvent(eventName))
            _eventLookup[eventName].RemoveListener(listener);
    }

    public void RemoveListener(string eventName, UnityAction<object, GameObject> listener)
    {
        if (isValidEvent(eventName))
            _eventLookup[eventName].RemoveListener(listener);
    }

    public void InvokeEvent(string eventName) => InvokeEvent(eventName, null);

    public void InvokeEvent(string eventName, object data) => InvokeEvent(eventName, data, null);

    public void InvokeEvent(string eventName, object data, GameObject source)
    {
        if (isValidEvent(eventName))
            _eventLookup[eventName].Invoke(data, source);
        else
            Debug.LogWarning($"EVENT NOT FOUND : {eventName}");
    }
}