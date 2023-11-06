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

    
    private void OnMouseDown()
    {
        //DEBUG
        getProduction();
        if (!onProduction)
        {
            owner.selectTownhall(this);
        }
    }

    public void produceUnit(GameObject newUnit)
    {
        producingUnit = newUnit;
        onProduction = true;


    }

    public void getProduction()
    {
        totalProduction = 0;
        totalProduction+= node.gameObject.GetComponent<NodeState>().production;

        foreach(Node neighbor in node.neighborNodes)
        {
            totalProduction += neighbor.gameObject.GetComponent<NodeState>().production;
        }
    }
}
