using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Node : MonoBehaviour
{
    public GameObject hexagon;

    public List<Node> neighborNodes;

    public Node parent;

    public bool Selected;

    public NodeState state;

    private Text productionUI;

    private GameManager gameManager;

    [Header("Probabilidades")]
    public float probImpenetrable;
    public float probHigh;
    public float probMed;
    public float probLow;

    [Header("Colores")]
    public Color hoverColor;
    public Color lowLevelCol;
    public Color mediumLevelCol;
    public Color highLevelCol;
    public Color impenetrableCol;

    [Header("Datos A*")]
    public float P;
    public float H, D, x, y, F;

    private MeshRenderer rend;

    private GenerateMap generation;

    private Color OGColor;

    private void Awake()
    {
        generation = FindObjectOfType<GenerateMap>();
        gameManager = FindObjectOfType<GameManager>();
        x = transform.position.x;
        y = transform.position.y;
    }

    private void Start()
    {
        productionUI = GameObject.Find("productionText").GetComponent<Text>();
        rend = GetComponent<MeshRenderer>();

        state = gameObject.AddComponent<NodeState>();

        float rand = Random.Range(0.0f, 1.0f);

        if (rand <= probImpenetrable)
        {
            P = 0;
            GetComponent<NodeState>().occupied = true;
        }
        else if (rand <= (probLow + probImpenetrable))
        {
            P = 1;
        }
        else if (rand <= (probMed + probLow + probImpenetrable))
        {
            P = 2;
        }
        else
        {
            P= 3;
        }

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
        FindNeighbors();
    }

    public void FindNeighbors()
    {
        StartCoroutine(findNeighborsCoroutine());
    }

    IEnumerator findNeighborsCoroutine()
    {
        yield return new WaitForSeconds(0.5f);

        neighborNodes.Clear();

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
        productionUI.text = "Production: " + gameObject.GetComponent<NodeState>().production;
        MoveUnitHere();
    }

    public void MoveUnitHere()
    {
        if(gameManager.currentPlayer.selectedUnit != null )
        {
            if (GetComponent<NodeState>().building != null)
            {
                if(GetComponent<NodeState>().building.owner != gameManager.currentPlayer.selectedUnit.owner)
                {
                    gameManager.currentPlayer.selectedUnit.tryAttackOnBuilding(GetComponent<NodeState>().building, this);
                } else
                {
                    gameManager.currentPlayer.selectedUnit.Move();
                }
            }
            else
            {
                gameManager.currentPlayer.selectedUnit.Move();
            }
        }
    }

    private void OnMouseEnter()
    {
        Node[] brothers = FindObjectsOfType<Node>();
        foreach (Node node in brothers)
        {
            node.Unselect();
        }
        Select();
        if (P == 0)
            return;

        if(gameManager.currentPlayer.selectedUnit != null)
        {
            gameManager.currentPlayer.selectedUnit.endNode = this;
            gameManager.currentPlayer.selectedUnit.CheckRoute();
        }
    }

    public void Select()
    {
        Selected = true;
        rend.material.color = OGColor - hoverColor;
    }

    public void Unselect()
    {
        Selected = false;
        rend.material.color = OGColor;
    }

    private void OnMouseExit()
    {
        Unselect();
    }

    public void GenerateNeighbors()
    {
        RaycastHit hit;

        Vector3 newPos;

        if(neighborNodes.Count == 6)
        {
            return;
        }

        if (!Physics.Raycast(transform.position, transform.TransformDirection(Vector3.left), out hit, 3.0f))
        {
            newPos= transform.position + new Vector3(-4.0f,0f,0f);
            generation.SpawnNewNeighbor(newPos, transform.rotation);
        }
        if (!Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out hit, 3.0f))
        {
            newPos = transform.position + new Vector3(4.0f, 0f, 0f);
            generation.SpawnNewNeighbor(newPos, transform.rotation);
        }
        if (!Physics.Raycast(transform.position, (transform.TransformDirection(Vector3.up) + transform.TransformDirection(Vector3.left)), out hit, 2.0f))
        {
            newPos = transform.position + new Vector3(-2.0f, 0f, 3.5f);
            generation.SpawnNewNeighbor(newPos, transform.rotation);
        }
        if (!Physics.Raycast(transform.position, (transform.TransformDirection(Vector3.up) + transform.TransformDirection(Vector3.right)), out hit, 2.0f))
        {
            newPos = transform.position + new Vector3(2.0f, 0f, 3.5f);
            generation.SpawnNewNeighbor(newPos, transform.rotation);
        }
        if (!Physics.Raycast(transform.position, (transform.TransformDirection(Vector3.down) + transform.TransformDirection(Vector3.left)), out hit, 2.0f))
        {
            newPos = transform.position + new Vector3(-2.0f, 0f, -3.5f);
            generation.SpawnNewNeighbor(newPos, transform.rotation);
        }
        if (!Physics.Raycast(transform.position, (transform.TransformDirection(Vector3.down) + transform.TransformDirection(Vector3.right)), out hit, 2.0f))
        {
            newPos = transform.position + new Vector3(2.0f, 0f, -3.5f);
            generation.SpawnNewNeighbor(newPos, transform.rotation);
        }
    }

    public void resetValues()
    {
        H = 0;
        D = 0;
        F = 0;
    }
}
