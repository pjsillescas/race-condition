using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

[RequireComponent(typeof(CarController))]
public class CarSpeedController : MonoBehaviour
{
	[SerializeField]
	private float MaxSpeedOutOfTrack = 10f;
	[SerializeField]
	private float MaxSpeedInTrack = 50f;

	[SerializeField]
	private GameObject[] Wheels;
	[SerializeField]
	private LayerMask RoadLayerMask;

	private CarController controller;
	private bool isOutOfTrack;

	// Start is called before the first frame update
	void Start()
	{
		controller = GetComponent<CarController>();
		isOutOfTrack = false;
	}

	public void Setup(float MaxSpeedOutOfTrack, float MaxSpeedInTrack)
	{
		this.MaxSpeedInTrack = MaxSpeedInTrack;
		this.MaxSpeedOutOfTrack = MaxSpeedOutOfTrack;
	}

	private bool CheckWheel(GameObject wheel)
	{
		var ray = new Ray(wheel.transform.position, Vector3.down);
		return !Physics.Raycast(ray, float.MaxValue, RoadLayerMask);
	}

	private bool IsOutOfTrack()
	{
		bool isOutOfTrack = false;

		foreach (GameObject wheel in Wheels)
		{
			if (CheckWheel(wheel))
			{
				isOutOfTrack = true;
			}
		}

		return isOutOfTrack;
	}

	// Update is called once per frame
	void Update()
	{
		if (IsOutOfTrack())
		{
			if (!isOutOfTrack)
			{
				isOutOfTrack = true;
				controller.SetMaxSpeed(MaxSpeedOutOfTrack);
			}
		}
		else
		{
			if (isOutOfTrack)
			{
				isOutOfTrack = false;
				controller.SetMaxSpeed(MaxSpeedInTrack);
			}
		}
	}
}
