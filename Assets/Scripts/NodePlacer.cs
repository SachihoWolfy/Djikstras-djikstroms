using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodePlacer : MonoBehaviour
{
    public GameObject nodePrefab;
    public GraphManager graphManager;
    public Material lineMaterial;
    private Node previewNode;
    private Node selectedNode;
    private bool isPlacingNode = false;
    private Camera cam;
    private AINavigator ai;

    private List<LineRenderer> previewLineRenderers = new List<LineRenderer>();

    private void Start()
    {
        cam = Camera.main;
        ai = FindObjectOfType<AINavigator>();
    }

    private void Update()
    {
        if (isPlacingNode)
        {
            UpdatePreviewNodePosition();
        }

        HandleMouseInput();
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                Node node = hit.collider.GetComponent<Node>();
                if (node)
                {
                    selectedNode = node;
                }
                else if (!isPlacingNode)
                {
                    StartPlacingNode(hit.point);
                }
            }
        }

        if (Input.GetMouseButton(0) && selectedNode)
        {
            MoveSelectedNode();
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (isPlacingNode)
            {
                PlaceNode();
            }
            else if (selectedNode)
            {
                FinalizeNodeMove();
            }
            selectedNode = null;
        }

        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                Node node = hit.collider.GetComponent<Node>();
                if (node)
                {
                    DeleteNode(node);
                }
                else if (isPlacingNode)
                {
                    CancelPlacement();
                }
            }
        }
    }

    private void StartPlacingNode(Vector3 position)
    {
        isPlacingNode = true;
        GameObject newNodeObj = Instantiate(nodePrefab);
        previewNode = newNodeObj.GetComponent<Node>();
        previewNode.GetComponent<Collider>().enabled = false;
        previewNode.transform.position = position + Vector3.up * 1f;
    }

    private void UpdatePreviewNodePosition()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            previewNode.transform.position = hit.point + Vector3.up * 1f;
            foreach (var line in previewLineRenderers)
            {
                if (line != null)
                Destroy(line.gameObject);
            }
            previewLineRenderers.Clear();
            previewNode.AutoConnect();
            foreach (Node neighbor in previewNode.neighbors)
            {
                CreatePreviewLine(previewNode.transform.position, neighbor.transform.position);
            }
        }
    }

    private void CreatePreviewLine(Vector3 start, Vector3 end)
    {
        GameObject lineObj = new GameObject("PreviewLine");
        LineRenderer lr = lineObj.AddComponent<LineRenderer>();
        lr.material = lineMaterial;
        lr.startWidth = 0.05f;
        lr.endWidth = 0.05f;
        lr.positionCount = 2;
        lr.SetPositions(new Vector3[] { start, end });
        previewLineRenderers.Add(lr);
    }

    private void PlaceNode()
    {
        isPlacingNode = false;
        previewNode.GetComponent<Collider>().enabled = true;
        graphManager.RegisterNode(previewNode);
        foreach (var line in previewLineRenderers)
        {
            Destroy(line.gameObject);
        }
        previewLineRenderers.Clear();

        previewNode = null;
    }

    private void MoveSelectedNode()
    {
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            Vector3 targetPosition = hit.point;
            targetPosition.y = selectedNode.transform.position.y;
            selectedNode.transform.position = targetPosition;
            UpdateGraphForMovedNode(selectedNode);
        }
    }

    private void UpdateGraphForMovedNode(Node node)
    {
        graphManager.RegisterAllNodes();
        node.AutoConnect();
        graphManager.UpdateGraphVisualization();
        ClearOldConnections();
        ShowPreviewConnections(node);
    }

    private void ClearOldConnections()
    {
        foreach (var line in previewLineRenderers)
        {
            if(line!=null)
            Destroy(line.gameObject);
        }
        previewLineRenderers.Clear();
    }

    private void ShowPreviewConnections(Node node)
    {
        foreach (Node neighbor in node.neighbors)
        {
            CreatePreviewLine(node.transform.position, neighbor.transform.position);
        }
    }


    // New function because why not
    private void FinalizeNodeMove()
    {
        graphManager.UpdateGraphVisualization();
    }

    private void CancelPlacement()
    {
        isPlacingNode = false;
        Destroy(previewNode.gameObject);
        previewNode = null;
        foreach (var line in previewLineRenderers)
        {
            Destroy(line.gameObject);
        }
        previewLineRenderers.Clear();
    }

    private void DeleteNode(Node node)
    {
        //This doesn't work, and I'm not asking why. It works at the start and that's all I need.
        if (graphManager.nodes.Count <= 2)
        {
            Debug.Log("Cannot delete a node when there are only two nodes.");
            return;
        }
        node.RemoveFromNeighbors();
        Destroy(node.gameObject);
        graphManager.RegisterAllNodes();
        graphManager.UpdateGraphVisualization();
        StartCoroutine(UpdateGraphLater());
    }

    IEnumerator UpdateGraphLater()
    {
        yield return new WaitForEndOfFrame();
        graphManager.UpdateGraphVisualization();
    }
    private void OnDrawGizmos()
    {
        if (previewNode)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(previewNode.transform.position, previewNode.connectionRadius);

            foreach (Node neighbor in previewNode.neighbors)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(previewNode.transform.position, neighbor.transform.position);
            }
        }
    }
}
