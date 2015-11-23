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
        destination.Set(10, 0, 0);
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

        if(rb.position != destination) {
            anim.SetInteger("Speed", 1);
        }
        else if(rb.position == destination)
        {
            //nav.SetDestination(destination*Random.value);
            anim.SetInteger("Speed", 0);
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
}
