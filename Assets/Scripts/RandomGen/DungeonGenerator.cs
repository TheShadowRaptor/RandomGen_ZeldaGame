using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    //public GameObject room;
    //public Vector2Int size;
    //public Vector2 offset;

    //private HashSet<Vector2Int> visited;
    //private List<GameObject> rooms;

    //private List<GameObject> generatedRooms;

    //// Start is called before the first frame update
    //void Start()
    //{
    //    StartCoroutine(GenerateDungeon());
    //}

    //IEnumerator GenerateDungeon()
    //{
    //    visited = new HashSet<Vector2Int>();
    //    rooms = new List<GameObject>();

    //    for (int x = 0; x < size.x; x++)
    //    {
    //        for (int y = 0; y < size.y; y++)
    //        {
    //            Vector2Int coordinates = new Vector2Int(x, y);
    //            if (!visited.Contains(coordinates))
    //            {
    //                yield return StartCoroutine(GenerateRoom(coordinates));
    //            }
    //        }
    //    }

    //    ConnectRooms();

    //    yield return null;
    //}

    //IEnumerator GenerateRoom(Vector2Int coordinates)
    //{
    //    List<bool> doorStatus = new List<bool>();

    //    // Check if room has a neighbor in each direction
    //    bool up = coordinates.y > 0 && visited.Contains(coordinates + Vector2Int.down);
    //    bool down = coordinates.y < size.y - 1 && visited.Contains(coordinates + Vector2Int.up);
    //    bool right = coordinates.x < size.x - 1 && visited.Contains(coordinates + Vector2Int.right);
    //    bool left = coordinates.x > 0 && visited.Contains(coordinates + Vector2Int.left);

    //    doorStatus.Add(!up);      // Index 0 represents the up door
    //    doorStatus.Add(!down);    // Index 1 represents the down door
    //    doorStatus.Add(!right);   // Index 2 represents the right door
    //    doorStatus.Add(!left);    // Index 3 represents the left door

    //    var newRoom = Instantiate(room, new Vector3(coordinates.x * offset.x, 0, coordinates.y * offset.y), Quaternion.identity, transform);
    //    newRoom.name += " " + coordinates.x + "-" + coordinates.y;
    //    rooms.Add(newRoom);

    //    visited.Add(coordinates);

    //    // Check the door states of the neighboring rooms and update the doorStatus list accordingly
    //    if (up)
    //    {
    //        int aboveX = coordinates.x;
    //        int aboveY = coordinates.y - 1;
    //        int aboveRoomIndex = aboveX * size.y + aboveY;
    //        GameObject aboveRoom = rooms[aboveRoomIndex];
    //        RoomBehaviour aboveRoomBehaviour = aboveRoom.GetComponent<RoomBehaviour>();
    //        bool[] aboveDoors = aboveRoomBehaviour.GetDoorStates();
    //        doorStatus[0] = aboveDoors[1];  // Set the up door state based on the down door of the above room
    //        aboveRoomBehaviour.UpdateDoorState(1, true); // Open the down door of the above room
    //    }

    //    if (down)
    //    {
    //        int belowX = coordinates.x;
    //        int belowY = coordinates.y + 1;
    //        int belowRoomIndex = belowX * size.y + belowY;
    //        GameObject belowRoom = rooms[belowRoomIndex];
    //        RoomBehaviour belowRoomBehaviour = belowRoom.GetComponent<RoomBehaviour>();
    //        bool[] belowDoors = belowRoomBehaviour.GetDoorStates();
    //        doorStatus[1] = belowDoors[0];  // Set the down door state based on the up door of the below room
    //        belowRoomBehaviour.UpdateDoorState(0, true); // Open the up door of the below room
    //    }

    //    if (right)
    //    {
    //        int rightX = coordinates.x + 1;
    //        int rightY = coordinates.y;
    //        int rightRoomIndex = rightX * size.y + rightY;
    //        GameObject rightRoom = rooms[rightRoomIndex];
    //        RoomBehaviour rightRoomBehaviour = rightRoom.GetComponent<RoomBehaviour>();
    //        bool[] rightDoors = rightRoomBehaviour.GetDoorStates();
    //        doorStatus[2] = rightDoors[3]; // Set the right door state based on the left door of the right room
    //        rightRoomBehaviour.UpdateDoorState(3, true); // Open the left door of the right room
    //    }

    //    if (left)
    //    {
    //        int leftX = coordinates.x - 1;
    //        int leftY = coordinates.y;
    //        int leftRoomIndex = leftX * size.y + leftY;
    //        GameObject leftRoom = rooms[leftRoomIndex];
    //        RoomBehaviour leftRoomBehaviour = leftRoom.GetComponent<RoomBehaviour>();
    //        bool[] leftDoors = leftRoomBehaviour.GetDoorStates();
    //        doorStatus[3] = leftDoors[2]; // Set the left door state based on the right door of the left room
    //        leftRoomBehaviour.UpdateDoorState(2, true); // Open the right door of the left room
    //    }

    //    newRoom.GetComponent<RoomBehaviour>().UpdateRoom(doorStatus.ToArray()); // Update the room's doors

    //    yield return null;
    //}

    //void ConnectRooms()
    //{
    //    foreach (GameObject room in rooms)
    //    {
    //        RoomBehaviour roomBehaviour = room.GetComponent<RoomBehaviour>();
    //        bool[] doors = roomBehaviour.GetDoorStates();


    //        if (doors[0]) // If the up door is closed
    //        {
    //            if (y > 0) // Check if there is a room above
    //            {
    //                int aboveRoomIndex = room.gameObject.transform.position.x + y - 1);
    //                GameObject aboveRoom = rooms[aboveRoomIndex];
    //                RoomBehaviour aboveRoomBehaviour = aboveRoom.GetComponent<RoomBehaviour>();
    //                aboveRoomBehaviour.UpdateDoorState(0, false); // Open the down door (index 1) of the above room
    //                roomBehaviour.UpdateDoorState(1, false); // Open the up door (index 0) of the current room
    //            }
    //        }

    //        if (doors[1]) // If the down door is closed
    //        {
    //            if (y < size.y - 1) // Check if there is a room below
    //            {
    //                int belowRoomIndex = x * size.y + (y + 1);
    //                GameObject belowRoom = rooms[belowRoomIndex];
    //                RoomBehaviour belowRoomBehaviour = belowRoom.GetComponent<RoomBehaviour>();
    //                belowRoomBehaviour.UpdateDoorState(1, false); // Open the up door (index 0) of the below room
    //                roomBehaviour.UpdateDoorState(0, false); // Open the down door (index 1) of the current room
    //            }
    //        }

    //        if (doors[2]) // If the right door is closed
    //        {
    //            if (x < size.x - 1) // Check if there is a room to the right
    //            {
    //                int rightRoomIndex = (x + 1) * size.y + y;
    //                GameObject rightRoom = rooms[rightRoomIndex];
    //                RoomBehaviour rightRoomBehaviour = rightRoom.GetComponent<RoomBehaviour>();
    //                rightRoomBehaviour.UpdateDoorState(3, false); // Open the left door (index 3) of the right room
    //                roomBehaviour.UpdateDoorState(2, false); // Open the right door (index 2) of the current room
    //            }
    //        }

    //        if (doors[3]) // If the left door is closed
    //        {
    //            if (x > 0) // Check if there is a room to the left
    //            {
    //                int leftRoomIndex = (x - 1) * size.y + y;
    //                GameObject leftRoom = rooms[leftRoomIndex];
    //                RoomBehaviour leftRoomBehaviour = leftRoom.GetComponent<RoomBehaviour>();
    //                leftRoomBehaviour.UpdateDoorState(2, false); // Open the right door (index 2) of the left room
    //                roomBehaviour.UpdateDoorState(3, false); // Open the left door (index 3) of the current room
    //            }
    //        }
    //    }
    //}
}