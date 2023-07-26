using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using static Grid;

public class DungeonGenerator : MonoBehaviour
{
    static public GameObject[] rooms;
    public GameObject room;
    public GameObject cubePrefab;

    public int roomLimit = 5;

    // Start is called before the first frame update
    void Start()
    {

    }

    Cell SpawnStartRoom()
    {
        GameObject spawnedRoom = Instantiate(room, Instance.startCell.adjPos, Quaternion.identity);
        spawnedRoom.name = $"Room: {Instance.startCell.oriPos.x} X| {Instance.startCell.oriPos.z} Z";
        rooms[0] = spawnedRoom;
        Instance.startCell.occupied = true;
        return Instance.startCell;
    }

    int roomCount = 0;
    public void GenerateRooms()
    {
        Cell currentCell = null;
        rooms = new GameObject[Instance.gridSizeX + Instance.gridSizeZ * 3];
        currentCell = SpawnStartRoom();

        for (int i = 1; i < roomLimit; i++)
        {
            Cell cellPos = NextCell(currentCell);
            GameObject spawnedRoom = Instantiate(room, cellPos.adjPos, Quaternion.identity);
            currentCell = cellPos;
            spawnedRoom.name = $"Room: {currentCell.oriPos.x} X| {currentCell.oriPos.z} Z";
            roomCount++;

        }
        //GameObject spawnedRoom = Instantiate(room, NextCell().adjPos, Quaternion.identity);
        //spawnedRoom.name = $"Room: {cell.oriPos.x} X| {cell.oriPos.z} Z";
        //rooms[roomCount] = spawnedRoom;
        //cell.occupied = true;

        //roomCount++;
        
        Debug.Log($"rooms {rooms.Length}");
        roomCount = 0;
        ConnectRooms();
    }

    int randomDirNum;
    bool skipCell = false;
    private Cell NextCell(Cell currentCell)
    {
        // Used for finding what direction to generate next
        randomDirNum = Random.Range(0, 3);
        Cell preCell = currentCell;

        bool loop = true;
        int loopTime = 0;
        while (loop)
        {
            switch (randomDirNum)
            {
                case 0:
                    // Up cell
                    if (IsValidCell(preCell.oriPos.x, preCell.oriPos.z + 1))
                    {
                        Cell upCell = cells[preCell.oriPos.x, preCell.oriPos.z + 1];
                        preCell.oriPos.x = currentCell.oriPos.x;
                        preCell.oriPos.z = currentCell.oriPos.z + 1;

                        preCell.adjPos.x = currentCell.adjPos.x;
                        preCell.adjPos.z = currentCell.adjPos.z + Instance.cellSize;

                        if (upCell.occupied)
                        {
                            skipCell = true;
                        }
                    }
                    else skipCell = true;
                    break;

                case 1:
                    // down cell
                    if (IsValidCell(preCell.oriPos.x, preCell.oriPos.z - 1))
                    {
                        Cell downCell = cells[preCell.oriPos.x, preCell.oriPos.z - 1];
                        preCell.oriPos.x = currentCell.oriPos.x;
                        preCell.oriPos.z = currentCell.oriPos.z - 1;

                        preCell.adjPos.x = currentCell.adjPos.x;
                        preCell.adjPos.z = currentCell.adjPos.z - Instance.cellSize;
                        if (downCell.occupied)
                        {
                            skipCell = true;
                        }
                    }
                    else skipCell = true;
                    break;

                case 2:
                    // Left cell
                    if (IsValidCell(preCell.oriPos.x - 1, preCell.oriPos.z))
                    {
                        Cell leftCell = cells[preCell.oriPos.x - 1, preCell.oriPos.z];
                        preCell.oriPos.x = currentCell.oriPos.x - 1;
                        preCell.oriPos.z = currentCell.oriPos.z;

                        preCell.adjPos.x = currentCell.adjPos.x - Instance.cellSize;
                        preCell.adjPos.z = currentCell.adjPos.z;
                        if (leftCell.occupied)
                        {
                            skipCell = true;
                        }
                    }
                    else skipCell = true;
                    break;

                case 3:
                    // Right cell
                    if (IsValidCell(preCell.oriPos.x + 1, preCell.oriPos.z))
                    {
                        Cell rightCell = cells[preCell.oriPos.x + 1, preCell.oriPos.z];
                        preCell.oriPos.x = currentCell.oriPos.x + 1;
                        preCell.oriPos.z = currentCell.oriPos.z;

                        preCell.adjPos.x = currentCell.adjPos.x + Instance.cellSize;
                        preCell.adjPos.z = currentCell.adjPos.z;
                        if (rightCell.occupied)
                        {
                            skipCell = true;
                        }
                    }
                    else skipCell = true;
                    break;
            }

            if (!skipCell)
            {
                loopTime += 1;
                if (loopTime == 1000)
                {
                    loop = false;
                    loopTime = 0;
                }

                if (IsValidCell(preCell.oriPos.x, preCell.oriPos.z))
                {
                    loop = false;
                    loopTime = 0;
                }
                else preCell = currentCell;
                skipCell = false;
            }
        }
        Debug.Log($"precell = {preCell.oriPos}");
        Debug.Log($"currentCell = {currentCell.oriPos}");
        return preCell;
    }

