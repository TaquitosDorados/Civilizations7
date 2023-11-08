using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public int productionGenerated = 0;
    public Player owner;
    public float Health;
    public NodeState node;
    public bool isTH;

    public void addProductionToNode()
    {
        node.production += productionGenerated;
        node.building = this;
    }
}
