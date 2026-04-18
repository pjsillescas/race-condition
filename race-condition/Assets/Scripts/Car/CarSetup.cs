using System;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class CarSetup : MonoBehaviour
{
    const string NONEXISTENT_CAR = "NONEXISTENT CAR";
    
    [Serializable]
    public class CarData
    {
        public List<CarSetupStruct> Types;
    }

    [Serializable]
    public class MaterialMapping {
        public CarSetupStruct.CarColor color;
        public Material material;
    }
    
    public string CarType;

    private CarData SetupTypes;
    public List<MaterialMapping> MaterialMappings;

    private void LoadFromFile()
    {
        string fileName = "cars";
        if (fileName != null && fileName != "")
        {
            var jsonTextFile = Resources.Load<TextAsset>(fileName);
            SetupTypes = (jsonTextFile != null) ? JsonUtility.FromJson<CarData>(jsonTextFile.text) : null;
        }

        if (SetupTypes == null)
        {
            SetupTypes = new CarData() { Types = new() };
        }
    }

	private void Awake()
	{
        LoadFromFile();
    }

    public CarSetupStruct GetCarSetup(string carType)
	{
        CarSetupStruct carSetup = new CarSetupStruct();
        carSetup.name = NONEXISTENT_CAR;

        foreach(var setup in SetupTypes.Types)
		{
            if(setup.name == carType)
			{
                carSetup = setup;
			}
		}

        return carSetup;
	}

    private Material GetMaterial(CarSetupStruct.CarColor color)
	{
        Material material = null;

        foreach(var mapping in MaterialMappings)
		{
            if(mapping.color == color)
			{
                material = mapping.material;
			}
		}

        return material;
	}

    public void Setup(string carType)
	{
        Debug.Log($"setting up {carType}");
        CarType = carType;

        var carSetup = GetCarSetup(carType);

        if(carSetup.name == carType)
		{
            Material material = GetMaterial(carSetup.color);

            if(material != null)
			{
                transform.Find("SkyCar/SkyCarBody").GetComponent<MeshRenderer>().material = material;
                transform.Find("SkyCar/SkyCarMudGuardFrontLeft").GetComponent<MeshRenderer>().material = material;
                transform.Find("SkyCar/SkyCarMudGuardFrontRight").GetComponent<MeshRenderer>().material = material;
                transform.Find("SkyCar/SkyCarSuspensionFrontLeft").GetComponent<MeshRenderer>().material = material;
                transform.Find("SkyCar/SkyCarSuspensionFrontRight").GetComponent<MeshRenderer>().material = material;
            }

            CarController carController = gameObject.GetComponent<CarController>();
            carController.SetFullTorqueOverAllWheels(carSetup.fullTorqueOverAllWheels);
            carController.SetMaximumSteerAngle(carSetup.maximumSteerAngle);
            carController.SetMaxSpeed(carSetup.maxTopSpeed);
        }
        else
		{
            Debug.LogError($"Car type {carType} not found");
		}
	}
}
