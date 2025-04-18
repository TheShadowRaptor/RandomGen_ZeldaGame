using System.Collections.Generic;
using UnityEngine;

public class KillableComponent : ObjectComponent
{
    public int health;
    public int maxHealth;

    // DamageType to multiplier (e.g. fireDamage => 1.5f)
    private Dictionary<EventController.DamageType, float> weaknesses = new Dictionary<EventController.DamageType, float>();

    public override void Initialize(List<ComponentParameter> prms)
    {
        base.Initialize(prms);

        maxHealth = parameterDictionary.GetInt("MaxHealth", 10);
        health = maxHealth;

        // Optional weaknesses (parameter name format: "WeakTo_fireDamage", value: 150 means 1.5x)
        foreach (var kvp in parameterDictionary)
        {
            if (kvp.Key.StartsWith("WeakTo_"))
            {
                string damageTypeStr = kvp.Key.Replace("WeakTo_", "");
                if (System.Enum.TryParse(damageTypeStr, out EventController.DamageType damageType))
                {
                    weaknesses[damageType] = kvp.Value / 100f; // Example: 150 ? 1.5x
                }
            }
        }

        Debug.Log($"[KillableComponent] Initialized: {health}/{maxHealth}");
    }

    public override bool SendEvent(EventController eventSent)
    {
        if (eventSent.eventName == EventController.Event.executeDealDamage)
        {
            if (!eventSent.eventParameters.TryGetValue((int)EventController.DamageType.normalDamage, out object baseDamageObj)) return false;

            int baseDamage = (int)baseDamageObj;
            EventController.DamageType damageType = eventSent.damageTypeName;

            float multiplier = weaknesses.TryGetValue(damageType, out float val) ? val : 1f;
            int finalDamage = Mathf.RoundToInt(baseDamage * multiplier);

            Debug.Log($"{owner.name} takes {finalDamage} {damageType} damage (base: {baseDamage}, multiplier: {multiplier})");

            health -= finalDamage;

            // Broadcast TookDamage
            EventController tookDamageEvent = new EventController(EventController.Event.attemptAttack, eventSent);
            tookDamageEvent.eventParameters[(int)EventController.DamageType.normalDamage] = finalDamage;
            owner.SendEvent(tookDamageEvent);

            if (health <= 0)
            {
                Debug.Log($"{owner.name} has died.");
                owner.SendEvent(new EventController(EventController.Event.executeDealDamage, eventSent));
                owner.gameObject.SetActive(false);
            }

            return true;
        }

        return base.SendEvent(eventSent);
    }
}
