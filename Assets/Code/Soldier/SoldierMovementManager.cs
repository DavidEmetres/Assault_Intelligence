using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoldierMovementManager : MonoBehaviour {

    private Vector3 position;
    private Vector3 velocity;
    private Quaternion rotation;
    private Vector3 desiredVelocity;
    private float minDistanceToTarget;
    private float smoothMove;
    private float smoothRot;
    private float maxVelocity;
    private float predictionPrecision;
	private Vector3 ahead;
	private Vector3 ahead2;

	public GameObject target;
	[HideInInspector] public SteeringBehaviours currentBehaviour;
	public bool followingPathToEnemyPlayer;
	public LayerMask nodeMask;
	public float maxVisionAhead;
	public float maxAvoidForce;

    private void Awake() {
		currentBehaviour = SteeringBehaviours.none;
    }

    private void Start() {
        position = transform.position;
        velocity = Vector3.zero;
        rotation = transform.rotation;
        smoothMove = 5f;
        smoothRot = 5f;
        predictionPrecision = 10f;
		followingPathToEnemyPlayer = false;

		if (GetComponent<Soldier> ().team == 1)
			nodeMask = LayerMask.GetMask ("T1Nodes");
		else
			nodeMask = LayerMask.GetMask ("T2Nodes");
    }

    private void Update() {
		if (target != null && currentBehaviour == SteeringBehaviours.none)
			currentBehaviour = SteeringBehaviours.pursuing;
		
        //UPDATE KINEMATICS CHARACTERISTICS;
		desiredVelocity = (desiredVelocity != Vector3.zero) ? desiredVelocity + CollisionAvoidance() : desiredVelocity;
        velocity = Vector3.Lerp(velocity, desiredVelocity, smoothMove * Time.deltaTime);
        position += velocity * maxVelocity * Time.deltaTime;

        if (velocity != Vector3.zero) {
            rotation = Quaternion.LookRotation(new Vector3(velocity.x, velocity.y, velocity.z), transform.up);
        }

        transform.position = position;
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, smoothRot * Time.deltaTime);

        //UPDATE STEERING BEHAVIOURS;
        switch (currentBehaviour) {
            case SteeringBehaviours.pursuing:
                Pursuit();
                break;
        }
    }

	private Vector3 CollisionAvoidance() {
		ahead = position + velocity.normalized * maxVisionAhead;
		ahead2 = position + velocity.normalized * maxVisionAhead * 0.5f;
		Vector3 avoidance = Vector3.zero;
		List<GameObject> mostThreaningObstacle = FindMostThreaningObstacle ();

//		if (mostThreaningObstacle != null) {
//			avoidance = new Vector3 (ahead.x - mostThreaningObstacle.transform.position.x, 0f, 0f);
//			avoidance.Normalize ();
//			avoidance.Scale (Vector3.one * maxAvoidForce);
//		}
		if (mostThreaningObstacle.Count > 0) {
			foreach (GameObject obs in mostThreaningObstacle) {
				avoidance = new Vector3 (ahead.x - obs.transform.position.x, 0f, 0f);
				avoidance.Normalize ();
				avoidance.Scale (Vector3.one * maxAvoidForce);
			}
		}
		else {
			avoidance.Scale (Vector3.zero);
		}

		return avoidance;
	}

	private List<GameObject> FindMostThreaningObstacle() {
		List<GameObject> mostThreaningObstacle = new List<GameObject>();
		foreach (GameObject enemy in GetComponent<Soldier>().alliesInZone) {
			bool collision = LineIntersectObstacle (enemy);
			bool inFront = (GetComponent<Soldier> ().team == 1) ? (enemy.transform.position.z > position.z) : (position.z > enemy.transform.position.z);

//			if (collision && inFront && (mostThreaningObstacle == null || Vector3.Distance (position, enemy.transform.position) < 
//				Vector3.Distance (position, mostThreaningObstacle.transform.position))) {
//				mostThreaningObstacle = enemy;
//			}

			if (collision && inFront) {
//				mostThreaningObstacle = enemy;
				mostThreaningObstacle.Add(enemy);
			}
		}

		return mostThreaningObstacle;
	}

	private bool LineIntersectObstacle(GameObject obstacle) {
		return Vector3.Distance (obstacle.transform.position, ahead) <= 1.5f ||
		Vector3.Distance (obstacle.transform.position, ahead2) <= 1.5f;
	}

    private void Pursuit() {
        if(target != null) {
			if (!followingPathToEnemyPlayer) {
				Vector3 predictedPos = Vector3.zero;

				RaycastHit hit;
				if (Physics.Raycast (transform.position, (target.transform.position - transform.position), out hit, 20f)) {
					if (hit.collider.tag != "unwalkable") {
						if (target.tag == GetComponent<Soldier> ().enemyPlayer.tag)
							predictedPos = target.transform.position;
						else {
							predictedPos = target.GetComponent<Soldier> ().GetPosition () + (target.GetComponent<Soldier> ().GetVelocity () *
							target.GetComponent<Soldier> ().GetMaxVelocity () * predictionPrecision * Time.deltaTime);
						}

						if (Vector3.Distance (position, predictedPos) > minDistanceToTarget) {
							desiredVelocity = (predictedPos - position).normalized;
							desiredVelocity = new Vector3 (desiredVelocity.x, 0f, desiredVelocity.z);
						}
						else {
							desiredVelocity = Vector3.zero;
							if (target == GetComponent<Soldier> ().enemyPlayer.gameObject && !GetComponent<Soldier> ().attackPlayer)
								currentBehaviour = SteeringBehaviours.none;
							else {
								if (target == GetComponent<Soldier> ().enemyPlayer.gameObject)
									GetComponent<Soldier> ().attackingPlayer = true;
								else
									GetComponent<Soldier> ().attackingPlayer = false;

								GetComponent<Soldier> ().Attack (target);
							}
						}
					}
					else {
						if (GetComponent<Soldier> ().enemiesInZone.Contains (target)) {
							GetComponent<Soldier> ().enemiesInZone.Remove (target);
							target = null;
						}
					}
				}
			}
			else {
				if (Vector3.Distance (position, target.transform.position) > 5f) {
					desiredVelocity = (target.transform.position - position).normalized;
					desiredVelocity = new Vector3 (desiredVelocity.x, 0f, desiredVelocity.z);
				}
				else {
					if (target.GetComponent<Node> ().nextNode == null) {
						desiredVelocity = Vector3.zero;
						transform.LookAt (target.transform);
						transform.eulerAngles = new Vector3 (0f, transform.eulerAngles.y, 0f);

						if (GetComponent<Soldier> ().attackPlayer) {
							GetComponent<Soldier> ().attackingPlayer = true;
							GetComponent<Soldier> ().Attack (GetComponent<Soldier> ().enemyPlayer.gameObject);
						}
					}
					else {
						target = target.GetComponent<Node> ().nextNode;
					}
				}
			}
        }
    }

	public void FollowPathToEnemyPlayer() {
		if (!followingPathToEnemyPlayer) {
			GameObject newTarget = null;
			Collider[] nodes = Physics.OverlapSphere (transform.position, 20f, nodeMask, QueryTriggerInteraction.Collide);

			for (int i = 0; i < nodes.Length; i++) {
				RaycastHit hit;
				if (Physics.Raycast (transform.position, (nodes[i].transform.position - transform.position), out hit, 20f)) {
					if (hit.collider.tag != "unwalkable") {
						if (newTarget == null) {
							newTarget = nodes [i].gameObject;
						}

						if (nodes [i].gameObject.GetComponent<Node> ().GetDistanceToEnemyPlayer (GetComponent<Soldier> ().team) <
							newTarget.GetComponent<Node> ().GetDistanceToEnemyPlayer (GetComponent<Soldier> ().team)) {
							newTarget = nodes [i].gameObject;
						}
					}
				}
			}

			if (newTarget != null) {
				target = newTarget;
				followingPathToEnemyPlayer = true;
			}
		}
	}

	public void SetSteeringBehaviour(SteeringBehaviours sb) {
		currentBehaviour = sb;
	}

	public void SetTarget(GameObject target) {
		this.target = target;
		followingPathToEnemyPlayer = false;
	}

	public GameObject GetTarget() {
		return target;
	}

	public void SetMaxVelocity(float velocity) {
		maxVelocity = velocity;
	}

    public Vector3 GetPosition() {
        return position;
    }

    public Vector3 GetVelocity() {
        return velocity;
    }

    public float GetMaxVelocity() {
        return maxVelocity;
    }

	public void SetMinDistance(float distance) {
		minDistanceToTarget = distance;
	}
}

public enum SteeringBehaviours
{
    pursuing,
	attacking,
	none
}
