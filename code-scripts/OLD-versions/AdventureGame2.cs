using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// With added code for a randomized correct departure gate

// AdventureGame is our Script we added to our Create-Empty "Game object" (called "Game") - will become the main script.
public class AdventureGame : MonoBehaviour {

	// Adding [SerializeField] makes the variable visible in the Unity Inspector (to which we can assign components)
	[SerializeField] Text textComponent;        // a "textbox" type component in Unity (on the Canvas) -> output
	[SerializeField] State startingState;       // a State value -> input/setting
	[SerializeField] State flightInfoState;		// Which state has the Flight Info "room" which displays the randomly selected correct gate
	[SerializeField] State departureGatesState;		// State that leads to the various gates: 0 will be for all incorrect Gates, 1 will before the correct Gate, 2 will be Return to Lobby

	// Varibles and consts needed for the game:
	const int maxDepartureGates = 4;			// We only allow for Gates G1 .. G4
	const string gatePlaceholder = "$GATE$";
	int correctDepartureGateNum = -1;			// the randomly selected correct departure gate, only once the Player checks the screens (zero-based), -1 = NOT SET YET
	State correctDepartureGateState;			
	State state;                                // the current state

	// Use this for initialization
	void Start () {
		state = startingState;                          // Initial state is whatever is set in the Inspector of the "Game object"
		textComponent.text = state.GetStateStory();     // update the .text property of the textComponent pointed to in the Unity UI on the Canvas (of what ever the state is)
        Debug.Log("Initial State: " + state.GetStateName());        // Print something to the Console in Unity
	}
	
	// Update is called once per frame
	void Update () {
		ManageState();                                  // process Player input
		UpdateStory();									// Update the display
	}

	private void UpdateStory() {
		// update the Canvas textComponent with the State's story text
		if (state == flightInfoState) {
			SetDepartureGate();                             // only sets the correct gate once
			textComponent.text = state.GetStateStory().Replace(gatePlaceholder, (correctDepartureGateNum + 1).ToString());     // Presents randomized gate "G#"
		} else if (state == correctDepartureGateState) {
			textComponent.text = state.GetStateStory().Replace(gatePlaceholder, (correctDepartureGateNum + 1).ToString());     // Presents randomized gate "G#"
		} else {
			textComponent.text = state.GetStateStory();
		}
	}

	private void SetDepartureGate() {
		if (correctDepartureGateNum < 0 ) {
			System.Random rand = new System.Random();
			correctDepartureGateNum = rand.Next(maxDepartureGates);		//0-based
		} 
	}
	
	private void ManageState() {
		State[] nextStates;
		if (state == departureGatesState) {
			var tempNextStates = state.GetNextStates();
			var incorrectGatesState = tempNextStates[0];	// first NextState on this State is for ALL incorrect Gates
			correctDepartureGateState = tempNextStates[1];	// second NextState on this State is for the correct Gate
			var lobbyState = tempNextStates[2];				// third NextState on this State is for the return to Lobby option
			nextStates = new State[maxDepartureGates + 1];	// Add 1 for the Lobby option
			// First populate with the incorrect gate States:
			for (var i = 0; i < maxDepartureGates; i++) {
				nextStates[i] = incorrectGatesState;
			}
			// Then add the correct state only IF the Player has visited the Flight Info state
			if (correctDepartureGateNum >= 0) {
				nextStates[correctDepartureGateNum] = correctDepartureGateState;
			}
			nextStates[nextStates.Length - 1] = lobbyState;     // last option
		} else {
			nextStates = state.GetNextStates();
		}
		
		for (int i = 0; i < nextStates.Length; i++) {
			// KeyCode.Alpha1 = 49, .Alpha2 = 50, etc
			if (Input.GetKeyDown(KeyCode.Alpha1 + i)) {
                state = nextStates[i];                      // Update the current state to one of the available nextStates
				Debug.Log("Current State: " + state.GetStateName());    // Prints only if there is a key pressed, once per key press
				// Could do something with StateName, eg. implement some logic here - ONCE per entering a state
			}
		}

        /* OLD way of doing it, but generates an error if Player presses a key beyond the bounds of nextStates[]
        // Allow for user pressing 1..5 to select what to do
		if (Input.GetKeyDown(KeyCode.Alpha1)) {
			state = nextStates[0];							// Update the current state to one of the available nextStates
		} else if (Input.GetKeyDown(KeyCode.Alpha2)) {
			state = nextStates[1];
		} else if (Input.GetKeyDown(KeyCode.Alpha3)) {
			state = nextStates[2];
		} else if (Input.GetKeyDown(KeyCode.Alpha4)) {
			state = nextStates[3];
		} else if (Input.GetKeyDown(KeyCode.Alpha5)) {
			state = nextStates[4];
		}
		*/

        // MOVED to Update(): textComponent.text = state.GetStateStory();		// update the Canvas textComponent with the new State's story text
	}
}
