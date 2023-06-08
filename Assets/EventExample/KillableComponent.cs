using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class KillableComponent : ObjectComponent
{
    public override void Initialize(List<ComponentParameter> prms) {
        base.Initialize(prms);
        maxHealth = parameterDictionary.GetInt("MaxHealth", 30);
        health = maxHealth;
        Debug.Log($"Initialized killable component");
    }

    public override bool SendEvent(EventExample eventSent)
    {
        if (eventSent.eventName == "ExecuteDealDamage") {
            int damageAmount = eventSent.eventParameters.GetInt("Damage", 0);
            damageAmount += eventSent.eventParameters.GetInt("FireDamage", 0);
            damageAmount += eventSent.eventParameters.GetInt("WaterDamage", 0);
            damageAmount += eventSent.eventParameters.GetInt("ElectricalDamage", 0);
            
            Debug.Log($"KillableComponent taking damage {damageAmount}");
            if (damageAmount > 0) {
                health -= damageAmount;
                EventExample tookDamageEvent = new EventExample("TookDamage", eventSent);
                owner.SendEvent(tookDamageEvent);
                // owner.transform.DOShakePosition(0.5f, 2f);
                if (health <= 0)
                {
                    //DIED!
                    Debug.Log("DED!");
                    EventExample dedEvent = new EventExample("Died", eventSent);
                    owner.SendEvent(dedEvent);
                    owner.gameObject.SetActive(false);
                    //owner.transform.DOScale(0, 0.4f).SetDelay(0.3f);
                }
                Debug.Log($"KillableComponent took damage {damageAmount} new health is {health}");
            }
            else return false;
        }
        return base.SendEvent(eventSent);
    }

    public int health;
    public int maxHealth;
}
