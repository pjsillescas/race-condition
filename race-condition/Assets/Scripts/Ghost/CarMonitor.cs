using Unity.Cinemachine;
using System;
using UnityEngine;

public class CarMonitor : MonoBehaviour
{
	public event Action<float> OnTick;
	public event Action<float, Transform> OnSampling;
	public event Action<float> OnEndRace;
	public event Action<float> OnEndLap;
	public event Action<int> OnBeginLap;

	public int MaxLaps = 3;

	[SerializeField]
	private float timeBetweenSamples = 0.25f;
	[SerializeField]
	private GhostLapData currentLapSO;           // Scriptable object
	[SerializeField]
	private GhostLapData bestLapSO;           // Scriptable object
	[SerializeField]
	private GameObject carToRecord;
	[SerializeField]
	private CinemachineCamera Camera;
	
	private float currenttimeBetweenSamples = 0.0f;
	private float currentLapTime;
	private bool doSample;
	private int currentLap;
	private bool canToggleCamera;

	// Start is called before the first frame update
	void Start()
	{
		currentLapTime = 0f;
		doSample = true;

		currentLap = 0;
		OnBeginLap?.Invoke(currentLap);
	}

	private void EndRace()
	{
		Debug.Log("End race");
		doSample = false;
		OnSampling?.Invoke(currentLapTime, carToRecord.transform);

		Camera.enabled = false;

		canToggleCamera = true;

		OnEndRace?.Invoke(currentLapTime);
	}

	public void ToggleCamera()
	{
		if (canToggleCamera)
		{
			Camera.enabled = !Camera.enabled;
		}
	}

	public void EndLap()
	{
		Debug.Log("endlap");
		OnSampling?.Invoke(currentLapTime, carToRecord.transform);

		OnEndLap?.Invoke(currentLapTime);

		currentLapTime = 0;
		currentLap++;

		if (currentLap >= MaxLaps)
		{
			currentLap = -1;
			EndRace();
		}
		else
		{
			OnBeginLap?.Invoke(currentLap);
		}
	}

	public void EndLapRepetition()
	{
		Debug.Log("endlaprepetition");
		currentLap++;
		//OnBeginLap?.Invoke(currentLap);

		if (currentLap >= MaxLaps)
		{
			currentLap = -1;
			EndRace();
		}
		else
		{
			OnBeginLap?.Invoke(currentLap);
		}
	}

	// Update is called once per frame
	void Update()
	{
		currentLapTime += Time.deltaTime;
		// A cada frame incrementamos el tiempo transcurrido 
		currenttimeBetweenSamples += Time.deltaTime;
		//Debug.Log($"currentLapTime {currentLapTime}");
		OnTick?.Invoke(currentLapTime);

		// Si el tiempo transcurrido es mayor que el tiempo de muestreo
		if (currenttimeBetweenSamples >= timeBetweenSamples && doSample)
		{
			OnSampling?.Invoke(currentLapTime, carToRecord.transform);
			// Guardamos la información para el fantasma
			// currentLapSO.AddNewData(currentLapTime, carToRecord.transform);
			// Dejamos el tiempo extra entre una muestra y otra
			currenttimeBetweenSamples -= timeBetweenSamples;
		}

	}
}
