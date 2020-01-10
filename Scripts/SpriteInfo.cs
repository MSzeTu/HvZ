using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteInfo : MonoBehaviour
{

    public float maxX;
    public float maxZ;
    public float minX;
    public float minZ;
    public float halfSize;
    BoxCollider box;
    public float radius;
    public Vector3 center;
    // Start is called before the first frame update
    void Start()
    {
        box = gameObject.GetComponent<BoxCollider>();
        halfSize = box.size.y / 2;    
    }

    // Update is called once per frame
    void Update()
    {
        center = gameObject.GetComponent<Renderer>().bounds.center;
        maxX = box.bounds.max.x;
        minX = box.bounds.min.x;
        maxZ = box.bounds.max.z;
        minZ = box.bounds.min.z;
        radius = Mathf.Sqrt(Mathf.Pow(maxX - center.x, 2) + Mathf.Pow((0), 2));
    }
}
