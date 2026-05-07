using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuWidget : MonoBehaviour
{
	private InputManager inputManager;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		inputManager = FindAnyObjectByType<InputManager>();
		inputManager.OnInteract += OnNewRound;
	}

	private void OnDestroy()
	{
		inputManager.OnInteract -= OnNewRound;
	}

	private void OnNewRound(object sender, EventArgs args)
	{
		//SceneManager.LoadScene("Playground");
		OpenSelectWidget();
	}

	private void OpenSelectWidget()
	{
		;
	}

	// Update is called once per frame
	void Update()
	{
		;
	}
}
