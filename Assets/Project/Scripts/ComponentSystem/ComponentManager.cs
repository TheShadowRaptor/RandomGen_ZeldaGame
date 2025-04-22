using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

static public class ComponentManager
{
    const int registryCompacity = 49999;
    static private Dictionary<int, GameComponent> registeredComponents = new Dictionary<int, GameComponent>();
    static private Queue<int> availableIDs = new Queue<int>();

    static private int GiveRegistryID()
    {
        if (availableIDs.Count > 0) return availableIDs.Dequeue();
        
        int id = 0;
        do
        {
            id++;
        } while (registeredComponents.ContainsKey(id));

        return id;
    }

    static public bool AttemptRegisterComponent(GameComponent _gameComponent)
    {
        if (registeredComponents.Count >= registryCompacity)
        {
            Debug.LogError($"{_gameComponent} can not be registered because component registry reached max compacity");
            return false;
        }
            
        registeredComponents.Add(GiveRegistryID(), _gameComponent);
        return true;
    }

    static public void UnregisterComponent(int _registerID)
    {
        if (registeredComponents.ContainsKey(_registerID))
        {
            registeredComponents.Remove(_registerID);
            availableIDs.Enqueue(_registerID);
        }
    }
}
