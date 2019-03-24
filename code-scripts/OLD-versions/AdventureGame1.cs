using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// AdventureGame is our Script we added to our Create-Empty "Game object" (called "Game") - will become the main script.
public class AdventureGame : MonoBehaviour {

	// Adding [SerializeField] makes the variable visible in the Unity Inspector (to which we can assign components)
	[SerializeField] Text textComponent;        // a "textbox" type component in Unity (on the Canvas) -> output
	[SerializeField] State startingState;       // a State value -> input/setting

	// Varibles and consts needed for the game:
	State state;                                // the current state

	// Use this for initialization
	void Start () {
		state = startingState;                          // Initial state is whatever is set in the Inspector of the "Game object"
		textComponent.text = state.GetStateStory();     // update the .text property of the textComponent pointed to in the Unity UI on the Canvas (of what ever the state is)
	}
	
	// Update is called once per frame
	void Update () {
		ManageState();                                  // process Player input
	}

	private void ManageState() {
		var nextStates = state.GetNextStates();
		
		for (int i = 0; i < nextStates.Length; i++) {
			// KeyCode.Alpha1 = 49, .Alpha2 = 50, etc
			if (Input.GetKeyDown(KeyCode.Alpha1 + i)) {
                state = nextStates[i];                      // Update the current state to one of the available nextStates
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

        textComponent.text = state.GetStateStory();		// update the Canvas textComponent with the new State's story text
	}
}
