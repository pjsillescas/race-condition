using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class CameraTrackingPoint : MonoBehaviour
{
	public static event EventHandler<List<CarTrackingDataDTO>> OnCarSorted;

	public class CarTrackingDataDTO
	{
		public CarController car;
		public float score;
		public int lap;
	}

	private List<CarTrackingDataDTO> cars;
	private SplineTrack circuit;


	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		circuit = FindAnyObjectByType<SplineTrack>();

		cars = null;
		StartCoroutine(DetectCars());
	}

	private IEnumerator DetectCars()
	{
		yield return new WaitForSeconds(1f);

		cars = new List<CarController>(FindObjectsByType<CarController>()) //
			.Select(controller => new CarTrackingDataDTO() { car = controller, score = 0f, lap = 0, }) //
			.ToList();

	}
	/*
	private Vector3 carPosition = Vector3.zero;
	private Vector3 circuitPosition = Vector3.zero;

	private void OnDrawGizmos()
	{
		Gizmos.DrawSphere(carPosition, 1f);
		Gizmos.DrawSphere(circuitPosition, 1f);
		Debug.Log($"circuitposition {circuitPosition}");
	}
	*/
	private Vector3 GetCurrentPosition()
	{
		var activeCars = cars.Where(data => data.car.isActiveAndEnabled).ToList();

		activeCars.ForEach(data =>
		{
			var carPosition = new Vector3(data.car.transform.position.x, 0, data.car.transform.position.z);
			circuit.GetClosestPointIndexTo(carPosition, out float index, out float distance);

			data.score = (distance < 20f) ? index : 0;
		});

		activeCars.Sort((data1, data2) =>
		{
			if (data1.lap == data2.lap)
			{
				if (data1.score == data2.score)
				{
					return 0;
				}
				else
				{
					return (data1.score < data2.score) ? -1 : 1;
				}
			}
			else
			{
				return data1.lap < data2.lap ? -1 : 1;
			}
		});

		OnCarSorted?.Invoke(this, activeCars);

		var weightedPositions = activeCars.Select(data => data.car.transform.position).ToList();

		for (int i = 0; i < weightedPositions.Count; i++)
		{
			weightedPositions[i] = GetWeight(i, weightedPositions.Count) * weightedPositions[i];
		}

		return weightedPositions.Aggregate(Vector3.zero, (acc, val) => val + acc) / activeCars.Count;
	}

	private float GetWeight(int index, int count)
	{
		var weights2 = new float[] { 0.5f, 1.5f };
		var weights3 = new float[] { 0.4f, 0.8f, 1.8f };

		float weight;
		weight = count switch
		{
			3 => weights3[index],
			2 => weights2[index],
			_ => 1f,
		};
		return weight;
	}

	// Update is called once per frame
	void Update()
	{
		if (cars == null || cars.Count == 0)
		{
			return;
		}

		var currentPosition = GetCurrentPosition();
		transform.position = circuit.GetClosestPointTo(currentPosition);
	}
}
