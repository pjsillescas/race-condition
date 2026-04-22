using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;
using static CameraTrackingPoint;

public class RacerWidget : MonoBehaviour
{
	[SerializeField]
	private TextMeshProUGUI Player1Text;
	[SerializeField]
	private TextMeshProUGUI Player2Text;
	[SerializeField]
	private TextMeshProUGUI Player3Text;
	[SerializeField]
	private TextMeshProUGUI Player4Text;

	private List<TextMeshProUGUI> playerTexts;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		playerTexts = new() { Player1Text, Player2Text, Player3Text, Player4Text };

		playerTexts.ForEach(text => text.text = "...");
	}

	private void OnEnable()
	{
		CameraTrackingPoint.OnCarSorted += OnCarSorted;
	}

	private void OnDisable()
	{
		CameraTrackingPoint.OnCarSorted -= OnCarSorted;
	}

	private void OnCarSorted(object sender, List<CarTrackingDataDTO> data)
	{
		if(playerTexts == null || playerTexts.Count == 0)
		{
			return;
		}

		data = new(data);
		data.Reverse();
		int k = 0;
		playerTexts.ForEach((text) => {
			if (k < data.Count)
			{
				var carName = data[k].car.GetCarData()?.carName ?? "IA";
				var score = data[k].score;
				text.text = $"{carName} {score}";
			}
			else
			{
				text.text = "...";
			}

			k++;
		});
	}
}
