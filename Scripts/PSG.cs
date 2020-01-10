using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSG : MonoBehaviour
{
    int x;
    int z;
    RaycastHit hit;
    Vector3 position;
    // Start is called before the first frame update
    void Start()
    {
        x = Random.Range(-100, 100);
        z = Random.Range(-100, 100);
        Physics.Raycast(new Vector3(x, 100, z), Vector3.down, out hit, Mathf.Infinity);
        position = hit.point;
        position.y += gameObject.GetComponent<SphereCollider>().radius;
        gameObject.transform.position = position;
    }

    // Update is called once per frame
    void Update()
    {
        foreach (GameObject human in Manager.humanList)
        {
            if (Vector3.Distance(human.transform.position, position) < 2)
            {
                x = Random.Range(-100, 100);
                z = Random.Range(-100, 100);
                Physics.Raycast(new Vector3(x, 100, z), Vector3.down, out hit, Mathf.Infinity);
                position = hit.point;
                position.y += gameObject.GetComponent<SphereCollider>().radius;
                gameObject.transform.position = position;
            }
        }
    }
}
