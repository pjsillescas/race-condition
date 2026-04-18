using UnityEngine;

public class Checkpoint : MonoBehaviour
{
	private CarMonitor carMonitor;

	// Start is called before the first frame update
	void Start()
	{
		carMonitor = FindAnyObjectByType<CarMonitor>();
	}

	// Update is called once per frame
	void Update()
	{

	}

	private void OnTriggerEnter(Collider other)
	{
		if (other.name == "ColliderBody" && other.transform.parent.parent.TryGetComponent(out PecCarUserControl controller) && controller.isActiveAndEnabled)
		{
			Debug.Log("End lap");
			carMonitor.EndLap();
		}

	}
}
