using UnityEngine;
using System.Collections;

public class RobotTestScriptFree : MonoBehaviour {

	private Animator anim;
    private Rigidbody rb;
    private float jumpTimer = 0;
    public float speed = 100;
    private NavMeshAgent nav;
    private Vector3 destination;

    void Start () {
	
		anim = this.gameObject.GetComponent<Animator> ();
        rb = GetComponent<Rigidbody>();
        nav = GetComponent<NavMeshAgent>();
        destination.Set(10, 0, 25);
        nav.SetDestination(destination);
    }

    // Update is called once per frame
    /// <summary>
    /// 
    /// </summary>
    void FixedUpdate () {

        //Controls the Input for running animations
        // 1: walk
        //2: Run
        //3: Jump

        Debug.Log("rb : " + rb.position);
        Debug.Log("destination : " + destination);
        // Debug.Log("boolx : " + (rb.position.x.Equals(destination.x)));
        // Debug.Log("boolz : " + (rb.position.z.Equals(destination.z)));
        //        Debug.Log("x " + (int) rb.position.x);
        //        Debug.Log("y " + (int) rb.position.y);
        //        Debug.Log("z " + (int) rb.position.z); 
        //        Debug.Log("dx " + destination.x);
        //        Debug.Log("dy " + destination.y);
        //        Debug.Log("dz " + destination.z);

        int x_dest = (int)destination.x;
        int y_dest = (int)destination.y;
        int x_pos = (int)rb.position.x;
        int y_pos = (int)rb.position.y;

        if (!(x_dest.Equals(x_pos) && y_dest.Equals(y_pos))) {
            anim.SetInteger("Speed", 1);
        }
        else
        {
            anim.SetInteger("Speed", 0);
            new_destination();
            Debug.Log("test");
        }

        //if (Input.GetButton("Vertical")||Input.GetButton("Horizontal")) anim.SetInteger("Speed", 1);
        //else if(Input.GetButton("Vertical")) anim.SetInteger("Speed", 1);
        //else anim.SetInteger("Speed", 0);

        if (Input.GetButton("Jump")) {

			jumpTimer = 1;
			anim.SetBool ("Jumping", true);

			}

		if (jumpTimer > 0.5) jumpTimer -= Time.deltaTime;
			else if (anim.GetBool ("Jumping") == true) anim.SetBool ("Jumping", false);

	}

    void new_destination()
    {
        destination.x = destination.x * Random.value;
        destination.z = destination.z * Random.value;
        nav.SetDestination(destination);
        Debug.Log("new_postition()");
    }
}
