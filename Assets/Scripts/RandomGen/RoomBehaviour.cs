using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomBehaviour : MonoBehaviour
{
    public List<GameObject> doors; // Reference to the door game objects
    private bool[] doorStates; // Array to store the state of each door

    // Update the room and its doors based on the provided door status
    public void UpdateRoom(bool[] doorStatus)
    {
        doorStates = doorStatus;

        // Update the state of each door
        for (int i = 0; i < doorStates.Length; i++)
        {
            doors[i].SetActive(doorStates[i]);
        }
    }

    public void UpdateDoorState(int doorIndex, bool isClosed)
    {
        if (doorIndex >= 0 && doorIndex < doors.Count)
        {
            doors[doorIndex].SetActive(isClosed);
        }
        else
        {
            Debug.LogError("Invalid door index: " + doorIndex);
        }
    }

    // Other methods and functionality of your RoomBehaviour script
    // ...
    public bool[] GetDoorStates()
    {
        return doorStates;
    }
}
