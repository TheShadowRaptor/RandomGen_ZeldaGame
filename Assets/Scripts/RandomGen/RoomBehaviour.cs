using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBehaviour : MonoBehaviour
{
    static public List<GameObject> rooms = new List<GameObject>();
    public List<GameObject> doors; // Reference to the door game objects

    private void OnEnable()
    {
        rooms.Add(this.gameObject);
    }

    private void OnDisable()
    {
        rooms.Clear();
    }

    public void UpdateDoorState(int doorIndex, bool isClosed)
    {
        doors[doorIndex].SetActive(isClosed);
    }
}
