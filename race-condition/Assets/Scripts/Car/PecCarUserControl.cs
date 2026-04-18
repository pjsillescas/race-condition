using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

[RequireComponent(typeof(CarController))]
public class PecCarUserControl : MonoBehaviour
{
	private CarController car;
	private InputManager inputManager;

	private void Start()
	{
		car = GetComponent<CarController>();
		inputManager = FindAnyObjectByType<InputManager>();
	}

	private void FixedUpdate()
	{
		Vector2 moveVector = inputManager.GetMoveVector();

		float h = moveVector.x;
		float v = moveVector.y;
		float vAccel = v;
		float handbrake = 0;
		car.Move(h, vAccel, v, handbrake);
	}
}
