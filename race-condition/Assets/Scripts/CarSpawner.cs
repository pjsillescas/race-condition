using System;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class CarSpawner : MonoBehaviour
{
	[SerializeField]
	private GameObject PlayerCarPrefab;
	[SerializeField]
	private GameObject AICarPrefab;
	[SerializeField]
	private List<Transform> DisabledPositions;

	private List<CarController> spawnedCars;

	private SplineTrack circuit;
	private TrafficLightController trafficLight;

	private void OnEnable()
	{
		TrafficLightController.OnRoundStarted += OnRoundStarted;
		Watchdog.OnNewRound += OnNewRound;
	}

	private void OnDisable()
	{
		TrafficLightController.OnRoundStarted -= OnRoundStarted;
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

		trafficLight = FindAnyObjectByType<TrafficLightController>();
		trafficLight.Disable();

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
			car.Setup(playerData, DisabledPositions[i]);

			if (car.TryGetComponent(out CircuitAIControl aiControl))
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
			var leftOffset = i % 2 == 0 ? 0 : 1;
			float lateralOffset = (leftOffset - carCount / 2f) * 4f + 6f;
			Vector3 right = Vector3.Cross(tangent, Vector3.up).normalized;


			int frontOffset = -(i / 2);
			float frontalOffset = frontOffset * 8f;
			Vector3 front = tangent.normalized;
			var spawnPosition = spawnPos + right * lateralOffset + frontalOffset * front;

			var car = spawnedCars[i];
			car.transform.SetPositionAndRotation(spawnPosition, spawnRot);

			var aiControl = car.GetComponent<CircuitAIControl>();
			if (aiControl != null)
			{
				aiControl.SetCurrentT(t);
			}

			car.Enable();
		}

		//raceStartManager.StartNewRound(EnableCars);
		if (trafficLight != null)
		{
			trafficLight.Enable();
			trafficLight.StartTrafficSequence();
		}

	}

	private void OnRoundStarted(object sender, EventArgs args)
	{
		spawnedCars.ForEach(car => car.EnableMovement());
		trafficLight.Disable();
	}
}
