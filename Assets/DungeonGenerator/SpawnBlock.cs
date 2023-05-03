using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnBlock : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        BlockGenerator.CreateFloorBlock();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


//create methods for the following additional functionality:

//    public GameObject chestPrefab; // The prefab for the chest that spawns in item rooms
//public GameObject miniBossPrefab; // The prefab for the mini boss that spawns in mini boss rooms
//public GameObject finalBossPrefab; // The prefab for the final boss that spawns in the final boss room
//public List<int> itemRoomIndexes; // The scene indexes for the item rooms
//public List<int> miniBossRoomIndexes; // The scene indexes for the mini boss rooms
//public List<int> finalBossRoomIndexes; // The scene indexes for the final boss rooms