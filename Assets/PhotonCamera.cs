using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class PhotonCamera : MonoBehaviour
{
    CinemachineFreeLook cam;

    private void Start()
    {
        cam = GetComponent<CinemachineFreeLook>();
    }

    public void setTarget(GameObject target)
    {
        cam.Follow = target.transform;
        cam.LookAt = target.transform;
    }
}
