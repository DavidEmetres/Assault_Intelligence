using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoldierCreator : MonoBehaviour {

    private int health;
	private int power;
	private int attackVel;
	private int movementVel;
	private bool distanceAttack;
	private bool attackPlayer;
	private int currentMana;
	private float manaRegenerationTime;
	private float manaTimer;
	private GameObject[] spawnPositions = new GameObject[3];

	public int totalMana;
	public GameObject creationBar;
	public Slider healthSlider;
	public Slider powerSlider;
	public Slider attackVelSlider;
	public Slider movementVelSlider;
	public Toggle distanceAttackToggle;
	public Toggle attackPlayerToggle;
	public GameObject leftSpawnPos;
	public GameObject centerSpawnPos;
	public GameObject rightSpawnPos;
	public GameObject soldierPrefab;

	private void Start() {
		currentMana = 1;
		manaRegenerationTime = 3f;	//ONE MANA EACH 3 SECONDS;
		spawnPositions[0] = leftSpawnPos;
		spawnPositions [1] = centerSpawnPos;
		spawnPositions [2] = rightSpawnPos;
	}

    private void Update() {
		manaTimer += Time.deltaTime;

		if (manaTimer >= manaRegenerationTime) {
			manaTimer = 0f;
			currentMana++;

			if (currentMana > totalMana)
				currentMana = totalMana;
		}
    }

	public void CreateRandomSoldier() {
		health = Random.Range (1, 6);
		power = Random.Range (1, 6);
		attackVel = Random.Range (1, 6);
		movementVel = Random.Range (1, 6);

		distanceAttack = (Random.value < 0.5f);
		attackPlayer = (Random.value < 0.5f);

		int i = Random.Range (0, 3);
		Vector3 spawnPos = new Vector3(spawnPositions [i].transform.position.x, soldierPrefab.transform.position.y,
			spawnPositions [i].transform.position.z);
//		Vector3 spawnPos = new Vector3(worldPos.x, soldierPrefab.transform.position.y, worldPos.z);

//		GameObject soldier = Instantiate (soldierPrefab, new Vector3(spawnPos.position.x, soldierPrefab.transform.position.y, spawnPos.position.z), Quaternion.identity) as GameObject;
		GameObject soldier = Instantiate (soldierPrefab, spawnPos, Quaternion.identity) as GameObject;
		soldier.GetComponent<Soldier> ().Initialize (GetComponent<PlayerBehaviour> ().team, health, power, attackVel, movementVel, distanceAttack, attackPlayer);
		soldier.transform.LookAt (soldier.GetComponent<Soldier> ().enemyPlayer.transform.position);
	}
}
