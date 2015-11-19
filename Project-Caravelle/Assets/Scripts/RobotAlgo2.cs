using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Robot : MonoBehaviour {
    public float speed;
    private Rigidbody rb;
    private int[,] map;
	private const int unknown = 0;
	private const int walkable = 1;
	private const int unwalkable = 2;
	public int width;
	public int height;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
		width = 5;
		heigh = 5;
        map = new int[width,height];
		for(int col = 0; col < width; col++)
		{
			for(int lin = 0; lin < height; lin++)
			{
				map[col, lin] = unknown;
			}
		}
    }
	// Update is called once per frame
	void FixedUpdate () {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        rb.AddForce(moveHorizontal*speed, 0, moveVertical*speed);
		addPositionOnMap();
		share();
	}
	void addCurrentPositionOnMap()
	{
		//TODO transform.position.
	}
	void move()
	{
		//TODO choose somewhere to go
	}

	void share() {
		var list = getRobotInRange();
		foreach(Robot robot in list)
		{
			robot.share(map)
		}
	}
	void share(int[,] other_map)
	{
		for(int col = 0; col < width; col++)
		{
			for(int lin = 0; lin < height; lin++)
			{
				if(map[col,lin] == unknown && other_map[col,lin] != unknown)
				{
					map[col,lin] = 	other_map[col,lin];
				}
			}
		}

	}
	void addPositionOnMap(int col, int lin, bool walk_able)
	{
		if(walk_able)
		{
			map[col,lin] = walkable
		}
		else
		{
			map[col,lin] = unwalkable
		}
	}
    void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.CompareTag("Target"))
        {
            //Time.deltaTime;
            Debug.Log(other.transform.position);
            Debug.Log(rb.velocity);
            other.gameObject.SetActive(false);
        }
    }
    List<Robot> getRobotInRange()
    {
        GameObject[] allRobot = GameObject.FindGameObjectsWithTag("Robot");
        List<Robot> listRobotInRange = new List<Robot>();
        foreach(GameObject robot in allRobot)
        {
            if(Vector3.Distance(robot.transform.position, transform.position) < 10)
            {
                listRobotInRange.Add(robot.GetComponent<Robot>());
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
}
