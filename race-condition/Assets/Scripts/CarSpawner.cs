using System.Collections.Generic;
using Unity.VisualScripting;
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

		var data = gameData.GetPlayersData();
		gameData.GetAIData().ForEach(d => data.Add(d));
		SpawnCars(data);
	}

	// Update is called once per frame
	void Update()
	{

	}

	public void SpawnCars(List<PlayerDataSO> playersData)
	{
		if (circuit == null || PlayerCarPrefab == null || PlayerCarPrefab == null)
		{
			return;
		}

		var carCount = playersData.Count;

		//float circuitLength = circuit.GetTotalLength();

		for (int i = 0; i < carCount; i++)
		{
			var playerData = playersData[i];
			Vector3 spawnPos = circuit.GetPoint(0f);
			Quaternion spawnRot = Quaternion.LookRotation(circuit.GetTangent(0f));

			float lateralOffset = (i - carCount / 2f) * 2f;
			Vector3 right = Vector3.Cross(circuit.GetTangent(0f), Vector3.up).normalized;
			spawnPos += right * lateralOffset;

			
			var carPrefab = playerData.isIA ? AICarPrefab : PlayerCarPrefab;
			CarController car = Instantiate(carPrefab, spawnPos, spawnRot).GetComponent<CarController>();

			var carName = playerData.isIA ? "ai" : "player";
			car.name = $"{carName}_{i}";
			//car.Setup(playerData);
			car.GetComponent<CarSetup>().Setup(playerData);
			
			if(car.TryGetComponent(out CircuitAIControl aiControl))
			{
				aiControl.SetCircuit(circuit, i);
				aiControl.ResetToStart();
			}

			spawnedCars.Add(car);
		}
	}
}
