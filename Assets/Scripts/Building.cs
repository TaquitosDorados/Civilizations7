using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Building : MonoBehaviour
{
    public int productionGenerated = 0;
    public Player owner;
    public float health;
    public NodeState node;
    public bool isTH;

    private GameManager GameManager;

    private void Start()
    {
        GameManager = FindObjectOfType<GameManager>();
    }

    public void addProductionToNode()
    {
        node.production += productionGenerated;
        node.building = this;
    }

    public void receiveDamage(float damage)
    {
        health -= damage;

        if (health <= 0)
        {
            if (isTH)
            {
                if (GetComponent<Townhall>().isCapital)
                {
                    GameManager.gameFinish(owner);
                }
                else
                {
                    owner.townhalls.Remove(GetComponent<Townhall>());
                    node.building = null;
                }
            }
            Destroy(gameObject);
        }
    }
}
