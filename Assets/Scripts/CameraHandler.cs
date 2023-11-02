using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    public Camera camera;
    public float moveAmout, edgeSize;

    private void Update()
    {
        if(Input.mousePosition.x > Screen.width - edgeSize)
        {
            camera.transform.position = new Vector3(camera.transform.position.x + moveAmout * Time.deltaTime, camera.transform.position.y, camera.transform.position.z);
        }
        if (Input.mousePosition.x < edgeSize)
        {
            camera.transform.position = new Vector3(camera.transform.position.x - moveAmout * Time.deltaTime, camera.transform.position.y, camera.transform.position.z);
        }
        if (Input.mousePosition.y > Screen.height - edgeSize)
        {
            camera.transform.position = new Vector3(camera.transform.position.x, camera.transform.position.y, camera.transform.position.z + moveAmout * Time.deltaTime);
        }
        if (Input.mousePosition.y < edgeSize)
        {
            camera.transform.position = new Vector3(camera.transform.position.x, camera.transform.position.y, camera.transform.position.z - moveAmout * Time.deltaTime);
        }
    }
}

