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
    public Player owner;
    public bool moving;

    private bool found;
    private GameManager gameManager;

    private void Start()
    {
        movementLeft = maxMovement;
        gameManager = FindObjectOfType<GameManager>();
        gameManager.onNextTurn += nextTurn;
    }

    private void nextTurn()
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

        int a = 0;

        while (!found && a<30)
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


            a = a+1;
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
        int a = 0;
        do
        {
            currentNode.Select();
            currentNode = currentNode.parent;
            a++;
        } while (currentNode != startNode && a<30);

        currentNode.Select();
    }

    public void Move()
    {
        if (moving)
            return;
        moving = true;
        currentNode = startNode;
        if (currentNode != endNode)
        {
            currentNode.Unselect();
            StartCoroutine(MoveCoroutine());
        }
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
        currentNode = null;
        endNode = null;
        moving = false;
    }

    private void OnMouseDown()
    {
        if(owner.selectedUnit == this)
        {
            owner.selectedUnit = null;
        } else
        {
            owner.selectedUnit = this;
        }
    }
}
