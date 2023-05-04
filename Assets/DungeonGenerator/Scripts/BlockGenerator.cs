using UnityEngine;

public class BlockGenerator : MonoBehaviour
{
    // Create a floor block
    public static GameObject CreateFloorBlock()
    {
        GameObject floorBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
        floorBlock.name = "FloorBlock";
        floorBlock.transform.localScale = new Vector3(2f, 1f, 2f);
        floorBlock.GetComponent<Renderer>().material.color = Color.grey;
        floorBlock.AddComponent<BoxCollider>().size = new Vector3(2f, 0.25f, 2f) * 1.25f;
        floorBlock.isStatic = true;
        return floorBlock;
    }

    // Create a wall block
    public static GameObject CreateWallBlock()
    {
        GameObject wallBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wallBlock.name = "WallBlock";
        wallBlock.transform.localScale = new Vector3(2f, 4f, 2f);
        wallBlock.GetComponent<Renderer>().material.color = Color.white;
        wallBlock.AddComponent<BoxCollider>().size = new Vector3(2f, 4f, 2f) * 1.25f;
        wallBlock.isStatic = true;
        return wallBlock;
    }

    // Create a pillar block
    public static GameObject CreatePillarBlock()
    {
        GameObject pillarBlock = GameObject.CreatePrimitive(PrimitiveType.Cube);
        pillarBlock.name = "PillarBlock";
        pillarBlock.transform.localScale = Vector3.one;
        pillarBlock.GetComponent<Renderer>().material.color = Color.grey;
        pillarBlock.AddComponent<BoxCollider>().size = Vector3.one * 1.25f;
        pillarBlock.isStatic = true;
        return pillarBlock;
    }
}
