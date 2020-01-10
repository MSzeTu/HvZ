using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Human : Vehicle
{
    float closestDist=9999999;
    public GameObject zombie;
    //Uses Vehicles start and update methods

    //Inherited method is used to 
    public override void CalcSteeringForces()
    {
        Vector3 ultForce = new Vector3(0,0,0);
        if (Manager.zombieList.Count > 0) //Avoids the closest Zombie
        {
            foreach (GameObject enemy in Manager.zombieList)
            {
                float distance = Vector3.Distance(gameObject.transform.position, enemy.transform.position);
                if (distance < closestDist)
                {
                    closestDist = distance;
                    zombie = enemy;
                }
            }     
        }
        if (zombie == null)
        {
            return;
        }
        if (Vector3.Distance(gameObject.transform.position, zombie.transform.position) < 30f)
        {
            ultForce += Evade(zombie);
        }
        else
        {
            ultForce += Wander();
        }
        for (int i = 0; i < Manager.obstacles.Length; i++)
        {
            ApplyForce(AvoidObstacle(Manager.obstacles[i], 10)); //Dodges around objects
        }
        ApplyForce(Seperate(Manager.humanList));
        ultForce.Normalize();
        ultForce = ultForce * maxSpeed;
        ApplyForce(ultForce); //Applies the force
    }
}
