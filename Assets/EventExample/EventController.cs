using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventController
{
    private static EventController instance;
    public enum Event
    {
        attemptAttack,
        attemptDealDamage,
        executeDealDamage
    }

    public enum DamageType
    {
        normalDamage,
        fireDamage,
    }

    public enum Object
    {
        attacker,
    }

    public Event eventName;
    public Object objectName;
    public DamageType damageTypeName;

    public Dictionary<int, object> eventParameters = new Dictionary<int, object>();

    public static EventController Instance
    {
        get
        {
            return instance;
        }
    }

    public override string ToString()
    {
        string exposeEvent = $"[Event: {eventName}]";
        foreach (var parameter in eventParameters)
        {
            exposeEvent += $" {parameter.Key}: {parameter.Value}";
        }
        return exposeEvent;
    }

    public EventController ChangeEventName(Event newName)
    {
        eventName = newName;
        return this;
    }

    public EventController(Event newName, EventController otherEvent)
    {
        this.eventName = newName;
        foreach (var parameter in otherEvent.eventParameters)
        {
            eventParameters[parameter.Key] = parameter.Value;
        }
    }


    public EventController(Event eventName, int parameterName, object parameterContent)
    {
        this.eventName = eventName;
        eventParameters[parameterName] = parameterContent;
    }

    public EventController(Event eventName, int parameterName, object parameterContent, int parameterName2, object parameterContent2)
    {
        this.eventName = eventName;
        eventParameters[parameterName] = parameterContent;
        eventParameters[parameterName2] = parameterContent2;
    }

    public EventController(Event eventName, int parameterName, object parameterContent, int parameterName2, object parameterContent2, int parameterName3, object parameterContent3)
    {
        this.eventName = eventName;
        eventParameters[parameterName] = parameterContent;
        eventParameters[parameterName2] = parameterContent2;
        eventParameters[parameterName3] = parameterContent3;
    }

    public EventController(Event eventName, int parameterName, object parameterContent, int parameterName2, object parameterContent2, int parameterName3, object parameterContent3, int parameterName4, object parameterContent4)
    {
        this.eventName = eventName;
        eventParameters[parameterName] = parameterContent;
        eventParameters[parameterName2] = parameterContent2;
        eventParameters[parameterName3] = parameterContent3;
        eventParameters[parameterName4] = parameterContent4;
    }
}
