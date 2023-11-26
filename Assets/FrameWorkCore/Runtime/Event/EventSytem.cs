using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnEvent(EventArgsBase e);
public static class GpEvtSystem
{
    private class RegisterInfo
    {
        public int handleIndex;
        public bool bActive = true;
        public OnEvent onEvent;
    }

    private class ListRegisterInfo
    {
        public bool bPuslishing = false;
        public List<RegisterInfo> registerInfos = new();
    }

    private class PublisherInfo
    {
        public int handleIndex;
        public Dictionary<int/*event type index*/, ListRegisterInfo> dicEventIndex2OnEvent = new Dictionary<int, ListRegisterInfo>();
    }

    private class SubscriberInfo
    {
        public int handleIndex;
        public Dictionary<int/*event type index*/, List<int/*publisher index*/>> dicEventIndex2Publisher = new Dictionary<int, List<int>>();
    }

    private static int sEventHandlerIndex = 0;
    private static Dictionary<int, PublisherInfo> dicPublisherInfo = new Dictionary<int, PublisherInfo>();
    private static Dictionary<int, SubscriberInfo> dicSubscriberInfo = new Dictionary<int, SubscriberInfo>();
    private static int sEventTypeIndex = 0;
    private static Dictionary<Type, int> dicEventType2Index = new Dictionary<Type, int>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void Init()
    {
        sEventHandlerIndex = 0;
        dicPublisherInfo = new Dictionary<int, PublisherInfo>();
        dicSubscriberInfo = new Dictionary<int, SubscriberInfo>();
        sEventTypeIndex = 0;
        dicEventType2Index = new Dictionary<Type, int>();
    }

    private static void InitEventHandle(IEventHandle eventHandle)
    {
        ++sEventHandlerIndex;
        eventHandle.EventHandlerIndex = sEventHandlerIndex;

        var publisherInfo = new PublisherInfo();
        publisherInfo.handleIndex = eventHandle.EventHandlerIndex;
        dicPublisherInfo.Add(publisherInfo.handleIndex, publisherInfo);

        var subscriberInfo = new SubscriberInfo();
        subscriberInfo.handleIndex = eventHandle.EventHandlerIndex;
        dicSubscriberInfo.Add(subscriberInfo.handleIndex, subscriberInfo);
    }

    private static int GetEventTypeIndex(Type type)
    {
        int index;
        if (!dicEventType2Index.TryGetValue(type, out index))
        {
            ++sEventTypeIndex;
            index = sEventTypeIndex;
            dicEventType2Index.Add(type, index);
        }
        return index;
    }

    public static void Register<T>(this IEventHandle publisherEventHandle, IEventHandle subscriberEventHandler, OnEvent onEvent) where T : EventArgsBase
    {
        Register(typeof(T), publisherEventHandle, subscriberEventHandler, onEvent);
    }

    public static void Register(Type type, IEventHandle publisherEventHandle, IEventHandle subscriberEventHandler, OnEvent onEvent)
    {
        PublisherInfo publisherInfo;
        if (publisherEventHandle.EventHandlerIndex == 0)
        {
            InitEventHandle(publisherEventHandle);
        }
        publisherInfo = dicPublisherInfo[publisherEventHandle.EventHandlerIndex];

        SubscriberInfo subscriberInfo;
        if (subscriberEventHandler.EventHandlerIndex == 0)
        {
            InitEventHandle(subscriberEventHandler);
        }
        subscriberInfo = dicSubscriberInfo[subscriberEventHandler.EventHandlerIndex];

        int evtTypeIndex = GetEventTypeIndex(type);

        //Duplicate Registration Check
        List<int> listEventType2Publisher;
        if (subscriberInfo.dicEventIndex2Publisher.TryGetValue(evtTypeIndex, out listEventType2Publisher))
        {
            if (listEventType2Publisher.Contains(publisherInfo.handleIndex))
            {
                Debug.LogWarning("Duplicate Registration");
                return;
            }
        }

        ListRegisterInfo listRegisterInfo;
        if (!publisherInfo.dicEventIndex2OnEvent.TryGetValue(evtTypeIndex, out listRegisterInfo))
        {
            listRegisterInfo = new ListRegisterInfo();
            publisherInfo.dicEventIndex2OnEvent.Add(evtTypeIndex, listRegisterInfo);
        }

        RegisterInfo registerInfo = new RegisterInfo
        {
            handleIndex = subscriberInfo.handleIndex,
            onEvent = onEvent
        };

        listRegisterInfo.registerInfos.Add(registerInfo);

        if (listEventType2Publisher == null)
        {
            listEventType2Publisher = new List<int>();
            subscriberInfo.dicEventIndex2Publisher.Add(evtTypeIndex, listEventType2Publisher);
        }

        listEventType2Publisher.Add(publisherInfo.handleIndex);
    }

    public static void UnRegister<T>(this IEventHandle publisherEventHandle, IEventHandle subscriberEventHandler) where T : EventArgsBase
    {
        UnRegister(typeof(T), publisherEventHandle, subscriberEventHandler);
    }

