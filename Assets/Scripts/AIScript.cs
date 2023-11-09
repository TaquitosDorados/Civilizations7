using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIScript : MonoBehaviour
{
    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }
    public void startTurn()
    {
        StartCoroutine(startTurnCoroutine());
    }

    IEnumerator startTurnCoroutine()
    {
        yield return new WaitForSeconds(2f);
        gameManager.finishAITurn();
    }
}
