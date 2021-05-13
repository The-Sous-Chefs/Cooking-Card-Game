using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TGen : MonoBehaviour
{

    [Space(10)]
    [Header("Terrain Generation")]
    public bool enableLive = false;
    public bool enableRandom = true;
    [Range(1, 10)]
    public int octaves = 1;
    public int resLength = 256;
    public int depth = 32;
    public int length = 1024;
    public float horizontalScale = 2.23f;
    public float verticalScale = 1f;
    public float offsetX = 100f;
    public float offsetY = 100f;

    [Space(10)]
    [Header("Road Generation")]
    public Mesh mesh;
    public GameObject roadMeshObject;
    public bool lineRender = true;
    public float padding = 20f;
    public float roadOffset = 1f;
    public float roadHeight = 0.5f;
    public float roadStep = 1.0f;
    public float terrainRoadStep = 25f;
    public float roadWidth = 10f;
    public float minRoadLength = 50f;
    public float maxRoadLength = 250f;
    public float roadDelta = 25f;
    public int recDepth = 7;
    public Material roadMaterial;

    [Space(10)]
    [Header("Building Generation")]
    public GameObject buildings;
    public float buildingOffset = 10f;
    public float buildingStep = 5f;
    public float buildingClearance = 5f;
    [Range(0, 31)]
    public int buildingsLayer;
    public string buildingTag = "Building";
    public string roadTag = "Road";
    public int spawnAttempts = 1000;
    public GameObject[] spawnables;
    public GameObject[] buildingSpawnables;

    [Space(10)]
    [Header("Extra")]
    public GameObject player;

    List<Vector2> points;

    // Start is called before the first frame update
    void Start()
    {
        if (Globals.buildings != null)
        {
            return;
        }

        mesh = new Mesh();
        mesh.MarkDynamic();
        mesh.Clear();
        roadMeshObject.GetComponent<MeshFilter>().mesh = mesh;
        roadMeshObject.GetComponent<MeshCollider>().sharedMesh = mesh;
        roadMeshObject.GetComponent<MeshRenderer>().material = roadMaterial;
        points = new List<Vector2>();
        if (enableRandom)
        {
            offsetX = Random.Range(100f, 999f);
            offsetY = Random.Range(100f, 999f);
        }
        GenerateTerrain();
        GenerateRoads();

        Globals.terrain = GetComponent<Terrain>();
        Globals.player = player;
        Globals.buildings = buildings;
        DontDestroyOnLoad(Globals.terrain);
        DontDestroyOnLoad(Globals.player);
        DontDestroyOnLoad(Globals.buildings);
    }

    // Update is called once per frame
    void Update()
    {
        if (enableLive)
        {
            GenerateTerrain();
        }
    }


    void GenerateRoads()
    {
        RoadGen rGen = new RoadGen(length, GetComponent<Terrain>().terrainData, padding, minRoadLength, maxRoadLength, roadDelta, recDepth, points);
        RoadGraph rGraph = rGen.GeneratePrimaryL();
        RoadGraph tGraph = rGen.TerrainAdapt(rGraph, terrainRoadStep);
        if (lineRender)
        {
            RoadLineRender(rGraph, new Color(0, 0, 0), "Floor Roads");
            RoadLineRender(tGraph, new Color(255, 255, 255), "Terrain Roads");
        }
        GenerateMesh(tGraph);
        for(int i = 0; i < points.Count; i++)
        {
            var p = points[i];
            var s = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            s.transform.position = new Vector3(p.x, i, p.y);
            s.transform.localScale = new Vector3(10, 10, 10);
        }
        GenerateBuildings(tGraph);
    }

    void GenerateBuildings(RoadGraph rGraph)
    {
        foreach (var a in rGraph.graph.Keys)
        {
            foreach (var b in rGraph.graph[a])
            {
                float start = 0f;
                float dest = (b - a).magnitude;
                if (rGraph.graph[a].Count > 2) start = buildingOffset;
                if (rGraph.graph[b].Count > 2) dest -= buildingOffset;
                for (var dist = start; dist <= dest; dist += buildingStep)
                {
                    (var point, var norm) = rGraph.GetPointAlong((a, b), dist);
                    var location = point + norm * (roadWidth + buildingClearance);
                    var tData = GetComponent<Terrain>().terrainData;
                    var resL = tData.heightmapResolution;
                    location.y = tData.GetInterpolatedHeight(location.x / resL, location.y / resL);

                    bool found = true;
                    var scaling = new Vector3(Random.Range(20f, 40f), Random.Range(40, 120f), Random.Range(20f, 60f));
                    location.y = tData.GetInterpolatedHeight(location.x / resL, location.y / resL) + scaling.y / 2f - 10f;
                    var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.SetParent(buildings.transform);
                    cube.tag = buildingTag;
                    cube.layer = buildingsLayer;
                    cube.transform.position = location + norm * scaling.x / 2f;
                    cube.transform.localScale = scaling;
                    Collider[] obstacles = Physics.OverlapBox(cube.transform.position, cube.transform.localScale / 2f + new Vector3(buildingClearance, buildingClearance, buildingClearance) / 3f, cube.transform.rotation);
                    var cubeCollider = cube.AddComponent<BoxCollider>();
                    foreach (var o in obstacles)
                    {
                        if ((o.tag == roadTag || o.tag == buildingTag) && !o.Equals(cubeCollider) && o.bounds.Intersects(cubeCollider.bounds))
                        {
                            cube.SetActive(false);
                            Destroy(cubeCollider);
                            Destroy(cube);
                            found = false;
                            break;
                        }
                    }
                    if (found)
                    {
                        int objIndex = Random.Range(0, buildingSpawnables.Length);
                        var objBuilding = buildingSpawnables[objIndex];
                        location.y = tData.GetInterpolatedHeight(location.x / resL, location.y / resL);
                        var rotation = objIndex == 0 ? Quaternion.identity : Quaternion.AngleAxis(Random.Range(0f, 360f), Vector3.up);
                        var inst = Instantiate(objBuilding, location, rotation);
                        inst.transform.SetParent(buildings.transform);
                        inst.tag = buildingTag;
                        inst.layer = buildingsLayer;
                        foreach (Transform t in inst.transform)
                        {
                            t.gameObject.tag = buildingTag;
                            t.tag = buildingTag;
                            t.gameObject.layer = buildingsLayer;
                        }
                        if (objIndex == 1)
                        {
                            inst.transform.localScale = new Vector3(10, 10, 10);
                            location.y += 15;
                            inst.transform.position = location;
                        } else if (objIndex == 2)
                        {
                            inst.transform.localScale = new Vector3(5, 5, 5);
                            location.y += 2.5f;
                            inst.transform.position = location;
                        }
                        Destroy(cubeCollider);
                        Destroy(cube);
                        inst.SetActive(true);
                        // cube.SetActive(true);
                    }

                }

            }
        }

        GameObject terrainDecals = new GameObject();
        for (int num = 0; num < spawnAttempts; num++)
        {
            float x = Random.Range(0f, resLength);
            float y = Random.Range(0f, resLength);
            int obj = Random.Range(0, spawnables.Length);
            var spawn = spawnables[obj];
            var collider = spawn.GetComponent<MeshCollider>();
            var bounds = collider.sharedMesh.bounds;
            var tData = GetComponent<Terrain>().terrainData;
            var height = tData.GetInterpolatedHeight(x / resLength, y / resLength);
            var objH = height + bounds.extents.y;
            var center = new Vector3(x, objH, y);
            var obstacles = Physics.OverlapBox(center, bounds.extents);
            if (obstacles.Length <= 1)
            {
                var inst = Instantiate(spawn, new Vector3(x, height, y), Quaternion.identity);
                inst.SetActive(true);
                inst.transform.SetParent(terrainDecals.transform);
            }
        }
    }

    void GenerateMesh(RoadGraph rGraph)
    {
        var meshed = new HashSet<(Vector3, Vector3)>();
        var vertices = new List<Vector3>();
        var tris = new List<int>();
        var tData = GetComponent<Terrain>().terrainData;
        var resL = tData.heightmapResolution;

        int count = 0;
        foreach (var a in rGraph.graph.Keys)
        {
            foreach (var b in rGraph.graph[a])
            {
                if (meshed.Contains((b, a))) continue;
                meshed.Add((a, b));

                float start = 0;
                float dest = (b - a).magnitude;
                if (rGraph.graph[a].Count > 2) start = roadOffset;
                if (rGraph.graph[b].Count > 2) dest -= roadOffset;

                Vector3 point;
                Vector3 norm;
                (point, norm) = rGraph.GetPointAlong((a, b), start);
                var p = point + norm * roadWidth;
                var height = tData.GetInterpolatedHeight(point.x / resL, point.z / resL);
                p.y = tData.GetInterpolatedHeight(p.x / resL, p.z / resL) + roadHeight;
                vertices.Add(p);
                p = point - norm * roadWidth;
                p.y = tData.GetInterpolatedHeight(p.x / resL, p.z / resL) + roadHeight;
                vertices.Add(p);
                for (var dist = start + roadStep; dist <= dest;)
                {
                    (point, norm) = rGraph.GetPointAlong((a, b), dist);
                    height = tData.GetInterpolatedHeight(point.x / resL, point.z / resL);

                    p = point + norm * roadWidth;
                    p.y = tData.GetInterpolatedHeight(p.x / resL, p.z / resL) + roadHeight;
                    vertices.Add(p);
                    p = point - norm * roadWidth;
                    p.y = tData.GetInterpolatedHeight(p.x / resL, p.z / resL) + roadHeight;
                    vertices.Add(p);

                    tris.Add(count);
                    tris.Add(count + 2);
                    tris.Add(count + 1);

                    tris.Add(count + 1);
                    tris.Add(count + 2);
                    tris.Add(count + 3);
                    count += 2;

                    if (dist == dest) break;
                    dist += roadStep;
                    if (dist > dest) dist = dest;
                }
                count += 2;


            }
        }
        mesh.vertices = vertices.ToArray();
        mesh.triangles = tris.ToArray();

        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        mesh.Optimize();

        // Why does collision fail without this?
        var collider = roadMeshObject.GetComponent<MeshCollider>();
        collider.convex = true;
        collider.isTrigger = true;
        collider.isTrigger = false;
        collider.convex = false;
    }

    void RoadLineRender(RoadGraph rGraph, Color color, string name)
    {
        GameObject roadLines = new GameObject(name);
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
                    line.SetPositions(new Vector3[] { a, b });
                    rendered.Add((a, b));
                    line.transform.SetParent(roadLines.transform);
                }
            }
        }
    }

    void GenerateTerrain()
    {
        Terrain terrain = GetComponent<Terrain>();
        TerrainData tData = terrain.terrainData;

        tData.heightmapResolution = resLength + 1;
        tData.size = new Vector3(length, depth, length);

        for(int i = 0; i < 3; i++)
        {
            var x = Random.Range(.3f * resLength, .7f * resLength);
            var y = Random.Range(.3f * resLength, .7f * resLength);
            points.Add(new Vector2(x, y));
        }


        float[,] heights = new float[resLength + 1, resLength + 1];
        for(int x = 0; x < resLength + 1; x++)
        {
            for(int y = 0; y < resLength + 1; y++)
            {

                float height = 0f;
                for (int i = 0; i < octaves; i++)
                {
                    float xCoord = (float) x /  (resLength + 1) * horizontalScale * Mathf.Pow(2, i);
                    float yCoord = (float) y /  (resLength + 1) * horizontalScale * Mathf.Pow(2, i);
                    height += Mathf.PerlinNoise(xCoord, yCoord) * (verticalScale / Mathf.Pow(2, i));
                }
                heights[x, y] = height;
            }
        }

        tData.SetHeights(0, 0, heights);
    }
}
