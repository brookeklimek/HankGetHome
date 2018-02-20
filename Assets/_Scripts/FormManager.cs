using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Firebase;
using Firebase.Auth;
using UnityEngine.SceneManagement; 

public class FormManager : MonoBehaviour {

	// UI Elements
	public InputField emailInput;
	public InputField passwordInput;

	public Button signUpButton;
	public Button loginButton;

	public AuthManager authManager;

	void Awake () {
		ToggleButtonStates (false);

		// Auth delegate subscription
		authManager.authCallback += HandleAuthCallback;
	}

	/// <summary>
	/// Validates the email input 
	/// </summary>
	public void ValidateEmail() {
		string email = emailInput.text;
		var regexPattern = "^\\w+([-+.']\\w+)*@\\w+([-.]\\w+)*\\.\\w+([-.]\\w+)*$";

			if(email != "" && Regex.IsMatch(email, regexPattern)) {
				ToggleButtonStates(true);
			}
			else {
				ToggleButtonStates(false);
			}

	}

	// Firebase methods
	public void OnSignUp() {
		authManager.SignUpNewUser (emailInput.text, passwordInput.text);
		Debug.Log("Sign In");
	}

	public void OnLogIn() {
		authManager.LoginExistingUser (emailInput.text, passwordInput.text);
		Debug.Log("Login");
	}
	// Update is called once per frame
	void Update () {
		
	}

	IEnumerator HandleAuthCallback(Task<Firebase.Auth.FirebaseUser> task, string operation) {
		if (task.IsFaulted || task.IsCanceled) {
		} else if (task.IsCompleted) {
			Firebase.Auth.FirebaseUser newPlayer = task.Result;
			Debug.LogFormat ("Welcome to HangGetHome {0}", newPlayer.Email);

			yield return new WaitForSeconds (1.0f);
			SceneManager.LoadScene ("Start_Scene");

		}
	}

	void OnDestroy() {
		authManager.authCallback -= HandleAuthCallback;
	}

	// Utilities
	private void ToggleButtonStates(bool toState) {
		signUpButton.interactable = toState;
		loginButton.interactable = toState;
	}

//	private void UpdateStatus(string message) {
//		statusText.text = message;
//	}
}
