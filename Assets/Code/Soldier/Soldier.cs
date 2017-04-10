using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Soldier : MonoBehaviour {

	private SoldierMovementManager mov;
	private float attackTimer;
	private DecisionTree dt;

	[HideInInspector] public PlayerBehaviour myPlayer;
	[HideInInspector] public PlayerBehaviour enemyPlayer;
	[HideInInspector] public int team;
	[HideInInspector] public int health;
	[HideInInspector] public int power;
	[HideInInspector] public int attackVel;
	[HideInInspector] public int movementVel;
	[HideInInspector] public bool distanceAttack;
	[HideInInspector] public bool attackPlayer;
	[HideInInspector] public List<GameObject> enemiesInZone = new List<GameObject> ();
	[HideInInspector] public bool playerInZone;
	[HideInInspector] public Soldier enemyAttacking;
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
		mov.SetMaxVelocity (movementVel);

		switch (attackVel) {
			case 1:
				timeBetweenAttacks = 1.5f;
				break;
			case 2:
				timeBetweenAttacks = 1.25f;
				break;
			case 3:
				timeBetweenAttacks = 1f;
				break;
			case 4:
				timeBetweenAttacks = 0.75f;
				break;
			case 5:
				timeBetweenAttacks = 0.5f;
				break;
		}

		if (team == 1) {
			myPlayer = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerBehaviour> ();
			enemyPlayer = GameObject.FindGameObjectWithTag ("EnemyPlayer").GetComponent<PlayerBehaviour> ();
			tag = "Enemy2";
		}
		else {
			myPlayer = GameObject.FindGameObjectWithTag ("EnemyPlayer").GetComponent<PlayerBehaviour> ();
			enemyPlayer = GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerBehaviour> ();
			tag = "Enemy1";
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

		if (mov.currentBehaviour == SteeringBehaviours.attacking) {
			Attack (mov.GetTarget ());
		}
	}

	private void Attack(GameObject target) {
		attackTimer += Time.deltaTime;

		if (attackTimer >= timeBetweenAttacks) {
			attackTimer = 0f;

			if (attackingPlayer)
				target.GetComponent<PlayerBehaviour> ().GetHurt (power);
			else
				target.GetComponent<Soldier> ().GetHurt (power);
		}
	}

	public void GetHurt(int power) {
		health -= power;

		if (health <= 0)
			Die();
	}

	private void Die() {

	}

	private void OnTriggerEnter(Collider other) {
		if (other.tag == ("Enemy" + team) && !enemiesInZone.Contains(other.gameObject)) {
			enemiesInZone.Add (other.gameObject);
		}

		if (other.tag == enemyPlayer.tag) {
			playerInZone = true;
		}
	}

	private void OnTriggerExit(Collider other) {
		if (other.tag == ("Enemy" + team) && enemiesInZone.Contains (other.gameObject)) {
			enemiesInZone.Remove (other.gameObject);
		}

		if (other.tag == enemyPlayer.tag && playerInZone) {
			playerInZone = false;
		}
	}

	public void SetTarget(GameObject target) {
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
}
