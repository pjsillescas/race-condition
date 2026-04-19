using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class DetectionTest : MonoBehaviour
{
	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{

	}

	private void OnEnable()
	{
		InGameObjectDetector.OnCarEliminated += OnCarEliminated;
		InGameObjectDetector.OnLastCarStanding += OnLastCarStanding;
	}

	private void OnCarEliminated(object sender, CarController carController)
	{
		Debug.Log($"eliminated {carController.name}");
	}

	private void OnLastCarStanding(object sender, CarController carController)
	{
		Debug.Log($"winner {carController.name}");
	}

	// Update is called once per frame
	void Update()
	{

	}
}
