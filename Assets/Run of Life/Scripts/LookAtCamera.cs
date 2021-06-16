using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    private Transform cam = null;
    [SerializeField] private bool lockX = false;
    [SerializeField] private bool lockY = false;
    [SerializeField] private bool lockZ = false;

    private void Awake()
    {
        cam = Camera.main.transform;
        Vector3 direction = cam.position - transform.position;
        direction.Normalize();
        Quaternion rot = Quaternion.LookRotation(direction);
        Vector3 rotEuler = rot.eulerAngles;
        Vector3 oldRot = transform.rotation.eulerAngles;
        if (lockX)
            rotEuler.x = oldRot.x;
        if (lockZ)
            rotEuler.z = oldRot.z;
        transform.rotation = Quaternion.Euler(rotEuler);

    }

    void Update()
    {
        /*Vector3 direction = cam.position - transform.position;
        direction.Normalize();
        Quaternion rot = Quaternion.LookRotation(direction);
        Vector3 rotEuler = rot.eulerAngles;
        Vector3 oldRot = transform.rotation.eulerAngles;
        if (lockX)
            rotEuler.x = oldRot.x;
        if (lockY)
            rotEuler.y = oldRot.y;
        if (lockZ)
            rotEuler.z = oldRot.z;
        transform.rotation = Quaternion.Euler(rotEuler);*/
    }

}
