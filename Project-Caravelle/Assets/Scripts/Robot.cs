using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Robot : MonoBehaviour {
    public float speed;
    private Rigidbody rb;
    private Dictionary<int, MapInformation> map;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        map = new Dictionary<int, MapInformation>();
        //Collider
    }
	// Update is called once per frame
	void FixedUpdate () {
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");
        rb.AddForce(moveHorizontal*speed, 0, moveVertical*speed);
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
