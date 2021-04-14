using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TGen : MonoBehaviour
{

    Mesh mesh;

    public bool lineRender = true;

    public bool enableLive = false;
    public bool enableRandom = true;

    public int octaves = 1;
    public int depth = 32;

    public int width = 256;
    public int length = 256;

    public float horizontalScale = 2.23f;
    public float verticalScale = 1f;

    public float offsetX = 100f;
    public float offsetY = 100f;


    // Start is called before the first frame update
    void Start()
    {

        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        if (enableRandom)
        {
            offsetX = Random.Range(100f, 999f);
            offsetY = Random.Range(100f, 999f);
        }
        GenerateTerrain();
        GenerateRoads();
    }

    // Update is called once per frame
    void Update()
    {
        if (enableLive)
        {
            GenerateTerrain();
            //GenerateRoads();
        }
    }


    void GenerateRoads()
    {
        RoadGen rGen = new RoadGen(width, length, GetComponent<Terrain>().terrainData);
        RoadGraph rGraph = rGen.GeneratePrimaryL();
        RoadGraph tGraph = rGen.TerrainAdapt(rGraph);
        //RoadGraph rGraph = rGen.GenerateRoadGraph();
        if (lineRender)
        {
            RoadLineRender(rGraph, new Color(0, 0, 0));
            RoadLineRender(tGraph, new Color(255, 255, 255));
            GenerateMesh(tGraph);
        } else
        {
            GenerateMesh(rGraph);
        }
    }


    void GenerateMesh(RoadGraph rGraph)
    {
        float start = 0f;
        float step = 1f;
        var meshed = new HashSet<(Vector3, Vector3)>();
        var vertices = new List<Vector3>();
        var tris = new List<int>();
        int count = 0;
        var tData = GetComponent<Terrain>().terrainData;
        float length = tData.heightmapResolution;
        foreach (var a in rGraph.graph.Keys)
        {
            foreach (var b in rGraph.graph[a])
            {
                if (!meshed.Contains((b, a)))
                {
                    meshed.Add((a, b));
                    (var prevPoint, var prevNorm) = rGraph.GetPointAlong((a, b), start);
                    var p = prevPoint + prevNorm;
                    p.y = tData.GetInterpolatedHeight(p.x / length, p.z / length) + .5f;
                    vertices.Add(p);
                    p = prevPoint - prevNorm;
                    p.y = tData.GetInterpolatedHeight(p.x / length, p.z / length) + .5f;
                    vertices.Add(p);
                    for (var dist = start + step; dist < (b-a).magnitude - start; dist += step)
                    {
                        (var point, var norm) = rGraph.GetPointAlong((a, b), dist);
                        p = point + norm;
                        p.y = tData.GetInterpolatedHeight(p.x / length, p.z / length) + .5f;
                        vertices.Add(p);
                        p = point - norm;
                        p.y = tData.GetInterpolatedHeight(p.x / length, p.z / length) + .5f;
                        vertices.Add(p);

                        tris.Add(count);
                        tris.Add(count + 2);
                        tris.Add(count + 1);

                        tris.Add(count + 1);
                        tris.Add(count + 2);
                        tris.Add(count + 3);
                        count += 2;
                    }
                    (prevPoint, prevNorm) = rGraph.GetPointAlong((a, b), (b - a).magnitude);
                    p = prevPoint + prevNorm;
                    p.y = tData.GetInterpolatedHeight(p.x / length, p.z / length) + .5f;
                    vertices.Add(p);
                    p = prevPoint - prevNorm;
                    p.y = tData.GetInterpolatedHeight(p.x / length, p.z / length) + .5f;
                    vertices.Add(p);
                    tris.Add(count);
                    tris.Add(count + 2);
                    tris.Add(count + 1);

                    tris.Add(count + 1);
                    tris.Add(count + 2);
                    tris.Add(count + 3);
                    count += 4;

                }
            }
        }
        mesh.vertices = vertices.ToArray();
        mesh.triangles = tris.ToArray();
    }

    void RoadLineRender(RoadGraph rGraph, Color color)
    {
        var rendered = new HashSet<(Vector3, Vector3)>();
        foreach (var a in rGraph.graph.Keys)
        {
            foreach (var b in rGraph.graph[a])
            {
                if (!rendered.Contains((b, a)))
                {
                    var go = new GameObject();
                    LineRenderer line = go.AddComponent<LineRenderer>();
                    line.startWidth = .1f;
                    line.endWidth = .1f;
                    line.startColor = color;
                    line.endColor = color;
                    line.SetPositions(new Vector3[] {a, b});
                    rendered.Add((a, b));
                }
            }
        }
    }

    void GenerateTerrain()
    {
        Terrain terrain = GetComponent<Terrain>();
        TerrainData tData = terrain.terrainData;

        tData.heightmapResolution = width + 1;
        tData.size = new Vector3(width, depth, length);


        float[,] heights = new float[width, length];
        for(int x = 0; x < width; x++)
        {
            for(int y = 0; y < length; y++)
            {

                float height = 0f;
                for (int i = 0; i < octaves; i++)
                {
                    float xCoord = (float) x /  width * horizontalScale * Mathf.Pow(2, i);
                    float yCoord = (float) y /  length * horizontalScale * Mathf.Pow(2, i);
                    height += Mathf.PerlinNoise(xCoord, yCoord) * (verticalScale / Mathf.Pow(2, i));
                }
                heights[x, y] = height;
            }
        }

        tData.SetHeights(0, 0, heights);
    }
}
