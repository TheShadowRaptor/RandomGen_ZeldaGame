using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

public class ObjectSystem : MonoBehaviour
{
    public List<ObjectComponentBlueprint> componentsBlueprint = new List<ObjectComponentBlueprint>();
    List<ObjectComponent> ObjectComponents = new List<ObjectComponent>();
    
    // Start is called before the first frame update
    void Start() {
        for (int i = 0; i < componentsBlueprint.Count; i++) {
            ObjectComponents.Add(componentsBlueprint[i].CreateObjectComponet(this));
        }
    }

    public ObjectSystem attackOtherObject;
    [ContextMenu("Do Attack")] // In Inspector
    public void DoAttack()
    {
        if (!invincibilityFrameActive) // Check if invincibility frames are active
        {
            EventController attemptAttackEvent = new EventController(EventController.Event.attemptAttack, (int)EventController.Object.attacker, this, (int)EventController.DamageType.normalDamage, 5);
            if (SendEvent(attemptAttackEvent))
            {
                var AttemptDealDamage = attemptAttackEvent.ChangeEventName(EventController.Event.attemptDealDamage);
                if (attackOtherObject.SendEvent(AttemptDealDamage))
                {
                    Debug.Log($"Send event AttemptAttack succeeded to {attackOtherObject}. {attemptAttackEvent}");
                    // Event succeeded, let's execute attack.
                    EventController executeAttack = AttemptDealDamage.ChangeEventName(EventController.Event.executeDealDamage);
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
    private List<Material[]> originalMaterials;

    private Material damagedMaterial;

    private IEnumerator ActivateInvincibilityFrames(ObjectSystem otherObject)
    {
        invincibilityFrameActive = true;

        // Store the original materials of the object and its children
        originalMaterials = new List<Material[]>();
        Renderer[] renderers = otherObject.gameObject.GetComponentsInChildren<Renderer>();

        if (damagedMaterial == null)
        {
            damagedMaterial = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            damagedMaterial.SetColor("_BaseColor", Color.red);
        }

        foreach (Renderer renderer in renderers)
        {
            // Store the original URP materials of each child renderer
            Material[] originalURPMaterials = renderer.sharedMaterials;
            originalMaterials.Add(originalURPMaterials);

            // Create a new array for the modified materials
            Material[] modifiedMaterials = new Material[originalURPMaterials.Length];

            // Assign the modified URP material to the array
            for (int i = 0; i < originalURPMaterials.Length; i++)
            {
                if (originalURPMaterials[i].shader.name.StartsWith("Universal Render Pipeline/"))
                {
                    modifiedMaterials[i] = damagedMaterial;
                }
                else
                {
                    modifiedMaterials[i] = originalURPMaterials[i];
                }
            }

            // Set the modified URP materials on the child renderer
            renderer.sharedMaterials = modifiedMaterials;
        }

        yield return new WaitForSeconds(invincibilityDuration);

        // Revert the URP materials of the child renderers back to the original materials
        for (int i = 0; i < renderers.Length; i++)
        {
            renderers[i].sharedMaterials = originalMaterials[i];
        }

        invincibilityFrameActive = false;
    }

    private void StartInvincibilityFrames()
    {
        StartCoroutine(ActivateInvincibilityFrames(attackOtherObject));
    }

    public bool SendEvent(EventController eventSent) {
        for (int i = 0; i < ObjectComponents.Count; i++) {
            if (!ObjectComponents[i].SendEvent(eventSent)) return false;
        }
        return true;
    }
}

[Serializable]
public class ObjectComponent {
    public ObjectSystem owner;
    public Dictionary<string, int> parameterDictionary = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
    public virtual void Initialize(List<ComponentParameter> parameters) {
        for (int i = 0; i < parameters.Count; i++) {
            parameterDictionary[parameters[i].parameterName] = parameters[i].parameterValue;
        }
    }
    
    public virtual bool SendEvent(EventController eventSent) {
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

    public ObjectComponent CreateObjectComponet(ObjectSystem newOwner) {
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