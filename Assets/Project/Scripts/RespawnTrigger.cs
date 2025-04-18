using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnTrigger : TriggerObject
{
    Transform respawnPoint;
    private void Start()
    {
        respawnPoint = transform.GetChild(0).transform;
    }

    private void Update()
    {
        if (this.TriggerHit(GetComponent<Collider>(), PlayerController.Instance.transform.position))
        {
            PlayerSpawn.MoveSpawn(respawnPoint);
        }
    }
}
