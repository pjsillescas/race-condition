using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

[RequireComponent(typeof(CarController))]
[RequireComponent(typeof(Rigidbody))]
public class CircuitAIControl : MonoBehaviour
{
	[SerializeField]
	private SplineTrack circuit;

	[SerializeField]
	private float waypointDistance = 10f;

	[SerializeField]
	private float lateralOffset = 2f;

	[SerializeField]
	private float steerSensitivity = 0.1f;

	[SerializeField]
	private float accelSensitivity = 0.05f;

	[SerializeField]
	private float brakeSensitivity = 0.5f;

	[SerializeField]
	private float cautiousSpeedFactor = 0.3f;

	[SerializeField]
	private int carIndex;

	[SerializeField]
	private bool ignoreOtherCars;

	private CarController carController;
	//private Rigidbody rigidbody;
	//private int currentWaypointIndex;
	private float currentT;
	private float randomOffset;
	private float lateralWanderTime;

	private void Awake()
	{
		carController = GetComponent<CarController>();
		//rigidbody = GetComponent<Rigidbody>();
		randomOffset = Random.Range(-lateralOffset, lateralOffset);
		lateralWanderTime = Random.Range(0f, 10f);
	}

	private void FixedUpdate()
	{
		if (carController == null)
		{
			return;
		}

		if (circuit == null || circuit.Points.Count < 2)
		{
			carController.Move(0, 0, 0, 1f);
			return;
		}

		float currentSpeed = carController.CurrentSpeed;
		float maxSpeed = carController.MaxSpeed;

		float desiredSpeed = maxSpeed;
		Vector3 targetPos = GetTargetPosition(out float targetAngle);

		if (!ignoreOtherCars)
		{
			targetPos = ApplyCarAvoidance(targetPos);
		}

		targetPos = ApplyLateralWander(targetPos);
		//var len = (targetPos - transform.position).magnitude;
		// Debug.Log($"{targetPos.x},{targetPos.y},{targetPos.z},{transform.position.x},{transform.position.y},{transform.position.z},{currentT},{len}");


		Vector3 localTarget = transform.InverseTransformPoint(targetPos);
		float distanceToTarget = localTarget.magnitude;

		float angleToTarget = Mathf.Atan2(localTarget.x, localTarget.z) * Mathf.Rad2Deg;
		float steer = Mathf.Clamp(angleToTarget * steerSensitivity, -1, 1) * Mathf.Sign(currentSpeed + 0.1f);

		bool isTurning = Mathf.Abs(angleToTarget) > 30f;
		bool isApproachingCorner = Mathf.Abs(targetAngle) > 45f;

		if (isApproachingCorner || isTurning)
		{
			desiredSpeed = Mathf.Lerp(maxSpeed, maxSpeed * cautiousSpeedFactor, 
				Mathf.Abs(targetAngle) / 180f);
		}

		float accelBrake = (desiredSpeed < currentSpeed) ? brakeSensitivity : accelSensitivity;
		float accel = Mathf.Clamp((desiredSpeed - currentSpeed) * accelBrake, -1f, 1f);

		//const float minDistanceToTarget = 5f;
		const float minDistanceToTarget = 10f;

		if (distanceToTarget < minDistanceToTarget)
		{
			currentT += waypointDistance / circuit.GetTotalLength();

			if (currentT > 1)
			{
				currentT -= 1f;
			}
		}

		carController.Move(steer, accel, accel, 0f);
	}

	private Vector3 GetTargetPosition(out float upcomingTurnAngle)
	{
		float t = currentT;
		float futureT = Mathf.Clamp01(t + 0.05f);

		Vector3 currentPos = circuit.GetPoint(t);
		Vector3 futurePos = circuit.GetPoint(futureT);

		Vector3 tangent = circuit.GetTangent(t);
		Vector3 right = Vector3.Cross(tangent, Vector3.up).normalized;

		float offset = (carIndex % 2 == 0) ? randomOffset : -randomOffset;
		offset += Mathf.Sin(Time.time + lateralWanderTime) * 0.5f;

		Vector3 targetPos = currentPos + right * offset;

		upcomingTurnAngle = Vector3.SignedAngle(transform.forward, futurePos - currentPos, Vector3.up);

		return targetPos;
	}

	private Vector3 ApplyLateralWander(Vector3 targetPos)
	{
		float wander = Mathf.Sin(Time.time * 0.5f + lateralWanderTime) * lateralOffset * 0.3f;
		Vector3 tangent = circuit.GetTangent(currentT);
		Vector3 right = Vector3.Cross(tangent, Vector3.up).normalized;
		return targetPos + right * wander;
	}

	private Vector3 ApplyCarAvoidance(Vector3 targetPos)
	{
		float detectionRadius = 8f;
		Collider[] nearbyCars = Physics.OverlapSphere(transform.position, detectionRadius);
		
		foreach (Collider col in nearbyCars)
		{
			if (col.gameObject == gameObject)
			{
				continue;
			}

			if (col.TryGetComponent<CircuitAIControl>(out var otherAI))
			{
				Vector3 toOther = col.transform.position - transform.position;
				float dot = Vector3.Dot(transform.forward, toOther.normalized);
				
				if (dot > 0 && toOther.magnitude < detectionRadius)
				{
					Vector3 localOther = transform.InverseTransformPoint(col.transform.position);
					float side = Mathf.Sign(localOther.x);
					targetPos += transform.right * side * 2f;
				}
			}
		}
		
		return targetPos;
	}

	public void SetCircuit(SplineTrack circuit, int carIndex)
	{
		this.circuit = circuit;
		this.carIndex = carIndex;
		currentT = 0f;
	}

	public void ResetToStart()
	{
		if (circuit != null && circuit.Points.Count > 0)
		{
			transform.SetPositionAndRotation(circuit.GetPoint(0f) + Vector3.up * 0.5f, Quaternion.LookRotation(circuit.GetTangent(0f)));
			currentT = 0f;
		}
	}
}