using System.Collections;
using UnityEngine;

public class HexGridGenerator : MonoBehaviour
{
    int columns;
    int rows;
    public Material material;
    public string State;
    public float Progress;

    private static class States
    {
        public const string CREATING_VERTICES = "Creating vertices";
        public const string CREATING_TRIANGLES = "Creating triangles";
        public const string MAPPING_UVS = "Initializing UVs";
    }

    int uvRows = 3;  // Number of rows in the texture
    int uvCols = 3;  // Number of cols in the texture

    private Color[] colors;
    private Mesh mesh;

    private Vector2[] uvVerts = new Vector2[6];
    private float uvRadius;
    private Vector2 uvDefault;
    private GameObject container;

    public IEnumerator GenerateMesh(int dimensions, string containerName)
    {
        this.rows = dimensions;
        this.columns = dimensions;
        uvRadius = 1.0f / uvCols / 2.0f;
        uvDefault = new Vector2(uvRadius, uvRadius);
        container = new GameObject();
        container.name = containerName;
        container.transform.rotation = this.transform.rotation;
        container.transform.SetParent(this.transform);
        MeshRenderer meshRenderer = container.AddComponent<MeshRenderer>();
        meshRenderer.material = material;

        yield return CreateHexMesh();
    }

    // Creates a mesh of hex objects 
    IEnumerator CreateHexMesh()
    {
        this.State = States.CREATING_VERTICES;
        var mf = container.AddComponent<MeshFilter>();
        mesh = new Mesh();
        mf.mesh = mesh;

        float vertSpacing = 2.0f * Mathf.Cos(30.0f * Mathf.Deg2Rad) * Constants.OVERWORLD_HEXAGON_R;
        float horzSpacing = Constants.OVERWORLD_HEXAGON_R + Mathf.Sin(30.0f * Mathf.Deg2Rad) * Constants.OVERWORLD_HEXAGON_R;

        Vector3 currPos = new Vector3(-columns / 2.0f * horzSpacing, rows / 2.0f * vertSpacing, 0.0f);

        // Layout vertices for a single hex cell
        Vector3[] hexVerts = new Vector3[6];
        hexVerts[0] = Vector3.zero;
        var v = Vector3.right;

        for (int i = 0; i < 6; i++)
        {
            hexVerts[i] = v * Constants.OVERWORLD_HEXAGON_R;
            uvVerts[i] = v * uvRadius;
            v = Quaternion.AngleAxis(60.0f, -Vector3.forward) * v;
        }

        // Create the vertices
        var vertices = new Vector3[rows * columns * 6];
        var currVert = 0;
        for (int i = 0; i < columns; i++)
        {
            for (var j = 0; j < rows; j++)
            {
                for (var k = 0; k < hexVerts.Length; k++)
                {
                    vertices[currVert++] = hexVerts[k] + currPos;
                }
                currPos.y -= vertSpacing;
            }
            currPos.x += horzSpacing;
            currPos.y = rows / 2.0f * vertSpacing;
            if (i % 2 == 0)
                currPos.y -= vertSpacing / 2.0f;
            if (i % 2 == 0)
            {
                this.Progress = (float)i / columns;
                yield return null;
            }
        }

        mf.mesh.vertices = vertices;

        //Create the triangles
        this.State = States.CREATING_TRIANGLES;
        var curr = 0;
        int[] triangles = new int[rows * columns * 4 * 3];
        int numTriangles = columns * rows * 6;
        for (int i = 0; i < numTriangles; i += 6)
        {
            triangles[curr++] = i + 0;
            triangles[curr++] = i + 1;
            triangles[curr++] = i + 5;

            triangles[curr++] = i + 5;
            triangles[curr++] = i + 1;
            triangles[curr++] = i + 4;

            triangles[curr++] = i + 4;
            triangles[curr++] = i + 1;
            triangles[curr++] = i + 2;

            triangles[curr++] = i + 2;
            triangles[curr++] = i + 3;
            triangles[curr++] = i + 4;

            if (i % 500 == 0)
            {
                this.Progress = (float)i / numTriangles;
                yield return null;
            }
        }

        mf.mesh.triangles = triangles;

        // Setup a based set of UV coordinates
        this.State = States.MAPPING_UVS;
        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < vertices.Length; i += 6)
        {
            for (int j = 0; j < 6; j++)
            {
                uvs[i + j] = uvVerts[j] + uvDefault;
            }

            if (i % 1000 == 0)
            {
                this.Progress = (float)i / vertices.Length;
                yield return null;
            }
        }

        mesh.uv = uvs;
        mesh.colors = colors;
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
    }

    // Sets a particular cell to a particular cell on the atlas
    public void SetUV(int row, int col, int atlasRow, int atlasCol)
    {
        if (row < 0 || row >= rows) return;
        if (col < 0 || col >= columns) return;
        if (atlasRow < 0 || atlasRow >= uvRows) return;
        if (atlasCol < 0 || atlasCol >= uvCols) return;

        Vector2[] uvs = mesh.uv;

        int bas = col * rows * 6 + row * 6;
        var offset = new Vector2(atlasCol * uvRadius * 2.0f + uvRadius, atlasRow * uvRadius * 2.0f + uvRadius);
        for (var i = 0; i < 6; i++)
        {
            uvs[bas + i] = uvVerts[i] + offset;
        }
        mesh.uv = uvs;
    }
}