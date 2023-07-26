using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    private static Grid instance;
    static public Cell[,] cells;
    public Cell startCell;
    public DungeonGenerator dungeonGenerator;

    public class Cell
    {
        public Vector3Int adjPos;
        public Vector3Int oriPos;
        public bool occupied = false;
    }

    public int gridSizeX = 10;
    public int gridSizeZ = 10;
    public int cellSize = 10;

    public static Grid Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<Grid>();
                if (instance == null)
                {
                    GameObject singletonObject = new GameObject();
                    instance = singletonObject.AddComponent<Grid>();
                    singletonObject.name = typeof(Grid).ToString() + " (Singleton)";
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return instance;
        }
    }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        cells = new Cell[gridSizeX, gridSizeZ];
        CreateGrid();
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void CreateGrid()
    {
        for (int x = 0; x < gridSizeX; x++)
        {
            for (int z = 0; z < gridSizeZ; z++)
            {
                cells[x, z] = new Cell();
                cells[x, z].oriPos = new Vector3Int(x, 0, z);
                cells[x, z].adjPos = new Vector3Int(x * cellSize, 0, z * cellSize);
                //Debug.Log($"Cell {cells[x, y]} = {cells[x, y].pos}");

                //cell.transform.position = cellPosition;
                //cell.transform.localScale = new Vector3(cellSize, 0.1f, cellSize);
                //cell.GetComponent<Renderer>().material.color = Color.white;
                // Add any other customizations or components you need for each grid cell
            }
        }

        startCell = cells[gridSizeX / 2, gridSizeZ / 2];
        dungeonGenerator.GenerateRooms();
    }
}
