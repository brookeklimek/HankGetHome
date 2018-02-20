using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialText : MonoBehaviour {

	public GameObject tutorialText;
	public GameObject lastText;


	void OnTriggerEnter(Collider other) {
		if (other.tag == "Player") {

			if (tutorialText != null) {
				tutorialText.SetActive (true);
			}

			if (lastText != null) {
				lastText.SetActive (false);
			}
		}

		Destroy (this.gameObject);
	}
}
