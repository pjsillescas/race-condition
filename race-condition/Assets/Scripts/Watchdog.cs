using System.Collections;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class Watchdog : MonoBehaviour
{
	private SplineTrack track;
	private InputManager inputManager;

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
		StartCoroutine(WaitForUser(index));
	}

	private IEnumerator WaitForUser(float index)
	{
		var endOfFrame = new WaitForEndOfFrame();
		while (!inputManager.GetNewRound())
		{
			yield return endOfFrame;
		}

		FindAnyObjectByType<CameraTrackingPoint>().ResetGame();
		FindAnyObjectByType<CarSpawner>().InitCarsInCircuit(index);
	}


	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		track = FindAnyObjectByType<SplineTrack>();
		inputManager = FindAnyObjectByType<InputManager>();
	}

	// Update is called once per frame
	void Update()
	{

	}
}
