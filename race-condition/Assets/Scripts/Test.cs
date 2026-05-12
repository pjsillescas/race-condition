using TMPro;
using UnityEngine;

public class Test : MonoBehaviour
{
	[SerializeField]
	private TextMeshPro text;

	private float speed = 0.1f;
	private float t;
	private SplineTrack circuit;

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{
		circuit = FindAnyObjectByType<SplineTrack>();
		t = 0f;
	}

	// Update is called once per frame
	void Update()
	{
		t += speed * Time.deltaTime;
		if (t > 1f)
		{
			t = 0f;
		}

		var position = circuit.GetPoint(t);
		transform.position = position;
		text.text = t.ToString();
	}
}
