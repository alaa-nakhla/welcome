using UnityEngine;

public class ConvexObjectGenerator : MonoBehaviour
{
    public int numVertices =8;
    public int numFaces = 6;

    private void Start()
    {
        // Create a new mesh to hold the vertices and faces
        Mesh mesh = new Mesh();

        // Generate the vertices
        Vector3[] vertices = new Vector3[numVertices];
        for (int i = 0; i < numVertices; i++)
        {
            float x = Random.Range(-1f, 1f);
            float y = Random.Range(-1f, 1f);
            float z = Random.Range(-1f, 1f);
            vertices[i] = new Vector3(x, y, z).normalized;
        }

        // Generate the faces
        int[] triangles = new int[numFaces * 3];
        for (int i = 0; i < numFaces; i++)
        {
            int j = i * 3;
            triangles[j] = 0;
            triangles[j + 1] = i + 1;
            triangles[j + 2] = i + 2;
        }
        triangles[numFaces * 3 - 1] = 1;

        // Assign the vertices and faces to the mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        // Create a new game object to hold the mesh
        GameObject obj = new GameObject("Convex Object");
        obj.AddComponent<MeshFilter>().mesh = mesh;
        obj.AddComponent<MeshRenderer>();
        obj.transform.position = new Vector3(-10, 10, -10);
        obj.transform.localScale = new Vector3(5f, 5f, 5f);
    }
}