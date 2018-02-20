using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	public float defaultRunSpeed = 7.0f;
	public float speedUpRate = 5.0f;
	// start animation
	public float waitStartForAnimation;


	private float runSpeed;
	private	int runDirection;
	private Vector3 movePlayer;
	Rigidbody rb;

	private bool grounded;
	private Animator anim; 

	public static bool canTurn;
	public float jumpForce;

	public int distance;



	//	public AudioClip jump;
	private Vector3 spawnPoint;


	void Start() {
		runSpeed = defaultRunSpeed;
		canTurn = false;
		anim = GetComponent <Animator> ();
		spawnPoint = transform.position;

		rb = GetComponent <Rigidbody> ();

	}

	void Update() {
		

		Debug.Log (GameManager.state);

		if(Time.time < waitStartForAnimation) {
			//anim.SetTrigger ("Moving");
			//if time is less than animation only run this and not rest of update
			//controller.Move (Vector3.forward * speed * Time.deltaTime);
			return;
		}


		if (GameManager.state == GameManager.GameState.playing) {
			anim.SetTrigger ("Moving");
		}


		if (GameManager.state == GameManager.GameState.gameover) {
			anim.SetTrigger ("Cry");

		}
		//
		//		if (GameManager.state != GameManager.GameState.playing) {
		//			return;
		//		}

		ProcessKeyInput();

		Move();
		//this.updateElapsedTimeLabel();
		CalculateDistance ();

	}

	void OnCollisionEnter(Collision other) {
		if(other.collider.tag == "Environment") {
			grounded = true;
		}
		else {
			grounded = false;
		}
	}

	public void CalculateDistance() {
		distance = (int)Vector3.Distance(spawnPoint, transform.position);
		GameManager.AddDistance (distance);
		//		Debug.Log ("distance" + distance);

	}

	public void SetSpeed(float modifier) {
		runSpeed += modifier;
	}



<<<<<<< HEAD
	// change powerups to enum and switch statements 
	public void JumpPowerUp() {
		jumpForce += 1.0f;
	}
=======
		if (Input.GetKeyDown (KeyCode.Space) && grounded) {
			anim.SetTrigger ("Jump");
			rb.AddForce (new Vector3 (0, jumpForce, 0), ForceMode.Impulse);
			//SoundManager.instance.PlaySingle (jump);
		}
		if (Input.GetKeyDown ("a") && canTurn) { 
			transform.Rotate (0, -90, 0);
			moveDirection = transform.forward;
			canTurn = false;
			}

		if (Input.GetKeyDown ("d") && canTurn) {
			transform.Rotate (0, 90, 0);
			moveDirection = transform.forward;
			canTurn = false;
			}
		
>>>>>>> 25abf5ff957649af82c8126bd6f5377b1bc6f910

	public void SpeedPowerUp() {
		runSpeed += 1.0f;
	}


	public void Respawn () {
		transform.position = spawnPoint;
		GameManager.state = GameManager.GameState.playing;

	}

	private void ProcessKeyInput() {
		if (Input.GetKey("a") && canTurn) { 
			runDirection -= 90;  
			canTurn = false;
		} else if (Input.GetKey("d") && canTurn) {
			runDirection += 90;
			canTurn = false;
		}

		if (Input.GetKey(KeyCode.Space) && grounded) {
			anim.SetTrigger ("Jump");
			rb.AddForce (new Vector3 (0, jumpForce, 0), ForceMode.Impulse);
			grounded = false;
			//SoundManager.instance.PlaySingle (jump);
		}
	}

	private void Move() {
		float x = Input.GetAxis ("Horizontal");
		//Mathf.Clamp (transform.position.x, -1, 1);
		rb.velocity = new Vector3 (x * runSpeed * Time.deltaTime, rb.velocity.y);
		transform.Translate (new Vector3 (x * runSpeed * Time.deltaTime, 0, 0));


		this.transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, runDirection, 0), 0.25f);
		Vector3 v = transform.forward * this.runSpeed;
		v.y = GetComponent<Rigidbody>().velocity.y;
		//v.x = Mathf.Clamp(transform.position.x, -1.25);

		GetComponent<Rigidbody>().velocity = v;
	}


}