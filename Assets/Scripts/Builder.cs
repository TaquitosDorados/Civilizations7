using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : MonoBehaviour
{
    public GameObject a;

    private void Update()
    {
        if (Vector3.Distance(transform.position, a.transform.position) < 13)
        {
            Debug.Log("adsdasd");
        }
    }
}
