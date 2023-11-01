using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public float health, production, charges, range, damage, maxMovement, movementLeft;
    public Node startNode,currentNode, endNode;
    public List<Node> closedList;
    public List<Node> openedList;

    private bool found;

    private void Start()
    {
        movementLeft = maxMovement;
    }

    public void CheckRoute()
    {
        found= false;
        closedList= new List<Node>();
        openedList= new List<Node>();
        Method();
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
            currentNode.Select();
            currentNode = currentNode.parent;
        } while (currentNode != startNode);

        currentNode.Select();
    }

    public void Move()
    {
        currentNode = startNode;
        if(currentNode != endNode )
            StartCoroutine(MoveCoroutine());
    }

    IEnumerator MoveCoroutine()
    {
        Node nextNode = currentNode;
        foreach(Node neighbor in currentNode.neighborNodes)
        {
            if(neighbor.Selected)
                nextNode = neighbor;
        }

        while (nextNode.P <= movementLeft && nextNode != endNode)
        {
            movementLeft -= nextNode.P;
            transform.position = new Vector3(nextNode.transform.position.x, transform.position.y, nextNode.transform.position.z);

            currentNode = nextNode;

            currentNode.Unselect();

            foreach (Node neighbor in currentNode.neighborNodes)
            {
                if (neighbor.Selected)
                    nextNode = neighbor;
            }

            yield return new WaitForSeconds(0.2f);
        }

        if(nextNode.P <= movementLeft && nextNode == endNode)
        {
            movementLeft -= nextNode.P;
            transform.position = new Vector3(nextNode.transform.position.x, transform.position.y, nextNode.transform.position.z);
            currentNode = nextNode;
        }

        startNode = currentNode;
    }
}
