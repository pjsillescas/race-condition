using UnityEngine;

public class JsonMonitor : MonoBehaviour
{
	[SerializeField]
	private GhostLapDataJson CurrentLapData;           // Scriptable object
	[SerializeField]
	private GhostLapDataJson CurrentRaceData;
	[SerializeField]
	private GhostLapDataJson BestLapData;           // Scriptable object

	private CarMonitor carMonitor;

	// Start is called before the first frame update
	void Start()
	{
		carMonitor = FindAnyObjectByType<CarMonitor>();

		carMonitor.OnSampling += OnMonitorSample;
		carMonitor.OnEndLap += OnEndLap;

		CurrentLapData.Reset();
		CurrentRaceData.Reset();
	}

	private void OnMonitorSample(float time, Transform transform)
	{
		CurrentLapData.AddNewData(time, transform);
		CurrentRaceData.AddNewData(time, transform);
	}

	private void OnEndLap(float time)
	{
		float bestLapDuration = BestLapData.GetDuration();
		if (bestLapDuration <= 0 || time < bestLapDuration)
		{
			Debug.Log("new best lap json");
			BestLapData.SetData(CurrentLapData.GetData());
		}

		CurrentLapData.Reset();
	}
}
