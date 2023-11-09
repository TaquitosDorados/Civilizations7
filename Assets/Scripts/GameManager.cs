using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public Player currentPlayer, humanPlayer, AIPlayer;
    public delegate void NextTurn();
    public event NextTurn onNextTurn;
    public GameObject Builder, Warrior, NextTurnUI;
    public Text resultText;

    private AIScript ai;

    private void Start()
    {
        ai = FindObjectOfType<AIScript>();
    }

    public void finishTurn()
    {
        ai.startTurn();
    }

    public void finishAITurn()
    {
        if (onNextTurn != null)
            onNextTurn();
        NextTurnUI.SetActive(true);
    }

    public void findSpots()
    {
        Camera camera = GetComponent<CameraHandler>().camera;

        Node[] nodes = FindObjectsOfType<Node>();

        int rand = Random.Range(0, nodes.Length);

        Vector3 newPos = new Vector3(nodes[rand].transform.position.x, Builder.transform.position.y, nodes[rand].transform.position.z);
        var playerBuilder = Instantiate(Builder, newPos, Builder.transform.rotation);
        playerBuilder.GetComponent<Unit>().owner = humanPlayer;
        playerBuilder.GetComponent<Unit>().startNode = nodes[rand];

        nodes[rand].GetComponent<NodeState>().occupied = true;

        camera.transform.position = new Vector3(nodes[rand].transform.position.x, camera.transform.position.y, nodes[rand].transform.position.z - 10f);

        foreach (Node neighbor in nodes[rand].neighborNodes)
        {
            if (!neighbor.GetComponent<NodeState>().occupied)
            {
                newPos = new Vector3(neighbor.transform.position.x, Warrior.transform.position.y, neighbor.transform.position.z);

                var playerWarrior = Instantiate(Warrior, newPos, Warrior.transform.rotation);
                playerWarrior.GetComponent<Unit>().owner = humanPlayer;
                playerWarrior.GetComponent<Unit>().startNode = neighbor;

                neighbor.GetComponent<NodeState>().occupied = true;

                break;
            }
        }

        Node helperNode = nodes[rand];
        nodes[rand] = nodes[nodes.Length - 1];
        nodes[nodes.Length - 1] = helperNode;

        //IA
        rand = Random.Range(0, nodes.Length - 1);


        newPos = new Vector3(nodes[rand].transform.position.x, Builder.transform.position.y, nodes[rand].transform.position.z);
        var AIBuilder = Instantiate(Builder, newPos, Builder.transform.rotation);
        AIBuilder.GetComponent<Unit>().owner = AIPlayer;
        AIBuilder.GetComponent<Unit>().startNode = nodes[rand];

        nodes[rand].GetComponent<NodeState>().occupied = true;

        foreach (Node neighbor in nodes[rand].neighborNodes)
        {
            if (!neighbor.GetComponent<NodeState>().occupied)
            {
                newPos = new Vector3(neighbor.transform.position.x, Warrior.transform.position.y, neighbor.transform.position.z);

                var AIWarrior = Instantiate(Warrior, newPos, Warrior.transform.rotation);
                AIWarrior.GetComponent<Unit>().owner = AIPlayer;
                AIWarrior.GetComponent<Unit>().startNode = neighbor;

                neighbor.GetComponent<NodeState>().occupied = true;

                break;
            }
        }

        NextTurnUI.SetActive(true);
    }

    public void gameFinish(Player loser)
    {
        if(loser == humanPlayer)
        {
            resultText.text = "DEFEAT";
        } else
        {
            resultText.text = "VICTORY";
        }
        resultText.gameObject.SetActive(true);
    }
}
