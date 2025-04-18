using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvulnerableComponent : ObjectComponent
{
    public override bool SendEvent(EventController eventSent)
    {
        if (eventSent.eventName == EventController.Event.attemptDealDamage) {
            //eventSent.eventParameters[EventController.Event.damage] = 0;
        }

        return true;
    }
}

public class ShieldComponent : ObjectComponent
{
    public override bool SendEvent(EventController eventSent)
    {
        if (eventSent.eventName == EventController.Event.attemptDealDamage)
        {
            //int damageAmount = eventSent.eventParameters.Get(EventController.Event.damage, 0);
            //eventSent.eventParameters[EventController.Event.damage] = damageAmount / 2;
        }

        return true;
    }
}


public class FireImmunity : ObjectComponent
{
    public override bool SendEvent(EventController eventSent)
    {
        if (eventSent.eventName == EventController.Event.attemptDealDamage)
        {
            //eventSent.eventParameters[EventController.Event.fireDamage] = 0;
        }
        return true;
    }
}

public class FireSword : ObjectComponent
{
    public int damageAmount = 5;
    public int fireDamageAmount = 5;

    public override void Initialize(List<ComponentParameter> parameters)
    {
        base.Initialize(parameters);
        //damageAmount = parameterDictionary.Get(EventController.Event.damage, 5);
        //fireDamageAmount = parameterDictionary.Get(EventController.Event.fireDamage, 5);
    }

    public override bool SendEvent(EventController eventSent)
    {
        if (eventSent.eventName == EventController.Event.attemptAttack)
        {
            //int damage = eventSent.eventParameters.Get(EventController.Event.damage, 0);
            //eventSent.eventParameters[EventController.Event.damage] = damage + damageAmount;
            //int currentFireDamage = eventSent.eventParameters.Get(EventController.Event.fireDamage, 0);
            //eventSent.eventParameters[EventController.Event.fireDamage] = currentFireDamage + fireDamageAmount;
        }

        return true;
    }
}

public class ThornyShield : ObjectComponent
{
    
}