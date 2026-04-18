using UnityEngine;

public class ScriptableObjectMonitor : MonoBehaviour
{
	public GhostLapData CurrentLapSO;           // Scriptable object
	public GhostLapData CurrentRaceSO;
	public GhostLapData BestLapSO;           // Scriptable object

	private CarMonitor carMonitor;

	// Start is called before the first frame update
	void Start()
	{
		carMonitor = FindAnyObjectByType<CarMonitor>();

		carMonitor.OnSampling += OnMonitorSample;
		carMonitor.OnEndLap += OnEndLap;

		CurrentLapSO.Reset();
		CurrentRaceSO.Reset();
	}

	private void OnMonitorSample(float time, Transform transform)
	{
		CurrentLapSO.AddNewData(time, transform);
		CurrentRaceSO.AddNewData(time, transform);
	}

	private void OnEndLap(float time)
	{
		float bestLapDuration = BestLapSO.GetDuration();
		if (bestLapDuration <= 0 || time < bestLapDuration)
		{
			Debug.Log("new best lap");
			BestLapSO.SetData(CurrentLapSO.GetData());
		}

		CurrentLapSO.Reset();
	}
}
