using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    public Camera cam;
    public Transform target;
    private int camPosInd;
    public Vector3[] lockedCameraPositions;
    void Start()
    {
        cam = GetComponent<Camera>();
        camPosInd = 0;

}
    void Update()   
    {
        CameraMovement();
    }
    private void CameraMovement()
    {
        if (target.position.y > lockedCameraPositions[camPosInd].y + cam.orthographicSize && camPosInd < lockedCameraPositions.Length - 1)
        {
            transform.position = lockedCameraPositions[camPosInd + 1];  //assuming that camera size is 10, -10 to take the bottom camera edge, +2 due to index counting from 0                                                                  
            camPosInd++;
        }
        if (target.position.y < lockedCameraPositions[camPosInd].y - cam.orthographicSize && camPosInd > 0)
        {
            transform.position = lockedCameraPositions[camPosInd - 1];
            camPosInd--;
        }
    }

}
