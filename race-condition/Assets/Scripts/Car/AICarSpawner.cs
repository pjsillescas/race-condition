using System.Collections.Generic;
using UnityEngine;

public class AICarSpawner : MonoBehaviour
{
	[SerializeField]
	private SplineTrack circuit;

	[SerializeField]
	private GameObject carPrefab;

	[SerializeField]
	private int carCount = 3;

	[SerializeField]
	private float spawnSpacing = 3f;

	[SerializeField]
	private float startDelay = 2f;

	private List<GameObject> spawnedCars = new List<GameObject>();
	private bool raceStarted;

	private void Start()
	{
		if (circuit == null || carPrefab == null) return;

		float circuitLength = circuit.GetTotalLength();
		float startOffset = 0f;

		for (int i = 0; i < carCount; i++)
		{
			Vector3 spawnPos = circuit.GetPoint(0f);
			Quaternion spawnRot = Quaternion.LookRotation(circuit.GetTangent(0f));

			float lateralOffset = (i - carCount / 2f) * 2f;
			Vector3 right = Vector3.Cross(circuit.GetTangent(0f), Vector3.up).normalized;
			spawnPos += right * lateralOffset;

			GameObject car = Instantiate(carPrefab, spawnPos, spawnRot);
			CircuitAIControl aiControl = car.GetComponent<CircuitAIControl>();
			
			if (aiControl != null)
			{
				aiControl.SetCircuit(circuit, i);
				aiControl.ResetToStart();
			}

			spawnedCars.Add(car);
		}

		Invoke(nameof(StartRace), startDelay);
	}

	private void StartRace()
	{
		raceStarted = true;
	}

	public void StopRace()
	{
		raceStarted = false;
		foreach (GameObject car in spawnedCars)
		{
			if (car != null)
			{
				CircuitAIControl ai = car.GetComponent<CircuitAIControl>();
				if (ai != null)
				{
					ai.enabled = false;
				}
			}
		}
	}

	public void SetCircuit(SplineTrack circuit)
	{
		this.circuit = circuit;
	}

	public void SetCarPrefab(GameObject prefab)
	{
		this.carPrefab = prefab;
	}

	public List<GameObject> GetSpawnedCars()
	{
		return spawnedCars;
	}

	public int ActiveCarCount
	{
		get
		{
			int count = 0;
			foreach (GameObject car in spawnedCars)
			{
				if (car != null) count++;
			}
			return count;
		}
	}
}