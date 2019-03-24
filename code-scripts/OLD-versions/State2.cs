using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "State")]			// Makes it appear as a Scriptable Object type in Unity (called 'State') - right-click Create... to instantiate more "States"
public class State : ScriptableObject {         // Using ScriptableObject rather than MonoBehavour to store text and whatnot

	[TextArea(10, 14)][SerializeField] string storyText;            // SerializeField = available in Inspector.	TextArea(minVisibleLines, numLinesBeforeScroll) = TextBox in the Inspector
	[SerializeField] State[] nextStates;							// An array of States (where to go next). Unity UI will let you define size and populate

	public string GetStateStory() {		// getter
		return storyText;
	}

	public State[] GetNextStates() {    // getter
		return nextStates;
	}

    public string GetStateName() {      // A getter added so we can see the name of this Scriptable Object, helpful for debugging
        return this.name;
    }

}
