using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class Watchdog : MonoBehaviour
{
	private SplineTrack track;

	private void OnEnable()
	{
		InGameObjectDetector.OnCarEliminated += OnCarEliminated;
		InGameObjectDetector.OnLastCarStanding += OnLastCarStanding;
	}

	private void OnDisable()
	{
		InGameObjectDetector.OnCarEliminated -= OnCarEliminated;
		InGameObjectDetector.OnLastCarStanding -= OnLastCarStanding;
	}

	private void OnCarEliminated(object sender, CarController carController)
	{
		carController.Disable();
	}

	private void OnLastCarStanding(object sender, CarController carController)
	{
		track.GetClosestPointIndexTo(carController.transform.position, out float index, out float _);
		//Debug.Log($"point to {carController.name}");
		FindAnyObjectByType<CarSpawner>().InitCarsInCircuit(index);
	}


	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		track = FindAnyObjectByType<SplineTrack>();
	}

	// Update is called once per frame
	void Update()
	{

	}
}
