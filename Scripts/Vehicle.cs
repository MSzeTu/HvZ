using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Vehicle : MonoBehaviour
{

    //components for obstacle avoidance
    public List<GameObject> threats;
    SpriteInfo info;
    //Materials for debug lines
    public Material forwardLine;
    public Material rightLine;
    public Material zombieLine;
    public Material obstacleLine;
    public GameObject futurePosMarker;
    //Vectors for movement
    public Vector3 position;
    public Vector3 acceleration;
    public Vector3 direction;
    public Vector3 velocity;
    public Vector3 center;
    private Vector3 futurePos;

    //Floats
    public float mass;
    public float maxSpeed;
    public float radius;
    //Raycast for grounding
    RaycastHit hit;
    //Variables needed for wrap or bounding

    // Start is called before the first frame update
    void Start()
    {
        threats = new List<GameObject>();
        info = gameObject.GetComponent<SpriteInfo>();
        radius = info.radius * 2;
        velocity = new Vector3(0, 0, 0);
        position = gameObject.transform.position;//gameObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        center = info.center;
        //Keeps the object grounded
        Physics.Raycast(new Vector3(0, 5, 0), Vector3.down, out hit, Mathf.Infinity); //Raycasts to keep the bananas level with the ground
        if (gameObject.GetComponent<Zombie>())
        {
            position.y = (hit.point.y + (gameObject.transform.localScale.y / 2)) - 2.5f;
        }
        else
        {
            position.y = 0;
        }
        MovementForm();
        CalcSteeringForces();
        //Wrap();
        Steer();
    }

    //Applies a force to the vehicle to move it
    public void ApplyForce(Vector3 force)
    {
        acceleration += force / mass;
    }

    //Applies fricitonal force
    public void ApplyFriction(float coeff)
    {
        Vector3 friction = velocity * -1;
        friction.Normalize();
        friction = friction * coeff;
        acceleration += friction;
    }

    //Seeks a position
    public Vector3 Seek(Vector3 targetPosition)
    {
        Vector3 desiredVelocity = targetPosition - position;
        desiredVelocity.Normalize();
        desiredVelocity = desiredVelocity * maxSpeed;
        Vector3 seekingForce = desiredVelocity - velocity;

        return seekingForce;
    }

    //Overloaded form of Seek that makes it pursue a GameObject instead
    public Vector3 Seek(GameObject target)
    {
        return Seek(target.transform.position);
    }
    //flees from a position
    public Vector3 Flee(Vector3 targetPosition)
    {
        Vector3 desiredVelocity = targetPosition - position;
        desiredVelocity.Normalize();
        desiredVelocity = desiredVelocity * maxSpeed;
        Vector3 fleeingForce = (desiredVelocity + position) - velocity;

        return fleeingForce;
    }

    //Overloaded form of Flee that makes it flee from a GameObject instead
    public Vector3 Flee(GameObject target)
    {
        return Flee(target.transform.position);
    }



    //Evades the entered gameobject using it's future position
    public Vector3 Evade(GameObject target)
    {
        Vector3 targetVelocity = target.GetComponent<Vehicle>().velocity;
        Vector3 targetFuturePosition = target.transform.position + (targetVelocity * 2); //calculate future position to evade
        Vector3 desiredVelocity = targetFuturePosition - position;
        desiredVelocity.Normalize();
        desiredVelocity = desiredVelocity * maxSpeed;
        Vector3 evadingForce = (desiredVelocity + position) - velocity;

        return evadingForce;
    }


    //Pursues the entered gameobject using it's future position
    public Vector3 Pursue(GameObject target)
    {
        Vector3 targetVelocity = target.GetComponent<Vehicle>().velocity;
        Vector3 targetFuturePosition = target.transform.position + (targetVelocity * 2); //calculate future position to pursue
        Vector3 desiredVelocity = targetFuturePosition - position;
        desiredVelocity.Normalize();
        desiredVelocity = desiredVelocity * maxSpeed;
        Vector3 seekingForce = desiredVelocity - velocity;

        return seekingForce;
    }

    //Handles the basic movement formula
    public void MovementForm()
    {
        velocity += acceleration * Time.deltaTime;
        position += velocity * Time.deltaTime;
        direction = velocity.normalized;
        if (direction != Vector3.zero)
        {
            transform.forward = new Vector3(direction.x, 0, direction.z);
        }

        acceleration = Vector3.zero;
        transform.position = position;
        futurePos = position + (velocity * 1);
    }


    //Keeps the object on the floorspace by steering it towards the center
    public void Steer()
    {
        if (position.x > 97 || position.x < -97 || position.z > 97 || position.z < -97)
        {
            ApplyForce(Seek(Vector3.zero) * 30);
        }
    }

    //Draws Debug Lines
    public void OnRenderObject()
    {
        if (Manager.showLines)
        {

            if (futurePosMarker == null && (gameObject.GetComponent<Zombie>() != null)) //Places FuturePos sphere if none
            {
                futurePosMarker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                futurePosMarker.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
            }
            else if (futurePosMarker == null && (gameObject.GetComponent<Zombie>() == null))
            {
                futurePosMarker = GameObject.CreatePrimitive(PrimitiveType.Sphere);
                futurePosMarker.GetComponent<Renderer>().material.SetColor("_Color", Color.magenta);
            }
            futurePosMarker.transform.position = futurePos; //Moves FuturePos sphere
            if (gameObject.GetComponent<Zombie>() != null) //Draws Targeting Debug line
            {
                zombieLine.SetPass(0);
                GL.Begin(GL.LINES);
                GL.Vertex(gameObject.transform.position);
                if (gameObject.GetComponent<Zombie>().human != null)
                {
                    GL.Vertex(gameObject.GetComponent<Zombie>().human.transform.position);
                }
                GL.End();
            }
            forwardLine.SetPass(0); //Draws Forward Debug Line
            GL.Begin(GL.LINES);
            GL.Vertex(gameObject.transform.position);
            GL.Vertex(((direction * 10f) + gameObject.transform.position));
            GL.End();

            rightLine.SetPass(0); //Draws Right Debug Line
            GL.Begin(GL.LINES);
            GL.Vertex(gameObject.transform.position);
            GL.Vertex(((transform.right * 10f) + gameObject.transform.position));
            GL.End();
        }
        else
        {
            if (futurePosMarker != null)
            {
                GameObject.Destroy(futurePosMarker); //Destroys Sphere when not in use. 
            }
        }
    }

    //Avoids obstacle by returning desired steering force
    protected Vector3 AvoidObstacle(GameObject obstacle, float safeDistance)
    {
        //Info for obstacle avoidance
        Vector3 vecToCenter = obstacle.transform.position - position;
        float dotForward = Vector3.Dot(vecToCenter, transform.forward);
        float dotRight = Vector3.Dot(vecToCenter, transform.right);
        float radiiSum = obstacle.GetComponent<Obstacle>().radius + radius;

        //Step 1: check for front or back
        if (dotForward < 0)
        {
            return Vector3.zero;
        }
        //Step 2: check for closeness
        if (vecToCenter.magnitude > safeDistance)
        {
            return Vector3.zero;
        }
        //Step 3: check for right or left
        if (radiiSum < Mathf.Abs(dotRight))
        {
            return Vector3.zero;
        }
        //Steer away from object
        Vector3 desiredVelocity;
        if (dotRight < 0) //Turn left
        {
            desiredVelocity = transform.right * maxSpeed * 25; //Scaled up for noticability
        }
        else //Turn right
        {
            desiredVelocity = -transform.right * maxSpeed * 25;
        }

        //Debug line to obstacle
        Debug.DrawLine(transform.position, obstacle.transform.position, Color.green);

        //returns steering force
        Vector3 steeringForce = desiredVelocity - velocity;
        return steeringForce;
    }


    //Abstract method for use by children
    public abstract void CalcSteeringForces();

    //Keeps vehicle seperate from neighboring vehicles
    protected Vector3 Seperate(List<GameObject> partners)
    {
        Vector3 ultForce = Vector3.zero;
        Vector3 desiredVelocity = Vector3.zero;
        //Info for seperation
        if (partners.Count == 0)
        {
            return desiredVelocity;
        }

        foreach (GameObject g in partners)
        {
            int turn = Random.Range(1, 3);
            Vector3 vecToCenter = g.transform.position - position;
            if (vecToCenter.magnitude < 5 && g != gameObject)
            {
                Vector3 movingTowards = this.position - g.transform.position;
                if (movingTowards.magnitude > 0)
                {
                    desiredVelocity += (movingTowards.normalized / movingTowards.magnitude) * 5; //generates force away from the neighbor object
                }
            }
        }

        return desiredVelocity;

    }

    //Causes object to wander around aimlessly
    protected Vector3 Wander()
    {

        //Step 1: Project a circle forwards and set it's radius
        Vector3 circleCenter = velocity.normalized + position;
        float circleRadius = 5;

        //Step 2: Calculate a random Angle
        float angle = Random.Range(0f, 360f);
        angle = angle * Mathf.Deg2Rad;
        //Step 3: Select a random location on the circle using the angle 
        Vector3 wanderTarget = Vector3.zero;
        wanderTarget.x = circleCenter.x + Mathf.Cos(angle) * circleRadius;
        wanderTarget.y = gameObject.transform.position.y;
        wanderTarget.z = circleCenter.z + Mathf.Sin(angle) * circleRadius;

        //Step 4: Seek the global coordinate of the spot
        //Vector3 globalPosition = (position - circleCenter) + (circleCenter - wanderTarget);
        return Seek(wanderTarget);
    }

    private void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0) && gameObject.GetComponent<Zombie>()) //Transforms a zombie into a human when clicked
        {
            Manager.zombieList.Remove(gameObject);
            Manager.humanList.Add(GameObject.Instantiate((GameObject)Resources.Load("Prefabs/Human"), position, Quaternion.identity));
            if (futurePosMarker != null)
            {
                GameObject.Destroy(futurePosMarker);
            }
            GameObject.Destroy(gameObject);
        }
        if (Input.GetMouseButtonDown(1)) //Teleports the target to a random location
        {
            RaycastHit hit;
            int x = Random.Range(-95, 95);
            int z = Random.Range(-95, 95);
            Physics.Raycast(new Vector3(x, 10, z), Vector3.down, out hit, Mathf.Infinity);
            position = hit.point;
        }
    }
}
