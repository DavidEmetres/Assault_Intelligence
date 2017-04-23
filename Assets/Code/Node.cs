using UnityEngine;
using System.Collections;

public class Node : MonoBehaviour {

	public GameObject nextNode;
	[HideInInspector] public float distToPlayer;
	[HideInInspector] public float distToEnemyPlayer;

	private void Start() {
		GameObject player = GameObject.FindWithTag ("Player");
		GameObject enemyPlayer = GameObject.FindWithTag ("EnemyPlayer");

		distToPlayer = Vector3.Distance (transform.position, player.transform.position);
		distToEnemyPlayer = Vector3.Distance (transform.position, enemyPlayer.transform.position);
	}

	public float GetDistanceToEnemyPlayer(int team) {
		float dist = (team == 1) ? distToEnemyPlayer : distToPlayer;
		return dist;
	}
}
