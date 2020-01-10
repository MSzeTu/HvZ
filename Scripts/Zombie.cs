using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Vehicle
{
    float closestDist=9999999;
    public GameObject human;
    //Uses Vehicles start and update methods

    public override void CalcSteeringForces()
    {
        if (human == null)
        {
            closestDist = 999999;
        }
        Vector3 ultForce = new Vector3(0, 0, 0);
        if (Manager.humanList.Count > 0)
        {
            foreach (GameObject target in Manager.humanList)
            {
                float distance = Vector3.Distance(gameObject.transform.position, target.transform.position);
                if (distance < closestDist) //Comapares distances to find the closest human
                {
                    closestDist = distance;
                    human = target;
                }
            }
            if (human != null) //Pursues the target
            {
                ultForce += Pursue(human);
            }
            for (int i = 0; i < Manager.obstacles.Length; i++) //Avoids obstacles
            {
                 ApplyForce(AvoidObstacle(Manager.obstacles[i], 20));
            }
            ApplyForce(Seperate(Manager.zombieList));
            ultForce.Normalize();
            ultForce = ultForce * maxSpeed;
            ApplyForce(ultForce); //Applies all the steering forces
        }
        else //Wanders and avoids obstacles if nothing is found to target
        {
            ApplyForce(Seperate(Manager.zombieList));
            for (int i = 0; i < Manager.obstacles.Length; i++)
            {
                ApplyForce(AvoidObstacle(Manager.obstacles[i], 20));
            }
            ultForce += Wander();
            ultForce.Normalize();
            ultForce = ultForce * maxSpeed;
            ApplyForce(ultForce);
        }
        
    }
}
