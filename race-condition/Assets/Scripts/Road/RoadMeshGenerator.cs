using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class RoadMeshGenerator : MonoBehaviour
{
    [SerializeField]
    private SplineTrack spline;

    [SerializeField]
    private float roadWidth = 8f;

    [SerializeField]
    private float segmentLength = 0.5f;

    [SerializeField]
    private float uvScale = 1f;

    [SerializeField]
    private Vector2 uvTiling = new Vector2(1f, 10f);

    [SerializeField]
    private float shoulderWidth = 0.5f;

    [SerializeField]
    private bool generateShoulders = true;

    private MeshFilter meshFilter;
    private MeshRenderer meshRenderer;

    public void Generate()
    {
        if (spline == null)
        {
            return;
        }

        meshFilter = GetComponent<MeshFilter>();
        meshRenderer = GetComponent<MeshRenderer>();

        if (meshFilter == null)
        {
            meshFilter = gameObject.AddComponent<MeshFilter>();
        }
        if (meshRenderer == null)
        {
            meshRenderer = gameObject.AddComponent<MeshRenderer>();
        }

        Mesh mesh = new ();
        mesh.name = "RoadMesh";

        float totalLength = spline.GetTotalLength();
        int segmentCount = Mathf.Max(1, Mathf.RoundToInt(totalLength / segmentLength));

        int vertexCount = (segmentCount + 1) * 2;
        int shoulderVertexCount = generateShoulders ? (segmentCount + 1) * 2 * 2 : 0;
        int totalVertices = vertexCount + shoulderVertexCount;

        Vector3[] vertices = new Vector3[totalVertices];
        Vector2[] uvs = new Vector2[totalVertices];
        int[] triangles = new int[segmentCount * 6 + (generateShoulders ? segmentCount * 12 : 0)];

        Vector3 up = Vector3.up;
        float accumulatedDistance = 0f;

        for (int i = 0; i <= segmentCount; i++)
        {
            float t = (float)i / segmentCount;
            Vector3 position = spline.GetPoint(t);
            Vector3 tangent = spline.GetTangent(t);

            Vector3 right = Vector3.Cross(tangent, up).normalized;
            if (right.sqrMagnitude < Mathf.Epsilon) right = Vector3.right;

            int leftIndex = i * 2;
            int rightIndex = i * 2 + 1;

            vertices[leftIndex] = position - right * (roadWidth * 0.5f);
            vertices[rightIndex] = position + right * (roadWidth * 0.5f);

            float v = accumulatedDistance * uvScale;
            uvs[leftIndex] = new Vector2(0f, v * uvTiling.y);
            uvs[rightIndex] = new Vector2(uvTiling.x, v * uvTiling.y);

            if (i < segmentCount)
            {
                int triIndex = i * 6;
                triangles[triIndex] = leftIndex;
                triangles[triIndex + 1] = leftIndex + 2;
                triangles[triIndex + 2] = rightIndex;
                triangles[triIndex + 3] = rightIndex;
                triangles[triIndex + 4] = leftIndex + 2;
                triangles[triIndex + 5] = rightIndex + 2;
            }

            if (i < segmentCount)
            {
                accumulatedDistance += Vector3.Distance(spline.GetPoint(t), spline.GetPoint((float)(i + 1) / segmentCount));
            }
        }

        if (generateShoulders)
        {
            GenerateShoulders(segmentCount, vertices, uvs, triangles, ref accumulatedDistance);
        }

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;
    }

    private void GenerateShoulders(int segmentCount, Vector3[] vertices, Vector2[] uvs, int[] triangles, ref float distance)
    {
        int roadVertexCount = (segmentCount + 1) * 2;
        int shoulderStartIndex = roadVertexCount;

        for (int i = 0; i <= segmentCount; i++)
        {
            float t = (float)i / segmentCount;
            Vector3 position = spline.GetPoint(t);
            Vector3 tangent = spline.GetTangent(t);
            Vector3 right = Vector3.Cross(tangent, Vector3.up).normalized;

            int leftOuter = shoulderStartIndex + i * 4;
            int leftInner = shoulderStartIndex + i * 4 + 1;
            int rightInner = shoulderStartIndex + i * 4 + 2;
            int rightOuter = shoulderStartIndex + i * 4 + 3;

            float innerOffset = roadWidth * 0.5f + 0.1f;
            float outerOffset = roadWidth * 0.5f + shoulderWidth + 0.1f;

            vertices[leftOuter] = position - right * outerOffset;
            vertices[leftInner] = position - right * innerOffset;
            vertices[rightInner] = position + right * innerOffset;
            vertices[rightOuter] = position + right * outerOffset;

            float v = distance * uvScale * uvTiling.y;
            float shoulderTiling = 2f;
            uvs[leftOuter] = new Vector2(0f, v * shoulderTiling);
            uvs[leftInner] = new Vector2(0.2f, v * shoulderTiling);
            uvs[rightInner] = new Vector2(0.8f, v * shoulderTiling);
            uvs[rightOuter] = new Vector2(1f, v * shoulderTiling);

            if (i < segmentCount)
            {
                int triIndex = segmentCount * 6 + i * 12;

                triangles[triIndex] = leftInner;
                triangles[triIndex + 1] = leftInner + 4;
                triangles[triIndex + 2] = leftOuter;
                triangles[triIndex + 3] = leftOuter;
                triangles[triIndex + 4] = leftInner + 4;
                triangles[triIndex + 5] = leftOuter + 4;

                triangles[triIndex + 6] = rightInner;
                triangles[triIndex + 7] = rightOuter;
                triangles[triIndex + 8] = rightInner + 4;
                triangles[triIndex + 9] = rightInner + 4;
                triangles[triIndex + 10] = rightOuter;
                triangles[triIndex + 11] = rightOuter + 4;
            }

            if (i < segmentCount)
            {
                distance += Vector3.Distance(spline.GetPoint(t), spline.GetPoint((float)(i + 1) / segmentCount));
            }
        }
    }

    public void SetSpline(SplineTrack spline)
    {
        this.spline = spline;
    }

    public void SetRoadWidth(float width)
    {
        roadWidth = width;
    }

    public void SetUVTiling(Vector2 tiling)
    {
        uvTiling = tiling;
    }

    public void SetUVScale(float scale)
    {
        uvScale = scale;
    }

    public void SetGenerateShoulders(bool generate)
    {
        generateShoulders = generate;
    }
}