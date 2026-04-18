using System;
using UnityEngine;

public class GhostController : MonoBehaviour
{
	public event Action OnAnyRepetitionFinished;
	
	[SerializeField]
	private GhostLapData LapData;
	[SerializeField]
	private GhostLapDataJson LapDataJson;
	[SerializeField]
	private float timeBetweenSamples = 0.25f;

	private float currenttimeBetweenPlaySamples;
	private int currentSampleToPlay;
	private Vector3 lastSamplePosition;
	private Vector3 nextPosition;
	private Quaternion lastSampleRotation;
	private Quaternion nextRotation;
	private bool destroyWhenFinished;
	private CarMonitor carMonitor;

	private void Awake()
	{
		destroyWhenFinished = true;
	}

	// Start is called before the first frame update
	void Start()
	{
		carMonitor = FindAnyObjectByType<CarMonitor>();

		carMonitor.OnEndLap += RestartGhost;
		carMonitor.OnEndRace += DestroyIfNotNeeded;
		StartGhost();

		lastSamplePosition = Vector3.zero;
		lastSampleRotation = Quaternion.identity;
	}

	public void SetDestroyWhenFinished(bool destroyWhenFinished)
	{
		this.destroyWhenFinished = destroyWhenFinished;
	}

	private void DestroyIfNotNeeded(float time)
	{
		if (destroyWhenFinished)
		{
			carMonitor.OnEndLap -= RestartGhost;
			carMonitor.OnEndRace -= DestroyIfNotNeeded;
			Destroy(gameObject);
		}
	}

	private void RestartGhost(float time)
	{
		Debug.Log("restarting ghost");
		StartGhost();
	}

	public void StartGhost()
	{
		currenttimeBetweenPlaySamples = 0;
		currentSampleToPlay = 0;
	}

	// Update is called once per frame
	protected void Update()
	{
		UpdateWithData(LapDataJson);
		//UpdateWithData(LapData);
	}

	private void UpdateWithData(IGhostLapData lapData)
	{
		if (lapData.GetNumSamples() <= 1)
		{
			return;
		}

		if (currentSampleToPlay >= lapData.GetNumSamples())
		{
			OnAnyRepetitionFinished?.Invoke();
			return;
		}

		// A cada frame incrementamos el tiempo transcurrido 
		currenttimeBetweenPlaySamples += Time.deltaTime;

		// Si el tiempo transcurrido es mayor que el tiempo de muestreo
		if (currenttimeBetweenPlaySamples >= timeBetweenSamples)
		{
			if (currentSampleToPlay == 0)
			{
				lapData.GetDataAt(currentSampleToPlay, out GhostDataRow dataRow1);
				nextPosition = dataRow1.position;
				nextRotation = dataRow1.rotation;
				currentSampleToPlay++;
			}

			// De cara a interpolar de una manera fluida la posición del coche entre una muestra y otra,
			// guardamos la posición y la rotación de la anterior muestra
			lastSamplePosition = nextPosition;
			lastSampleRotation = nextRotation;

			// Cogemos los datos del scriptable object
			lapData.GetDataAt(currentSampleToPlay, out GhostDataRow dataRow);

			nextPosition = dataRow.position;
			nextRotation = dataRow.rotation;

			// Dejamos el tiempo extra entre una muestra y otra
			currenttimeBetweenPlaySamples -= timeBetweenSamples;

			// Incrementamos el contador de muestras
			currentSampleToPlay++;
		}

		if (currentSampleToPlay > 1)
		{
			float percentageBetweenFrames = currenttimeBetweenPlaySamples / timeBetweenSamples;

			var position = Vector3.Slerp(lastSamplePosition, nextPosition, percentageBetweenFrames);
			var rotation = Quaternion.Slerp(lastSampleRotation, nextRotation, percentageBetweenFrames);
			transform.SetPositionAndRotation(position, rotation);
		}
	}
}
