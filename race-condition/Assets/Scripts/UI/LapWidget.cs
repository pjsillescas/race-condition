using System.Collections.Generic;
using UnityEngine;

public class LapWidget : MonoBehaviour
{
	[SerializeField]
	private List<LapPanel> LapPanels;

	private CarMonitor carMonitor;

	private void Start()
	{
		carMonitor = FindAnyObjectByType<CarMonitor>();

		carMonitor.OnBeginLap += CarMonitor_OnBeginLap;
		carMonitor.OnEndRace += CarMonitor_OnEndRace;
	}

	private void CarMonitor_OnBeginLap(int lapNumber)
	{
		SwitchToLap(lapNumber + 1);
	}

	private void CarMonitor_OnEndRace(float time)
	{
		SwitchToLap(LapPanels.Count + 1);

		carMonitor.OnEndRace -= CarMonitor_OnEndRace;
	}

	public void SwitchToLap(int lapNumber)
	{
		Debug.Log($"switch to lap {lapNumber}");
		foreach (LapPanel lapPanel in LapPanels)
		{
			lapPanel.SetIsActive(lapNumber == lapPanel.LapNumber);
		}
	}
}
