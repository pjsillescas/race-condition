using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Vehicles.Car;

public class Watchdog : MonoBehaviour
{
	public static event EventHandler<float> OnNewRound;

	private SplineTrack track;
	private InputManager inputManager;
	private bool isGameOver;

	private void OnEnable()
	{
		InGameObjectDetector.OnCarEliminated += OnCarEliminated;
		InGameObjectDetector.OnLastCarStanding += OnLastCarStanding;
		RacerWidgetAdvanced.OnGameWon += OnGameWon;
	}

	private void OnDisable()
	{
		InGameObjectDetector.OnCarEliminated -= OnCarEliminated;
		InGameObjectDetector.OnLastCarStanding -= OnLastCarStanding;
		RacerWidgetAdvanced.OnGameWon -= OnGameWon;
	}

	private void OnGameWon(object sender, CarController controller)
	{
		isGameOver = true;
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

		if (isGameOver)
		{
			SceneManager.LoadScene("MainMenu");
		}
		else
		{
			OnNewRound?.Invoke(this, index);
		}
	}


	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		isGameOver = false;

		track = FindAnyObjectByType<SplineTrack>();
		inputManager = FindAnyObjectByType<InputManager>();
	}

	// Update is called once per frame
	void Update()
	{

	}
}
