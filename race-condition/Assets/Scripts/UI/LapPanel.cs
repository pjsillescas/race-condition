using TMPro;
using UnityEngine;

public class LapPanel : MonoBehaviour
{
	public int LapNumber;
	public TextMeshProUGUI TitleText;
	public TextMeshProUGUI TimeText;

	private bool isActive;
	private CarMonitor carMonitor;

	private void Awake()
	{
		TitleText.text = $"Lap {LapNumber}";
		isActive = false;
	}

	// Start is called before the first frame update
	void Start()
	{
		carMonitor = FindAnyObjectByType<CarMonitor>();

		carMonitor.OnTick += CarMonitor_OnTick;
		carMonitor.OnEndRace += CarMonitor_OnEndRace;
		carMonitor.OnBeginLap += CarMonitor_OnBeginLap;
	}

	private void CarMonitor_OnEndRace(float time)
	{
		Debug.Log("end race panel " + LapNumber);
		carMonitor.OnTick -= CarMonitor_OnTick;
		carMonitor.OnEndRace -= CarMonitor_OnEndRace;
	}

	private void CarMonitor_OnBeginLap(int lapNumber)
	{
		Debug.Log($"begin lap {lapNumber}");
		if (lapNumber + 1 == this.LapNumber)
		{
			TitleText.color = Color.green;
			TitleText.fontSize = 45;
		}
		else
		{
			TitleText.color = Color.white;
			TitleText.fontSize = 36;
		}
	}

	private void CarMonitor_OnTick(float time)
	{
		if (isActive)
		{
			TimeText.text = time.ToString("000.00");
		}
	}

	public void SetIsActive(bool isActive)
	{
		Debug.Log($"set active {LapNumber} {isActive}");
		this.isActive = isActive;
	}
}
