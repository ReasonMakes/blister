using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionCube : MonoBehaviour
{
    public GlobalSelection globalSelection;
    [System.NonSerialized] public bool additiveSelection = true;

    public void CreateCube(Vector3[] cubeVertices)
    {
        //Define mesh
        //Vertex positions

        //Generates in this order:
        //Bottom of cube first
        //Top-left, top-right, bottom-left, bottom-right
        //Then top of cube, following same pattern

        Vector3[] vertices = {
            cubeVertices[0],
            cubeVertices[1],
            cubeVertices[2],
            cubeVertices[3],

            cubeVertices[4],
            cubeVertices[5],
            cubeVertices[6],
            cubeVertices[7],
        };

        //Tris
        int[] triangles = {
            0, 2, 1, //bottom
			1, 2, 3,
            4, 5, 6, //top
			6, 5, 7,
            7, 1, 3, //right
			7, 5, 1,
            4, 6, 0, //left
			0, 6, 2,
            1, 5, 4, //back
			0, 1, 4,
            6, 7, 3, //front
			2, 6, 3
        };

        //SET MESH
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.Optimize();
        mesh.RecalculateNormals();
        mesh.name = "Selection Cube Mesh";

        //UPDATE COLLIDER
        GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (additiveSelection)
        {
            globalSelection.selectedTable.addSelected(other.gameObject);
        }
        else
        {
            globalSelection.selectedTable.removeSelected(other.gameObject);
        }
    }
}
