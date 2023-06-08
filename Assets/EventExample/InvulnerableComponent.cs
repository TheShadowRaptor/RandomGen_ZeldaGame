using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvulnerableComponent : ObjectComponent
{
    public override bool SendEvent(EventExample eventSent)
    {
        if (eventSent.eventName == "AttemptDealDamage") {
            eventSent.eventParameters["Damage"] = 0;
        }

        return true;
    }
}

public class ShieldComponent : ObjectComponent
{
    public override bool SendEvent(EventExample eventSent)
    {
        if (eventSent.eventName == "AttemptDealDamage")
        {
            int damageAmount = eventSent.eventParameters.GetInt("Damage", 0);
            eventSent.eventParameters["Damage"] = damageAmount/2;
        }

        return true;
    }
}


public class FireImmunity : ObjectComponent {
    public override bool SendEvent(EventExample eventSent) {
        if (eventSent.eventName == "AttemptDealDamage") {
            eventSent.eventParameters["FireDamage"] = 0;
        }
        return true;
    }
}

public class FireSword : ObjectComponent
{
    public int damageAmount = 5;
    public int fireDamageAmount = 5;
    public override void Initialize(List<ComponentParameter> parameters)  {
        base.Initialize(parameters);
        damageAmount = parameterDictionary.GetInt("Damage", 5);
        fireDamageAmount = parameterDictionary.GetInt("FireDamage", 5);
    }

    public override bool SendEvent(EventExample eventSent) {
        if (eventSent.eventName == "AttemptAttack")
        {
            int damage = eventSent.eventParameters.GetInt("Damage", 0);
            eventSent.eventParameters["Damage"] = damage + damageAmount; 
            int currentFireDamage = eventSent.eventParameters.GetInt("FireDamage", 0); 
            eventSent.eventParameters["FireDamage"] = currentFireDamage+fireDamageAmount;
        }

        return true;
    }
}

public class ThornyShield : ObjectComponent
{
    
}