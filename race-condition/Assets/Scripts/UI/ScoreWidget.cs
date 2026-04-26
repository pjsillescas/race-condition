using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreWidget : MonoBehaviour
{
	[SerializeField]
	private List<RawImage> Images;
	[SerializeField]
	private Material EmptyMaterial;
	[SerializeField]
	private Material ScoreMaterial;

	private Material scoreMaterial;
	private int score;

	public void AddScore()
	{
		if (score >= Images.Count)
		{
			return;
		}

		Images[score].material = scoreMaterial;
		score++;
	}

	public int GetScore() => score;

	public void SubtractScore()
	{
		if (score <= 0)
		{
			return;
		}

		Images[score].material = EmptyMaterial;
		score--;
	}

	public void SetScoreMaterial(Material material)
	{
		scoreMaterial = material;
	}

	public void ResetScore()
	{
		score = 5;

		for (int i = 0; i < Images.Count; i++)
		{
			Images[i].material = (i < Images.Count / 2) ? scoreMaterial : EmptyMaterial;
		}
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		scoreMaterial = ScoreMaterial;
		//ResetScore();
	}
}
