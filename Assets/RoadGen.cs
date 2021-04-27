using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoadGen
{
    public int length;
    public TerrainData tData;
    public float padding;
    public float minRoadLength;
    public float maxRoadLength;
    public float roadDelta;
    public int recDepth;

    public RoadGen(int length, TerrainData tData, float padding, float minRoadLength, float maxRoadLength, float roadDelta, int recDepth)
    {
        this.length = length;
        this.tData = tData;
        this.padding = padding;
        this.minRoadLength = minRoadLength;
        this.maxRoadLength = maxRoadLength;
        this.roadDelta = roadDelta;
        this.recDepth = recDepth;
    }

    public RoadGraph GenerateRoadGraph()
    {
        RoadGraph rGraph = new RoadGraph();
        rGraph.AddEdge(new Vector3(0f, 0f, 0f), new Vector3(length, 0f,  length), true);
        rGraph.AddEdge(new Vector3(0f, 0f, 0f), new Vector3(length, 0f, 0f), true);
        rGraph.AddEdge(new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, length), true);
        rGraph.AddEdge(new Vector3(length, 0f, 0f), new Vector3(length, 0f, length / 2), true);

        return rGraph;
    }

    public RoadGraph GeneratePrimaryL()
    {
        RoadGraph rGraph = new RoadGraph();

        float x = (float)length / 2;
        float y = (float)length / 2;

        rGraph.AddEdge(new Vector3(0f + padding, 0f, 0f + padding), new Vector3(0f + padding, 0f, length - padding), true);
        rGraph.AddEdge(new Vector3(0f + padding, 0f, 0f + padding), new Vector3(length - padding, 0f, 0f + padding), true);
        rGraph.AddEdge(new Vector3(length - padding, 0f, length - padding), new Vector3(0f + padding, 0f, length - padding), true);
        rGraph.AddEdge(new Vector3(length - padding, 0f, length - padding), new Vector3(length - padding, 0f, 0f + padding), true);
        for (int i = 0; i < 10; i++)
        {
            x = Random.Range(padding, length - padding);
            y = Random.Range(padding, length - padding);
            float h = Random.Range(-2 * Mathf.PI, 0);
            PrimaryLRecurse(x, y, h, rGraph, recDepth);
        }
        // PrimaryLRecurse(x, y, 2 * Mathf.PI / 3, rGraph, recDepth);
        // PrimaryLRecurse(x, y, 4 * Mathf.PI / 3, rGraph, recDepth);
        // PrimaryLRecurse(x, y, 0, rGraph, recDepth);

        return rGraph;
    }

    public RoadGraph TerrainAdapt(RoadGraph rGraph, float tStep)
    {
        RoadGraph tGraph = new RoadGraph();
        var visited = new HashSet<(Vector3, Vector3)>();
        foreach (var a in rGraph.graph.Keys)
        {
            foreach(var b in rGraph.graph[a])
            {
                if (visited.Contains((b, a))) continue;
                visited.Add((a, b));
                float length = tData.heightmapResolution;
                Vector3 start = new Vector3(a.x, tData.GetInterpolatedHeight(a.x / length, a.z / length), a.z);
                Vector3 dest = new Vector3(b.x, tData.GetInterpolatedHeight(b.x / length, b.z / length), b.z);

                var current = new Vector3(start.x, start.y, start.z);

                int maxSteps = 1000;
                while (maxSteps --> 0)
                {
                    Vector3 next = NextStep(start, dest, current, tStep);
                    tGraph.AddEdge(current, next, true);
                    if (next.Equals(dest)) break;
                    current = next;
                }
            }
        }

        return tGraph;
    }

    private Vector3 NextStep(Vector3 a, Vector3 b, Vector3 current, float stepSize)
    {
        Vector3 dir = (b - current);
        dir.y = 0;
        dir = dir.normalized * stepSize;
        if ((b-current).magnitude <= stepSize)
        {
            return b;
        }
        float sDist = (current - a).magnitude;
        float fDist = (current - b).magnitude;
        float iratio = sDist / (sDist + fDist);
        iratio = Mathf.Abs(.5f - iratio);
        float angleStart = (-Mathf.PI / 8f) * iratio;
        float angleEnd = -angleStart;
        float angleIncrement = angleEnd / 2f;
        float minDeviance = Mathf.Infinity;
        Vector3 minDevianceStep = new Vector3();
        for (float angle = angleStart; angle <= angleEnd; angle += angleIncrement)
        {
            // Get the Step Vector
            float x = Mathf.Cos(angle) * dir.x - Mathf.Sin(angle) * dir.z;
            float z = Mathf.Sin(angle) * dir.x + Mathf.Cos(angle) * dir.z;
            Vector3 step = new Vector3(x, 0, z);
            step = current + step;
            step.y = tData.GetInterpolatedHeight(step.x / tData.heightmapResolution, step.z / tData.heightmapResolution);

            // Find Deviation from Expected Height
            float aDist = (step - a).magnitude;
            float bDist = (step - b).magnitude;
            float ratio = aDist / (aDist + bDist);
            float expectedHeight = a.y + (b.y - a.y) * ratio;

            if (Mathf.Abs(expectedHeight - step.y) < minDeviance)
            {
                minDeviance = Mathf.Abs(expectedHeight - step.y);
                minDevianceStep = step;
            }
        }

        dir = current + dir;
        dir.y = tData.GetInterpolatedHeight(dir.x / tData.heightmapResolution, dir.z / tData.heightmapResolution);
        return (minDevianceStep + dir) / 2;
    }

    private void PrimaryLRecurse(float x, float y, float heading, RoadGraph rGraph, int depth)
    {
        if (depth < 0)
            return;
        depth--;

        heading += Random.Range(-Mathf.PI / 8, Mathf.PI / 8);
        float roadLength = Random.Range(minRoadLength, maxRoadLength);

        float xD = x + roadLength * Mathf.Cos(heading);
        float yD = y + roadLength * Mathf.Sin(heading);
        if (xD < 0f + padding || xD > length - padding || yD < 0f + padding || yD > length - padding)
        {
            return;
        }
        bool intersected;
        bool cancel;
        Vector3 intersect;

        (intersect, intersected, cancel) = rGraph.DeltaCheck(new Vector3(xD, 0f, yD), new Vector3(x, 0f, y), roadDelta);
        xD = intersect.x;
        yD = intersect.z;

        if (cancel)
        {
            return;
        }
        rGraph.AddEdge(new Vector3(x, 0f, y), intersect, true);
        if (intersected)
        {
            return;
        }

        int branches = Random.Range(1, 4);
        if (branches == 1)
        {
            var newHeading = heading + Mathf.PI * Random.Range(-1, 2) / 2f;
            PrimaryLRecurse(xD, yD, newHeading, rGraph, depth);
        } else if (branches == 2)
        {
            var newHeading = heading + Mathf.PI * Random.Range(-1, 1) / 2f;
            PrimaryLRecurse(xD, yD, newHeading, rGraph, depth);
            PrimaryLRecurse(xD, yD, newHeading + Mathf.PI / 2f, rGraph, depth);
        } else if (branches == 3)
        {
            PrimaryLRecurse(xD, yD, heading, rGraph, depth - 1);
            PrimaryLRecurse(xD, yD, heading + Mathf.PI / 2f, rGraph, depth);
            PrimaryLRecurse(xD, yD, heading - Mathf.PI / 2f, rGraph, depth);
        }

    }
}


