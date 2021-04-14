using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RoadGen
{
    public int width;
    public int length;
    public TerrainData tData;

    public RoadGen(int width, int length, TerrainData tData)
    {
        this.width = width;
        this.length = length;
        this.tData = tData;
    }

    public RoadGraph GenerateRoadGraph()
    {
        RoadGraph rGraph = new RoadGraph();
        rGraph.AddEdge(new Vector3(0f, 0f, 0f), new Vector3(length, 0f,  width), true);
        rGraph.AddEdge(new Vector3(0f, 0f, 0f), new Vector3(length, 0f, 0f), true);
        rGraph.AddEdge(new Vector3(0f, 0f, 0f), new Vector3(0f, 0f, width), true);
        rGraph.AddEdge(new Vector3(length, 0f, 0f), new Vector3(length, 0f, width / 2), true);

        return rGraph;
    }

    public RoadGraph GeneratePrimaryL()
    {
        RoadGraph rGraph = new RoadGraph();

        float x = (float)length / 2;
        float y = (float)width / 2;

        PrimaryLRecurse(x, y, 2 * Mathf.PI / 3, rGraph, 5);
        // PrimaryLRecurse(x, y, 4 * Mathf.PI / 3, rGraph, 5);
        // PrimaryLRecurse(x, y, 0, rGraph, 5);

        return rGraph;
    }

    public RoadGraph TerrainAdapt(RoadGraph rGraph)
    {
        RoadGraph tGraph = new RoadGraph();
        foreach (var a in rGraph.graph.Keys)
        {
            foreach(var b in rGraph.graph[a])
            {
                float length = tData.heightmapResolution;
                Vector3 start = new Vector3(a.x, tData.GetInterpolatedHeight(a.x / length, a.z / length), a.z);
                Vector3 dest = new Vector3(b.x, tData.GetInterpolatedHeight(b.x / length, b.z / length), b.z);

                if (tGraph.graph.ContainsKey(start) && tGraph.graph.ContainsKey(dest))
                    continue;

                var current = new Vector3(start.x, start.y, start.z);

                int maxSteps = 100;
                while (maxSteps --> 0)
                {
                    Vector3 next = NextStep(start, dest, current, 5f);
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

        return minDevianceStep;
    }

    private void PrimaryLRecurse(float x, float y, float heading, RoadGraph rGraph, int depth)
    {
        if (depth < 0)
            return;
        depth--;

        heading += Random.Range(-Mathf.PI / 8, Mathf.PI / 8);
        float roadLength = Random.Range(15f, 55f);

        float xD = x + roadLength * Mathf.Cos(heading);
        float yD = y + roadLength * Mathf.Sin(heading);
        bool intersected;

        (xD, yD, intersected) = rGraph.DeltaCheck(xD, yD, 5f);

        rGraph.AddEdge(new Vector3(x, 0f, y), new Vector3(xD, 0f, yD), true);
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

        foreach (var i in graph.Keys)
        {
            foreach(var j in graph[i])
            {

            }
        }

        if (!graph[a].Contains(b))
            graph[a].Add(b);
        if (!graph[b].Contains(a))
            graph[b].Add(a);

        return true;
    }

    public (float, float, bool) DeltaCheck(float x, float y, float delta)
    {
        var coord = new Vector3(x, 0f, y);

        // Checking for point point intersection
        foreach (var p in graph.Keys)
        {
            if ((coord - p).magnitude <= delta)
            {
                // TODO: Check for allowable angle
                return (p.x, p.z, true);
            }
        }

        // TODO: Checking for point road intersection
        foreach (var a in graph.Keys)
        {
            foreach (var b in graph[a])
            {
                var M = b - a;
                float t = Vector3.Dot((coord - a), M) / Vector3.Dot(M, M);
                var P = a + t * M;
                var d = (coord - P).magnitude;
                if (d <= delta)
                {
                    return (P.x, P.z, true);
                }
            }
        }


        return (x, y, false);
    }

    public Vector3 InterpolateSpline((Vector3, Vector3) road, float dist)
    {
        var a = road.Item1;
        var b = road.Item2;
        var l = a;
        var r = b;

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

        /*
        var along = d * dist + a;
        var n = new Vector3(d.x, 0, d.z).normalized;
        n = new Vector3(-d.z, 0, d.x).normalized;
        */

        var along = InterpolateSpline(road, dist);
        var a = InterpolateSpline(road, dist - 0.01f);
        var b = InterpolateSpline(road, dist + 0.01f);
        var n = (b - a).normalized;
        n = new Vector3(-n.z, 0, n.x).normalized;


        return (along, n);

    }

}
