using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cameras : MonoBehaviour
{

    //Camera array that holds a reference to every camera in the scene
    public Camera[] cameras;
    //Current Camera
    private int currentCameraIndex;
    // Start is called before the first frame update
    void Start()
    {
        currentCameraIndex = 0;

        //Turn all cameras off, except the default one
        for (int i = 1; i < cameras.Length; i++)
        {
            cameras[i].gameObject.SetActive(false);
        }
        //If any cameras added to controller, enable first one
        if (cameras.Length > 0)
        {
            cameras[0].gameObject.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //Press the 'C' key to cycle through the cameras in the array
        if (Input.GetKeyDown(KeyCode.C))
        {
            //Cycle to the next camera
            currentCameraIndex++;
            //If the cameraIndex is in bound, set this camera to active and the last one to inactive
            if (currentCameraIndex < cameras.Length)
            {
                cameras[currentCameraIndex - 1].gameObject.SetActive(false);
                cameras[currentCameraIndex].gameObject.SetActive(true);
            }
            //If last camera, cycle back to first
            else
            {
                cameras[currentCameraIndex - 1].gameObject.SetActive(false);
                currentCameraIndex = 0;
                cameras[currentCameraIndex].gameObject.SetActive(true);
            }
        }
        for (int i = 0; i < cameras.Length; i++)
        {
            if (i != currentCameraIndex)
            {
                cameras[i].enabled = false;
            }
            else
            {
                cameras[i].enabled = true;
            }
        }
    }

}
