using UnityEngine;

public static class EnemySpawner
{
    // An array of enemy prefabs to spawn
    public static GameObject[] enemyPrefabs;

    // The minimum and maximum number of enemies to spawn
    public static int minEnemies = 1;
    public static int maxEnemies = 3;

    // The minimum and maximum distance between enemies
    public static float minDistance = 2f;
    public static float maxDistance = 5f;
    private static float sqrDistance;

    // Method to randomly spawn enemies on the floor blocks
    public static void SpawnEnemies(GameObject[] groundPrefab, int gridSize, GameObject[] groundPrefabs)
    {
        Debug.Log("Starting enemy spawner");

        // Loop through all the floor blocks
        for (int x = 0; x < gridSize; x++)
        {
            for (int z = 0; z < gridSize; z++)
            {
                // Get the position of the floor block
                Vector3 floorPos = new Vector3(x * 4, 0, z * 4);

                // Check if the floor block is not a wall or pillar
                if (groundPrefab[x + z * gridSize] != null)
                {
                    Debug.Log("Spawning enemies on floor block at " + floorPos);

                    // Calculate the number of enemies to spawn
                    int numEnemies = Random.Range(minEnemies, maxEnemies + 1);
                    Debug.Log("Spawning " + numEnemies + " enemies");

                    // Loop through and spawn each enemy
                    for (int i = 0; i < numEnemies; i++)
                    {
                        // Calculate a random position to spawn the enemy
                        Vector3 enemyPos = floorPos + new Vector3(Random.Range(-2f, 2f), 0f, Random.Range(-2f, 2f));
                        float distance = 0f;

                        int maxIterations = 100;
                        int iterationCount = 0;
                        do
                        {
                            // Calculate a random distance between enemies
                            distance = Random.Range(minDistance, maxDistance);
                            Debug.Log("Trying to spawn enemy " + i + " at " + enemyPos + " with distance " + distance);

                            // Calculate a random direction to move the enemy
                            Vector2 direction = Random.insideUnitCircle.normalized;

                            // Calculate the new position of the enemy
                            enemyPos += new Vector3(direction.x * distance, 0f, direction.y * distance);

                            // Calculate the squared distance to each existing enemy
                            float sqrDistance = float.MaxValue;
                            GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                            foreach (GameObject enemy in enemies)
                            {
                                sqrDistance = Mathf.Min(sqrDistance, (enemy.transform.position - enemyPos).sqrMagnitude);
                            }
                            Debug.Log("Checking distance from existing enemies. Shortest distance is " + sqrDistance);

                            // Increment the iteration count
                            iterationCount++;

                            // Check if the maximum number of iterations has been reached
                            if (iterationCount >= maxIterations)
                            {
                                Debug.LogWarning("Max iterations reached in enemy spawner");
                                break;
                            }

                            // Repeat until the enemy is far enough away from all other enemies
                        } while (sqrDistance < distance * distance);

                        // Spawn a random enemy prefab at the calculated position
                        GameObject enemyPrefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Length)];

                        GameObject.Instantiate(enemyPrefab, enemyPos, Quaternion.identity);
                        Debug.Log("Enemy spawned at position: " + enemyPos);

                        // Check if the new enemy is too close to any other enemy
                        GameObject[] existingEnemies = GameObject.FindGameObjectsWithTag("Enemy");
                        foreach (GameObject existingEnemy in existingEnemies)
                        {
                            if ((existingEnemy.transform.position - enemyPos).sqrMagnitude < sqrDistance)
                            {
                                Debug.LogWarning("New enemy too close to existing enemy");
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
}
