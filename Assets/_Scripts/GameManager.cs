using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
	public enum GameState { menu, playing, paused, gameover };

	public static int level = 1;
	public static int maxLevel = 3;
	public static int distanceToNextLevel = 150; // change to 150
	public static int increaseSpeed = 5;

	public static int score = 0;
	public static float distance = 0;
	public static GameState state;
	public static GameObject player;
	public static bool pausedGame;

	// UI Elements

	public Text scoreText;
	public Text distanceText;
	public GameObject deathText;
	public Text highScoreText;

	//public GameObject nextLevelTextObject = null;
	//pausedTextObject
	//gameOverTextObject or wonTextObject
	//play textObject probobaly a menu 
	//nextLevel or nextPhaseTextObject

	public GameObject nextLevelTextObject;

	void Start () {
		state = GameState.playing;
		player = GameObject.FindGameObjectWithTag ("Player");
		//for right now until we figure out flow 
		//nextLevelTextObject.SetActive (false);
		pausedGame = false;
		}
	
	void Update () {

		scoreText.text = "" + score;
		distanceText.text = "00" + distance;


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

			if (PlayerPrefs.HasKey ("HighScore")) {
				if (PlayerPrefs.GetInt ("HighScore") > score) {
					highScoreText.text = "HighScore: " + 
										PlayerPrefs.GetInt ("HighScore");
				} else {
					PlayerPrefs.SetInt ("HighScore", score);
					highScoreText.text = "HighScore: " +
										PlayerPrefs.GetInt ("HighScore");
				}
			} else {
				PlayerPrefs.SetInt ("HighScore", score);
				highScoreText.text = "HighScore: " +
									PlayerPrefs.GetInt ("HighScore");
			}

			 //menu SetActive to true
			//play again Set Active to true
			deathText.SetActive(true);

			if (Input.GetKeyDown(KeyCode.R)) { // press play again on menu button
				Restart ();
				state = GameState.playing;
			}
		}

		//When to Increase level/make game harder
		if(distance >= distanceToNextLevel) {
			Debug.Log ("Ready for next level");
			if(level == maxLevel) {
				SceneManager.LoadScene ("HankHome");
			}
			else {
				distanceToNextLevel *= 2;
				level++;
				SceneManager.LoadScene ("Level" + level);
				player.SendMessage ("SetSpeed");


			}
		}

	}

	// Public Methods
	public static void AddScore(int points) {
		score += points;
		Debug.Log (score);

	}

	public static void AddDistance(float distanceRan) {
		
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


	//Private Methods
	private void Restart() {
		score = 0;
		distance = 0;
		//decide what to do with level
		//maybe keep level same for demo so can get to end
		player.SendMessage ("Respawn");
		Scene scene = SceneManager.GetActiveScene ();
		SceneManager.LoadScene (scene.name);

	}


//	private void Restart() {
//		// should we ever?
//	}

	public void LoadLevel(string sceneName) {
		Debug.Log("Clicked!");
		SceneManager.LoadScene (sceneName);
	}
}
