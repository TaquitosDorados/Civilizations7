using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Player currentPlayer, humanPlayer, AIPlayer;
    public delegate void NextTurn();
    public event NextTurn onNextTurn;
    public GameObject Builder, Warrior;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(onNextTurn!=null)
            onNextTurn();
        }
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
                playerWarrior.GetComponent<Unit>().startNode = nodes[rand];

                neighbor.GetComponent<NodeState>().occupied = true;

                break;
            }
        }
                
    }


}
