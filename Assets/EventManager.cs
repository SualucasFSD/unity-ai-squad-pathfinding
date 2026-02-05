using System.Collections.Generic;
using UnityEngine;

public class EventManager
{
    public enum KindOfEvent
    {
        OnDetect,
    }
    public delegate void MethodToSuscribe(params object[] Parameters);
    static Dictionary<KindOfEvent, MethodToSuscribe> _events;

    public static void Suscribe(KindOfEvent _type, MethodToSuscribe _method)
    {
        if (_events == null) _events = new Dictionary<KindOfEvent, MethodToSuscribe>();
        //Similar a _events ??= new Dictionary<EventType, MethodToSuscribe>();
        if (!_events.ContainsKey(_type))
        {
            _events.Add(_type, null);
        }
        //Similar a: _events.TryAdd(_type, null);
        _events[_type] += _method;
    }

    public static void Unscribe(KindOfEvent _type, MethodToSuscribe _method)
    {
        if (_events == null) return;
        if (!_events.ContainsKey(_type)) { return; }
        _events[_type] -= _method;
    }
    public static void Ejecute(KindOfEvent _type)
    {
        if(_events == null) return;
        if(!_events.ContainsKey(_type)) return;
        if(_events[_type]==null) return;
        _events[_type]();
    }
    public static void ResetEvent()
    {
        _events.Clear();
    }
}
