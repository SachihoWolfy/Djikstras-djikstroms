using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public float connectionRadius = 5f;
    public List<Node> neighbors = new List<Node>();
    public float cost = 1f;
    public float minCost = 1f;
    public float maxCost = 10f;
    public Renderer nodeRenderer;

    private void Start()
    {
        nodeRenderer = GetComponent<Renderer>();
        AutoConnect();
        UpdateColor();
    }
    public void AdjustCost(float delta)
    {
        cost = Mathf.Clamp(cost + delta,1f,10f);
        UpdateColor();
    }
    private void UpdateColor()
    {
        float t = Mathf.InverseLerp(minCost, maxCost, cost);
        Color nodeColor = Color.Lerp(Color.green, Color.red, t);
        nodeRenderer.material.color = nodeColor;
    }
    public void AutoConnect()
    {
        neighbors.Clear(); 
        Node[] allNodes = FindObjectsOfType<Node>();
        foreach (Node node in allNodes)
        {
            if (node == this || node == null || !node.gameObject.activeInHierarchy) continue;
            float distance = 9999f;
            try
            {
                distance = Vector3.Distance(transform.position, node.transform.position);
            }
            catch
            {
                distance = 9999f;
            }
            if (distance <= connectionRadius)
            {
                AddNeighbor(node);
            }
        }
    }


    public void AddNeighbor(Node node)
    {
        if (node == null || !node.gameObject.activeInHierarchy) return;

        if (!neighbors.Contains(node))
        {
            neighbors.Add(node);
            if (!node.neighbors.Contains(this))
            {
                node.neighbors.Add(this);
            }
        }
    }
    public void RemoveFromNeighbors()
    {
        foreach (var neighbor in neighbors)
        {
            if (neighbor != null && neighbor.gameObject.activeInHierarchy)
            {
                neighbor.neighbors.Remove(this);
            }
        }
    }

    private void OnDestroy()
    {
        RemoveFromNeighbors();
    }
}
