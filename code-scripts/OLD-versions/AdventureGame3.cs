using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;			// required for the Text type

// Last Updated Jan 5, 2019
// Unity 2D course, Text 101
// With added code for a randomized correct departure gate, and needing to meet objectives

// AdventureGame is our Script we added to our Create-Empty "Game object" (called "Game") - will become the main script.
public class AdventureGame : MonoBehaviour {

	// Adding [SerializeField] makes the variable visible in the Unity Inspector (to which we can assign components)
	// So all of these are set in the Unity Editor:
	[SerializeField] Text textComponent;        // a "textbox" type component in Unity (on the Canvas) -> output
	[SerializeField] State startingState;       // a State value -> input/setting
	[SerializeField] State flightInfoState;		// Which state has the Flight Info "room" which displays the randomly selected correct gate
	[SerializeField] State departureGatesState;     // State that leads to the various gates: 0 will be for all incorrect Gates, 1 will before the correct Gate, 2 will be Return to Lobby
	[SerializeField] State successState;            // Final/SuccessState for this Scene's Game Object - IF all objectives are met
	[SerializeField] State failedObjectivesState;	// If the requiredObjectives are not met, go to this state instead of the successState
	[SerializeField] string[] requiredObjectives;	// List of "objectives" the Player much obtain/accomplish to finish. Must match any objectives on the States

	// Internal varibles and consts needed for the game:
	const int maxDepartureGates = 4;			// We only allow for Gates G1 .. G4
	const string placeholder = "$REPLACEME$";	// use in the State text fields for when we need to replace text dynamically
	int correctDepartureGateNum = -1;			// the randomly selected correct departure gate, only once the Player checks the screens (zero-based), -1 = NOT SET YET
	State correctDepartureGateState;            // Specified on the departureGateState as the 2nd possible gate
	Hashtable completedObjectives = new Hashtable();        // to hold all the Objectives and if they're completed or not (True/False)
	State state;                                // the current state
	

	// Unity methid: Use this for initialization
	void Start () {
		state = startingState;                          // Initial state is whatever is set in the Inspector of the "Game object"
		textComponent.text = state.GetStateStory();     // update the .text property of the textComponent pointed to in the Unity UI on the Canvas (of what ever the state is)
		InitaliseObjectives();							// From the list of required objectives, forced to lower-case in case of typos in Unity Editor
        Debug.Log("Initial State: " + state.GetStateName());        // Print something to the Console in Unity
	}
	
	// Inialise the Objectives hash
	private void InitaliseObjectives() {
		foreach (var objective in requiredObjectives) {
			completedObjectives.Add(objective.ToLower(), false);			// Initially none of them have been completed
		}
	}

	// Unity method: Update is called once per frame, even if the Player does nothing, called over and over
	void Update () {
		ManageState();                                  // process Player input
		UpdateStoryText();								// Update the display/textComponent
	}

	// update the Canvas textComponent with the State's story text, customizing for certain States
	private void UpdateStoryText() {
		if (state == flightInfoState) {
			SetDepartureGate();                             // only sets the correct gate once, and only when the Player makes it to this State
			textComponent.text = state.GetStateStory().Replace(placeholder, (correctDepartureGateNum + 1).ToString());     // Presents randomized gate "G#"
		} else if (state == correctDepartureGateState) {
			textComponent.text = state.GetStateStory().Replace(placeholder, (correctDepartureGateNum + 1).ToString());     // Presents randomized gate "G#"
		} else if (state == failedObjectivesState) {
			string objectivesList = String.Join(" and ", requiredObjectives);					// Could check and only return MISSED objectives, but this will do
			textComponent.text = state.GetStateStory().Replace(placeholder, objectivesList);	// Presents Objective1 and Objective2 and Etc and ...
		} else {
			textComponent.text = state.GetStateStory();
		}
	}

	// Sets the Departure Gate to a random number, but only once
	private void SetDepartureGate() {
		if (correctDepartureGateNum < 0 ) {
			System.Random rand = new System.Random();
			correctDepartureGateNum = rand.Next(maxDepartureGates);		//0-based
		} 
	}
	
	// Text101 method to manage the State Machine
	private void ManageState() {
		var nextStates = CheckAndReturnNextStates();		// a new method that checks the current state and returns the correct Next States depending on the current state
		// New way of checking for KeyPress -> NextState, old way gave out of bounds error if Player pressed a number greater than possible nextStates
		for (int i = 0; i < nextStates.Length; i++) {
			// KeyCode.Alpha1 = 49, .Alpha2 = 50, etc -- so this allows up to 9 or 10 States but only checks up to number of possible nextStates (unsure about '0')
			if (Input.GetKeyDown(KeyCode.Alpha1 + i)) {
				//
				// Code in this block is only called ONCE per entering a State, outside here it's called every Frame
				//

				state = VerifyObjectives(nextStates[i]);		// Update the current state to one of the available nextStates, after verifiying in a new method if all objectives met
				// See if the Player has achieved any game objectives by making it to this State
				var stateObjective = state.GetAchievedObjective();
				if (!String.IsNullOrEmpty(stateObjective)) {
					completedObjectives[stateObjective.ToLower()] = true;
					Debug.Log("Achieved Objective: " + stateObjective);
				}
				Debug.Log("Current State: " + state.GetStateName());    // Prints only if there is a key pressed, once per key press
				Debug.Log("Mins to add: " + state.GetStateMinutes());
			}
		}
	}

	// Verifies a possible Next State the Player is about to move to, to ensure all Objectives have been met
	private State VerifyObjectives(State potentialNextState) {
		if (potentialNextState == successState) {
			//PrintHashtable (completedObjectives);      // Displays the Hashtable
			if (completedObjectives.ContainsValue(false)) {
				Debug.Log("Unable to Succeed, missing objectives!");
				return failedObjectivesState;
			} else {
				Debug.Log("All objectives met, Success!");
				return potentialNextState;
			}
		} else {
			return potentialNextState;
		}
	}

	// Prints a Hashtable to the Unity console, for debugging
	private void PrintHashtable(Hashtable ht) {
		foreach (string key in ht.Keys) {
			Debug.Log(key + ": " + ht[key]); 
		}
	}

	// Returns an array of Next States - checks if Player is at the Departure Gate and adjusts if needed
	private State[] CheckAndReturnNextStates() {
		State[] nextStates;
		if (state == departureGatesState) {
			var tempNextStates = state.GetNextStates();
			var incorrectGatesState = tempNextStates[0];    // first NextState on this State is for ALL incorrect Gates
			correctDepartureGateState = tempNextStates[1];  // second NextState on this State is for the correct Gate
			var lobbyState = tempNextStates[2];             // third NextState on this State is for the return to Lobby option
			nextStates = new State[maxDepartureGates + 1];  // Add 1 for the Lobby option last
			// First populate with the incorrect gate States:
			for (var i = 0; i < maxDepartureGates; i++) {
				nextStates[i] = incorrectGatesState;
			}
			// Then add the correct state only IF the Player has visited the Flight Info state
			if (correctDepartureGateNum >= 0) {
				nextStates[correctDepartureGateNum] = correctDepartureGateState;
			}
			nextStates[nextStates.Length - 1] = lobbyState;     // last option
			return nextStates;
		} else {
			return state.GetNextStates();
		}
	}

}
