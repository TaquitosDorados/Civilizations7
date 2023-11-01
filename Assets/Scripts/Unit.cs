using System.Collections;
using System.Collections.Generic;
using System.Net;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public float health, production, charges, range, damage, movement;
    public Node startNode,currentNode, endNode;
    public List<Node> closedList;
    public List<Node> openedList;

    private bool found;

    public void CheckRoute()
    {

    }

    public void Method()
    {
        openedList.Add(startNode);
        currentNode = startNode;

        while (!found)
        {
            currentNode = openedList[0];

            foreach (Node node in openedList)
            {
                if (node.F < currentNode.F)
                {
                    currentNode = node;
                }
            }

            openedList.Remove(currentNode);
            closedList.Add(currentNode);

            if (currentNode == endNode)
            {
                found = true;
                Retrace();              
            }

            foreach (Node neighbor in currentNode.neighborNodes)
            {
                if (neighbor.P == 0 || closedList.Contains(neighbor))
                    continue;

                if (currentNode.D + 1 < neighbor.D || !openedList.Contains(neighbor))
                {
                    giveValuesToNode(neighbor);
                    neighbor.parent = currentNode;

                    if (!openedList.Contains(neighbor))
                    {
                        openedList.Add(neighbor);
                    }
                }


            }
        }
    }

    void giveValuesToNode(Node _node)
    {
        _node.D = currentNode.D + 1;
        _node.H = Mathf.Abs(_node.x - endNode.x) + Mathf.Abs(_node.y - endNode.y);
        _node.F = _node.D + _node.H + _node.P;
    }

    void Retrace()
    {
        do
        {
            currentNode.Selected();
            currentNode = currentNode.parent;
        } while (currentNode != startNode);

        currentNode.Selected();
    }
}
