using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.Vehicles.Car;

public class PlayerSelectionWidget : MonoBehaviour
{
	[SerializeField]
	private PlayerDataSO playerDataSO;

	[SerializeField]
	private List<PlayerDataSO> Adversaries = new();

	[SerializeField]
	private List<CarDataSO> cars = new();

	[SerializeField]
	private List<LevelDataSO> levels = new();

	[SerializeField]
	private Transform characterContainer;

	[SerializeField]
	private TMP_InputField playerNameInput;

	[SerializeField]
	private TextMeshProUGUI carNameText;
	[SerializeField]
	private TextMeshProUGUI levelNameText;

	[SerializeField]
	private float carRotationSpeed = 50f;
	[SerializeField]
	private float levelRotationSpeed = 50f;

	[SerializeField]
	private bool autoRotate = true;

	private int currentCarIndex;
	private int currentLevelIndex;

	private GameObject currentCharacterInstance;
	private GameObject currentLevelInstance;
	private InputManager inputManager;

	private void Awake()
	{
		currentCarIndex = 0;
		currentLevelIndex = 0;
	}

	private void Start()
	{
		inputManager = FindAnyObjectByType<InputManager>();
		inputManager.OnInteract += OnSelectCar;
		inputManager.OnMove += OnCarCarousel;

		DisplayCurrentCar();
		DisplayCurrentLevel();
	}

	private PlayerDataSO GetRandomAdversary()
	{
		var index = UnityEngine.Random.Range(0, Adversaries.Count);
		return Adversaries[index];
	}

	private void LoadAdversaries()
	{
		var gameData = FindAnyObjectByType<GameData>();

		var adversaries = new List<PlayerDataSO>();
		var numAdversaries = levels[currentLevelIndex].numIARacers;
		for (int i = 0; i < numAdversaries; i++)
		{
			//var index = UnityEngine.Random.Range(0, Adversaries.Count);
			//var ai = Adversaries[index];
			var ai = GetRandomAdversary();
			while (adversaries.Contains(ai))
			{
				//index = UnityEngine.Random.Range(0, Adversaries.Count);
				//ai = Adversaries[index];
				ai = GetRandomAdversary();
			}

			adversaries.Add(ai);
		}

		gameData.SetAIData(adversaries);
	}

	private void OnSelectCar(object sender, EventArgs args)
	{
		playerDataSO.carData = cars[currentCarIndex];
		playerDataSO.playerName = playerNameInput.text;
		Debug.Log($"player data {playerDataSO.playerName}");

		LoadAdversaries();

		SceneManager.LoadScene(levels[currentLevelIndex].LevelSceneName);
	}

	void Update()
	{
		HandleCarRotation();
	}

	public bool IsReady()
	{
		return true;
	}

	private void OnCarCarousel(object sender, Vector2 data)
	{
		var threshold = 0.01f;
		var x = data.x;

		// Car selection
		if (x >= threshold)
		{
			DisplayNextCar();
		}
		else if (x <= -threshold)
		{
			DisplayPreviousCar();
		}

		// Level selection
		var y = data.y;
		if (y >= threshold)
		{
			DisplayNextLevel();
		}
		else if (y <= -threshold)
		{
			DisplayPreviousLevel();
		}

	}

	private void HandleCarRotation()
	{
		if (currentCharacterInstance != null && autoRotate)
		{
			currentCharacterInstance.transform.Rotate(Vector3.up, carRotationSpeed * Time.deltaTime);
		}
	}

	private void HandleLevelRotation()
	{
		if (currentLevelInstance != null && autoRotate)
		{
			currentLevelInstance.transform.Rotate(Vector3.up, levelRotationSpeed * Time.deltaTime);
		}
	}

