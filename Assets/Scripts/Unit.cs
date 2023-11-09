using System.Collections;
using System.Collections.Generic;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    public float health, maxHealth, production, charges, range, damage, maxMovement, movementLeft;
    public Node startNode,currentNode, endNode;
    public List<Node> closedList;
    public List<Node> openedList;
    public Player owner;
    public bool moving;
    public bool isBuilder;

    private bool found;
    private GameManager gameManager;

    private void Start()
    {
        health = maxHealth;
        movementLeft = maxMovement;
        gameManager = FindObjectOfType<GameManager>();
        gameManager.onNextTurn += nextTurn;
    }

    private void nextTurn()
    {
        if (movementLeft == maxMovement)
        {
            health += 2;
            if (health > maxHealth)
            {
                health = maxHealth;
            }
        }
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
        startNode.D = 0;
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

            if (currentNode == null)
            {
                return;
            }

            foreach (Node neighbor in currentNode.neighborNodes)
            {
                if (neighbor.P == 0 || closedList.Contains(neighbor) || neighbor.GetComponent<NodeState>().occupied)
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
            if(currentNode==null)
            { return; }
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
        startNode.GetComponent<NodeState>().occupied = false;

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
        startNode.GetComponent<NodeState>().occupied = true;
        startNode.D = 0;
        currentNode = null;
        endNode = null;
        moving = false;
    }

    private void OnMouseDown()
    {
        GameObject.Find("Healthbar").GetComponent<Slider>().value = health / maxHealth;

        if (!owner.isCPU)
        {

            if (owner.selectedUnit == this)
            {
                owner.selectedUnit = null;
            }
            else
            {
                owner.selectedUnit = this;
            }
        } else
        {
            if (gameManager.humanPlayer.selectedUnit != null)
            {
                gameManager.humanPlayer.selectedUnit.tryAttackOnUnit(this);
            }
        }
    }

    public void receiveDamage(float damage)
    {
        Debug.Log("attacked");
        health -= damage;

        if (health <= 0)
        {
            if (owner.selectedUnit == this)
                owner.selectedUnit = null;
            Destroy(gameObject);
        }
    }

    public void tryAttackOnUnit(Unit attackedUnit)
    {
        if (movementLeft < 1)
            return;
        Debug.Log("trying attack");
        startNode.D = 0;
        found = false;
        closedList = new List<Node>();
        openedList = new List<Node>();
        endNode = attackedUnit.startNode;
        openedList.Add(startNode);
        currentNode = startNode;

        int a = 0;

        while (!found && a < 30)
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


            a = a + 1;
        }

        if (endNode.D <= range)
        {
            movementLeft = 0;
            float damageToReceive = attackedUnit.damage / 2;
            attackedUnit.receiveDamage(damage);
            if(range == 1)
                receiveDamage(damageToReceive);
        }

        GameObject.Find("Healthbar").GetComponent<Slider>().value = health / maxHealth;
    }

    public void tryAttackOnBuilding(Building attackedBuilding, Node _node)
    {
        if (movementLeft < 1)
            return;

        Debug.Log("trying attack");
        startNode.D = 0;
        found = false;
        closedList = new List<Node>();
        openedList = new List<Node>();
        endNode = _node;
        openedList.Add(startNode);
        currentNode = startNode;

        int a = 0;

        while (!found && a < 30)
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


            a = a + 1;
        }

        if (endNode.D <= range)
        {
            movementLeft = 0;
            attackedBuilding.receiveDamage(damage);
            if (range == 1 && attackedBuilding.isTH)
                receiveDamage(5);
        }
    }
}
