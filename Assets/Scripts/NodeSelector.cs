using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NodeSelector : MonoBehaviour
{
    public Camera mainCamera;
    public GraphManager graphManager;
    public GameObject selector;
    public SpriteRenderer mrSelector;
    public SpriteRenderer unselectedSprite;
    public GameObject hudControls;
    private Node previouslySelectedNode;
    private Color previouslySelectedColor;

    private void Update()
    {
        GetNodeUnderMouse();
        if (Input.GetMouseButtonDown(2))
        {
            Node selectedNode = GetNodeUnderMouse();
            if (selectedNode != null)
            {
                SetTargetForAllNavigators(selectedNode);
            }
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        if (Input.GetKeyDown(KeyCode.F))
        {
            hudControls.SetActive(!hudControls.activeSelf);
        }
    }

    private Node GetNodeUnderMouse()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            Node node = hit.collider.GetComponent<Node>();
            if (node != null)
            {
                if(previouslySelectedNode == node && previouslySelectedColor.CompareRGB(node.nodeRenderer.material.color))
                {
                    mrSelector.color = Color.yellow;
                }
                else if(previouslySelectedNode == node)
                {
                    mrSelector.color = node.nodeRenderer.material.color;
                }
                else
                {
                    mrSelector.color = Color.yellow;
                    previouslySelectedNode = node;
                    previouslySelectedColor = node.nodeRenderer.material.color;
                }
                selector.transform.position = node.transform.position;
                mrSelector.enabled = true;
                unselectedSprite.enabled = false;
            }
            else
            {
                selector.transform.position = hit.point;
                mrSelector.enabled = false;
                unselectedSprite.enabled = true;
            }
                return node;
        }
        mrSelector.enabled = false;
        unselectedSprite.enabled = true;
        return null;
    }

    private void SetTargetForAllNavigators(Node targetNode)
    {
        AINavigator[] navigators = FindObjectsOfType<AINavigator>();
        foreach (AINavigator navigator in navigators)
        {
            navigator.SetTargetNode(targetNode);
        }
    }
}
