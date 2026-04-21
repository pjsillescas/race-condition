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
	}

	private void OnTriggerExit(Collider other)
	{
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
	}
}
