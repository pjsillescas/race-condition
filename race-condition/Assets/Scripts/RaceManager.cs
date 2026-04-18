using UnityEngine;

public class RaceManager : MonoBehaviour
{
	[SerializeField]
	private string CarType;
	[SerializeField]
	private CarSetup PlayerCar;
	[SerializeField]
	private CarSetup GhostCar;

	private CarMonitor carMonitor;
	// Start is called before the first frame update
	void Start()
	{
		carMonitor = FindAnyObjectByType<CarMonitor>();
		carMonitor.OnEndRace += OnEndRace;

		CarType = GameData.CarType;
		PlayerCar.Setup(CarType);
		GhostCar.Setup(CarType);
	}

	private void OnEndRace(float time)
	{
	}
}
