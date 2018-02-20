using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour {

	public Vector3 animationOffset;
	public float animationDuration = 2.0f;
//
//
//	private Vector3 startOffset;
//	private Transform player;
	private Vector3 moveCamera;

	private float transition = 0.0f;
//	public static bool turning;
//	//private bool playerturn;
//
//	void Start() {
//		player = GameObject.FindGameObjectWithTag ("Player").transform;
//		startOffset = transform.position - player.position;
//		turning = false;
//		//playerturn = true;
//	}
//
//	void Update() {
//		moveCamera = player.position + startOffset;
//		transform.position = moveCamera;
//		// X
//
//		//Y 
//		moveCamera.y = Mathf.Clamp (moveCamera.y, 1, 3);
//
//		if (transition > 1) {
//			transform.position = moveCamera;
//		}
//		else {
//			// Game start animation
//			transform.position = Vector3.Lerp (moveCamera + animationOffset, moveCamera, transition);
//			//take 2 seconds for transition to equal 1 then game starts.
//			transition += Time.deltaTime * 1 / animationDuration;
//			// rotate the camera to look at player + 1 u
//			transform.LookAt (player.position + Vector3.up);
//		}
//
//
//
////		
//
//
//	}

	public Transform player;

	public Vector3 offsetPosition;

	private Space offsetPositionSpace = Space.Self;
	private bool lookAt = true;

	void Update () {
		moveCamera = player.position + offsetPosition;

		if(transition > 1) {
			Refresh ();
		}
		else {
			// Game start animation
			transform.position = Vector3.Lerp (moveCamera + animationOffset, moveCamera, transition);
			//take 2 seconds for transition to equal 1 then game starts.
			transition += Time.deltaTime * 1 / animationDuration;
			// rotate the camera to look at player + 1 u
			transform.LookAt (player.position + Vector3.down);		}
			//
	}

	public void Refresh() {
		if (player == null) {
			Debug.Log ("Missing Player Transform");
			return;
		}
		 //POSITION
		if(offsetPositionSpace == Space.Self) {
			transform.position = player.TransformPoint (offsetPosition);
		}
		else {
			transform.position = player.position + offsetPosition;
		}
		 //ROTATION
		if(lookAt) {
			transform.LookAt (player);
		}
		else {
			transform.rotation = player.rotation;
		}
	}


}
