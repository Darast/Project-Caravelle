using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class RobotAlgorithm1 : MonoBehaviour
{
    public float speed;
    private Rigidbody rb;
    private int[,] map;
    private const int unknown = 0;
    private const int walkable = 1;
    private const int unwalkable = 2;
    public int width;
    public int height;
    private int half_height;
    private int half_width;
    private Vector3 stopPosition = new Vector3(0, 0, 0);
    private bool hasStopped = false;
    private NavMeshAgent nma;
    private int target_x;
    private int target_z;
    private int x;
    private int z;

    private Animator anim;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        nma = GetComponent<NavMeshAgent>();
        map = new int[width, height];
        half_height = height / 2;
        half_width = width / 2;
        for (int col = 0; col < width; col++)
        {
            for (int lin = 0; lin < height; lin++)
            {
                map[col, lin] = unknown;
            }
        }

        anim = this.gameObject.GetComponent<Animator>();
        target_x = (int)rb.position.x /*+half_width*/;
        target_z = (int)rb.position.z /*+half_height*/;
        x = target_x /*- half_width*/;
        z = target_z /*- half_height*/;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        addCurrentPositionOnMap();
        share();
        int result = isMapComplete();
        if (result != width * height && hasStopped == false)
        {
            move();
        }
        else
        {
            stop();
        }

    }
    void stop()
    {
        if (hasStopped == false)
        {
            rb.velocity = new Vector3(0, 0, 0);
            rb.angularVelocity = new Vector3(0, 0, 0);
            stopPosition = transform.position;
            hasStopped = true;
        }
        transform.position = stopPosition;
    }
    void addCurrentPositionOnMap()
    {
        addPositionOnMap((int)Math.Round(transform.position.x), (int)Math.Round(transform.position.z), true);
    }
    void move()
    {

        int x_pos = (int)rb.position.x;
        int z_pos = (int)rb.position.z;
        

        Vector3 pos_vect = new Vector3(x_pos, rb.position.y, z_pos);
        Vector3 dest_vect = new Vector3(x, rb.position.y, z);

        if (Vector3.Distance(pos_vect, dest_vect) > 2)
        {
            anim.SetInteger("Speed", 1);
        }
        else
        {
            anim.SetInteger("Speed", 0);
            target_x = (int) (target_x + UnityEngine.Random.value) % width;
            target_z = (int) (target_z + UnityEngine.Random.value) % height;

            x = target_x /*- half_width*/;
            z = target_z /*- half_height*/;
            nma.SetDestination(new Vector3(x, rb.position.y, z));
        }
        Debug.Log("(" + x + "," + z + ")");
        Debug.Log(x_pos     + ", " + z_pos);

        //Trouver le chemin vers la case inconnu la plus proche
        //Aller à la position suivante avec le vecteur (ma_position-position_suivante) ... ou l'inverse ...
    }

    void share()
    {
        var list = getRobotInRange();
        foreach (RobotAlgorithm1 robot in list)
        {
            robot.share(map);
        }
    }
    void share(int[,] other_map)
    {
        for (int col = 0; col < width; col++)
        {
            for (int lin = 0; lin < height; lin++)
            {
                if (map[col, lin] == unknown && other_map[col, lin] != unknown)
                {
                    map[col, lin] = other_map[col, lin];
                }
            }
        }

    }
    void addPositionOnMap(int col, int lin, bool walk_able)
    {
        if (map[col /*+ half_width*/, lin /*+ half_height*/] == unknown)
        {
            Debug.Log("Found (" + (col/* + half_width*/) + "," + (lin /*+ half_height*/) + ")");
        }
        if (walk_able)
        {
            map[col/* + half_width*/, lin/* + half_height*/] = walkable;
        }
        else
        {
            map[col /*+ half_width*/, lin/* + half_height*/] = unwalkable;
        }
    }
    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Wall")
        {
            Debug.Log(collision.contacts[0].point);
            //Time.deltaTime;
            Debug.Log(rb.velocity);
            //collision.gameObject.SetActive(false);
        }
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
    int convertPositionToUniqueId(int x, int z)
    {
        int xx = x * 2;
        if (x < 0)
        {
            xx = -x * 2 + 1;
        }
        int zz = z * 2;
        if (zz < 0)
        {
            zz = -z * 2 + 1;
        }
        return (xx + zz) * (xx + zz + 1) / 2 + zz;
    }
    int isMapComplete()
    {
        int sum = 0;
        for (int col = 0; col < width; col++)
            for (int lin = 0; lin < height; lin++)
                if (map[col, lin] != unknown)
                    sum++;
        return sum;
    }
}
