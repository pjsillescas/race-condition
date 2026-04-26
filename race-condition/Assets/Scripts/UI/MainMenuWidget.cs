using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuWidget : MonoBehaviour
{
	private InputManager inputManager;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		inputManager = FindAnyObjectByType<InputManager>();
	}

	// Update is called once per frame
	void Update()
	{
		if (inputManager.GetNewRound())
		{
			SceneManager.LoadScene("Playground");
		}
	}
}
