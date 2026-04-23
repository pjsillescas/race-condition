using System;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class CarSetup : MonoBehaviour
{
	[Serializable]
	public class MaterialMapping
	{
		public CarSetupStruct.CarColor color;
		public Material material;
	}

	public void Setup(PlayerDataSO playerSetup)
	{
		var carSetup = playerSetup.carData;
		Material material = carSetup.material;

		if (material != null)
		{
			transform.Find("SkyCar/SkyCarBody").GetComponent<MeshRenderer>().material = material;
			transform.Find("SkyCar/SkyCarMudGuardFrontLeft").GetComponent<MeshRenderer>().material = material;
			transform.Find("SkyCar/SkyCarMudGuardFrontRight").GetComponent<MeshRenderer>().material = material;
			transform.Find("SkyCar/SkyCarSuspensionFrontLeft").GetComponent<MeshRenderer>().material = material;
			transform.Find("SkyCar/SkyCarSuspensionFrontRight").GetComponent<MeshRenderer>().material = material;
		}

		CarController carController = gameObject.GetComponent<CarController>();
		carController.Setup(playerSetup);
		gameObject.GetComponent<CarSpeedController>().Setup(carSetup.maxSpeedOutOfTrack, carSetup.maxSpeedInTrack);
	}
}
