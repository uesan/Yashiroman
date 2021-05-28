using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Camera))]
public class CameraMove : MonoBehaviour
{
    public GameObject playerObj;

    void LateUpdate()
    {
        MoveCamera();
    }
    void MoveCamera()
    {
        //横方向だけ追従
        transform.position = new Vector3(playerObj.transform.position.x, transform.position.y, transform.position.z);
    }
}