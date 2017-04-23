using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Soldier : MonoBehaviour {

	private SoldierMovementManager mov;
	private DecisionTree dt;
	private float attackTimer;

	[HideInInspector] public PlayerBehaviour myPlayer;
	[HideInInspector] public PlayerBehaviour enemyPlayer;
	public int team;
	public float health;
	public int power;
	public int attackVel;
	public int movementVel;
	public bool distanceAttack;
	public bool attackPlayer;
	[HideInInspector] public List<GameObject> enemiesInZone = new List<GameObject> ();
	[HideInInspector] public List<GameObject> alliesInZone = new List<GameObject> ();
	[HideInInspector] public bool playerInZone;
	[HideInInspector] public GameObject enemyAttacking;
	[HideInInspector] public bool attackingPlayer;
	[HideInInspector] public float timeBetweenAttacks;

	public void Initialize(int team, int health, int power, int attackVel, int movementVel, bool distanceAttack, bool attackPlayer) {
		this.team = team;
		this.health = health;
		this.power = power;
		this.attackVel = attackVel;
		this.movementVel = movementVel;
		this.distanceAttack = distanceAttack;
		this.attackPlayer = attackPlayer;

		mov = GetComponent<SoldierMovementManager> ();

		switch (movementVel) {
			case 1:
				mov.SetMaxVelocity (5f);
				break;
			case 2:
				mov.SetMaxVelocity (8f);
				break;
			case 3:
				mov.SetMaxVelocity (10f);
				break;
			case 4:
				mov.SetMaxVelocity (13f);
				break;
			case 5:
				mov.SetMaxVelocity (16f);
				break;
		}

		switch (attackVel) {
			case 1:
				timeBetweenAttacks = 3f;
				break;
			case 2:
				timeBetweenAttacks = 2.5f;
				break;
			case 3:
				timeBetweenAttacks = 2f;
				break;
			case 4:
				timeBetweenAttacks = 1.5f;
				break;
			case 5:
				timeBetweenAttacks = 1f;
				break;
		}

		if (distanceAttack)
			mov.SetMinDistance (20f);
		else
			mov.SetMinDistance (10f);

		if (team == 1) {
			myPlayer = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerBehaviour> ();
			enemyPlayer = GameObject.FindGameObjectWithTag ("EnemyPlayer").GetComponent<PlayerBehaviour> ();
			tag = "Enemy2";
			transform.GetChild (0).GetComponent<MeshRenderer> ().material.color = Color.red;
		}
		else {
			myPlayer = GameObject.FindGameObjectWithTag ("EnemyPlayer").GetComponent<PlayerBehaviour> ();
			enemyPlayer = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerBehaviour> ();
			tag = "Enemy1";
			transform.GetChild (0).GetComponent<MeshRenderer> ().material.color = Color.blue;
		}

		//DECISION TREE GENERATION;

		//CONDITION NODES;
		IsPlayerInMyZoneAndIsMyObjective cNode1 = new IsPlayerInMyZoneAndIsMyObjective(this);
		AreSomeoneAttackingMe cNode2 = new AreSomeoneAttackingMe (this);
		AreSomeAttackingMyPlayerAndIAmCloseToHim cNode3 = new AreSomeAttackingMyPlayerAndIAmCloseToHim (this);
		AreSoldiersMyObjective cNode4 = new AreSoldiersMyObjective (this);
		ThereAreSoldiersInMyAttackZone cNode5 = new ThereAreSoldiersInMyAttackZone (this);

		//ACTION NODES;
		AttackEnemyPlayer aNode1 = new AttackEnemyPlayer(this);
		AttackCloserAttackingEnemy aNode2 = new AttackCloserAttackingEnemy (this);
		AttackCloserPlayerAttacker aNode3 = new AttackCloserPlayerAttacker (this);
		AttackEnemyWithLowerHP aNode4 = new AttackEnemyWithLowerHP (this);

		//SET CHILDREN;
		cNode1.SetChildren(aNode1, cNode2);
		cNode2.SetChildren (aNode2, cNode3);
		cNode3.SetChildren (aNode3, cNode4);
		cNode4.SetChildren (cNode5, aNode1);
		cNode5.SetChildren (aNode4, aNode1);

		dt = new DecisionTree (cNode1);
    }

	private void Update() {
		dt.TraverseTree ();
		attackTimer += Time.deltaTime;
	}

	public void Attack(GameObject target) {
		if (attackTimer >= timeBetweenAttacks) {
			attackTimer = 0f;
			if (attackingPlayer)
				target.GetComponent<PlayerBehaviour> ().GetHurt (power);
			else {
				enemyAttacking = target;
				target.GetComponent<Soldier> ().GetHurt (power);
			}
		}
	}

	public void GetHurt(int power) {
		health -= power/10f;

		if (health <= 0f)
			Die();
	}

	private void Die() {
		for (int i = 0; i < enemiesInZone.Count; i++) {
			if (enemiesInZone [i].GetComponent<Soldier> ().enemiesInZone.Contains (this.gameObject))
				enemiesInZone [i].GetComponent<Soldier> ().enemiesInZone.Remove (this.gameObject);
		}

		for (int i = 0; i < alliesInZone.Count; i++) {
			if (alliesInZone [i].GetComponent<Soldier> ().alliesInZone.Contains (this.gameObject))
				alliesInZone [i].GetComponent<Soldier> ().alliesInZone.Remove (this.gameObject);
		}

		if (playerInZone) {
			if (enemyPlayer.enemiesInZone.Contains (this.gameObject))
				enemyPlayer.enemiesInZone.Remove (this.gameObject);
		}

		Destroy (gameObject);
	}

	private void OnTriggerEnter(Collider other) {
		if (other.tag == ("Enemy" + team) && !enemiesInZone.Contains(other.gameObject)) {
			RaycastHit hit;
			if (Physics.Raycast (transform.position, (other.transform.position - transform.position), out hit, 20f)) {
				if(hit.collider.tag != "unwalkable")
					enemiesInZone.Add (other.gameObject);
			}
		}

		int otherTeam = (team == 1) ? 2 : 1;
		if (other.tag == ("Enemy" + otherTeam) && !alliesInZone.Contains(other.gameObject)) {
			RaycastHit hit;
			if (Physics.Raycast (transform.position, (other.transform.position - transform.position), out hit, 20f)) {
				if(hit.collider.tag != "unwalkable")
					alliesInZone.Add (other.gameObject);
			}
		}

		if (other.tag == enemyPlayer.tag) {
			playerInZone = true;
		}
	}

	private void OnTriggerExit(Collider other) {
		if (other.tag == ("Enemy" + team) && enemiesInZone.Contains (other.gameObject)) {
			enemiesInZone.Remove (other.gameObject);
		}

		int otherTeam = (team == 1) ? 2 : 1;
		if (other.tag == ("Enemy" + otherTeam) && alliesInZone.Contains(other.gameObject)) {
			alliesInZone.Remove (other.gameObject);
		}

		if (other.tag == enemyPlayer.tag && playerInZone) {
			playerInZone = false;
		}
	}

	public void SetTarget(GameObject target) {
//		if (target == enemyPlayer)
//			attackingPlayer = true;
//		else
//			attackingPlayer = false;
		attackingPlayer = false;
		mov.SetTarget (target);
	}

	public Vector3 GetPosition() {
		return mov.GetPosition ();
	}

	public Vector3 GetVelocity() {
		return mov.GetVelocity ();
	}

	public float GetMaxVelocity() {
		return mov.GetMaxVelocity ();
	}

	private void OnDrawGizmos() {
		if (mov.target != null) {
			Gizmos.color = (team == 1) ? Color.red : Color.blue;
			Gizmos.DrawRay (transform.position, (mov.target.transform.position - transform.position));
		}
	}
}
