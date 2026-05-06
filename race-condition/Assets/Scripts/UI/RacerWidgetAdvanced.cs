using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;
using static CameraTrackingPoint;

public class RacerWidgetAdvanced : MonoBehaviour
{
	public static event EventHandler<CarController> OnGameWon;

	[SerializeField]
	private TextMeshProUGUI PlayerText;
	[SerializeField]
	private TextMeshProUGUI LapText;
	[SerializeField]
	private TextMeshProUGUI PositionText;
	[SerializeField]
	private ScoreWidget ScoreWidget;
	[SerializeField]
	private WinnerWidget WinnerWidget;

	private CarController carController;

	public void Init(CarController controller)
	{
		carController = controller;

		var playerData = controller.GetPlayerData();

		ResetWidget(playerData.playerName);
		ScoreWidget.SetScoreMaterial(playerData.scoreMaterial);
		ScoreWidget.ResetScore();
	}

	private void ResetWidget(string playerName)
	{
		PlayerText.text = playerName;
		LapText.text = "0";
		PositionText.text = "--";
	}

	private void OnEnable()
	{
		CameraTrackingPoint.OnCarSorted += OnCarSorted;
		InGameObjectDetector.OnCarEliminated += OnCarEliminated;
		InGameObjectDetector.OnLastCarStanding += OnLastCarStanding;
	}

	private void OnDisable()
	{
		CameraTrackingPoint.OnCarSorted -= OnCarSorted;
		InGameObjectDetector.OnCarEliminated -= OnCarEliminated;
		InGameObjectDetector.OnLastCarStanding -= OnLastCarStanding;
	}

	private void OnCarSorted(object sender, List<CarTrackingDataDTO> cars)
	{
		/*
		var index = -1;
		var carFound = false;
		while (index < cars.Count && !carFound)
		{
			index++;
			if (index < cars.Count && cars[index].car.Equals(carController))
			{
				carFound = true;
			}
		}

		if (carFound)
		{
			PositionText.text = (cars.Count - index).ToString();
			LapText.text = cars[index].lap.ToString();
		}
		else
		{
			PositionText.text = "--";
			LapText.text = "0";
		}
		*/
	}

	private void OnCarEliminated(object sender, CarController controller)
	{
		if (controller.Equals(carController))
		{
			// eliminated view
			;
		}
	}

	private void OnLastCarStanding(object sender, CarController controller)
	{
		if (controller.Equals(carController))
		{
			//Debug.Log($"add score to {controller.name}");
			ScoreWidget.AddScore();

			WinnerWidget.Display(controller.GetPlayerData().playerName);
			
			if (ScoreWidget.GetScore() >= 10)
			{
				OnGameWon?.Invoke(this, controller);
			}
		}
		else
		{
			ScoreWidget.SubtractScore();
		}
	}

	private void Awake()
	{
		ResetWidget("---");
		carController = null;
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{

	}
}