	/*
	public void SetCars(List<CarDataSO> newCars)
	{
		cars = newCars;
		if (cars.Count > 0)
		{
			currentCarIndex = 0;
			DisplayCurrentCar();
		}
	}

	public void AddCar(GameObject prefab, string name)
	{
		cars.Add(new() { CarPrefab = prefab, carName = name });
		if (cars.Count == 1)
		{
			currentCarIndex = 0;
			DisplayCurrentCar();
		}
	}

	public void ClearCars()
	{
		cars.Clear();
		DestroyCurrentCar();
		currentCarIndex = 0;
	}
	*/

	public void Activate()
	{
		gameObject.SetActive(true);

		if (cars.Count > 0)
		{
			DisplayCurrentCar();
			DisplayCurrentLevel();
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

		currentCarIndex = (currentCarIndex + 1) % cars.Count;
		DisplayCurrentCar();
	}

	public void DisplayPreviousCar()
	{
		if (cars.Count == 0)
		{
			return;
		}

		currentCarIndex = (currentCarIndex - 1 + cars.Count) % cars.Count;
		DisplayCurrentCar();
	}

	public void SelectCurrentCar()
	{
		if (cars.Count == 0)
		{
			return;
		}

		Debug.Log($"selected character");

		playerDataSO.carData = cars[currentCarIndex];
	}

	public GameObject GetSelectedCar()
	{
		if (cars.Count == 0)
		{
			return null;
		}

		return cars[currentCarIndex].CarPrefab;
	}

	public string GetSelectedCharacterName()
	{
		if (cars.Count == 0)
		{
			return "";
		}

		return cars[currentCarIndex].carName;
	}
	/*
	public int GetCurrentCarIndex()
	{
		return currentCarIndex;
	}

	public int GetCarCount()
	{
		return cars.Count;
	}
	*/

	private void DisplayCurrentCar()
	{
		if (cars.Count == 0)
		{
			return;
		}

		DestroyCurrentCar();

		var option = cars[currentCarIndex];
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
		DestroyCurrentLevel();

		if (inputManager != null)
		{
			inputManager.OnInteract -= OnSelectCar;
			inputManager.OnMove -= OnCarCarousel;
		}
	}

	public void DisplayNextLevel()
	{
		if (levels.Count == 0)
		{
			return;
		}

		currentLevelIndex = (currentLevelIndex + 1) % levels.Count;
		DisplayCurrentLevel();
	}

	public void DisplayPreviousLevel()
	{
		if (levels.Count == 0)
		{
			return;
		}

		currentLevelIndex = (currentLevelIndex - 1 + levels.Count) % levels.Count;
		DisplayCurrentLevel();
	}

	public GameObject GetSelectedLevel()
	{
		if (levels.Count == 0)
		{
			return null;
		}

		return levels[currentLevelIndex].LevelPrefab;
	}

	public string GetSelectedLevelName()
	{
		if (levels.Count == 0)
		{
			return "";
		}

		return levels[currentLevelIndex].LevelName;
	}

	public int GetCurrentLevelIndex()
	{
		return currentLevelIndex;
	}

	public int GetLevelCount()
	{
		return levels.Count;
	}

	private void DisplayCurrentLevel()
	{
		if (levels.Count == 0)
		{
			return;
		}

		DestroyCurrentLevel();

		var option = levels[currentLevelIndex];
		if (option.LevelPrefab != null)
		{
			currentLevelInstance = Instantiate(option.LevelPrefab, Vector3.zero, Quaternion.identity);
			if (currentLevelInstance.TryGetComponent(out TrackInitializer initializer))
			{
				initializer.Initialize();
			}
			//currentCharacterInstance.transform.SetLocalPositionAndRotation(Vector3.zero, new Quaternion(0, 0, 0, 0));

			//currentCharacterInstance.GetComponent<CarController>().enabled = false;
			//currentCharacterInstance.GetComponent<Rigidbody>().useGravity = false;
		}

		if (levelNameText != null)
		{
			levelNameText.text = option.LevelName;
		}
	}

	private void DestroyCurrentLevel()
	{
		if (currentLevelInstance != null)
		{
			Destroy(currentLevelInstance);
			currentLevelInstance = null;
		}
	}
}
