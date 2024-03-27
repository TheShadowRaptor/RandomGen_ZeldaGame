using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerObject : MonoBehaviour
{
    public bool TriggerHit(Collider collider, Vector3 objectPosition)
    {
        return collider.bounds.Contains(objectPosition);
    }
}
