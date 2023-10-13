using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public List<Node> neighborNodes;

    public Node parent;

    [Header("Colores")]
    public Color hoverColor;
    public Color lowLevelCol;
    public Color mediumLevelCol;
    public Color highLevelCol;
    public Color impenetrableCol;

    [Header("Datos A*")]
    public int P;
    public int D, x, y, F;

    private MeshRenderer rend;

    private Color OGColor;

    private void Start()
    {
        rend = GetComponent<MeshRenderer>();

        switch (P)
        {
            case 0:
                rend.material.color = impenetrableCol;
                break;
            case 1:
                rend.material.color = lowLevelCol;
                break;
            case 2:
                rend.material.color = mediumLevelCol;
                break;
            case 3:
                rend.material.color = highLevelCol;
                break;
            default:
                break;
        }

        OGColor = rend.material.color;

        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.left), out hit, 5.0f))
        {
            neighborNodes.Add(hit.transform.gameObject.GetComponent<Node>());
        }
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out hit, 5.0f))
        {
            neighborNodes.Add(hit.transform.gameObject.GetComponent<Node>());
        }
        if (Physics.Raycast(transform.position, (transform.TransformDirection(Vector3.up) + transform.TransformDirection(Vector3.left)), out hit, 3.0f))
        {
            neighborNodes.Add(hit.transform.gameObject.GetComponent<Node>());
        }
        if (Physics.Raycast(transform.position, (transform.TransformDirection(Vector3.up) + transform.TransformDirection(Vector3.right)), out hit, 3.0f))
        {
            neighborNodes.Add(hit.transform.gameObject.GetComponent<Node>());
        }
        if (Physics.Raycast(transform.position, (transform.TransformDirection(Vector3.down) + transform.TransformDirection(Vector3.left)), out hit, 3.0f))
        {
            neighborNodes.Add(hit.transform.gameObject.GetComponent<Node>());
        }
        if (Physics.Raycast(transform.position, (transform.TransformDirection(Vector3.down) + transform.TransformDirection(Vector3.right)), out hit, 3.0f))
        {
            neighborNodes.Add(hit.transform.gameObject.GetComponent<Node>());
        }
    }

    private void OnMouseDown()
    {
        Debug.Log("A");
    }

    private void OnMouseEnter()
    {
        rend.material.color = OGColor - hoverColor;
    }

    private void OnMouseExit()
    {
        rend.material.color = OGColor;
    }
}
