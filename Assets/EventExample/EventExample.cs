using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventExample {
    public string eventName;
    public Dictionary<string, object> eventParameters = new Dictionary<string, object>(StringComparer.Ordinal);

    public override string ToString()
    {
        string exposeEvent = $"[Event: {eventName}]";
        foreach (var parameter in eventParameters) {
            exposeEvent += $" {parameter.Key}: {parameter.Value}";
        }
        return exposeEvent;
    }

    public EventExample ChangeEventName(string newName) {
        eventName = newName;
        return this;
    }
    
    public EventExample(string newName, EventExample otherEvent) {
        this.eventName = newName;
        foreach (var parameter in otherEvent.eventParameters) {
            eventParameters[parameter.Key] = parameter.Value;
        }
    }

    
    public EventExample(string eventName, string parameterName, object parameterContent) {
        this.eventName = eventName;
        eventParameters[parameterName] = parameterContent;
    }
    
    public EventExample(string eventName, string parameterName, object parameterContent, string parameterName2, object parameterContent2) {
        this.eventName = eventName;
        eventParameters[parameterName] = parameterContent;
        eventParameters[parameterName2] = parameterContent2;
    }
    
    public EventExample(string eventName, string parameterName, object parameterContent, string parameterName2, object parameterContent2, string parameterName3, object parameterContent3) {
        this.eventName = eventName;
        eventParameters[parameterName] = parameterContent;
        eventParameters[parameterName2] = parameterContent2;
        eventParameters[parameterName3] = parameterContent3;
    }
    public EventExample(string eventName, string parameterName, object parameterContent, string parameterName2, object parameterContent2, string parameterName3, object parameterContent3, string parameterName4, object parameterContent4) {
        this.eventName = eventName;
        eventParameters[parameterName] = parameterContent;
        eventParameters[parameterName2] = parameterContent2;
        eventParameters[parameterName3] = parameterContent3;
        eventParameters[parameterName4] = parameterContent4;
    }
}
