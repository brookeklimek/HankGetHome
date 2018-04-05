using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnObjects : MonoBehaviour {

	//GameObjects used for identifying spawn points and object to spawn
	public GameObject spawnObjectContainer;
	public GameObject objectToSpawn;

	//Lists for all possible and chosen spawn points
	List<Transform> spawnPositions = new List<Transform>();
	List<int> chosenSpawns = new List<int> ();

	//Integers to set number of spawn objects
	public int minNumberOfObjects = 0;
	public int maxNumberOfObjects = 0;

	//Private ints used in methods below
	private int numberOfObjectsToSpawn;
	private int randomNumber;
	private int chosenIndexValue;
	private int numberOfSpawnPoints;

	public Vector3 spawnRotation =  new Vector3(131f, 27f, 0f);

	// Use this for initialization
	void Start () {
		//Create list of all possible spawn points
		foreach (Transform child in spawnObjectContainer.transform) {
			spawnPositions.Add (child);
		}

		numberOfSpawnPoints = spawnPositions.Count;
		numberOfObjectsToSpawn = Random.Range (minNumberOfObjects, maxNumberOfObjects);

		//Debug.Log ("Number of Spawn Points = " + numberOfSpawnPoints);
		//Debug.Log ("Number of Objects I Will Spawn = " + numberOfObjectsToSpawn);

		InstantiateObjects ();
	}

	//Method to instantiate Objects based on chosen spawn point position and defined rotation
	void InstantiateObjects() {
		for (var i = 0; i < numberOfObjectsToSpawn; i++) {
			chosenIndexValue = GenerateRandomNumber ();
			GameObject.Instantiate (objectToSpawn, spawnPositions [chosenIndexValue].position, Quaternion.Euler(spawnRotation));
		}
	}

	//Method to generate a random number indicating a position not currently chosen to spawn an object
	int GenerateRandomNumber() {
		randomNumber = Random.Range (0, numberOfSpawnPoints);

		if (chosenSpawns.IndexOf (randomNumber) >= 0) {
			GenerateRandomNumber ();
		} else {
			chosenSpawns.Add (randomNumber);
		}

		return randomNumber;
	}

}