public class RoadGraph
{
    public Dictionary<Vector3, HashSet<Vector3>> graph;


    public RoadGraph()
    {
        graph = new Dictionary<Vector3, HashSet<Vector3>>();
    }

    public bool AddVertex(Vector3 point)
    {
        if (graph.ContainsKey(point))
            return false;


        graph.Add(point, new HashSet<Vector3>());
        return true;
    }

    public bool AddEdge(Vector3 a, Vector3 b, bool createVertex = false)
    {
        if (!graph.ContainsKey(a))
            if (!createVertex)
                return false;
            else
                AddVertex(a);
        if (!graph.ContainsKey(b))
            if (!createVertex)
                return false;
            else
                AddVertex(b);

        if (!graph[a].Contains(b))
            graph[a].Add(b);
        if (!graph[b].Contains(a))
            graph[b].Add(a);

        return true;
    }

    private float Cross(Vector2 x, Vector2 y)
    {
        return x.x * y.y - x.y * y.x;
    }

    public (Vector3, bool, bool) DeltaCheck(Vector3 coord, Vector3 oldCoord, float delta)
    {
        // Checking for point point intersection
        foreach (var p in graph.Keys)
        {
            if ((coord - p).magnitude <= delta)
            {
                // Check for allowable angle
                if (graph[p].Count >= 4)
                {
                    return (new Vector3(), false, true);
                }
                foreach (var b in graph[p])
                {
                    var x = (oldCoord - p).normalized;
                    var y = (b - p).normalized;
                    if (Mathf.Abs(Vector3.Dot(x, y)) > .7f)
                    {
                        return (new Vector3(), false, true);
                    }
                }
                return (new Vector3(p.x, 0f, p.z), true, false);
            }
        }

        // Checking for point road intersection
        foreach (var a in graph.Keys)
        {
            if (a.Equals(oldCoord)) continue;
            foreach (var b in graph[a])
            {
                if (b.Equals(oldCoord)) continue;
                var M = b - a;
                float t = Vector3.Dot((coord - a), M) / Vector3.Dot(M, M);
                var P = a + t * M;
                var d = (coord - P).magnitude;
                if (d <= delta)
                {
                    if (Vector3.Dot((P-a).normalized, (P-b).normalized) > 0)
                    {
                        continue;
                    }
                    if (Mathf.Abs(Vector3.Dot((b - a).normalized, (P - oldCoord).normalized)) > 0.7f) {
                        return (coord, false, true);
                    }
                    var c = new Vector3(P.x, 0f, P.z);
                    graph[a].Remove(b);
                    graph[b].Remove(a);
                    AddEdge(a, c, true);
                    AddEdge(b, c, true);
                    return (c, true, false);
                }
            }
        }

        // Checking for road road intersections
        foreach (var a in graph.Keys)
        {
            if (a.Equals(oldCoord)) continue;
            foreach (var b in graph[a])
            {
                if (b.Equals(oldCoord)) continue;
                var q = new Vector2(oldCoord.x, oldCoord.z);
                var s = new Vector2(coord.x, coord.z);
                var p = new Vector2(b.x, b.z);
                var r = new Vector2(a.x, a.z);

                Vector2 s1 = new Vector2(s.x - q.x, s.y - q.y);
                Vector2 s2 = new Vector2(r.x - p.x, r.y - p.y);
                float c = (-s1.y * (q.x - p.x) + s1.x * (q.y - p.y)) / (-s2.x * s1.y + s1.x * s2.y);
                float t = (s2.x * (q.y - p.y) - s2.y * (q.x - p.y)) / (-s2.x * s1.y + s1.x * s2.y);

                if (c >= 0 && c <= 1 && t >= 0 && t <= 1)
                {
                    return (coord, false, true);
                }


                /**
                var q = new Vector2(oldCoord.x, oldCoord.z);
                var s = new Vector2(coord.x, coord.z);
                var p = new Vector2(b.x, b.z);
                var r = new Vector2(a.x, a.z);

                var t = Cross(q - p, s) / Cross(r, s);
                var u = Cross(q - p, r) / Cross(r, s);
                if (Cross(r, s) == 0 && Cross(q - p, r) == 0)
                {
                    var t0 = Vector2.Dot(q - p, r) / Vector2.Dot(r, r);
                    var t1 = Vector2.Dot(q + s - p, r) / Vector2.Dot(r, r);
                    if (t0 >= 0 && t0 <= 1 || t1 >= 0 && t1 <= 1)
                    {
                        Debug.Log("Colinear");
                        return (coord, false, true);
                    }
                }
                else if (Cross(r, s) == 0 && Cross(q - p, r) != 0) continue;
                else if (Cross(r, s) != 0 && t >= 0 && t <= 1 && u >= 0 && u <= 1)
                {
                    // TODO: FIX THIS
                    Debug.Log("Road Intersection");
                    Debug.Log("Q: " + q);
                    Debug.Log("S: " + s);
                    Debug.Log("P: " + p);
                    Debug.Log("R: " + r);
                    return (coord, false, true);
                    var isect = p + t * r;
                    var i = new Vector3(isect.x, 0f, isect.y);

                    foreach (var x in new Vector3[] {b, a})
                    {
                        if ((i-x).magnitude <= delta)
                        {
                            if (graph[x].Count >= 4) return (coord, false, true);
                            foreach (var y in graph[x])
                            {
                                var g = (oldCoord - x).normalized;
                                var h = (y - x).normalized;
                                if (Vector3.Dot(g, h) > .5) return (new Vector3(), false, true);
                            }
                            return (x, true, false);
                        }
                    }
                    // if (Mathf.Abs(Vector3.Dot(i-a, i-b) > 0.5f)
                }
                **/
            }
        }


        return (coord, false, false);
    }

