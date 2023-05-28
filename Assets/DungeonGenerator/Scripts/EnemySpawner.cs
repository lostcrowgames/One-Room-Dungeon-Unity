using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI;
using System.Linq;

public class EnemySpawner : MonoBehaviour
{
    // An array of enemy prefabs to spawn, sorted by difficulty level
    public GameObject[] enemyPrefabs;

    // The minimum and maximum number of enemies to spawn, increased for higher levels
    public int baseMinEnemies = 1;
    public int baseMaxEnemies = 3;
    public int enemyIncreasePerLevel = 1;
    private int currentMinEnemies;
    private int currentMaxEnemies;

    // The minimum and maximum distance between enemies
    public float minDistance = 2f;
    public float maxDistance = 5f;
    private float sqrDistance;

    // A reference to the navmesh generator
    public NavMeshGenerator navMeshGenerator;

    // The current level of the game
    private int currentLevel = 1;

    // Spawn enemies when the navmesh is ready
    private void Start()
    {
        if (navMeshGenerator != null)
        {
            // Compute the square of the maximum distance between enemies
            sqrDistance = maxDistance * maxDistance;

            // Calculate the current number of enemies to spawn based on the base values and current level
            currentMinEnemies = baseMinEnemies + (currentLevel - 1) * enemyIncreasePerLevel;
            currentMaxEnemies = baseMaxEnemies + (currentLevel - 1) * enemyIncreasePerLevel;

            // Spawn the initial wave of enemies
            SpawnEnemies();
        }
        else
        {
            Debug.LogError("Navmesh generator not set on enemy spawner!");
        }
    }

    // Spawn a random array of enemies based on the current level
    public void SpawnEnemies()
    {
        // Get a random number of enemies to spawn based on the current level
        int numEnemies = Random.Range(baseMinEnemies * currentLevel, baseMaxEnemies * currentLevel + 1);

        // Get all game objects with the "Ground" tag
        GameObject[] groundObjects = GameObject.FindGameObjectsWithTag("Ground");

        // Shuffle the enemy prefabs array
        ShuffleArray(enemyPrefabs);

        // Shuffle the spawn points array
        ShuffleArray(groundObjects);

        // Spawn the enemies
        for (int i = 0; i < numEnemies && groundObjects.Length > 0; i++)
        {
            // Get a random spawn point
            Vector3 spawnPoint = groundObjects[0].transform.position;

            // Get a random enemy prefab
            GameObject enemyPrefab = enemyPrefabs[i % enemyPrefabs.Length];

            // Instantiate the enemy prefab at the spawn point
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint, Quaternion.identity);
            enemy.transform.SetParent(transform); // Set the parent of the spawned object to the parent of the script

            // Remove the used spawn point
            groundObjects = groundObjects.Skip(1).ToArray();

            // Randomize the distance to the next enemy
            float distance = Random.Range(minDistance, maxDistance);

            // Find the nearest navmesh position to the next spawn point
            UnityEngine.AI.NavMeshHit hit;
            if (UnityEngine.AI.NavMesh.SamplePosition(spawnPoint + distance * Vector3.forward, out hit, sqrDistance, UnityEngine.AI.NavMesh.AllAreas))
            {
                enemy.transform.position = hit.position;
            }
        }
    }

    // Shuffle an array using the Fisher-Yates algorithm
    private void ShuffleArray<T>(T[] array)
    {
        for (int i = array.Length - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            T temp = array[i];
            array[i] = array[j];
            array[j] = temp;
        }
    }

    // Increase the current level of the game
    public void IncreaseLevel()
    {
        currentLevel++;
    }
}