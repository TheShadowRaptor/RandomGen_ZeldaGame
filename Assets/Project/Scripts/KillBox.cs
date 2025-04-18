using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillBox : TriggerObject
{
    [SerializeField] private int damage = 3;
    void Update()
    {
        if (this.TriggerHit(GetComponent<Collider>(), PlayerController.Instance.transform.position))
        {
            PlayerController.Instance.TakeDamage(damage);
            PlayerSpawn.SpawnPlayer();
        }
    }
}
