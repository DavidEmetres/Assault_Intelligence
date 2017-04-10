using UnityEngine;
using System.Collections;

public class SoldierMovementManager : MonoBehaviour {

    private Vector3 position;
    private Vector3 velocity;
    private Quaternion rotation;
    private Vector3 desiredVelocity;

    private float minDistanceToTarget;
    private float slowDownRadius;
    private float arrivalForce;
    private float smoothMove;
    private float smoothRot;
    private float maxVelocity;
    private float predictionPrecision;
	private GameObject target;

	[HideInInspector] public SteeringBehaviours currentBehaviour;

    private void Awake() {
		currentBehaviour = SteeringBehaviours.none;
    }

    private void Start() {
        minDistanceToTarget = 1f;
        position = transform.position;
        velocity = Vector3.zero;
        rotation = transform.rotation;
        smoothMove = 5f;
        smoothRot = 5f;
        maxVelocity = 10f;
        minDistanceToTarget = 3f;
        slowDownRadius = 10f;
        arrivalForce = 1f;
        predictionPrecision = 10f;
    }

    private void Update() {
		if (target != null && currentBehaviour == SteeringBehaviours.none)
			currentBehaviour = SteeringBehaviours.pursuing;
		
        //UPDATE KINEMATICS CHARACTERISTICS;
        velocity = Vector3.Lerp(velocity, desiredVelocity, smoothMove * Time.deltaTime);
        position += velocity * maxVelocity * Time.deltaTime;
        if (velocity != Vector3.zero) {
            rotation = Quaternion.LookRotation(new Vector3(velocity.x, velocity.y, velocity.z), transform.up);
        }

        transform.position = position;
        transform.rotation = Quaternion.Lerp(transform.rotation, rotation, smoothRot * Time.deltaTime);

        //UPDATE STEERING BEHAVIOURS;
        switch (currentBehaviour) {
//            case SteeringBehaviours.seeking:
//                Seek();
//                break;
//            case SteeringBehaviours.arriving:
//                Arrival();
//                break;
            case SteeringBehaviours.pursuing:
                Pursuit();
                break;
//            case SteeringBehaviours.fleeing:
//                Flee();
//                break;
        }
    }

//    private void Seek() {
//        if(target != null) {
//            if(Vector3.Distance(position, target) > slowDownRadius) {
//                desiredVelocity = (target - position).normalized;
//            }
//            else {
//                currentBehaviour = SteeringBehaviours.arriving;
//            }
//        }
//    }
//
//    private void Arrival() {
//        if(target != null) {
//            if(Vector3.Distance(position, target) > 0f) {
//                arrivalForce = Vector3.Distance(position, target) / slowDownRadius;
//                desiredVelocity = (target - position).normalized;
//                desiredVelocity *= arrivalForce;
//            }
//            else if(Vector3.Distance(position, target) > slowDownRadius) {
//                currentBehaviour = SteeringBehaviours.seeking;
//            }
//            else {
//                desiredVelocity = Vector3.zero;
//                currentBehaviour = SteeringBehaviours.none;
//            }
//        }
//    }

    private void Pursuit() {
        if(target != null) {
			Vector3 predictedPos = Vector3.zero;

			if (target.tag == GetComponent<Soldier> ().enemyPlayer.tag)
				predictedPos = target.transform.position;
			else {
				predictedPos = target.GetComponent<Soldier> ().GetPosition () + (target.GetComponent<Soldier> ().GetVelocity () * 
					target.GetComponent<Soldier> ().GetMaxVelocity () * predictionPrecision * Time.deltaTime);
			}
            
            if (Vector3.Distance(position, predictedPos) > minDistanceToTarget) {
                desiredVelocity = (predictedPos - position).normalized;
            }
            else {
                desiredVelocity = Vector3.zero;
				if (target == GetComponent<Soldier> ().enemyPlayer.gameObject && !GetComponent<Soldier> ().attackPlayer)
					currentBehaviour = SteeringBehaviours.none;
				else
					currentBehaviour = SteeringBehaviours.attacking;
            }
        }
    }

//    private void Flee()
//    {
//        if (objective != null)
//        {
//            Vector3 predictedPos = objective.GetPosition() + (objective.GetVelocity() * objective.GetMaxVelocity() * predictionPrecision * Time.deltaTime);
//
//            if (Vector3.Distance(position, predictedPos) > minDistanceToTarget)
//            {
//                desiredVelocity = (position - predictedPos).normalized;
//            }
//            else {
//                desiredVelocity = Vector3.zero;
//                currentBehaviour = SteeringBehaviours.none;
//            }
//        }
//    }

	public void SetSteeringBehaviour(SteeringBehaviours sb) {
		currentBehaviour = sb;
	}

//	public void SetTarget(Vector3 target) {
//		this.target = target;
//	}

	public void SetTarget(GameObject target) {
		this.target = target;
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
}

public enum SteeringBehaviours
{
//    seeking,
//    arriving,
    pursuing,
//    fleeing,
	attacking,
	none
}
