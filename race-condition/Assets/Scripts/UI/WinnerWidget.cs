using System;
using TMPro;
using UnityEngine;

public class WinnerWidget : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI PlayerText;

	private void OnEnable()
	{
		Watchdog.OnNewRound += OnNewRound;
	}

	private void OnDisable()
	{
		Watchdog.OnNewRound -= OnNewRound;
	}

	private void OnNewRound(object sender, float index)
	{
		Hide();
	}

	public void Display(string player)
    {
        PlayerText.text = player;
        gameObject.SetActive(true);
    }

	public void Hide()
	{
		gameObject.SetActive(false);
	}

	private void Start()
	{
		Hide();
	}
}
