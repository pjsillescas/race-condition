using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class InGameObjectDetector : MonoBehaviour
{
	public static event EventHandler<List<CarController>> OnCarsDetected;
	public static event EventHandler<CarController> OnCarEliminated;
	public static event EventHandler<CarController> OnLastCarStanding;

	private List<CarController> cars;
	private CameraManager cameraManager;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		StartCoroutine(RefreshCarsCoroutine());
		cameraManager = FindAnyObjectByType<CameraManager>();
	}

	private IEnumerator RefreshCarsCoroutine()
	{
		yield return new WaitForSeconds(0.5f);
		ResetComponent();
		OnCarsDetected?.Invoke(this, cars);
	}

	private void ResetComponent()
	{
		cars = new(FindObjectsByType<CarController>());
	}

	private void OnTriggerExit(Collider other)
	{
		var controller = other.gameObject.GetComponentInParent<CarController>();
		if (controller != null && controller.GetIsEnabled())
		{
			controller.Disable();
			cars.Remove(controller);
			OnCarEliminated?.Invoke(this, controller);

			if (cars.Count == 1)
			{
				var lastCar = cars.First();
				lastCar.Disable();
				var camera = lastCar.GetFocusCamera();
				cameraManager.SetCamera(camera);
				Debug.Log($"{lastCar.name} wins round");

				OnLastCarStanding?.Invoke(this, lastCar);

				ResetComponent();
			}
		}
	}
}
