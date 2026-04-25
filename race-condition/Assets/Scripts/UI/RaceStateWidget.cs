using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class RaceStateWidget : MonoBehaviour
{
	[SerializeField]
	List<RacerWidgetAdvanced> RacerWidgets;

	private void OnEnable()
	{
		InGameObjectDetector.OnCarsDetected += OnCarsDetected;
	}

	private void OnDisable()
	{
		InGameObjectDetector.OnCarsDetected -= OnCarsDetected;
	}

	private void OnCarsDetected(object sender, List<CarController> cars)
	{
		int i = 0;
		cars.ForEach(car => {
			if (i < RacerWidgets.Count)
			{
				RacerWidgets[i].Init(car);
			}
			i++;
		});
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
