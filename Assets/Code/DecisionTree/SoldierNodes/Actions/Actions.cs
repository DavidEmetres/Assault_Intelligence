using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AttackEnemyPlayer : BaseNode {

	private BaseNode leftChild;
	private BaseNode rightChild;
	private Soldier agent;

	public AttackEnemyPlayer(Soldier agent) {
		this.agent = agent;
	}

	public bool Evaluate() {
		agent.SetTarget (agent.enemyPlayer.gameObject);

		return true;
	}

	public void SetChildren(BaseNode leftChild, BaseNode rightChild) {
		this.leftChild = leftChild;
		this.rightChild = rightChild;
	}

	public BaseNode GetLeftChild() {
		return leftChild;
	}

	public BaseNode GetRightChild() {
		return rightChild;
	}
}

public class AttackCloserAttackingEnemy : BaseNode {

	private BaseNode leftChild;
	private BaseNode rightChild;
	private Soldier agent;

	public AttackCloserAttackingEnemy(Soldier agent) {
		this.agent = agent;
	}

	public bool Evaluate() {
		GameObject target = null;

		for (int i = 0; i < agent.enemiesInZone.Count; i++) {
			if (agent.enemiesInZone [i].GetComponent<Soldier> ().attackingPlayer) {
				if (target == null)
					target = agent.enemiesInZone [i];

				if (Vector3.Distance (agent.transform.position, agent.enemiesInZone [i].transform.position) < Vector3.Distance (agent.transform.position, target.transform.position))
					target = agent.enemiesInZone [i];
			}
		}

		agent.SetTarget (target);

		return true;
	}

	public void SetChildren(BaseNode leftChild, BaseNode rightChild) {
		this.leftChild = leftChild;
		this.rightChild = rightChild;
	}

	public BaseNode GetLeftChild() {
		return leftChild;
	}

	public BaseNode GetRightChild() {
		return rightChild;
	}
}

public class AttackCloserPlayerAttacker : BaseNode {

	private BaseNode leftChild;
	private BaseNode rightChild;
	private Soldier agent;

	public AttackCloserPlayerAttacker(Soldier agent) {
		this.agent = agent;
	}

	public bool Evaluate() {
		GameObject target = null;

		for (int i = 0; i < agent.enemyPlayer.enemiesInZone.Count; i++) {
			if (agent.enemyPlayer.enemiesInZone [i].GetComponent<Soldier> ().attackingPlayer) {
				if (target == null)
					target = agent.enemyPlayer.enemiesInZone [i];

				if (Vector3.Distance (agent.transform.position, agent.enemyPlayer.enemiesInZone [i].transform.position) < Vector3.Distance (agent.transform.position, target.transform.position))
					target = agent.enemyPlayer.enemiesInZone [i];
			}
		}

		agent.SetTarget (target);

		return true;
	}

	public void SetChildren(BaseNode leftChild, BaseNode rightChild) {
		this.leftChild = leftChild;
		this.rightChild = rightChild;
	}

	public BaseNode GetLeftChild() {
		return leftChild;
	}

	public BaseNode GetRightChild() {
		return rightChild;
	}
}

public class AttackEnemyWithLowerHP : BaseNode {

	private BaseNode leftChild;
	private BaseNode rightChild;
	private Soldier agent;

	public AttackEnemyWithLowerHP(Soldier agent) {
		this.agent = agent;
	}

	public bool Evaluate() {
		GameObject target = null;

		for (int i = 0; i < agent.enemiesInZone.Count; i++) {
			if (target == null)
				target = agent.enemiesInZone [i];

			if (agent.enemiesInZone [i].GetComponent<Soldier> ().health < target.GetComponent<Soldier> ().health)
				target = agent.enemiesInZone [i];
		}

		agent.SetTarget (target);

		return true;
	}

	public void SetChildren(BaseNode leftChild, BaseNode rightChild) {
		this.leftChild = leftChild;
		this.rightChild = rightChild;
	}

	public BaseNode GetLeftChild() {
		return leftChild;
	}

	public BaseNode GetRightChild() {
		return rightChild;
	}
}