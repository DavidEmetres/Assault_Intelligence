using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerBehaviour : MonoBehaviour {

	private SoldierCreator creator;
	private int playerHealth;
	private float timeBetweenCreation = 2f;
	private float creationTimer;

	public int team;
	public List<GameObject> enemiesInZone = new List<GameObject> ();

	private void Awake () {
		team = (tag == "Player") ? 1 : 2;
		creator = GetComponent<SoldierCreator> ();
	}
	
	private void Update () {
		creationTimer += Time.deltaTime;

		if (creationTimer >= timeBetweenCreation) {
			creationTimer = 0f;

			creator.CreateRandomSoldier ();
		}
	}

	public void GetHurt(int power) {
		playerHealth -= power;
	}

	private void OnTriggerEnter(Collider other) {
		if (other.tag == ("Enemy" + team) && !enemiesInZone.Contains(other.gameObject)) {
			enemiesInZone.Add (other.gameObject);
		}
	}

	private void OnTriggerExit(Collider other) {
		if (other.tag == ("Enemy" + team) && enemiesInZone.Contains (other.gameObject)) {
			enemiesInZone.Remove (other.gameObject);
		}
	}
}
