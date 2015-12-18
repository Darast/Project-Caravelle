using UnityEngine;
using System;
using System.Collections.Generic;


public class nexusScript : MonoBehaviour {

    private int[,] map;
    private const int unknown = 0;
    private const int walkable = 1;
    private const int unwalkable = 2;
    public int map_knowledge = 0;
    private int width;
    private int height;

    GameObject terrain = null;
    mapScript1 script;

    // Use this for initialization
    void Start () {
        terrain = GameObject.FindGameObjectWithTag("Maze");
        script = terrain.GetComponent<mapScript1>();

        width = script.mask.GetUpperBound(0) + 1;
        height = script.mask.GetUpperBound(1) + 1;
        map = new int[width, height];

        for (int col = 0; col < width; col++)
        {
            for (int lin = 0; lin < height; lin++)
            {
                map[col, lin] = unknown;
            }
        }

    }
	
	// Update is called once per frame
	void Update () {
	
	}


    public void shareFromRobot(int[,] other_map, RobotAlgorithm1 robot)
    {
        for (int col = 0; col < width; col++)
        {
            for (int lin = 0; lin < height; lin++)
            {
                if (map[col, lin] == unknown && other_map[col, lin] != unknown)
                {
                    map[col, lin] = other_map[col, lin];
                    map_knowledge++;
                    //Debug.Log("[Nexus] Je connais " + map_knowledge + " de la carte");
                }
            }
        }
        robot.shareFromNexus(map);
    }

    List<RobotAlgorithm1> getRobotInRange()
    {
        GameObject[] allRobot = GameObject.FindGameObjectsWithTag("Robot");
        List<RobotAlgorithm1> listRobotInRange = new List<RobotAlgorithm1>();
        foreach (GameObject robot in allRobot)
        {
            if (Vector3.Distance(robot.transform.position, transform.position) < 10)
            {
                listRobotInRange.Add(robot.GetComponent<RobotAlgorithm1>());
            }
        }
        return listRobotInRange;
    }

}
