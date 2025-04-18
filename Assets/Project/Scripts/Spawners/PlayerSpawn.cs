using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    static public Vector3 playerSpawnPos;
    static public Vector3 camSpawnPos;
    // Start is called before the first frame update
    void Awake()
    {
        playerSpawnPos = transform.position;
    }

    private void OnDisable()
    {
        playerSpawnPos = transform.position;
    }

    static public void SpawnPlayer() 
    {
        Debug.Log(PlayerController.Instance.gameObject.transform.position = playerSpawnPos);
        PlayerController.Instance.gameObject.transform.position = playerSpawnPos;
    }

    static public void MoveSpawn(Transform newPos) 
    {
        playerSpawnPos = newPos.position;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawCube(this.transform.position, Vector3.one);
    }
}