    public static void UnRegister(Type type, IEventHandle publisherEventHandle, IEventHandle subscriberEventHandler)
    {
        if (publisherEventHandle.EventHandlerIndex == 0)
        {
            Debug.LogWarning("publisherEventHandle.EventHandlerIndex == 0");
            return;
        }

        if (subscriberEventHandler.EventHandlerIndex == 0)
        {
            Debug.LogWarning("publisherEventHandle.EventHandlerIndex == 0");
            return;
        }

        PublisherInfo publisherInfo;
        if (!dicPublisherInfo.TryGetValue(publisherEventHandle.EventHandlerIndex, out publisherInfo))
        {
            Debug.LogWarning("publisherInfo == null");
            return;
        }

        var typeIndex = GetEventTypeIndex(type);

        ListRegisterInfo listRegisterInfo = null;
        if (!publisherInfo.dicEventIndex2OnEvent.TryGetValue(typeIndex, out listRegisterInfo))
        {
            //Debug.LogWarning("listRegisterInfo == null");
            return;
        }
        RegisterInfo registerInfo = listRegisterInfo.registerInfos.Find(registerInfo => registerInfo.handleIndex == subscriberEventHandler.EventHandlerIndex);
        if (registerInfo == null)
        {
            Debug.LogWarning("registerInfo == null");
            return;
        }

        if (listRegisterInfo.bPuslishing)
        {
            registerInfo.bActive = false;
        }
        else
        {
            listRegisterInfo.registerInfos.Remove(registerInfo);
        }

        dicSubscriberInfo[subscriberEventHandler.EventHandlerIndex].dicEventIndex2Publisher[typeIndex].Remove(publisherEventHandle.EventHandlerIndex);
    }
    public static void ClearUpEventHandle(this IEventHandle eventHandler)
    {
        if (eventHandler.EventHandlerIndex == 0)
        {
            Debug.LogWarning("eventHandler.EventHandlerIndex == 0");
            return;
        }
        RemovePublisherInfo(eventHandler);
        RemoveSubscriberInfo(eventHandler);
        eventHandler.EventHandlerIndex = 0;
    }

    private static void RemovePublisherInfo(IEventHandle eventHandler)
    {
        PublisherInfo publisherInfo;
        if (!dicPublisherInfo.TryGetValue(eventHandler.EventHandlerIndex, out publisherInfo))
        {
            return;
        }
        dicPublisherInfo.Remove(eventHandler.EventHandlerIndex);
        foreach (var eventTypeIndex2RegisterInfo in publisherInfo.dicEventIndex2OnEvent)
        {
            foreach (var registerInfo in eventTypeIndex2RegisterInfo.Value.registerInfos)
            {
                dicSubscriberInfo[registerInfo.handleIndex].dicEventIndex2Publisher[eventTypeIndex2RegisterInfo.Key].Remove(eventHandler.EventHandlerIndex);
            }
        }
    }

    private static void RemoveSubscriberInfo(IEventHandle eventHandler)
    {
        SubscriberInfo subscriberInfo;
        if (!dicSubscriberInfo.TryGetValue(eventHandler.EventHandlerIndex, out subscriberInfo))
        {
            return;
        }
        dicSubscriberInfo.Remove(eventHandler.EventHandlerIndex);
        foreach (var eventTypeIndex2Publisher in subscriberInfo.dicEventIndex2Publisher)
        {
            foreach (var publisherHandlerIndex in eventTypeIndex2Publisher.Value)
            {
                ListRegisterInfo listRegisterInfo = dicPublisherInfo[publisherHandlerIndex].dicEventIndex2OnEvent[eventTypeIndex2Publisher.Key];
                RegisterInfo registerInfo = listRegisterInfo.registerInfos.Find(registerInfo => registerInfo.handleIndex == eventHandler.EventHandlerIndex);
                if (listRegisterInfo.bPuslishing)
                {
                    registerInfo.bActive = false;
                }
                else
                {
                    listRegisterInfo.registerInfos.Remove(registerInfo);
                }
            }
        }
    }

    public static void PostEvent<T>(this IEventHandle eventHandle, T eventBase) where T : EventArgsBase
    {
        if (eventHandle.EventHandlerIndex == 0)
        {
            Debug.LogWarning("eventHandle.EventHandlerIndex == 0");
            EventArgsPool.Release(eventBase);
            return;
        }

        PublisherInfo publisherInfo = dicPublisherInfo[eventHandle.EventHandlerIndex];

        ListRegisterInfo listRegisterInfo;
        if (!publisherInfo.dicEventIndex2OnEvent.TryGetValue(GetEventTypeIndex(typeof(T)), out listRegisterInfo))
        {
            EventArgsPool.Release(eventBase);
            return;
        }

        listRegisterInfo.bPuslishing = true;
        bool bWaitClear = false;
        // not available foreach,May add new items while traversing
        for (int i = 0; i < listRegisterInfo.registerInfos.Count; ++i)
        {
            var registerInfo = listRegisterInfo.registerInfos[i];
            if (registerInfo.bActive)
            {
                registerInfo.onEvent(eventBase);
            }
            else
            {
                bWaitClear = true;
            }
        }
        if (bWaitClear)
        {
            listRegisterInfo.registerInfos.RemoveAll(registerInfo => !registerInfo.bActive);
        }
        listRegisterInfo.bPuslishing = false;
        EventArgsPool.Release(eventBase);
    }
}

public class GpEventMgr : IEventHandle
{
    private static GpEventMgr _instance;
    public static GpEventMgr Instance
    {
        get
        {
            _instance ??= new GpEventMgr();
            return _instance;
        }
    }

    public int EventHandlerIndex { get; set; }
}