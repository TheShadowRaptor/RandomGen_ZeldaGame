using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TGTools 
{
    public class ObjectPool
    {
        public GameObject[] objects;
        public ObjectPool(Transform parent, string objectName, int maxNumber)
        {
            objects = new GameObject[maxNumber];

            foreach (GameObject obj in objects)
            {
                obj.name = objectName;
                obj.transform.parent = parent;
                obj.SetActive(false);
            }
        }

        public void SpawnObject(Vector3 position)
        {
            foreach (GameObject obj in objects)
            {
                if (obj.activeSelf == false)
                {
                    obj.SetActive(true);
                    obj.transform.position = position;
                    break;
                }
            }
        }
    }
}
