using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class CollectableManager : MonoBehaviour
{
    [Header("CollectableSettings")]
    [SerializeField] List<CollectableData> collectableDataObjects;
    [SerializeField] private float moveSpeed = 2f;

    private List<GameObject> spawnedCollectables = new List<GameObject>();

    private int poolNumber = 100;
    private Vector3 playerPos;

    private void Update()
    {
        playerPos = PlayerController.Instance.transform.position;

        //Spin();
        MoveObjectsTowardsPlayer();

        if (Input.GetKeyDown(KeyCode.I))
        {
            SpawnCollectable();
        }
    }

    private void FixedUpdate()
    {
        MoveObjectsTowardsPlayer();
    }

    public void SpawnCollectable(CollectableData collectableData, Vector3 pos) 
    {
        GameObject obj = Instantiate(collectableData.prefab);
        obj.transform.position = pos;
        obj.name = collectableData.collectableName;
        spawnedCollectables.Add(obj);
    }

    private void MoveObjectsTowardsPlayer()
    {
        foreach (GameObject obj in spawnedCollectables)
        {
            if (obj.activeSelf)
            {
                obj.transform.position = Vector3.MoveTowards(obj.transform.position, playerPos, moveSpeed * Time.deltaTime);
            }
        }
    }

    private void Spin()
    {
        foreach (GameObject obj in spawnedCollectables)
        {
            obj.transform.DORotate(new Vector3(0, 360, 0), 1f, RotateMode.Fast);
        }
    }
}



