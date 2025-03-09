using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AINavigator : MonoBehaviour
{
    public GraphManager graphManager;
    public float speed = 3f;
    private LineRenderer pathLineRenderer; 
    private List<Node> path = new List<Node>();
    private int currentIndex = 0;
    private Node currentTarget;
    public bool caresAboutCost;

    private void Start()
    {
        CreatePathLineRenderer();
        PickNewTarget();
    }
    private void Update()
    {
        
        if (pathLineRenderer == null)
        {
            CreatePathLineRenderer();
        }

        if (path.Count == 0)
        {
            PickNewTarget();
            return;
        }

        if (IsAnyNodeInvalid())
        {
            RecalculatePath();
            return;
        }
        MoveTowards(path[currentIndex].transform.position);
        if (Vector3.Distance(transform.position, path[currentIndex].transform.position) < 0.1f)
        {
            currentIndex++;
            if (currentIndex >= path.Count)
                PickNewTarget();
        }
        UpdatePathLineRenderer();
    }

    private void MoveTowards(Vector3 targetPosition)
    {
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
    }

    private void PickNewTarget()
    {
        if (graphManager.nodes.Count == 0) return;

        Node randomTarget;
        do
        {
            randomTarget = graphManager.nodes[Random.Range(0, graphManager.nodes.Count)];
        } while (randomTarget == currentTarget && randomTarget != null);

        currentTarget = randomTarget;
        path = graphManager.GetShortestPath(GetClosestNode(), currentTarget, caresAboutCost);
        currentIndex = 0;
        UpdatePathLineRenderer();
    }

    private Node GetClosestNode()
    {
        Node closest = null;
        float minDist = float.MaxValue;

        foreach (Node node in graphManager.nodes)
        {
            if (node != null && node.gameObject.activeInHierarchy)
            {
                float dist = Vector3.Distance(transform.position, node.transform.position);
                if (dist < minDist)
                {
                    minDist = dist;
                    closest = node;
                }
            }
        }
        return closest;
    }

    public void RecalculatePath()
    {
        path.Clear();
        path = graphManager.GetShortestPath(GetClosestNode(), currentTarget, caresAboutCost);
        currentIndex = 0;
        UpdatePathLineRenderer();
    }

    private bool IsAnyNodeInvalid()
    {
        foreach (Node node in path)
        {
            if (node == null || !node.gameObject.activeInHierarchy)
            {
                return true;
            }
        }
        return false; 
    }

    private void UpdatePathLineRenderer()
    {
        if (path.Count == 0) return;
        pathLineRenderer.positionCount = path.Count;

        for (int i = 0; i < path.Count; i++)
        {
            if (path[i] != null && path[i].gameObject.activeInHierarchy)
            {
                pathLineRenderer.SetPosition(i, path[i].transform.position);
            }
        }
    }

    private void CreatePathLineRenderer()
    {
        if (pathLineRenderer == null)
        {
            GameObject lineObject = new GameObject("PathLine");
            pathLineRenderer = lineObject.AddComponent<LineRenderer>();
            pathLineRenderer.startWidth = 0.2f;
            pathLineRenderer.endWidth = 0.2f;
            pathLineRenderer.material = new Material(Shader.Find("Sprites/Default"));
            if (caresAboutCost)
            {
                pathLineRenderer.startColor = Color.green;
                pathLineRenderer.endColor = Color.green;
            }
            else
            {
                pathLineRenderer.startColor = Color.red;
                pathLineRenderer.endColor = Color.red;
            }
        }
    }
    public void SetTargetNode(Node target)
    {
        if (target == null) return;

        currentTarget = target;
        path = graphManager.GetShortestPath(GetClosestNode(), currentTarget, caresAboutCost);
        currentIndex = 0;
        UpdatePathLineRenderer();
    }

}
