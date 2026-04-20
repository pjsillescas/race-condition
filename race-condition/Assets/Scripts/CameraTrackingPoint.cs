using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class CameraTrackingPoint : MonoBehaviour
{
	private List<CarController> cars;
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

		cars = new(FindObjectsByType<CarController>());
		Debug.Log($"detected {cars.Count} cars");

	}

	private Vector3 GetCurrentPosition()
	{
		var activeCars = cars.Where(car => car.isActiveAndEnabled).ToList();

		return activeCars.Select(car => car.transform.position).Aggregate(Vector3.zero, (acc, val) => val + acc) / activeCars.Count;
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
