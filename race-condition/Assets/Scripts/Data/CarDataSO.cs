using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

[CreateAssetMenu(fileName = "CarDataSO", menuName = "Scriptable Objects/CarDataSO")]
public class CarDataSO : ScriptableObject
{
	public string carName;

	public float maxSpeedInTrack = 50f;
	public float maxSpeedOutOfTrack = 10f;

	public Vector3 CentreOfMassOffset = Vector3.zero;
	public float MaximumSteerAngle = 25f;
	public float SteerHelper = 0.644f; // 0 is raw physics , 1 the car will grip in the direction it is facing
	public float TractionControl = 1f; // 0 is no traction control, 1 is full interference
	public float FullTorqueOverAllWheels = 2500f;
	public float ReverseTorque = 500f;
	public float MaxHandbrakeTorque = 1.0e+8f;
	public float Downforce = 100f;
	public SpeedType SpeedType = SpeedType.MPH;
	public float Topspeed = 150;
	public static int NoOfGears = 5;
	public float RevRangeBoundary = 1f;
	public float SlipLimit = 0.3f;
	public float BrakeTorque = 20000f;
	public Material material;
	public GameObject CarPrefab;
}
