using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RepetitionController : GhostController
{
    private bool isActive;

	private void Awake()
	{
        isActive = false;
	}

    public void SetIsActive(bool isActive)
	{
        this.isActive = isActive;

        if(isActive)
		{
            OnAnyRepetitionFinished += OnRepetitionFinished;
		}
	}

    private void OnRepetitionFinished()
	{
        SceneManager.LoadScene("MainMenuScene");
    }

    // Update is called once per frame
    protected new void Update()
    {
        if(isActive)
		{
            base.Update();
		}
    }
}
