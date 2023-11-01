using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateMap : MonoBehaviour
{
    public int numOfGenerations = 1;
    public GameObject hexagon;

    private List<Vector3> usedPos;

    public void SpawnNewNeighbor(Vector3 pos, Quaternion quaternion)
    {
            if (usedPos.Contains(pos))
            {
                return;
            }
        
            GameObject newHex = Instantiate(hexagon, pos, quaternion);
            usedPos.Add(pos);
        
    }

    private void Start()
    {
        StartCoroutine(Generate());
        usedPos = new List<Vector3>();
        usedPos.Add(Vector3.zero);
    }

    IEnumerator Generate()
    {
        yield return new WaitForSeconds(2);

        for(int i = 0; i<numOfGenerations; i++)
        {
            Node[] nodes = FindObjectsOfType<Node>();
            for (int j = 0; j < nodes.Length; j++)
            {
                nodes[j].GenerateNeighbors();
            }
            for (int j = 0; j < nodes.Length; j++)
            {
                nodes[j].FindNeighbors();
            }
            yield return new WaitForSeconds(.2f);
        }
    }
}
