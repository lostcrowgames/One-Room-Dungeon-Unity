# One Room Dungeon Generator Unity Script

This Unity script generates a one-room dungeon using prefabs for the floor, walls, corners, and pillars. The dungeon is randomly generated, making each playthrough unique. 

![image](https://user-images.githubusercontent.com/132179334/236115179-c69c37ca-3196-4cc4-8c00-331b8bef23a0.png)

## Requirements

- Unity 2021.3.24f1 or later

## Installation

1. Clone or download the repository.
2. Open the Unity project.
3. Open the `OneRoomDungeon` scene.
4. Attach the `DungeonGenerator` script to an empty game object in the scene.
5. Assign the required prefabs to the script variables in the inspector.

## Usage

1. Run the scene.
2. A one-room dungeon will be generated with random floor tiles, walls, and pillars.
3. Use the WASD keys or arrow keys to move around the dungeon.
4. The enemy spawner will randomly spawn enemies in the dungeon.
5. The navmesh baker generates a navmesh for the enemies to navigate.

## Script Variables

- `dungeonSize`: The number of pillars in the dungeon.
- `groundPrefab`: An array of floor tile prefabs.
- `gridSize`: The size of the dungeon grid.
- `pillarPrefabs`: An array of pillar prefabs.
- `wallPrefab`: An array of wall prefabs.
- `cornerPrefab`: An array of corner wall prefabs.

## Contributing

If you find a bug or have an idea for a new feature, please open an issue or submit a pull request. Contributions are always welcome!

## License

This project is licensed under the Apache License 2.0 - see the [LICENSE.md](LICENSE.md) file for details.

## Acknowledgments

These scripts were developed by Robin Cormie @ Lost Crow Games and are provided as-is without any warranty or guarantee of fitness for any particular purpose. Use at your own risk.

## Additional Credits
This project uses NavMeshComponents, developed by Unity Technologies. You can find the original repository at https://github.com/Unity-Technologies/NavMeshComponents.