    public Vector3 InterpolateSpline((Vector3, Vector3) road, float dist)
    {
        var a = road.Item1;
        var b = road.Item2;
        var l = a;
        var r = b;

        if (graph[a].Count == 2)
            foreach (var c in graph[a])
                if (!c.Equals(b))
                    l = c;
        if (graph[b].Count == 2)
            foreach (var c in graph[b])
                if (!c.Equals(a)) r = c;

        var t = dist / (b - a).magnitude;
        if (graph[a].Count == 2)
        {
            foreach (var x in graph[a])
            {
                if (!x.Equals(b)) l = x;
            }
        }
        if (graph[b].Count == 2)
        {
            foreach (var x in graph[b])
            {
                if (!x.Equals(a)) r = x;
            }
        }

        var v0 = 2 * a;
        var v1 = b - l;
        var v2 = 2 * l - 5 * a + 4 * b - r;
        var v3 = -1 * l + 3 * a - 3 * b + r;
        var along = .5f * (v0 + (v1 * t) + (v2 * t * t) + (v3 * t * t * t));

        return along;
    }
    public (Vector3, Vector3) GetPointAlong((Vector3, Vector3) road, float dist)
    {
        var along = InterpolateSpline(road, dist);
        var a = InterpolateSpline(road, dist - 0.01f);
        var b = InterpolateSpline(road, dist + 0.01f);
        var n = (b - a).normalized;
        n = new Vector3(-n.z, 0, n.x).normalized;


        return (along, n);

    }

}
