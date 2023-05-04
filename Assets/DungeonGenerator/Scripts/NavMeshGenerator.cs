using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshGenerator : MonoBehaviour
{
    public NavMeshSurface[] navMeshSurfaces;
    public Transform[] objectsToRotate;

    public void BakeNavMesh()
    {
        foreach (Transform objectToRotate in objectsToRotate)
        {
            objectToRotate.localRotation = Quaternion.Euler(new Vector3(0, Random.Range(0, 360), 0));
        }

        foreach (NavMeshSurface navMeshSurface in navMeshSurfaces)
        {
            navMeshSurface.BuildNavMesh();
        }
    }
}
