using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class BackBooster : MonoBehaviour
{
    private CarController carController;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        carController = GetComponentInParent<CarController>();
    }

	private void OnTriggerEnter(Collider other)
	{
        var controller = other.GetComponentInParent<CarController>();
        if(controller != null)
        {
			Debug.Log("crash");
		}
	}
}