    private void ConnectRooms()
    {
        foreach (Cell cell in cells)
        {
            if (cell.occupied)
            {
                Debug.Log($"Grid Size {cells.Length}");
                //Debug.Log($"cell {dbugCount}");
                // Up
                if (IsValidCell(cell.oriPos.x, cell.oriPos.z + 1)) // Up cell
                {
                    Cell upCell = cells[cell.oriPos.x, cell.oriPos.z + 1];
                    if (upCell.occupied)
                    {
                        rooms[roomCount].GetComponent<RoomBehaviour>().UpdateDoorState(0, false);
                        //Debug.Log("works");
                        //GameObject cube = Instantiate(cubePrefab, new Vector3Int(upCell.adjPos.x, upCell.adjPos.y, upCell.adjPos.z - 9), Quaternion.identity);
                        //cube.transform.localScale = new Vector3(2,2,2);
                        //cube.name = $"Cube_Up {cell.oriPos.x} X| {cell.oriPos.z} Z"; 
                    }
                }
                else
                {
                    rooms[roomCount].GetComponent<RoomBehaviour>().UpdateDoorState(0, true);
                    //Debug.LogError("NotValidUPCell");
                }

                // Down
                if (IsValidCell(cell.oriPos.x, cell.oriPos.z - 1)) // down cell
                {
                    Cell downCell = cells[cell.oriPos.x, cell.oriPos.z - 1];
                    if (downCell.occupied)
                    {
                        rooms[roomCount].GetComponent<RoomBehaviour>().UpdateDoorState(1, false);
                        //Debug.Log("works");
                        //GameObject cube = Instantiate(cubePrefab, new Vector3Int(downCell.adjPos.x, downCell.adjPos.y, downCell.adjPos.z + 1), Quaternion.identity);
                        //cube.transform.localScale = new Vector3(2, 2, 2);
                        //cube.name = $"Cube_Down {cell.oriPos.x} X| {cell.oriPos.z} Z";
                    }
                }
                else
                {
                    rooms[roomCount].GetComponent<RoomBehaviour>().UpdateDoorState(1, true);
                    //Debug.LogError("NotValidLeftCell");
                }

                // Left
                if (IsValidCell(cell.oriPos.x - 1, cell.oriPos.z)) // left cell
                {
                    Cell leftCell = cells[cell.oriPos.x - 1, cell.oriPos.z];
                    if (leftCell.occupied)
                    {
                        rooms[roomCount].GetComponent<RoomBehaviour>().UpdateDoorState(2, false);
                        //Debug.Log("works");
                        //GameObject cube = Instantiate(cubePrefab, new Vector3Int(leftCell.adjPos.x + 6, leftCell.adjPos.y, leftCell.adjPos.z - 5), Quaternion.identity);
                        //cube.transform.localScale = new Vector3(2, 2, 2);
                        //cube.name = $"Cube_Left {cell.oriPos.x} X| {cell.oriPos.z} Z";
                    }
                }
                else
                {
                    rooms[roomCount].GetComponent<RoomBehaviour>().UpdateDoorState(2, true);
                    //Debug.LogError("NotValidLeftCell");
                }

                // Right
                if (IsValidCell(cell.oriPos.x + 1, cell.oriPos.z)) // right cell
                {
                    Cell rightCell = cells[cell.oriPos.x + 1, cell.oriPos.z];
                    if (rightCell.occupied)
                    {
                        rooms[roomCount].GetComponent<RoomBehaviour>().UpdateDoorState(3, false);
                        //Debug.Log("works");
                        //GameObject cube = Instantiate(cubePrefab, new Vector3Int(rightCell.adjPos.x - 6, rightCell.adjPos.y, rightCell.adjPos.z - 5), Quaternion.identity);
                        //cube.transform.localScale = new Vector3(2, 2, 2);
                        //cube.name = $"Cube_Right {cell.oriPos.x} X| {cell.oriPos.z} Z";
                    }
                }
                else
                {
                    rooms[roomCount].GetComponent<RoomBehaviour>().UpdateDoorState(3, true);
                    //Debug.LogError("NotValidUPCell");
                }
                roomCount++;
        
                roomCount = 0;
            }
            else
            {
                rooms[roomCount].GetComponent<RoomBehaviour>().UpdateDoorState(0, true);
                rooms[roomCount].GetComponent<RoomBehaviour>().UpdateDoorState(1, true);
                rooms[roomCount].GetComponent<RoomBehaviour>().UpdateDoorState(2, true);
                rooms[roomCount].GetComponent<RoomBehaviour>().UpdateDoorState(3, true);
            }
        }
    }

    private bool IsValidCell(int x, int z)
    {
        return x >= 0 && x < Instance.gridSizeX && z >= 0 && z < Instance.gridSizeZ;
    }
}