using UnityEngine;
using System.Collections;

public class IsPlayerInMyZoneAndIsMyObjective : BaseNode {

	private BaseNode leftChild;
	private BaseNode rightChild;
	private Soldier agent;

	public IsPlayerInMyZoneAndIsMyObjective(Soldier agent) {
		this.agent = agent;
	}

	public bool Evaluate() {
		if (agent.playerInZone && agent.attackPlayer) {
			return true;
		}
		else {
			return false;
		}
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

public class AreSomeoneAttackingMe : BaseNode {

	private BaseNode leftChild;
	private BaseNode rightChild;
	private Soldier agent;

	public AreSomeoneAttackingMe(Soldier agent) {
		this.agent = agent;
	}

	public bool Evaluate() {
		foreach (GameObject enemy in agent.enemiesInZone) {
			if (enemy.GetComponent<Soldier> ().enemyAttacking == agent.gameObject)
				return true;
		}

		return false;
//		for (int i = 0; i < agent.enemiesInZone.Count; i++) {
//			if (agent.enemiesInZone [i].GetComponent<Soldier> ().enemyAttacking == agent.gameObject) {
//				return true;
//			}
//		}
//
//		return false;
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

public class AreSomeAttackingMyPlayerAndIAmCloseToHim : BaseNode {

	private BaseNode leftChild;
	private BaseNode rightChild;
	private Soldier agent;

	public AreSomeAttackingMyPlayerAndIAmCloseToHim(Soldier agent) {
		this.agent = agent;
	}

	public bool Evaluate() {
		if (Vector3.Distance (agent.transform.position, agent.myPlayer.transform.position) < Vector3.Distance (agent.transform.position, agent.enemyPlayer.transform.position)) {
			for (int i = 0; i < agent.myPlayer.enemiesInZone.Count; i++) {
				if (agent.myPlayer.enemiesInZone [i].GetComponent<Soldier> ().attackingPlayer) {
					return true;
				}
			}
		}

		return false;
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

public class AreSoldiersMyObjective : BaseNode {

	private BaseNode leftChild;
	private BaseNode rightChild;
	private Soldier agent;

	public AreSoldiersMyObjective(Soldier agent) {
		this.agent = agent;
	}

	public bool Evaluate() {
		if (agent.attackPlayer) {
			return false;
		}
		else {
			return true;
		}
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

public class ThereAreSoldiersInMyAttackZone : BaseNode {

	private BaseNode leftChild;
	private BaseNode rightChild;
	private Soldier agent;

	public ThereAreSoldiersInMyAttackZone(Soldier agent) {
		this.agent = agent;
	}

	public bool Evaluate() {
		if (agent.enemiesInZone.Count > 0) {
			return true;
		}
		else {
			return false;
		}
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