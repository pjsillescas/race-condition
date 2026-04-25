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
		score = 0;
		Images.ForEach(image => image.material = EmptyMaterial);
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		scoreMaterial = ScoreMaterial;
		ResetScore();
	}
}
