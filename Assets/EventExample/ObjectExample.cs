using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ObjectExample : MonoBehaviour
{
    public List<ObjectComponentBlueprint> componentsBlueprint = new List<ObjectComponentBlueprint>();
    List<ObjectComponent> ObjectComponents = new List<ObjectComponent>();
    
    // Start is called before the first frame update
    void Start() {
        for (int i = 0; i < componentsBlueprint.Count; i++) {
            ObjectComponents.Add(componentsBlueprint[i].CreateObjectComponet(this));
        }
    }

    public ObjectExample attackOtherObject;
    [ContextMenu("Do Attack")]
    public void DoAttack()
    {
        if (!invincibilityFrameActive) // Check if invincibility frames are active
        {
            EventExample attemptAttackEvent = new EventExample("AttemptAttack", "Attacker", this, "Damage", 5);
            if (SendEvent(attemptAttackEvent))
            {
                var AttemptDealDamage = attemptAttackEvent.ChangeEventName("AttemptDealDamage");
                if (attackOtherObject.SendEvent(AttemptDealDamage))
                {
                    Debug.Log($"Send event AttemptAttack succeeded to {attackOtherObject}. {attemptAttackEvent}");
                    // Event succeeded, let's execute attack.
                    EventExample executeAttack = AttemptDealDamage.ChangeEventName("ExecuteDealDamage");
                    if (attackOtherObject.SendEvent(executeAttack))
                    {
                        Debug.Log($"Send event ExecuteAttack succeeded to {attackOtherObject} {executeAttack}");

                        // Start invincibility frames
                        StartInvincibilityFrames();
                    }
                    else
                    {
                        Debug.Log($"Send event ExecuteAttack failed to {attackOtherObject} {executeAttack}");
                    }
                }
                else
                {
                    Debug.Log($"Send event AttemptAttak failed to {attackOtherObject} {attemptAttackEvent}");
                }
            }
        }
    }

    private bool invincibilityFrameActive = false;
    private float invincibilityDuration = 0.4f;

    private IEnumerator ActivateInvincibilityFrames()
    {
        invincibilityFrameActive = true;
        yield return new WaitForSeconds(invincibilityDuration);
        invincibilityFrameActive = false;
    }

    private void StartInvincibilityFrames()
    {
        StartCoroutine(ActivateInvincibilityFrames());
    }

    public bool SendEvent(EventExample eventSent) {
        for (int i = 0; i < ObjectComponents.Count; i++) {
            if (!ObjectComponents[i].SendEvent(eventSent)) return false;
        }
        return true;
    }
}

[Serializable]
public class ObjectComponent {
    public ObjectExample owner;
    public Dictionary<string, int> parameterDictionary = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
    public virtual void Initialize(List<ComponentParameter> parameters) {
        for (int i = 0; i < parameters.Count; i++) {
            parameterDictionary[parameters[i].parameterName] = parameters[i].parameterValue;
        }
    }
    
    public virtual bool SendEvent(EventExample eventSent) {
        return true;
    }
}

public static class ParameterExtensionMethods {
    public static int GetInt(this Dictionary<string, int> dict, string parameterName, int defaultInt)=> dict.TryGetValue(parameterName, out int foundInt) ? foundInt : defaultInt;

    public static int GetInt(this Dictionary<string, object> dict, string parameterName, int defaultInt)
    {
        if (dict.TryGetValue(parameterName, out object foundInt) && foundInt is int) {
            return (int) foundInt;
        } 
        return defaultInt;
    }
}

[Serializable]
public class ObjectComponentBlueprint {
    public string componentName;
    public List<ComponentParameter> parameters = new List<ComponentParameter>();

    public ObjectComponent CreateObjectComponet(ObjectExample newOwner) {
        Assembly assembly = Assembly.GetExecutingAssembly();

        // Get the Type of the class using the class name
        Type classType = assembly.GetType(componentName);
        if (classType != null) {
            // Create an instance of the class type
            ObjectComponent instance = (ObjectComponent)Activator.CreateInstance(classType);
            instance.owner = newOwner;
            instance.Initialize(parameters);
            return instance;
        }
        else
        {
            Debug.LogError("Class not found: " + componentName);
            var newInstance = new ObjectComponent();
            newInstance.owner = newOwner;
            newInstance.Initialize(parameters);
            return newInstance;
        }
    }
}

[Serializable]
public struct ComponentParameter {
    public string parameterName;
    public int parameterValue;
}