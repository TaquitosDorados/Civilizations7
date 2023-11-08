using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public Unit selectedUnit;
    public bool isCPU;
    public Townhall selectedTownhall;
    public List<Townhall> townhalls;
    private GameManager gameManager;
    [Header("UI ELEMENTS")]
    public GameObject THUI;
    public GameObject BuilderUI;
    public Text chargesTxt;
    public GameObject btnCity;
    public GameObject btnMine;
    public GameObject btnFarm;

    private void Start()
    {
        gameManager = FindAnyObjectByType<GameManager>();
        gameManager.onNextTurn += nextTurn;
        townhalls= new List<Townhall>();
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

    private void Update()
    {
        if (selectedUnit == null)
        {
            if(BuilderUI!= null)
                BuilderUI.SetActive(false);
            return;
        }
            

        if (selectedUnit.isBuilder && selectedUnit.startNode.GetComponent<NodeState>().building == null)
        {
            chargesTxt.text = "Charges: " + selectedUnit.charges;
            if (selectedUnit.charges >= 3)
            {
                btnCity.SetActive(true);
            } else
            {
                btnCity.SetActive(false);
            }

            if(selectedUnit.startNode.P == 1)
            {
                btnFarm.SetActive(true);
            } else
            {
                btnFarm.SetActive(false);
            }

            if (selectedUnit.startNode.P == 3)
            {
                btnMine.SetActive(true);
            } else
            {
                btnMine.SetActive(false);
            }

            BuilderUI.SetActive(true);
        }
        else
        {
            BuilderUI.SetActive(false);
        }
    }

    public void buildHere(GameObject building)
    {
        Vector3 newPos = new Vector3(selectedUnit.startNode.transform.position.x, building.transform.position.y, selectedUnit.startNode.transform.position.z);

        var newBuilding = Instantiate(building, newPos, building.transform.rotation);
        newBuilding.GetComponent<Building>().owner = this;
        newBuilding.GetComponent<Building>().node = selectedUnit.startNode.GetComponent<NodeState>();
        newBuilding.GetComponent<Building>().addProductionToNode();

        selectedUnit.startNode.GetComponent<NodeState>().building = newBuilding.GetComponent<Building>();

        if (building.GetComponent<Building>().isTH)
        {
            newBuilding.GetComponent<Townhall>().owner = this;
            newBuilding.GetComponent<Townhall>().node = selectedUnit.startNode;
            townhalls.Add(newBuilding.GetComponent<Townhall>());
            townhalls[0].isCapital = true;
            Destroy(selectedUnit.gameObject);
            selectedUnit = null;
        } else
        {
            if (selectedUnit.charges - 1 <= 0)
            {
                Destroy(selectedUnit.gameObject);
                selectedUnit = null;
            } else
            {
                selectedUnit.charges--;
            }
        }
    }


}
