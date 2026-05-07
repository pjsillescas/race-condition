using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityStandardAssets.Vehicles.Car;

public class PlayerSelectionWidget : MonoBehaviour
{
	[SerializeField]
	private PlayerDataSO playerDataSO;

	[SerializeField]
	private List<CarDataSO> cars = new();

	[SerializeField]
	private Transform characterContainer;

	[SerializeField]
	private TMP_InputField playerNameInput;

	[SerializeField]
	private TextMeshProUGUI carNameText;

	[SerializeField]
	private float rotationSpeed = 50f;

	[SerializeField]
	private bool autoRotate = true;

	private int currentIndex;
	private GameObject currentCharacterInstance;
	private InputManager inputManager;

	private void Awake()
	{
		currentIndex = 0;
	}

	private void Start()
	{
		inputManager = FindAnyObjectByType<InputManager>();
		inputManager.OnInteract += OnSelectCar;
		inputManager.OnMove += OnCarCarousel;

		DisplayCurrentCar();
	}

	private void OnSelectCar(object sender, EventArgs args)
	{
		playerDataSO.carData = cars[currentIndex];
		playerDataSO.playerName = playerNameInput.text;
		Debug.Log($"player data {playerDataSO.playerName}");
	}

	void Update()
	{
		HandleRotation();
	}

	public bool IsReady()
	{
		return true;
	}

	private void OnCarCarousel(object sender, Vector2 data)
	{
		var threshold = 0.01f;
		var x = data.x;

		if (x >= threshold)
		{
			DisplayNextCar();
		}
		else if (x <= -threshold)
		{
			DisplayPreviousCar();
		}
	}

	private void HandleRotation()
	{
		if (currentCharacterInstance != null && autoRotate)
		{
			currentCharacterInstance.transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);
		}
	}

	public void SetCars(List<CarDataSO> newCars)
	{
		cars = newCars;
		if (cars.Count > 0)
		{
			currentIndex = 0;
			DisplayCurrentCar();
		}
	}

	public void AddCar(GameObject prefab, string name)
	{
		cars.Add(new() { CarPrefab = prefab, carName = name });
		if (cars.Count == 1)
		{
			currentIndex = 0;
			DisplayCurrentCar();
		}
	}

	public void ClearCars()
	{
		cars.Clear();
		DestroyCurrentCar();
		currentIndex = 0;
	}

	public void Activate()
	{
		gameObject.SetActive(true);

		if (cars.Count > 0)
		{
			DisplayCurrentCar();
		}
	}

	public void Deactivate()
	{
		gameObject.SetActive(false);
	}

	public void DisplayNextCar()
	{
		if (cars.Count == 0)
		{
			return;
		}

		currentIndex = (currentIndex + 1) % cars.Count;
		DisplayCurrentCar();
	}

	public void DisplayPreviousCar()
	{
		if (cars.Count == 0)
		{
			return;
		}

		currentIndex = (currentIndex - 1 + cars.Count) % cars.Count;
		DisplayCurrentCar();
	}

	public void SelectCurrentCar()
	{
		if (cars.Count == 0)
		{
			return;
		}

		Debug.Log($"selected character");

		playerDataSO.carData = cars[currentIndex];
	}

	public GameObject GetSelectedCar()
	{
		if (cars.Count == 0)
		{
			return null;
		}

		return cars[currentIndex].CarPrefab;
	}

	public string GetSelectedCharacterName()
	{
		if (cars.Count == 0)
		{
			return "";
		}

		return cars[currentIndex].carName;
	}

	public int GetCurrentIndex()
	{
		return currentIndex;
	}

	public int GetCarCount()
	{
		return cars.Count;
	}

	private void DisplayCurrentCar()
	{
		if (cars.Count == 0)
		{
			return;
		}

		DestroyCurrentCar();

		var option = cars[currentIndex];
		if (option.CarPrefab != null)
		{
			currentCharacterInstance = Instantiate(option.CarPrefab, characterContainer);
			currentCharacterInstance.transform.SetLocalPositionAndRotation(Vector3.zero, new Quaternion(0, 180, 0, 0));
			currentCharacterInstance.GetComponent<CarController>().enabled = false;
			currentCharacterInstance.GetComponent<Rigidbody>().useGravity = false;

			CenterCameraOnCharacter();
		}

		if (carNameText != null)
		{
			carNameText.text = option.carName;
		}
	}

	private void CenterCameraOnCharacter()
	{
		Camera.main.transform.position = characterContainer.position + new Vector3(0f, 1f, 3f);
		Camera.main.transform.LookAt(characterContainer.position + Vector3.up);
	}

	private void DestroyCurrentCar()
	{
		if (currentCharacterInstance != null)
		{
			Destroy(currentCharacterInstance);
			currentCharacterInstance = null;
		}
	}

	private void OnDestroy()
	{
		DestroyCurrentCar();
		if (inputManager != null)
		{
			inputManager.OnInteract -= OnSelectCar;
			inputManager.OnMove -= OnCarCarousel;
		}
	}
}
