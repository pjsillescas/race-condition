using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Cinemachine;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

[RequireComponent(typeof(BoxCollider))]
public class CameraTrackingPoint : MonoBehaviour
{
	public static event EventHandler<List<CarTrackingDataDTO>> OnCarSorted;

	public class CarTrackingDataDTO
	{
		public CarController car;
		public float sortScore;
		public int lap;
	}

	[SerializeField]
	private CinemachineCamera GameCamera;
	
	private List<CarTrackingDataDTO> cars;
	private SplineTrack circuit;
	private CameraManager cameraManager;
	private BoxCollider boxCollider;
	private Vector3 boxColliderCenter;
	private Vector3 oldPosition;

	private void OnEnable()
	{
		Checkpoint.OnEndLap += OnControllerEndLap;
		Watchdog.OnNewRound += OnNewRound;
		CarController.OnCarSpawned += OnCarSpawned;
	}

	private void OnDisable()
	{
		Checkpoint.OnEndLap -= OnControllerEndLap;
		Watchdog.OnNewRound -= OnNewRound;
		CarController.OnCarSpawned -= OnCarSpawned;
	}

	private void OnNewRound(object sender, float e)
	{
		ResetGame();
	}

	private void OnControllerEndLap(object sender, CarController controller)
	{
		var car = cars.First(car => car.car == controller);
		car.lap++;

		SortCars();
	}

#if UNITY_EDITOR
	private void OnDrawGizmosSelected()
	{
		if (Application.isPlaying)
		{
			Gizmos.DrawWireCube(transform.position + boxCollider.center, 65f * Vector3.one);

			Gizmos.DrawSphere(boxCollider.center + transform.position, 2f);
		}
	}
#endif


	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		circuit = FindAnyObjectByType<SplineTrack>();
		cameraManager = FindAnyObjectByType<CameraManager>();
		boxCollider = GetComponent<BoxCollider>();
		boxColliderCenter = boxCollider.center;
		oldPosition = circuit.GetClosestPointTo(Vector3.zero);

		cars = new();
		//StartCoroutine(DetectCars());

		ResetGame();
	}

	public void ResetGame()
	{
		Debug.Log("setting game camera");
		cameraManager.SetCamera(GameCamera);
	}

	private IEnumerator DetectCars()
	{
		yield return new WaitForSeconds(0.5f);

		cars = new List<CarController>(FindObjectsByType<CarController>()) //
			.Select(controller => new CarTrackingDataDTO() { car = controller, sortScore = 0f, lap = 0, }) //
			.ToList();
	}
	
	private void OnCarSpawned(object sender, CarController controller)
	{
		cars.Add(new CarTrackingDataDTO() { car = controller, sortScore = 0f, lap = 0, });
	}

	private List<CarTrackingDataDTO> SortCars()
	{
		var activeCars = cars.Where(data => data.car.GetIsEnabled()).ToList();

		activeCars.ForEach(data =>
		{
			var carPosition = new Vector3(data.car.transform.position.x, 0, data.car.transform.position.z);
			circuit.GetClosestPointIndexTo(carPosition, out float index, out float distance);

			data.sortScore = (distance < 20f) ? index : 0;
		});

		activeCars.Sort((data1, data2) =>
		{
			if (data1.lap == data2.lap)
			{
				if (data1.sortScore == data2.sortScore)
				{
					return 0;
				}
				else
				{
					return (data1.sortScore < data2.sortScore) ? -1 : 1;
				}
			}
			else
			{
				return data1.lap < data2.lap ? -1 : 1;
			}
		});

		OnCarSorted?.Invoke(this, activeCars);

		return activeCars;
	}

	private Vector3 GetCurrentPosition()
	{
		var activeCars = SortCars();

		var weightedPositions = activeCars.Select(data => data.car.transform.position).ToList();

		for (int i = 0; i < weightedPositions.Count; i++)
		{
			weightedPositions[i] = GetWeight(i, weightedPositions.Count) * weightedPositions[i];
		}

		return weightedPositions.Aggregate(Vector3.zero, (acc, val) => val + acc) / weightedPositions.Count;
	}

	private float GetWeight(int index, int count)
	{
		var weights2 = new float[] { 0.5f, 1.5f };
		var weights3 = new float[] { 0.3f, 0.5f, 2.2f };
		var weights4 = new float[] { 0.1f, 0.3f, 0.4f, 3.2f };

		float weight;
		weight = count switch
		{
			4 => weights4[index],
			3 => weights3[index],
			2 => weights2[index],
			_ => 1f,
		};
		return weight;
	}

	//const float COLLIDER_OFFSET = 5f;
	const float COLLIDER_OFFSET = 7f;

	// Update is called once per frame
	void Update()
	{
		if (cars == null || cars.Count == 0)
		{
			return;
		}

		var currentPosition = GetCurrentPosition();
		var position = circuit.GetClosestPointTo(currentPosition);
		var direction = (position - oldPosition).normalized;
		oldPosition = position;
		transform.position = position;

		boxCollider.center = boxColliderCenter + direction * COLLIDER_OFFSET;
	}
}
