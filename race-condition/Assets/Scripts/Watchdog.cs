using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class Watchdog : MonoBehaviour
{
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
		Debug.Log($"point to {carController.name}");
		FindAnyObjectByType<CarSpawner>().InitCarsInCircuit(0f);
	}


	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}
}
