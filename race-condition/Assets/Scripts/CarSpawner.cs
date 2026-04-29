using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class CarSpawner : MonoBehaviour
{
	[SerializeField]
	private GameObject PlayerCarPrefab;
	[SerializeField]
	private GameObject AICarPrefab;

	private List<CarController> spawnedCars;

	private SplineTrack circuit;

	private void OnEnable()
	{
		Watchdog.OnNewRound += OnNewRound;
	}

	private void OnDisable()
	{
		Watchdog.OnNewRound -= OnNewRound;
	}

	private void OnNewRound(object sender, float index)
	{
		InitCarsInCircuit(index);
	}


	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		spawnedCars = new();
		circuit = FindAnyObjectByType<SplineTrack>();

		var gameData = FindAnyObjectByType<GameData>();

		var data = gameData.GetPlayersData();
		gameData.GetAIData().ForEach(d => data.Add(d));
		SpawnCars(data);
	}

	public void SpawnCars(List<PlayerDataSO> playersData)
	{
		if (circuit == null || PlayerCarPrefab == null || PlayerCarPrefab == null)
		{
			return;
		}

		var carCount = playersData.Count;

		// spawn cars
		for (int i = 0; i < carCount; i++)
		{
			var playerData = playersData[i];

			var carPrefab = playerData.isIA ? AICarPrefab : PlayerCarPrefab;
			CarController car = Instantiate(carPrefab, Vector3.zero, Quaternion.identity).GetComponent<CarController>();
			car.Disable();

			var carName = playerData.isIA ? "ai" : "player";
			car.name = $"{carName}_{i}";
			car.Setup(playerData);
			
			if(car.TryGetComponent(out CircuitAIControl aiControl))
			{
				aiControl.SetCircuit(circuit, i - 1);
				aiControl.ResetToStart();
			}

			spawnedCars.Add(car);
		}

		// place cars
		InitCarsInCircuit(0f);
	}

	public void InitCarsInCircuit(float t)
	{
		Vector3 spawnPos = circuit.GetPoint(t);
		var tangent = circuit.GetTangent(t);

		Quaternion spawnRot = Quaternion.LookRotation(tangent);
		var carCount = spawnedCars.Count;
		for (int i = 0; i < carCount; i++)
		{
			float lateralOffset = (i - carCount / 2f) * 3f + 2f;
			Vector3 right = Vector3.Cross(tangent, Vector3.up).normalized;
			var spawnPosition = spawnPos + right * lateralOffset;

			var car = spawnedCars[i];
			car.transform.SetPositionAndRotation(spawnPosition, spawnRot);

			var aiControl = car.GetComponent<CircuitAIControl>();
			if(aiControl != null)
			{
				aiControl.SetCurrentT(t);
			}

			//StartCoroutine(EnableCarCoroutine(car));
			car.Enable();
		}
	}

	private IEnumerator EnableCarCoroutine(CarController car)
	{
		car.Disable();
		
		yield return new WaitForSeconds(0.5f);
		
		car.Enable();
	}
}
