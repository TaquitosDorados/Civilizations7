using UnityEngine;

public class Node : MonoBehaviour
{
    public Color hoverColor;

    MeshRenderer rend;
    Color startColor;

    private void Start()
    {
        rend = GetComponent<MeshRenderer>();
        startColor = rend.material.color;
    }

    private void OnMouseDown()
    {
        Debug.Log("A");
    }

    private void OnMouseEnter()
    {
        rend.material.color = hoverColor;
    }

    private void OnMouseExit()
    {
        rend.material.color = startColor;
    }
}
