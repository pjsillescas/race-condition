using System.Collections.Generic;
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
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		spawnedCars = new();
		circuit = FindAnyObjectByType<SplineTrack>();

		var gameData = FindAnyObjectByType<GameData>();

		SpawnCars(gameData.GetPlayersData(), gameData.GetAIData());
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void SpawnCars(List<PlayerDataSO> playersData, List<PlayerDataSO> aiData)
	{
		if (circuit == null || PlayerCarPrefab == null || PlayerCarPrefab == null)
		{
			return;
		}

		var numPlayerCars = playersData.Count;
		var numAiCars = aiData.Count;
		var carCount = numPlayerCars + numAiCars;

		//float circuitLength = circuit.GetTotalLength();

		for (int i = 0; i < carCount; i++)
		{
			Vector3 spawnPos = circuit.GetPoint(0f);
			Quaternion spawnRot = Quaternion.LookRotation(circuit.GetTangent(0f));

			float lateralOffset = (i - carCount / 2f) * 2f;
			Vector3 right = Vector3.Cross(circuit.GetTangent(0f), Vector3.up).normalized;
			spawnPos += right * lateralOffset;

			
			var carPrefab = (i >= numPlayerCars) ? AICarPrefab : PlayerCarPrefab;
			CarController car = Instantiate(carPrefab, spawnPos, spawnRot).GetComponent<CarController>();

			var carName = (i >= numPlayerCars) ? "player" : "ai";
			car.name = $"{carName}_{i}";
			var playerData = (i >= numPlayerCars) ? aiData[i - numPlayerCars] : playersData[i];
			//car.Setup(playerData);
			car.GetComponent<CarSetup>().Setup(playerData.carData);
			
			if(car.TryGetComponent(out CircuitAIControl aiControl))
			{
				aiControl.SetCircuit(circuit, i - numPlayerCars);
				aiControl.ResetToStart();
			}

			spawnedCars.Add(car);
		}
	}
}
