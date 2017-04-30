using UnityEngine;
using System.Collections;

public class MLBehaviour : MonoBehaviour {

	private Hashtable playerMatrix = new Hashtable();

	private void Update() {
		if (Input.GetKeyDown (KeyCode.F1)) {
			string state = "1|1|1|1|1|1|1|1";
			int action = 10;

			UpdateMatrix (state, action);

			Debug.Log ("UPDATED MATRIX");
		}

		if (Input.GetKeyDown (KeyCode.F2)) {
			string state = "1|1|1|1|1|1|1|1";

			if (playerMatrix.ContainsKey(state)) {
				int[] actionVector = (int[])playerMatrix[state];
				foreach (int i in actionVector) {
					Debug.Log (i);
				}
			}
		}
	}

	public int SelectAction() {
//		int[] state = GetCurrentState ();
//
//		int action = MachineLearning.Instance.GetBestAction (state);
//
//		UpdateMatrix (state, action);
//
		return 0;
	}

	private int[] GetCurrentState() {
		int[] t = new int[0];

		return t;
	}

	public void UpdateMatrix(string state, int action) {
		if (!playerMatrix.ContainsKey (state)) {
			int[] actionVector = new int[72];
			actionVector [action] += 1;
			playerMatrix.Add (state, actionVector);
		}
		else {
			int[] actionVector = (int[])playerMatrix [state];
			actionVector [action] += 1;
			playerMatrix [state] = actionVector;
		}
	}
}
