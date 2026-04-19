using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class InGameObjectDetector : MonoBehaviour
{
	public static event EventHandler<CarController> OnCarEliminated;
	public static event EventHandler<CarController> OnLastCarStanding;

	private List<CarController> cars;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		StartCoroutine(DetectCars());
	}

	private IEnumerator DetectCars()
	{
		yield return new WaitForSeconds(0.5f);
		cars = new(FindObjectsByType<CarController>());
		Debug.Log($"detector detected {cars.Count}");
	}

	private void OnTriggerEnter(Collider other)
	{
		Debug.Log($"trigger enter {other.name}");
	}

	private void OnTriggerExit(Collider other)
	{
		Debug.LogWarning($"exiting {other.name}");
		var controller = other.gameObject.GetComponentInParent<CarController>();
		if (controller != null)
		{
			cars.Remove(controller);
			OnCarEliminated?.Invoke(this, controller);

			if (cars.Count == 1)
			{
				OnLastCarStanding?.Invoke(this, cars.First());
			}
		}
		/*
		if (other.gameObject.TryGetComponent(out CarController carController) && cars != null)
		{
			cars.Remove(carController);
			OnCarEliminated?.Invoke(this, carController);

			if (cars.Count == 1)
			{
				OnLastCarStanding?.Invoke(this, cars.First());
			}
		}
		*/
	}
}
