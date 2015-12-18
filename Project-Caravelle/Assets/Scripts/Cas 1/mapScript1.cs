    using UnityEngine;
using System.Collections;
using System;

public class mapScript1 : MonoBehaviour {

    public int[,] mask;
    //private const int unknown = 0;
    private const int walkable = 1;
    private const int unwalkable = 2;

    public int width;
    public int height;
    public int offset= 2;

    //public Boolean started = false;

    private Transform map;

    private MonoBehaviour[] RobotScriptList;
    public int robotStopped = 0;

    private float startTime = 0;
    private float endTime;
    public float chrono = 0;

    // Use this for initialization
    void Start () {
        map = (Transform)GetComponent<Transform>();
        //mask = new int[(int) GetComponent<Renderer>().bounds.size.x, (int) GetComponent<Renderer>().bounds.size.z];
        mask = new int[width, height];

        for (int i = 0; i < width; ++i)
        {
            for(int j = 0; j < height; ++j)
            {
                mask[i, j] = walkable;
            }
        }

        foreach (Transform child in map)
        {
            if (child.CompareTag("Wall"))
            {
                float x = child.transform.position.x;
                float z = child.transform.position.z;
                float size_x= child.GetComponent<Renderer>().bounds.size.x;
                float size_z= child.GetComponent<Renderer>().bounds.size.z;

                if (size_x > 1)
                {
                    for (int i = (int)(x - (Math.Round(size_x/2))); i < (int)(x + (Math.Round(size_x/2))); ++i)
                    {
                        //Debug.Log("X");
                        //Debug.Log(i+","+z);
                        mask[(int)(i + offset), (int)(z + offset)] = unwalkable;
                    }
                }
                else if (size_z > 1)
                {
                    for (int i = (int)(z - (Math.Round(size_z/2))); i < (int)(z + (Math.Round(size_z/2)-1)); ++i)
                    {
                        //Debug.Log("Z");
                        //Debug.Log(x+","+i);

                        mask[(int)(x + offset),(int)(i + offset)] = unwalkable;
                    }
                }
                else
                {
                    mask[(int)(x + offset), (int)(z + offset)] = unwalkable;
                }
            }
        }

        //started = true;
        RobotScriptList = GameObject.FindObjectsOfType<RobotAlgorithm1>() as MonoBehaviour[];
        foreach(MonoBehaviour Robot in RobotScriptList)
        {
            Robot.enabled = true;
        }

        MonoBehaviour nexusScript = GameObject.FindObjectOfType<nexusScript>() as MonoBehaviour;
        if(nexusScript != null)
        {
            nexusScript.enabled = true;
        }

    }

    // Update is called once per frame
    void Update () {
        robotStopped = 0;
	    foreach(RobotAlgorithm1 Robot in RobotScriptList)
        {
            if (Robot.hasStopped)
            {
                robotStopped++;
            }
        }

        if (robotStopped < RobotScriptList.Length)
        {
            endTime = Time.time;
            chrono = endTime - startTime;
        }
	}
}
