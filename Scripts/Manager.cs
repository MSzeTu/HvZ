using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Manager : MonoBehaviour
{
    public static GameObject[] obstacles;
    public static List<GameObject> humanList;
    public static List<GameObject> zombieList;
    public static bool showLines;
    int x;
    int z;
    int y;
    int humanSize;
    int zombieSize;
    RaycastHit hit;
    // Start is called before the first frame update
    void Start()
    {
        obstacles = GameObject.FindGameObjectsWithTag("Obstacle");
        humanList = new List<GameObject>();
        zombieList = new List<GameObject>();
        for (int i = 0; i < 5; i++) //Instantiate 5 humans and 5 zombies
        {
            x = Random.Range(-95, 95);
            z = Random.Range(-95, 95);
            Physics.Raycast(new Vector3(x, 10, z), Vector3.down, out hit, Mathf.Infinity);
            humanList.Add(GameObject.Instantiate((GameObject)Resources.Load("Prefabs/Human"), hit.point, Quaternion.identity));
        }
        for (int j = 0; j < 3; j++) //Instantiates 3 Zombies
        {
            x = Random.Range(-95, 95);
            z = Random.Range(-95, 95);
            Physics.Raycast(new Vector3(x, 10, z), Vector3.down, out hit, Mathf.Infinity);
            zombieList.Add(GameObject.Instantiate((GameObject)Resources.Load("Prefabs/Zombie"), hit.point, Quaternion.identity));
        }

    }

    // Update is called once per frame
    void Update()
    {  
        InputManager();
        DetectCollision();
    }

    //Detects collision of humans and zombies using AABB
    public void DetectCollision()
    {
        for (int i = 0; i < zombieList.Count; i++)
        {
            for (int h = 0; h < humanList.Count; h++)
            {
                SpriteInfo zInfo = zombieList[i].GetComponent<SpriteInfo>();
                SpriteInfo hInfo = humanList[h].GetComponent<SpriteInfo>();
                if (hInfo.minX < zInfo.maxX && hInfo.minZ < zInfo.maxZ && hInfo.maxZ > zInfo.minZ && hInfo.maxX > zInfo.minX)
                {
                    Vector3 spawnPosition = humanList[h].transform.position;
                    spawnPosition = new Vector3(spawnPosition.x, spawnPosition.y + zInfo.halfSize, spawnPosition.z);
                    GameObject target = humanList[h];
                    if (humanList[h].GetComponent<Vehicle>().futurePosMarker != null)
                    {
                        GameObject.Destroy(humanList[h].GetComponent<Vehicle>().futurePosMarker); //Remove the marker for futurepos to prevent issues
                    }
                    humanList.RemoveAt(h);
                    GameObject.Destroy(target);
                    zombieList.Add(GameObject.Instantiate((GameObject)Resources.Load("Prefabs/Zombie"),spawnPosition , Quaternion.identity)); //Spawn a zombie at the humans position
                }
            }
        }
    }

    //Shows GUI that lets you turn Debug lines on or off
    public void OnGUI()
    {
        GUI.color = Color.black;
        GUI.skin.box.fontSize = 15;
        GUI.Box(new Rect(10, 10, 325, 100), "Press L to show or hide Debug Lines. \nPress P to shuffle Tiger Positions \nPress C to switch Cameras \nPress R to restart");
        GUI.color = Color.white;
        GUI.Box(new Rect(10,Screen.height-100, 275, 100), "Press H to spawn Spiders. \nPress Z to spawn Tigers \nFirst Person Controls: Move with WASD \nCurse Tigers with left click. \nShuffle a targets position with right click.");
    }

    //Handles Input
    public void InputManager()
    {
        if (Input.GetKeyDown(KeyCode.P)) //places all zombies at random locations
        {
            for(int i=0; i < zombieList.Count; i++)
            {
                x = Random.Range(-100, 100);
                z = Random.Range(-100, 100);
                Physics.Raycast(new Vector3(x, 100, z), Vector3.down, out hit, Mathf.Infinity);
                zombieList[i].GetComponent<Zombie>().position = hit.point;
            }
        }
        if (Input.GetKeyDown(KeyCode.R)) //Reloads the scene
        {
            Scene scene = SceneManager.GetActiveScene();
            SceneManager.LoadScene(scene.name);
        }
        if (Input.GetKeyDown(KeyCode.L)) //toggles debug lines
        {
            if (showLines == false)
            {
                showLines = true;
            }
            else
            {
                showLines = false;
            }
        }
        if (Input.GetKeyDown(KeyCode.H)) //toggles debug lines
        {
            x = Random.Range(-95, 95);
            z = Random.Range(-95, 95);
            Physics.Raycast(new Vector3(x, 10, z), Vector3.down, out hit, Mathf.Infinity);
            humanList.Add(GameObject.Instantiate((GameObject)Resources.Load("Prefabs/Human"), hit.point, Quaternion.identity));
        }
        if (Input.GetKeyDown(KeyCode.Z)) //toggles debug lines
        {
            x = Random.Range(-95, 95);
            z = Random.Range(-95, 95);
            Physics.Raycast(new Vector3(x, 10, z), Vector3.down, out hit, Mathf.Infinity);
            zombieList.Add(GameObject.Instantiate((GameObject)Resources.Load("Prefabs/Zombie"), hit.point, Quaternion.identity));
        }
    }


}
