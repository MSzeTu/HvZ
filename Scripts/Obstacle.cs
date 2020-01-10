using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public SphereCollider information;
    public float radius;
    public Vector3 center;
    // Start is called before the first frame update
    void Start()
    {
        information = gameObject.GetComponent<SphereCollider>();
        radius = information.radius;
        center = GetComponent<Renderer>().bounds.center;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
