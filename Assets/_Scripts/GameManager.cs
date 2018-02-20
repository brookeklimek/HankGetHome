using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
	public enum GameState { menu, playing, paused, gameover };

	public static int level = 1;
	public static int maxLevel = 3;
	public static int scoreToNextLevel = 30;
	public static int increaseSpeed = 5;

	public static int score = 0;
	public static int distance = 0;
	public static GameState state;
	public static GameObject player;
	public static bool pausedGame;

	// UI Elements
<<<<<<< HEAD
	public Text scoreText;
	public Text distanceText;
=======
	//public Text scoreText = null;
	//public GameObject nextLevelTextObject = null;
	//pausedTextObject
	//gameOverTextObject or wonTextObject
	//play textObject probobaly a menu 
	//nextLevel or nextPhaseTextObject
>>>>>>> 25abf5ff957649af82c8126bd6f5377b1bc6f910

	public GameObject nextLevelTextObject;

	void Start () {
		state = GameState.playing;
		player = GameObject.FindGameObjectWithTag ("Player");
		//for right now until we figure out flow 
		//nextLevelTextObject.SetActive (false);
		pausedGame = false;
		}
	
	void Update () {
<<<<<<< HEAD
		scoreText.text = "" + score;
		distanceText.text = "00" + distance;
=======
		//scoreText.text = "" + score;
>>>>>>> 25abf5ff957649af82c8126bd6f5377b1bc6f910

		 //Pause Game
		if (pausedGame) { 
			if(state == GameState.playing) {
				// Time.timeScale stops everything
				Time.timeScale = 0;
				state = GameState.paused;
			}
			// Use this code in menu to unpause game 
//			else if (state == GameState.paused) {
//				Time.timeScale = 1;
//				state = GameState.playing;
//			}
		}

		//GameState 
		if (state == GameState.playing) {
			
			// Set text objects .SetActive to false
		}
		else if (state == GameState.paused) {
			// set pausedTextObject.SetActive(true);
		}
		else if (state == GameState.gameover) {
			 //menu SetActive to true
			//play again Set Active to true
			if (Input.GetKeyDown(KeyCode.R)) { // press play again on menu button
				Restart ();
				state = GameState.playing;
			}
		}

		//When to Increase level/make game harder
		if(score >= scoreToNextLevel) {
			if(level == maxLevel) {
				return;
			}
			else {
				scoreToNextLevel *= 2;
				level++;
				GetComponent<PlayerController> ().SetSpeed (increaseSpeed);

			}
		}
	}

	// Public Methods
	public static void AddScore(int points) {
		score += points;
		Debug.Log (score);
	}

	public static void AddDistance(int distanceRan) {
		distance = distanceRan;
	}

//	public static void KnockOut () {
//
//		// player animation goes here? 
//
//
//		Camera.main.transform.position = new Vector3 (
//			player.transform.position.x,
//			player.transform.position.y,
//			Camera.main.transform.position.z
//		);
//	}

//	public static void LevelUp () {
//		// When to do this?
//		//nextLevelTextObject.SetActive (true);
//		//SceneManager.LoadScene ("Level" + level);
//	}

	public static void PauseGame() {
		pausedGame = true;
	}

<<<<<<< HEAD
	//Private Methods
	private void Restart() {
		score = 0;
		distance = 0;
		//decide what to do with level
		//maybe keep level same for demo so can get to end
		player.SendMessage ("Respawn");
		SceneManager.LoadScene ("Environment_Scene");

	}


=======
//	private void Restart() {
//		// should we ever?
//	}

	public void LoadLevel(string sceneName) {
		Debug.Log("Clicked!");
		SceneManager.LoadScene (sceneName);
	}
>>>>>>> 25abf5ff957649af82c8126bd6f5377b1bc6f910
}
