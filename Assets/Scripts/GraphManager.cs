using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GraphManager : MonoBehaviour
{
    public List<Node> nodes = new List<Node>();
    public Material lineMaterial;
    private List<LineRenderer> lineRenderers = new List<LineRenderer>();

    private void Awake()
    {
        RegisterAllNodes();
        UpdateGraphVisualization();
    }

    public void RegisterAllNodes()
    {
        nodes.Clear();
        nodes.AddRange(FindObjectsOfType<Node>());
        foreach (Node node in nodes)
        {
            node.AutoConnect();
        }
        UpdateGraphVisualization();
    }

    public void RegisterNode(Node newNode)
    {
        if (!nodes.Contains(newNode))
        {
            nodes.Add(newNode);
            newNode.AutoConnect();
            foreach (Node node in nodes)
            {
                node.AutoConnect();
            }

            UpdateGraphVisualization();
        }
    }

    public void UpdateGraphVisualization()
    {
        lineRenderers.AddRange(FindObjectsOfType<LineRenderer>());
        foreach (var line in lineRenderers)
        {
            if(line != null)
            Destroy(line.gameObject);
        }
        lineRenderers.Clear();
        foreach (Node node in nodes)
        {
            foreach (Node neighbor in node.neighbors)
            {
                if (node == null) continue;
                if (node.transform.position != neighbor.transform.position)
                {
                    CreateLineRenderer(node.transform.position, neighbor.transform.position);
                }
            }
        }
    }

    private void CreateLineRenderer(Vector3 start, Vector3 end)
    {
        GameObject lineObj = new GameObject("Line");
        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.material = lineMaterial;
        lr.startWidth = 0.1f;
        lr.endWidth = 0.1f;
        lr.positionCount = 2;
        lr.SetPositions(new Vector3[] { start, end });
        lineRenderers.Add(lr);
    }

    public List<Node> GetShortestPath(Node start, Node target)
    {
        return DijkstraPathfinder.FindPath(start, target);
    }
}
