﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraClipping : MonoBehaviour
{
    [HideInInspector] public List<MeshRenderer> listobj;

    private void Update()
    {
        RayCastSeeThrough();
    }

    void RayCastSeeThrough()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.gameObject.transform.position,
            Camera.main.gameObject.transform.forward, out hit, 6) &&//Needs to be adjustable not have 6 as its parameter.
            !hit.collider.gameObject.CompareTag("Player"))
        {
            //Saves memory space as there will be less variables to check through a list.
            MeshRenderer objectMesh = hit.transform.gameObject.GetComponent<MeshRenderer>();
            if (objectMesh.enabled == true)//Because this is being called in update, it is always being called.
            {
                AddToList(objectMesh);
            }

        }
        else if (Physics.Raycast(Camera.main.gameObject.transform.position,
            Camera.main.gameObject.transform.forward, out hit) &&
            hit.collider.gameObject.CompareTag("Player"))
        {
            SetBack();
        }

    }
    void SetBack()//Setting the meshRenderer back to active
    {
        for (int i = 0; i < listobj.Count; i++)
        {
            var value = listobj[i].GetComponent<MeshRenderer>();
            value.enabled = true;
        }
    }
    void AddToList(MeshRenderer obj)//Setting the mesh renderer back to inactive.
    {
        obj.enabled = false;
        listobj.Add(obj);
    }
}
