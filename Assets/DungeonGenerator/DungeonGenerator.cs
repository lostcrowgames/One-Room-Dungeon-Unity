using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public int dungeonSize = 20;
    public int gridSize = 10;
    public GameObject[] groundPrefabs;
    public GameObject[] pillarPrefabs;
    public GameObject[] wallPrefabs;
    public GameObject[] cornerPrefabs;

    private GameObject[,] grid;

    [SerializeField] private GameObject[] enemyPrefabs;
    [SerializeField] private int numberOfEnemies = 4;

    [SerializeField] private NavMeshGenerator navMeshGenerator;

    void Start()
    {
        GenerateDungeon();
    }

    void GenerateDungeon()
    {
        GenerateFloor();
        GenerateWalls();
        GeneratePillars();

        // Bake NavMesh
        navMeshGenerator.BakeNavMesh();

        // Spawn enemies on the floor blocks
        EnemySpawner.SpawnEnemies(enemyPrefabs, numberOfEnemies, groundPrefabs);
    }

    void GenerateFloor()
    {
        grid = new GameObject[gridSize, gridSize];

        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                int index = Random.Range(0, groundPrefabs.Length);
                GameObject ground = Instantiate(groundPrefabs[index], new Vector3(x * 4, 0, z * 4), Quaternion.identity);
                ground.transform.parent = transform;
                grid[x, z] = ground;
            }
        }
    }

    void GenerateWalls()
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                if (x == 0 || x == gridSize - 1 || z == 0 || z == gridSize - 1)
                {
                    Vector3 wallPos = new Vector3(x * 4, 2, z * 4);
                    Quaternion wallRot = Quaternion.identity;

                    if ((x == 0 && z == 0) || (x == 0 && z == gridSize - 1) ||
                        (x == gridSize - 1 && z == 0) || (x == gridSize - 1 && z == gridSize - 1))
                    {
                        // Corner wall
                        int index = Random.Range(0, cornerPrefabs.Length);
                        float xOffset = 0;
                        float zOffset = 0;
                        Quaternion cornerRot = Quaternion.identity;

                        if (x == 0)
                        {
                            xOffset = -2;
                            if (z == 0)
                            {
                                cornerRot = Quaternion.Euler(0, 0, 0);
                            }
                            else
                            {
                                cornerRot = Quaternion.Euler(0, 0, 0);
                            }
                        }
                        else if (x == gridSize - 1)
                        {
                            xOffset = 2;
                            if (z == 0)
                            {
                                cornerRot = Quaternion.identity;
                            }
                            else
                            {
                                cornerRot = Quaternion.Euler(0, 0, 0);
                            }
                        }

                        if (z == 0)
                        {
                            zOffset = -2;
                        }
                        else if (z == gridSize - 1)
                        {
                            zOffset = 2;
                        }

                        GameObject cornerWall = Instantiate(cornerPrefabs[index], new Vector3((x * 4) + xOffset, 2, (z * 4) + zOffset), cornerRot);
                        cornerWall.transform.parent = transform;
                    }
                    else
                    {
                        // Regular wall
                        if (x == 0)
                        {
                            wallPos += new Vector3(-2, 0, 0);
                            wallRot = Quaternion.Euler(0, 90, 0);
                        }
                        else if (x == gridSize - 1)
                        {
                            wallPos += new Vector3(2, 0, 0);
                            wallRot = Quaternion.Euler(0, -90, 0);
                        }
                        else if (z == 0)
                        {
                            wallPos += new Vector3(0, 0, -2);
                            wallRot = Quaternion.Euler(0, 180, 0);
                        }
                        else if (z == gridSize - 1)
                        {
                            wallPos += new Vector3(0, 0, 2);
                            wallRot = Quaternion.identity;
                        }

                        GameObject wall = Instantiate(wallPrefabs[Random.Range(0, wallPrefabs.Length)], wallPos, wallRot);
                        wall.transform.parent = transform;
                    }
                }
            }
        }
    }

    void GeneratePillars()
    {
        for (int i = 0; i < gridSize; i++)
        {
            int x = Random.Range(1, gridSize - 1) * 4;
            int z = Random.Range(1, gridSize - 1) * 4;
            int index = Random.Range(0, pillarPrefabs.Length);
            GameObject pillar = Instantiate(pillarPrefabs[index], new Vector3(x, 1, z), Quaternion.identity);
            pillar.transform.parent = transform;
        }
    }

    }