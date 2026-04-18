using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct CarSetupStruct
{
	[Serializable]
	public enum CarColor {
		Red,
		Blue,
		Yellow,
	}

	public CarColor color;
	public string name;
	public float maxTopSpeed;
	public float maximumSteerAngle;
	public float fullTorqueOverAllWheels;

	public CarSetupStruct Clone(CarSetupStruct b)
	{
		CarSetupStruct a;

		a.color = b.color;
		a.name = b.name;
		a.maxTopSpeed = b.maxTopSpeed;
		a.maximumSteerAngle = b.maximumSteerAngle;
		a.fullTorqueOverAllWheels = b.fullTorqueOverAllWheels;

		return a;
	}
}
