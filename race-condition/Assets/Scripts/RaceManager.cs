using UnityEngine;

public class RaceManager : MonoBehaviour
{
	[SerializeField]
	private CarSetup PlayerCar;
	[SerializeField]
	private CarDataSO PlayerCarData;

	// Start is called before the first frame update
	void Start()
	{
		PlayerCar.Setup(PlayerCarData);
	}
}
