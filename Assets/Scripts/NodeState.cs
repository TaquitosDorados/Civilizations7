using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeState : MonoBehaviour
{
    public int production;
    public bool occupied;
    public Building building;

    private void Start()
    {
        production = Random.Range(0, 4);
    }
}
