using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIScript : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject builderPrefab;
    public GameObject warriorPrefab, archerPrefab, THPrefab, minePrefab, farmPrefab;
    [SerializeField]
    private List<Unit> MilitaryUnits;
    [SerializeField]
    private List<Unit> Builders;
    [SerializeField]
    private List<Unit> EnemyUnits;
    [SerializeField]
    private int turn = 0;
    private GameManager gameManager;
    private Player AIPlayer;

    private void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
        AIPlayer = GetComponent<Player>();
    }
    public void startTurn()
    {
        turn++;
        findMyUnits();
        StartCoroutine(startTurnCoroutine());
    }

    private void findMyUnits()
    {
        MilitaryUnits = new List<Unit>();
        Builders = new List<Unit>();
        EnemyUnits = new List<Unit>();

        Unit[] allUnits = FindObjectsOfType<Unit>();

        foreach(Unit unit in allUnits)
        {
            if(unit.owner == AIPlayer)
            {
                if (unit.isBuilder)
                {
                    Builders.Add(unit);
                } else
                {
                    MilitaryUnits.Add(unit);
                }
            } else
            {
                EnemyUnits.Add(unit);
            }
        }
    }

    IEnumerator startTurnCoroutine()
    {
        if (turn == 1)
        {
            //Construir TH
            AIPlayer.selectedUnit = Builders[0];
            AIPlayer.buildHere(THPrefab);

            //Producir en TH's
            foreach (Townhall townhall in AIPlayer.townhalls)
            {
                AIPlayer.selectedTownhall = townhall;

                if (!townhall.onProduction)
                {
                    if (Builders.Count < 2)
                    {
                        AIPlayer.produceUnit(builderPrefab);
                    }
                    else
                    {
                        if (Random.Range(0f, 1f) < 0.5)
                        {
                            AIPlayer.produceUnit(archerPrefab);
                        }
                        else
                        {
                            AIPlayer.produceUnit(warriorPrefab);
                        }
                    }
                }
            }

            //Mover Unidades
            foreach (Unit unit in MilitaryUnits)
            {
                yield return new WaitForSeconds(1f);
                StartCoroutine(choosePasive(unit));
            }
        } else if (turn < 20)
        {
            //Constructores
            foreach (Unit builder in Builders)
            {
                bool success = false;
                yield return new WaitForSeconds(1f);
                AIPlayer.selectedUnit = builder;
                foreach (Townhall th in AIPlayer.townhalls)
                {
                    foreach (Node neighbor in th.node.neighborNodes)
                    {
                        if (!neighbor.GetComponent<NodeState>().occupied && neighbor.GetComponent<NodeState>().building == null && (neighbor.P == 1 || neighbor.P == 3))
                        {
                            neighbor.selectNodeToMethod();
                            yield return new WaitForSeconds(.2f);
                            neighbor.MoveUnitHere();
                            yield return new WaitForSeconds(1f);
                            if (AIPlayer.selectedUnit.startNode == neighbor)
                            {
                                tryToBuild();
                            }
                            success = true;
                            break;
                        }
                    }
                    if (success)
                        break;
                }
                //NO HAY ESPACIO PARA CONSTRUCCIONES
                if (!success && AIPlayer.selectedUnit.charges==3)
                {

                    foreach (Townhall th in AIPlayer.townhalls)
                    {
                        foreach (Node neighbor in th.node.neighborNodes)
                        {
                            foreach(Node neighborOfNeighbor in neighbor.neighborNodes)
                            {
                                if(!neighborOfNeighbor.GetComponent<NodeState>().occupied 
                                    && !th.node.neighborNodes.Contains(neighborOfNeighbor)
                                    && neighbor.GetComponent<NodeState>().building == null)
                                {
                                    neighborOfNeighbor.selectNodeToMethod();
                                    yield return new WaitForSeconds(.2f);
                                    neighborOfNeighbor.MoveUnitHere();

                                    if (AIPlayer.selectedUnit.startNode == neighborOfNeighbor)
                                    {
                                        tryToBuildTH();
                                    }

                                    success = true;
                                    break;
                                }
                            }
                            if (success)
                                break;
                        }
                        if (success)
                            break;
                    }
                }
            }

            //Producir en TH's
            foreach (Townhall townhall in AIPlayer.townhalls)
            {
                AIPlayer.selectedTownhall = townhall;

                if (!townhall.onProduction)
                {
                    if (Builders.Count < 2)
                    {
                        AIPlayer.produceUnit(builderPrefab);
                    }
                    else
                    {
                        float rand = Random.Range(0f, 1f);
                        if ( rand < 0.3f)
                        {
                            AIPlayer.produceUnit(archerPrefab);
                        }
                        else if(rand < 0.6f)
                        {
                            AIPlayer.produceUnit(warriorPrefab);
                        } else 
                        {
                            AIPlayer.produceUnit(builderPrefab);
                        }
                    }
                }
            }

            //Mover Unidades
            foreach (Unit unit in MilitaryUnits)
            {
                yield return new WaitForSeconds(1f);
                StartCoroutine(choosePasive(unit));
            }
        } else
        {
            //Constructores
            foreach (Unit builder in Builders)
            {
                bool success = false;
                yield return new WaitForSeconds(1f);
                AIPlayer.selectedUnit = builder;
                foreach (Townhall th in AIPlayer.townhalls)
                {
                    foreach (Node neighbor in th.node.neighborNodes)
                    {
                        if (!neighbor.GetComponent<NodeState>().occupied && neighbor.GetComponent<NodeState>().building == null && (neighbor.P == 1 || neighbor.P == 3))
                        {
                            neighbor.selectNodeToMethod();
                            yield return new WaitForSeconds(.2f);
                            neighbor.MoveUnitHere();
                            yield return new WaitForSeconds(1f);
                            if (AIPlayer.selectedUnit.startNode == neighbor)
                            {
                                tryToBuild();
                            }
                            success = true;
                            break;
                        }
                    }
                    if (success)
                        break;
                }
                //NO HAY ESPACIO PARA CONSTRUCCIONES
                if (!success && AIPlayer.selectedUnit.charges == 3)
                {

                    foreach (Townhall th in AIPlayer.townhalls)
                    {
                        foreach (Node neighbor in th.node.neighborNodes)
                        {
                            foreach (Node neighborOfNeighbor in neighbor.neighborNodes)
                            {
                                if (!neighborOfNeighbor.GetComponent<NodeState>().occupied
                                    && !th.node.neighborNodes.Contains(neighborOfNeighbor)
                                    && neighbor.GetComponent<NodeState>().building == null)
                                {
                                    neighborOfNeighbor.selectNodeToMethod();
                                    yield return new WaitForSeconds(.2f);
                                    neighborOfNeighbor.MoveUnitHere();

                                    if (AIPlayer.selectedUnit.startNode == neighborOfNeighbor)
                                    {
                                        tryToBuildTH();
                                    }

                                    success = true;
                                    break;
                                }
                            }
                            if (success)
                                break;
                        }
                        if (success)
                            break;
                    }
                }
            }

            //Producir en TH's
            foreach (Townhall townhall in AIPlayer.townhalls)
            {
                AIPlayer.selectedTownhall = townhall;

                if (!townhall.onProduction)
                {
                    if (Builders.Count < 1)
                    {
                        AIPlayer.produceUnit(builderPrefab);
                    }
                    else
                    {
                        float rand = Random.Range(0f, 1f);
                        if (rand < 0.4f)
                        {
                            AIPlayer.produceUnit(archerPrefab);
                        }
                        else if (rand < 0.8f)
                        {
                            AIPlayer.produceUnit(warriorPrefab);
                        }
                        else
                        {
                            AIPlayer.produceUnit(builderPrefab);
                        }
                    }
                }
            }

            //Mover Unidades
            foreach (Unit unit in MilitaryUnits)
            {
                yield return new WaitForSeconds(1f);
                StartCoroutine(ChooseAggro(unit));
            }
        }
        yield return new WaitForSeconds(2f);
        gameManager.finishAITurn();
    }

    IEnumerator choosePasive(Unit _unit)
    {
        AIPlayer.selectedUnit = _unit;

        Unit closestEnemy = EnemyUnits[0];

        foreach(Unit enemy in EnemyUnits)
        {
            if(Vector3.Distance(_unit.transform.position, enemy.transform.position) < Vector3.Distance(_unit.transform.position, closestEnemy.transform.position))
            {
                closestEnemy = enemy;
            }
        }

        if(Vector3.Distance(_unit.transform.position, closestEnemy.transform.position) < 13 && _unit.health/_unit.maxHealth >= closestEnemy.health / closestEnemy.maxHealth)
        {
            //ATTACK UNIT
            Debug.Log("Attack!");
            foreach(Node neighbor in closestEnemy.startNode.neighborNodes)
            {
                if(!neighbor.GetComponent<NodeState>().occupied)
                {
                    if(neighbor.GetComponent<NodeState>().building != null)
                    {
                        if (neighbor.GetComponent<NodeState>().building.owner != AIPlayer)
                        {
                            foreach (Node neighborBuild in neighbor.neighborNodes)
                            {
                                if (!neighborBuild.GetComponent<NodeState>().occupied && neighborBuild.GetComponent<NodeState>().building == null)
                                {
                                    neighborBuild.selectNodeToMethod();
                                    yield return new WaitForSeconds(.2f);
                                    neighborBuild.MoveUnitHere();
                                    yield return new WaitForSeconds(1f);
                                    neighbor.MoveUnitHere();
                                    yield break;
                                }
                            }
                        }
                    } else
                    {
                        Debug.Log("No Obstacles, attack");
                        neighbor.selectNodeToMethod();
                        yield return new WaitForSeconds(.2f);
                        neighbor.MoveUnitHere();
                        yield return new WaitForSeconds(1f);
                        closestEnemy.startNode.MoveUnitHere();
                        yield break;
                    }
                }
            }
        } else
        {
            StartCoroutine(moveUnitToRandomLocation(_unit));
        }
    }

    IEnumerator ChooseAggro(Unit _unit)
    {
        AIPlayer.selectedUnit = _unit;

        Unit closestEnemy = EnemyUnits[0];

        foreach (Unit enemy in EnemyUnits)
        {
            if (Vector3.Distance(_unit.transform.position, enemy.transform.position) < Vector3.Distance(_unit.transform.position, closestEnemy.transform.position))
            {
                closestEnemy = enemy;
            }
        }
        Building[] allBuilds = FindObjectsOfType<Building>();
        List<Building> enemyBuilds = new List<Building>();
        foreach(Building build in allBuilds)
        {
            if (build.owner != AIPlayer)
                enemyBuilds.Add(build);
        }

        Building closestEnemyBuilding = enemyBuilds[0];

        foreach(Building build in enemyBuilds)
        {
            if (Vector3.Distance(_unit.transform.position, build.transform.position) < Vector3.Distance(_unit.transform.position, closestEnemyBuilding.transform.position))
            {
                closestEnemyBuilding = build;
            }
        }

        if(Vector3.Distance(_unit.transform.position, closestEnemy.transform.position) < Vector3.Distance(_unit.transform.position, closestEnemyBuilding.transform.position))
        {
            if(_unit.health/_unit.maxHealth >= closestEnemy.health / closestEnemy.maxHealth)
            {
                //Atacar Unidad
                foreach (Node neighbor in closestEnemy.startNode.neighborNodes)
                {
                    if (!neighbor.GetComponent<NodeState>().occupied)
                    {
                        neighbor.selectNodeToMethod();
                        yield return new WaitForSeconds(.2f);
                        neighbor.MoveUnitHere();
                        yield return new WaitForSeconds(1f);
                        closestEnemy.startNode.MoveUnitHere();
                        yield break;
                    }
                }
            }
            else
            {
                //Atacar Build
                foreach (Node neighbor in closestEnemyBuilding.node.GetComponent<Node>().neighborNodes)
                {
                    if (!neighbor.GetComponent<NodeState>().occupied)
                    {
                        neighbor.selectNodeToMethod();
                        yield return new WaitForSeconds(.2f);
                        neighbor.MoveUnitHere();
                        yield return new WaitForSeconds(1f);
                        closestEnemyBuilding.node.GetComponent<Node>().MoveUnitHere();
                        yield break;
                    }
                }
            }
        } else
        {
            //Atacar Build
            foreach(Node neighbor in closestEnemyBuilding.node.GetComponent<Node>().neighborNodes)
            {
                if (!neighbor.GetComponent<NodeState>().occupied)
                {
                    neighbor.selectNodeToMethod();
                    yield return new WaitForSeconds(.2f);
                    neighbor.MoveUnitHere();
                    yield return new WaitForSeconds(1f);
                    closestEnemyBuilding.node.GetComponent<Node>().MoveUnitHere();
                    yield break;
                }
            }
        }
    }

    IEnumerator moveUnitToRandomLocation(Unit _unit)
    {
        AIPlayer.selectedUnit = _unit;

        Node[] nodes = FindObjectsOfType<Node>();

        int rand;

        do
        {
            rand = Random.Range(0, nodes.Length);
        } while (nodes[rand].GetComponent<NodeState>().occupied);

        if (nodes[rand].GetComponent<NodeState>().building != null)
        {
            if (nodes[rand].GetComponent<NodeState>().building.owner != AIPlayer)
            {
                foreach (Node neighbor in nodes[rand].neighborNodes)
                {
                    if (!neighbor.GetComponent<NodeState>().occupied && neighbor.GetComponent<NodeState>().building == null)
                    {
                        neighbor.selectNodeToMethod();
                        yield return new WaitForSeconds(.2f);
                        neighbor.MoveUnitHere();
                        nodes[rand].MoveUnitHere();
                        break;
                    }
                }
            } else
            {
                nodes[rand].selectNodeToMethod();
                yield return new WaitForSeconds(.2f);
                nodes[rand].MoveUnitHere();
            }
        }
        else
        {
            nodes[rand].selectNodeToMethod();
            yield return new WaitForSeconds(.2f);
            nodes[rand].MoveUnitHere();
        }
    }

    private void tryToBuild()
    {
        if(AIPlayer.selectedUnit.startNode.P == 1)
        {
            AIPlayer.buildHere(farmPrefab);
        } else if(AIPlayer.selectedUnit.startNode.P == 3)
        {
            AIPlayer.buildHere(minePrefab);
        }
    }

    private void tryToBuildTH()
    {
        AIPlayer.buildHere(THPrefab);
    }
}

/* LA AI EMPIEZA PASIVA EN LOS PRIMEROS 10 TURNOS, TIENE QUE TENER MINIMO DOS CONSTRUCTORES, LOS CUALES PRIMERO LLENARAN 
 * CADA ESPACIO ALREDEDOR DE LOS TH, SINO HARAN OTRO TH.
 * EN SU MODO PASIVO SOLO PATRULLARA, PERO DETECTA ENEMIGOS CERCA Y LOS ATACA SI TIENEN MAS % DE VIDA QUE ELLOS
 * DESPUES DE LOS 10 TURNOS, ENTRA EN ACCION Y ATACA EN LA SIGUIENTE PRIORITIZACION:
 * UNIDAD ENEMIGA CERCANA, ESTRUCTURA ENEMIGA CERCANA
 */
