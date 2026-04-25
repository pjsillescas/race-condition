using System;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class Checkpoint : MonoBehaviour
{
	public static event EventHandler<CarController> OnEndLap;
	//private CarMonitor carMonitor;

	// Start is called before the first frame update
	void Start()
	{
		//carMonitor = FindAnyObjectByType<CarMonitor>();

	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.transform.parent != null //
			&& other.transform.parent.TryGetComponent(out CarController controller) //
			&& controller.GetIsEnabled())
		{
			OnEndLap?.Invoke(this, controller);
			//Debug.Log("End lap");
			//carMonitor.EndLap();
		}

	}
}
