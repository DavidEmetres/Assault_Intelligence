using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerBehaviour : MonoBehaviour {

	private SoldierCreator creator;
	private float creationTimer;

	//PROVISIONAL AUTOMATIC CPU;
	private float timeBetweenCreation = 3f;

	public float playerHealth;
	[HideInInspector] public int team;
	[HideInInspector] public List<GameObject> enemiesInZone = new List<GameObject> ();

	private void Awake () {
		playerHealth = 20f;
		team = (tag == "Player") ? 1 : 2;
		creator = GetComponent<SoldierCreator> ();
	}
	
	private void Update () {
		creationTimer += Time.deltaTime;

		if (creationTimer >= timeBetweenCreation) {
			creationTimer = 0f;

			creator.CreateRandomSoldier ();
		}
//
//		if (Input.GetKeyDown (KeyCode.Space) && tag == "Player") {
//			creator.CreateRandomSoldier ();
//		}

//		if (Input.GetMouseButtonDown (0) && tag == "Player") {
//			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
//			RaycastHit hit;
//			if(Physics.Raycast (ray, out hit, Mathf.Infinity)) {
//				if (hit.collider.name == "Battleground") {
//					creator.CreateRandomSoldier (hit.point);
//				}
//			}
//		}
//
//		if (Input.GetMouseButtonDown (1) && tag == "EnemyPlayer") {
//			Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
//			RaycastHit hit;
//			if(Physics.Raycast (ray, out hit, Mathf.Infinity)) {
//				if (hit.collider.name == "Battleground") {
//					creator.CreateRandomSoldier (hit.point);
//				}
//			}
//		}
	}

	public void GetHurt(int power) {
		playerHealth -= power/10f;
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
