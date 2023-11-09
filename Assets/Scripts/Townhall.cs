using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Townhall : MonoBehaviour
{
    public bool isCapital;
    public Player owner;
    public bool onProduction;
    public GameObject producingUnit;
    public float totalProduction = 0;
    public Node node;

    public float productionLeft;

    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        gameManager.onNextTurn += getProduction;
    }

    private void OnMouseDown()
    {
        if (!onProduction)
        {
            owner.selectTownhall(this);
        }
    }

    public void produceUnit(GameObject newUnit)
    {
        producingUnit = newUnit;
        onProduction = true;
        productionLeft = producingUnit.GetComponent<Unit>().production;
    }

    public void getProduction()
    {
        totalProduction = 0;
        totalProduction+= node.gameObject.GetComponent<NodeState>().production;

        foreach(Node neighbor in node.neighborNodes)
        {
            totalProduction += neighbor.gameObject.GetComponent<NodeState>().production;
        }

        if (onProduction)
        {
            productionLeft -= totalProduction;
            if (productionLeft <= 0)
            {
                unitProduced();
            }
        }
    }

    private void unitProduced()
    {
        Debug.Log("Trying Build");
        foreach(Node neighbor in node.neighborNodes)
        {
            if (!neighbor.GetComponent<NodeState>().occupied)
            {
                //Vector3 newPos = new Vector3(neighbor.transform.position.x, producingUnit.transform.position.y, neighbor.transform.position.z);
                var newUnit = Instantiate(producingUnit);
                newUnit.transform.position = new Vector3(neighbor.transform.position.x, newUnit.transform.position.y, neighbor.transform.position.z);
                newUnit.GetComponent<Unit>().owner = owner;
                newUnit.GetComponent<Unit>().startNode = neighbor;

                neighbor.GetComponent<NodeState>().occupied = true;
                producingUnit = null;
                onProduction = false;
                break;
            }
        }
    }
}
