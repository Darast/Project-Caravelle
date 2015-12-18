using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class RobotAlgorithme2 : MonoBehaviour {
    private Rigidbody rb;
    private static int count_robot = 0;
    private int[,] map;
    private const int unknown = 0;
    private const int walkable = 1;
    private const int unwalkable = 2;
    private const int targeted = 3;
    private int width;
    private int height;
    private Vector3 stopPosition = new Vector3(0,0,0);
    public bool hasStopped = false;
    private NavMeshAgent nma;
    private Vector3[] otherRobotPosition;
    private int[] timestampOtherRobotPosition;
    private GameObject terrain = null;
    private mapScript2 script;
    private int offset;
    private Animator anim;
    private int idxNextRobotMobileBase = 0;

    public int target_x = 0;
    public int target_z = 0;
    public int range_wireless = 10;
    public float percent;
    public int sight_range = 2;
    public Boolean map_complete = false;
    public int id;
    public Boolean isMobileBase = false;

    void Start()
    {
        terrain = GameObject.FindGameObjectWithTag("Maze");
        script = terrain.GetComponent<mapScript2>();
        offset = script.offset;
        id = count_robot;
        count_robot++;

        MonoBehaviour[] RobotScriptList1 = GameObject.FindObjectsOfType<RobotAlgorithme2>() as MonoBehaviour[];
        int countRobot = RobotScriptList1.Length;
        otherRobotPosition = new Vector3[countRobot];
        timestampOtherRobotPosition = new int[countRobot];
        
        for (int i = 0; i < countRobot; ++i)
        {
            timestampOtherRobotPosition[i] = 0;
            otherRobotPosition[i] = new Vector3(0, 0, 0);
        }

        rb = GetComponent<Rigidbody>();
        nma = GetComponent<NavMeshAgent>();
        width = script.mask.GetUpperBound(0) + 1;
        height = script.mask.GetUpperBound(1) + 1;

        map = new int[width, height];

        for (int col = 0; col < width; col++)
            for (int lin = 0; lin < height; lin++)
                map[col, lin] = unknown;
        anim = this.gameObject.GetComponent<Animator>();
    }
    void FixedUpdate()
    {
        otherRobotPosition[id] = transform.position;
        timestampOtherRobotPosition[id]++;

        see();
        share();
        
        if (!isMapComplete() && hasStopped == false)
        {
            move();
            int result = countUnknownCell();
            percent = (float)result / (height * width);
        }
        else
            stop();
    }
    void stop()
    {
        anim.SetInteger("Speed", 0);
        if (hasStopped == false)
        {
            rb.velocity = new Vector3(0, 0, 0);
            rb.angularVelocity = new Vector3(0, 0, 0);
            stopPosition = transform.position;
            hasStopped = true;
        }
        transform.position = stopPosition;
    }
    void move()
    {
        if(Input.GetKey(KeyCode.A))
            stop();
        anim.SetInteger("Speed", 1);
        if (!isMobileBase)
        {
            if (map[target_x, target_z] != unknown)
                getNextTarget();
            int x = target_x - offset;
            int z = target_z - offset;

            nma.SetDestination(new Vector3(x, rb.position.y, z));
            //Trouver le chemin vers la case inconnu la plus proche
            //Aller à la position suivante avec le vecteur (ma_position-position_suivante) ... ou l'inverse ...
        }
        else
        {
            if (Vector3.Distance(otherRobotPosition[idxNextRobotMobileBase], transform.position) < 2)
            {
                idxNextRobotMobileBase++;
                if (idxNextRobotMobileBase == id)
                    idxNextRobotMobileBase++;
                idxNextRobotMobileBase = idxNextRobotMobileBase % count_robot;
            }
            nma.SetDestination(otherRobotPosition[idxNextRobotMobileBase]);
        }
    }
    void getNextTarget()
    {
        double min_dist = 2*width*height;
        Boolean found_target = false;
        Int16 c = 0;
        for (int col = 0; col < width; col++)
            for (int lin = 0; lin < height; lin++)
                if (map[col, lin] == unknown)
                {
                    double dist = Vector3.Distance(transform.position, new Vector3(col - offset, 0, lin - offset));
                    if (dist < min_dist)
                    {
                        found_target = true;
                        target_x = col;
                        target_z = lin;
                        min_dist = dist;
                    }
                }
                else if (map[col, lin] == targeted)
                {
                    c++;
                }
        if (found_target == false && c > 0)
        {
            for (int col = 0; col < width; col++)
                for (int lin = 0; lin < height; lin++)
                    if (map[col, lin] == targeted)
                        map[col, lin] = unknown;
        }
    }
    void share()
    {
        var list = getRobotInRange();
        foreach (RobotAlgorithme2 robot in list)
        {
            if (robot != this)
            {
                robot.share(map);
                robot.shareRobotPosition(timestampOtherRobotPosition, otherRobotPosition);
               if(!hasStopped)
                    robot.informTarget(target_x, target_z, transform.position);
            }
        }
    }
    void share(int[,] other_map)
    {
        for (int col = 0; col < width; col++)
            for (int lin = 0; lin < height; lin++)
                if ((map[col, lin] == unknown || map[col, lin] == targeted) && other_map[col, lin] != unknown && other_map[col, lin] != targeted)
                    map[col, lin] = other_map[col, lin];
    }
    void see()
    {
        int x = (int)rb.position.x + offset;
        int z = (int)rb.position.z + offset;

        for (int i = x - sight_range; i < x + sight_range; ++i)
        {
            for (int j = z - sight_range; j < z + sight_range; ++j)
            {
                if ((i >= 0) && (j >= 0) && (i <= script.mask.GetUpperBound(0)) && (j <= script.mask.GetUpperBound(1)))
                {
                    addPositionOnMap(i, j, script.mask[i, j] == walkable);
                }
            }
        }
    }
    void informTarget(int x, int z, Vector3 posRobot)
    {
        if (x == target_x && z == target_z)
        {
           
            Vector3 myPos = transform.position;
            Vector3 posTarget = new Vector3(x, 0, z);
            if (Vector3.Distance(posRobot, posTarget) < Vector3.Distance(myPos, posTarget))
            {
                map[x, z] = targeted;
            }
        }
        else if(map[x, z] == unknown)
        {
            map[x, z] = targeted;
        }
    }
    void shareRobotPosition(int[] timestamp, Vector3[] pos)
    {
        for (int i = 0; i < count_robot; ++i)
        {
            if (i != id && timestampOtherRobotPosition != null && timestamp[i] > timestampOtherRobotPosition[i])
            {
                otherRobotPosition[i] = pos[i];
                timestampOtherRobotPosition[i] = timestamp[i];
            }
        }
    }
    void addPositionOnMap(int col, int lin, bool walk_able)
    {
        if (walk_able)
        {
            map[col, lin] = walkable;
        }
        else
        {
            map[col, lin] = unwalkable;
        }
    }
    List<RobotAlgorithme2> getRobotInRange()
    {
        GameObject[] allRobot = GameObject.FindGameObjectsWithTag("Robot");
        List<RobotAlgorithme2> listRobotInRange = new List<RobotAlgorithme2>();
        foreach (GameObject robot in allRobot)
            if (Vector3.Distance(robot.transform.position, transform.position) < range_wireless)
                listRobotInRange.Add(robot.GetComponent<RobotAlgorithme2>());
        return listRobotInRange;
    }
    int countUnknownCell()
    {
        int sum = 0;
        for (int col = 0; col < width; col++)
            for (int lin = 0; lin < height; lin++)
                if (map[col, lin] != unknown)
                    sum++;
        return sum;
    }
    bool isMapComplete()
    {
        for (int col = 0; col < width; col++)
            for (int lin = 0; lin < height; lin++)
                if (map[col, lin] == walkable)
                    if (!isCellsAroundKnown(col, lin))
                        return false;
        map_complete = true;
        return true;
    }
    bool isCellsAroundKnown(int col, int lin)
    {
        for (int delta_col = -1; delta_col < 2; delta_col++)
            for (int delta_lin = -1; delta_lin < 2; delta_lin++)
                if (col + delta_col >= 0 && lin + delta_lin >= 0 && col + delta_col < width && lin + delta_lin < height)
                    if (map[col + delta_col, lin + delta_lin] != walkable && map[col + delta_col, lin + delta_lin] != unwalkable)
                        return false;
        return true;
    }
}
