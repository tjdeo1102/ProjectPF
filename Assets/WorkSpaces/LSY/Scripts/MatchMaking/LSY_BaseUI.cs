using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;


public class LSY_BaseUI : MonoBehaviour
{
    private Dictionary<string, GameObject> gameObjectDic;
    private Dictionary<(string, System.Type), Component> componentDic;

    protected void Bind()
    {
        Transform[] transforms = GetComponentsInChildren<Transform>(true);
        gameObjectDic = new Dictionary<string, GameObject>(transforms.Length << 2);

        foreach (Transform child in transforms)
        {
            gameObjectDic.TryAdd(child.gameObject.name, child.gameObject);
        }

        componentDic = new Dictionary<(string, System.Type), Component>();
    }

    protected void BindAll()
    {
        Transform[] transforms = GetComponentsInChildren<Transform>(true);
        gameObjectDic = new Dictionary<string, GameObject>(transforms.Length << 2);

        foreach (Transform child in transforms)
        {
            gameObjectDic.TryAdd(child.gameObject.name, child.gameObject);
        }

        Component[] components = GetComponentsInChildren<Component>(true);
        componentDic = new Dictionary<(string, System.Type), Component>(components.Length << 4);
        foreach (Component child in components)
        {
            componentDic.TryAdd((child.gameObject.name, components.GetType()), child);
        }
    }

    public GameObject GetUI(in string name)
    {
        gameObjectDic.TryGetValue(name, out GameObject gameObject);
        return gameObject;
    }

    public T GetUI<T>(in string name) where T : Component
    {
        (string, System.Type) key = (name, typeof(T));


        componentDic.TryGetValue(key, out Component component);
        if (component != null)
            return component as T;

        gameObjectDic.TryGetValue(name, out GameObject gameObject);
        if (gameObject == null)
            return null;

        component = gameObject.GetComponent<T>();
        if (component == null)
            return null;

        componentDic.TryAdd(key, component);
        return component as T;
    }

    public enum EventType { Click, Enter, Exit, Up, Down, Move, BeginDrag, EndDrag, Drag, Drop }
    public void AddEvent(in string name, EventType eventType, UnityAction<PointerEventData> callback)
    {
        gameObjectDic.TryGetValue(name, out GameObject gameObject);
        if (gameObject == null)
            return;

        LSY_EventReceiver receiver = gameObject.GetOrAddComponent<LSY_EventReceiver>();
        switch (eventType)
        {
            case EventType.Click:
                receiver.OnClicked += callback;
                break;
            case EventType.Enter:
                receiver.OnEntered += callback;
                break;
            case EventType.Exit:
                receiver.OnExited += callback;
                break;
            case EventType.Up:
                receiver.OnUped += callback;
                break;
            case EventType.Down:
                receiver.OnDowned += callback;
                break;
            case EventType.Move:
                receiver.OnMoved += callback;
                break;
            case EventType.BeginDrag:
                receiver.OnBeginDraged += callback;
                break;
            case EventType.EndDrag:
                receiver.OnEndDraged += callback;
                break;
            case EventType.Drag:
                receiver.OnDraged += callback;
                break;
            case EventType.Drop:
                receiver.OnDroped += callback;
                break;

        }
    }

    public void RemoveEvent(in string name, EventType eventType, UnityAction<PointerEventData> callback)
    {
        gameObjectDic.TryGetValue(name, out GameObject gameObject);
        if (gameObject == null)
            return;

        LSY_EventReceiver receiver = gameObject.GetOrAddComponent<LSY_EventReceiver>();
        switch (eventType)
        {
            case EventType.Click:
                receiver.OnClicked -= callback;
                break;
            case EventType.Enter:
                receiver.OnEntered -= callback;
                break;
            case EventType.Exit:
                receiver.OnExited -= callback;
                break;
            case EventType.Up:
                receiver.OnUped -= callback;
                break;
            case EventType.Down:
                receiver.OnDowned -= callback;
                break;
            case EventType.Move:
                receiver.OnMoved -= callback;
                break;
            case EventType.BeginDrag:
                receiver.OnBeginDraged -= callback;
                break;
            case EventType.EndDrag:
                receiver.OnEndDraged -= callback;
                break;
            case EventType.Drag:
                receiver.OnDraged -= callback;
                break;
            case EventType.Drop:
                receiver.OnDroped -= callback;
                break;

        }
    }
}