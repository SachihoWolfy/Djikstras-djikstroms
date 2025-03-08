using System.Collections.Generic;
using UnityEngine;

public static class DijkstraPathfinder
{
    public static List<Node> FindPath(Node start, Node target)
    {
        Dictionary<Node, float> distances = new Dictionary<Node, float>();
        Dictionary<Node, Node> previous = new Dictionary<Node, Node>();
        List<Node> unvisited = new List<Node>();
        foreach (Node node in GameObject.FindObjectsOfType<Node>())
        {
            distances[node] = float.MaxValue;
            previous[node] = null;
            unvisited.Add(node);
        }

        distances[start] = 0;

        while (unvisited.Count > 0)
        {
            unvisited.Sort((a, b) => distances[a].CompareTo(distances[b]));
            Node current = unvisited[0];
            unvisited.Remove(current);
            if (current == null || !current.gameObject.activeInHierarchy)
            {
                continue;
            }
            if (current == target)
                return ConstructPath(previous, target);
            foreach (Node neighbor in current.neighbors)
            {
                if (neighbor == null || !neighbor.gameObject.activeInHierarchy)
                {
                    continue;
                }

                float alt = distances[current] + Vector3.Distance(current.transform.position, neighbor.transform.position);
                if (alt < distances[neighbor])
                {
                    distances[neighbor] = alt;
                    previous[neighbor] = current;
                }
            }
        }

        return new List<Node>();
    }

    private static List<Node> ConstructPath(Dictionary<Node, Node> previous, Node target)
    {
        List<Node> path = new List<Node>();
        for (Node at = target; at != null; at = previous[at])
        {
            path.Insert(0, at);
        }
        return path;
    }
}
