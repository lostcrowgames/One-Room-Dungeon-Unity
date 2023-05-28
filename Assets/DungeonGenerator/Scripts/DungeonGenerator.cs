using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public int gridWidth = 5;
    public int gridLength = 5;
    public GameObject[] groundPrefabs;
    public int maxPillars = 4;
    public PillarGenerationType pillarGenerationType;
    public GameObject[] pillarPrefabs;
    public GameObject[] wallPrefabs;
    public GameObject[] cornerPrefabs;

    [SerializeField] private bool generateDoors;
    public GameObject[] doorPrefabs;

    [SerializeField] private DungeonDirection[] doorDirections;


    private GameObject[,] grid;
    private GameObject[,] doorGrid;

    [SerializeField] private NavMeshGenerator navMeshGenerator;
    [SerializeField] private EnemySpawner enemySpawner;

    [SerializeField] private bool generateCheckered;
    [SerializeField] private bool generateSymmetrical;

    public enum DungeonDirection
    {
        North,
        South,
        East,
        West
    }

    public enum PillarGenerationType
    {
        Symmetrical,
        Checkered
    }

    void Start()
    {
        doorGrid = new GameObject[gridWidth, gridLength];  // Initialize the door grid

        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        // Generate the floor first. This is necessary because all other elements (walls, doors, pillars) are placed relative to the floor tiles.
        GenerateFloor();

        // Generate doors before walls. This is because the walls need to know where the doors are, to avoid placing a wall where a door should be. 
        // We store the location of the doors in the doorGrid when they are generated, which the GenerateWalls method checks to see if a wall should be placed.
        GenerateDoors();

        // Walls are generated next, after the doors. Now that we know where the doors are, we can safely generate the walls around the outer edges of the floor, 
        // while avoiding the locations where we placed doors.
        GenerateWalls();

        // Pillars are generated after walls and doors because they are placed at random points within the dungeon. They are not dependent on the location of walls or doors, 
        // so they can be generated last. They also don't block pathfinding, so they don't affect the NavMesh.
        GeneratePillars();

        // After all the static elements (floor, walls, doors, pillars) of the dungeon have been generated, we can bake the NavMesh. 
        // The NavMesh is a mesh that Unity's pathfinding system uses to navigate around obstacles in the environment. 
        // It needs to be baked after all static elements that could potentially block movement have been placed, hence it's done after floor, walls, doors and pillars generation.
        if (navMeshGenerator != null)
        {
            navMeshGenerator.BakeNavMesh();
        }

        // Finally, we spawn enemies. This is done after the NavMesh has been baked because the enemies will likely use the NavMesh to navigate around the dungeon.
        // Therefore, we need to make sure the NavMesh accurately represents the final state of the dungeon before we start spawning enemies.
        if (enemySpawner != null)
        {
            enemySpawner.SpawnEnemies();
        }
    }


    void GenerateFloor()
    {
        grid = new GameObject[gridWidth, gridLength];

        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridLength; z++)
            {
                int index = Random.Range(0, groundPrefabs.Length);
                GameObject ground = Instantiate(groundPrefabs[index], new Vector3(x * 4, 0, z * 4), Quaternion.identity);
                ground.transform.parent = transform;
                grid[x, z] = ground;
            }
        }
    }

    private void GenerateDoors()
    {
        // Check if door generation is enabled
        if (generateDoors)
        {
            // Loop over each specified door direction
            foreach (DungeonDirection direction in doorDirections)
            {
                int x = 0;
                int z = 0;

                // Position and rotation depend on the chosen direction
                Quaternion doorRot = Quaternion.identity;
                Vector3 doorOffset = Vector3.zero;  // We are using Vector3 for offset to handle offset in x and z axis

                switch (direction)
                {
                    case DungeonDirection.North:
                        x = gridWidth / 2;
                        z = gridLength - 1;
                        doorRot = Quaternion.Euler(0, 0, 0);
                        doorOffset = new Vector3(0, 0, 2);  // Set offset in z axis
                        break;
                    case DungeonDirection.South:
                        x = gridWidth / 2;
                        z = 0;
                        doorRot = Quaternion.Euler(0, 180, 0);
                        doorOffset = new Vector3(0, 0, -2);  // Set offset in z axis
                        break;
                    case DungeonDirection.West:
                        x = 0;
                        z = gridLength / 2;
                        doorRot = Quaternion.Euler(0, -90, 0);
                        doorOffset = new Vector3(-2, 0, 0);  // Set offset in x axis
                        break;
                    case DungeonDirection.East:
                        x = gridWidth - 1;
                        z = gridLength / 2;
                        doorRot = Quaternion.Euler(0, -270, 0);
                        doorOffset = new Vector3(2, 0, 0);  // Set offset in x axis
                        break;
                }

                // Create door with a position depending on the selected direction and apply offset
                Vector3 doorPos = new Vector3(x * 4, 2, z * 4) + doorOffset;
                GameObject door = Instantiate(doorPrefabs[Random.Range(0, doorPrefabs.Length)], doorPos, doorRot);

                // Assign the generated door to the parent dungeon
                door.transform.parent = transform;

                // Save the door into our doorGrid for future reference, so we can check if a door already exists at this position
                doorGrid[x, z] = door;
            }
        }
    }

    void GenerateWalls()
    {
        // Loop over the entire grid to determine where walls should be placed
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridLength; z++)
            {
                // Walls should be on the outer edges of the grid
                if (x == 0 || x == gridWidth - 1 || z == 0 || z == gridLength - 1)
                {
                    // Check if there is a door in this spot on the grid
                    if (doorGrid[x, z] != null)
                    {
                        // There is a door here, don't place a wall and continue to the next grid spot
                        continue;
                    }

                    Vector3 wallPos = new Vector3(x * 4, 2, z * 4);
                    Quaternion wallRot = Quaternion.identity;

                    if ((x == 0 && z == 0) || (x == 0 && z == gridLength - 1) ||
                        (x == gridWidth - 1 && z == 0) || (x == gridWidth - 1 && z == gridLength - 1))
                    {
                        // This is a corner wall
                        int index = Random.Range(0, cornerPrefabs.Length);
                        float xOffset = 0;
                        float zOffset = 0;
                        Quaternion cornerRot = Quaternion.identity;

                        if (x == 0)
                        {
                            xOffset = -2;
                            if (z == 0)
                            {
                                cornerRot = Quaternion.Euler(0, 0, 0); // Back right corner
                            }
                            else
                            {
                                cornerRot = Quaternion.Euler(0, 90, 0); // Front right corner
                            }
                        }
                        else if (x == gridWidth - 1)
                        {
                            xOffset = 2;
                            if (z == 0)
                            {
                                cornerRot = Quaternion.Euler(0, -90, 0); // Back left corner
                            }
                            else
                            {
                                cornerRot = Quaternion.Euler(0, 180, 0); // Front left corner
                            }
                        }

                        if (z == 0)
                        {
                            zOffset = -2;
                        }
                        else if (z == gridLength - 1)
                        {
                            zOffset = 2;
                        }

                        GameObject cornerWall = Instantiate(cornerPrefabs[index], new Vector3((x * 4) + xOffset, 2, (z * 4) + zOffset), cornerRot);
                        cornerWall.transform.parent = transform;
                    }
                    else
                    {
                        // This is a regular wall
                        if (x == 0)
                        {
                            wallPos += new Vector3(-2, 0, 0);
                            wallRot = Quaternion.Euler(0, 90, 0);
                        }
                        else if (x == gridWidth - 1)
                        {
                            wallPos += new Vector3(2, 0, 0);
                            wallRot = Quaternion.Euler(0, -90, 0);
                        }
                        else if (z == 0)
                        {
                            wallPos += new Vector3(0, 0, -2);
                            wallRot = Quaternion.Euler(0, 180, 0);
                        }
                        else if (z == gridLength - 1)
                        {
                            wallPos += new Vector3(0, 0, 2);
                            wallRot = Quaternion.identity;
                        }

                        // Instantiate the wall and parent it to the dungeon
                        GameObject wall = Instantiate(wallPrefabs[Random.Range(0, wallPrefabs.Length)], wallPos, wallRot);
                        wall.transform.parent = transform;
                    }
                }
            }
        }
    }

    void GeneratePillars()
    {
        switch (pillarGenerationType)
        {
            case PillarGenerationType.Symmetrical:
                GenerateSymmetricalPillars();
                break;

            case PillarGenerationType.Checkered:
                GenerateCheckeredPillars();
                break;

            default:
                Debug.Log("Invalid pillar generation type!");
                break;
        }
    }

    void GenerateSymmetricalPillars()
    {
        int pillarCount = 0; // Keep track of how many pillars have been generated

        // Loop over the grid rows
        for (int z = 0; z < gridLength; z++)
        {
            // Only place pillars on even-numbered rows for symmetry
            if (z % 2 == 0)
            {
                for (int x = 0; x < gridWidth; x++)
                {
                    // Only create a pillar if we haven't hit the maximum
                    if (pillarCount < maxPillars)
                    {
                        int index = Random.Range(0, pillarPrefabs.Length);
                        GameObject pillar = Instantiate(pillarPrefabs[index], new Vector3(x * 4, 1, z * 4), Quaternion.identity);
                        pillar.transform.parent = transform;

                        pillarCount++; // Increase the count of generated pillars
                    }
                }
            }
        }
    }

    void GenerateCheckeredPillars()
    {
        int pillarCount = 0; // Keep track of how many pillars have been generated

        // Loop over the entire grid
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridLength; z++)
            {
                // Check if we should place a pillar here for a checkered pattern
                if ((x + z) % 2 == 0)
                {
                    // Only create a pillar if we haven't hit the maximum
                    if (pillarCount < maxPillars)
                    {
                        int index = Random.Range(0, pillarPrefabs.Length);
                        GameObject pillar = Instantiate(pillarPrefabs[index], new Vector3(x * 4, 1, z * 4), Quaternion.identity);
                        pillar.transform.parent = transform;

                        pillarCount++; // Increase the count of generated pillars
                    }
                }
            }
        }
    }
}