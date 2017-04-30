using UnityEngine;
using System.Collections;

public class MachineLearning : MonoBehaviour {

	private Hashtable trainMatrix = new Hashtable ();

	public static MachineLearning Instance;

	private void Awake() {
		if (Instance == null) {
			Instance = this;
			DontDestroyOnLoad (gameObject);
		}
		else {
			Destroy (gameObject);
		}

		LoadMatrix ();
	}

	public void AddValues(Hashtable playerMatrix, bool winner) {
		//UPDATE TRAIN MATRIX ADDING OR SUBSTRACTING PLAYER MATRIX VALUES;
	}

	public int GetBestAction(int[] state) {
		//SEARCH THE BEST ACTION AVAILABLE FOR THE STATE PASSED;
		return 0;
	}

	private void LoadMatrix() {
		//LOAD TRAIN MATRIX FROM MEMORY;
	}

	private void SaveMatrix() {
		//SAVE TRAIN MATRIX IN MEMORY;
	}
}
