using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Last Updated Jan 6, 2019
// Unity 2D course, Text 101
// With added code for a getting the Name of the State, and what Objectives the Player accomplishes by reaching this State, and the amount of time this State adds to the clock

[CreateAssetMenu(menuName = "State")]			// Makes it appear as a Scriptable Object type in Unity (called 'State') - right-click Create... to instantiate more "States"
public class State : ScriptableObject {         // Using ScriptableObject rather than MonoBehavour to store text and whatnot

	[TextArea(10, 14)][SerializeField] string storyText;            // SerializeField = available in Inspector.	TextArea(minVisibleLines, numLinesBeforeScroll) = TextBox in the Inspector
	[SerializeField] State[] nextStates;                            // An array of States (where to go next). Unity UI will let you define size and populate
	[SerializeField] string achievedObjective;                      // What the Player "gets" or "accomplished" by making it to this State
	[SerializeField] int  minutesForState;							// How much time passes entering this State, Unity defaults to 0


	public string GetStateStory() {		// getter
		return storyText;
	}

	public State[] GetNextStates() {    // getter
		return nextStates;
	}

	public string GetAchievedObjective() {    // getter added - for puzzles
		return achievedObjective;
	}

	public int GetStateMinutes() {    // getter added - for game timer
		return minutesForState;
	}

	public string GetStateName() {      // A getter added so we can see the name of this Scriptable Object, helpful for debugging
        return this.name;
    }

}
