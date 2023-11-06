using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Unit selectedUnit;
    public bool isCPU;
    public Townhall selectedTownhall;
    public List<Townhall> townhalls;
    public GameObject THUI;
    private GameManager gameManager;

    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        gameManager.onNextTurn += nextTurn;
    }

    private void nextTurn()
    {
        selectedTownhall = null;
        selectedUnit = null;
    }
    public void selectTownhall(Townhall th)
    {
        selectedTownhall = th;
        if (!isCPU)
        {
            THUI.SetActive(true);
        }
    }

    public void produceUnit(GameObject newUnit)
    {
        if (selectedTownhall != null)
        {
            selectedTownhall.produceUnit(newUnit);
            selectedTownhall = null;
        }

        if (!isCPU)
        {
            THUI.SetActive(false);
        }
    }

}
