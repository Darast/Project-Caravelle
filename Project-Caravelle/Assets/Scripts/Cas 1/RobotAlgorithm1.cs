using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEditor;

public class RobotAlgorithm1 : MonoBehaviour
{
    private Rigidbody rb;
    private int[,] map;
    private const int unknown = 0;
    private const int walkable = 1;
    private const int unwalkable = 2;
    public int map_knowledge = 0;
    private int width;
    private int height;
    public bool hasStopped = false;
    private NavMeshAgent nma;
    private int target_x;
    private int target_z;

    private Vector3 dest_vect;
    private Vector3 pos_vect;

    private const int sight_range = 5;
    private const int credits = 5;
    private int destination_reached = 0;
    public Boolean toBase = false;

    GameObject terrain = null;
    mapScript1 mapScript;
    int offset;

    GameObject nexus;
    private Vector3 nexus_pos;

    private Animator anim;


    void Start()
    {

        terrain = GameObject.FindGameObjectWithTag("Maze");
        mapScript = terrain.GetComponent<mapScript1>();
        offset = mapScript.offset;

        nexus = GameObject.FindGameObjectWithTag("Nexus");
        nexus_pos = nexus.GetComponent<Transform>().position;

        rb = GetComponent<Rigidbody>();
        nma = GetComponent<NavMeshAgent>();
        width = mapScript.mask.GetUpperBound(0) + 1;
        height = mapScript.mask.GetUpperBound(1) + 1;
        map = new int[width, height];

        for (int col = 0; col < width; col++)
        {
            for (int lin = 0; lin < height; lin++)
            {
                map[col, lin] = unknown;
            }
        }

        anim = this.gameObject.GetComponent<Animator>();
        target_x = (int)(UnityEngine.Random.value * 100) % width;
        target_z = (int)(UnityEngine.Random.value * 100) % height;
        pos_vect = rb.position;
        /*dest_vect = new Vector3(target_x, rb.position.y, target_z);
        nma.SetDestination(dest_vect);*/
        newDestination();
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        see();
        
        if ((map_knowledge < (width * height)) && !hasStopped)
        {
            move();
        }
        else if (!hasStopped)
        {
            nma.SetDestination(nexus_pos);
            hasStopped = true;
        }
        else if ((Vector3.Distance(nexus_pos, rb.position) < 2) && hasStopped)
        {
            shareWithNexus();
            stop();
        }
    }

    void stop()
    {
        anim.SetInteger("Speed", 0);
        //Debug.Log("stop");
        rb.velocity = new Vector3(0, 0, 0);
        rb.angularVelocity = new Vector3(0, 0, 0);
    }

    void move()
    {

        pos_vect.Set(rb.position.x, rb.position.y, rb.position.z);
        
        if (((Vector3.Distance(pos_vect, nexus_pos) >= 2) && toBase) || (Vector3.Distance(pos_vect, dest_vect) >= 2))
        {
            anim.SetInteger("Speed", 1);
        }
        else if ((Vector3.Distance(nexus_pos, rb.position) < 2) && toBase)
        {
            anim.SetInteger("Speed", 0);
            shareWithNexus();
            toBase = false;
            newDestination();
        }
        else
        {
            anim.SetInteger("Speed", 0);
            newDestination();
        }
        //Debug.Log("(" + x + "," + z + ")");
        //Debug.Log(x_pos     + ", " + z_pos);
    }

    void newDestination()
    {
        int count = 0;
        //Debug.Log("destination reached " + destination_reached);
        if (destination_reached >= credits)
        {
            nma.SetDestination(nexus_pos);
            dest_vect.Set(nexus_pos.x, nexus_pos.y, nexus_pos.z);
            //Debug.Log("Heading to Nexus");
            destination_reached = 0;
            toBase = true;
        }
        else
        {
            
            while ((map[target_x, target_z] != unknown) && (map_knowledge < width * height) && (count < width))
            {
                target_x = (target_x + (sight_range / 2)) % width;
                target_z = (target_z + ((target_x < (int)rb.position.x) ? sight_range / 2 : 0)) % height;
                count++;
            }

            dest_vect.x = target_x - offset;
            dest_vect.z = target_z - offset;
            nma.SetDestination(dest_vect);
            //Debug.Log("Heading to : " + dest_vect.x + "," + dest_vect.z);
            destination_reached++;
        }
    }

    void shareWithNexus()
    {
        nexus.GetComponent<nexusScript>().shareFromRobot(map,this);
        //Debug.Log("Sharing with Nexus");
        
    }

    public void shareFromNexus(int[,] other_map)
    {
        for (int col = 0; col < width; col++)
        {
            for (int lin = 0; lin < height; lin++)
            {
                if (map[col, lin] == unknown && other_map[col, lin] != unknown)
                {
                    map[col, lin] = other_map[col, lin];
                    map_knowledge++;
                   // Debug.Log("Je connais " + map_knowledge + " de la carte");
                }
            }
        }
    }

    void see()
    {
        int x = (int)rb.position.x + offset;
        int z = (int)rb.position.z + offset;

        for (int i = x - sight_range; i < x + sight_range; ++i)
        {
            for (int j = z - sight_range; j < z + sight_range; ++j)
            {
                if ((i >= 0) && (j >= 0) && (i <= mapScript.mask.GetUpperBound(0)) && (j <= mapScript.mask.GetUpperBound(1)))
                {
                    addPositionOnMap(i, j, mapScript.mask[i, j] == walkable);
                }
            }
        }
    }

    void addPositionOnMap(int col, int lin, bool walk_able)
    {
        if (map[col, lin] == unknown)
        {
            map_knowledge++;
            //Debug.Log("Found (" + col + "," + lin + ")");
            //Debug.Log("Je connais " + map_knowledge + " de la carte");
        }
        if (walk_able)
        {
            map[col, lin] = walkable;
        }
        else
        {
            map[col, lin] = unwalkable;
        }
    }

}
