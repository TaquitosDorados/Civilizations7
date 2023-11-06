using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public Player currentPlayer, humanPlayer, AIPlayer;
    public delegate void NextTurn();
    public event NextTurn onNextTurn;
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if(onNextTurn!=null)
            onNextTurn();
        }
    }


}
