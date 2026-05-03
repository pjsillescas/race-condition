using System;
using System.Collections;
using UnityEngine;

public class TrafficLightController : MonoBehaviour
{
	public enum LightState
	{
		Red,
		Yellow,
		Green,
		Off
	}

	public static event EventHandler OnRoundStarted;

	[SerializeField]
	private MeshRenderer redLightRenderer;
	[SerializeField]
	private MeshRenderer yellowLightRenderer;
	[SerializeField]
	private MeshRenderer greenLightRenderer;

	[SerializeField]
	private float redLightDuration = 3f;
	[SerializeField]
	private float yellowLightDuration = 1f;
	[SerializeField]
	private float greenLightDuration = 5f;

	[SerializeField]
	private Material redOnMaterial;
	[SerializeField]
	private Material redOffMaterial;
	[SerializeField]
	private Material yellowOnMaterial;
	[SerializeField]
	private Material yellowOffMaterial;
	[SerializeField]
	private Material greenOnMaterial;
	[SerializeField]
	private Material greenOffMaterial;

	public void Enable()
	{
		gameObject.SetActive(true);
	}

	public void Disable()
	{
		gameObject.SetActive(false);
	}

	public void StartTrafficSequence()
	{
		StopAllCoroutines();
		StartCoroutine(TrafficSequence());
	}

	private IEnumerator TrafficSequence()
	{
		SetState(LightState.Off);
		SetState(LightState.Red);

		yield return new WaitForSeconds(redLightDuration);

		SetState(LightState.Yellow);

		yield return new WaitForSeconds(yellowLightDuration);

		SetState(LightState.Green);

		yield return new WaitForSeconds(greenLightDuration);

		OnRoundStarted?.Invoke(this, EventArgs.Empty);
	}

	private void SetState(LightState state)
	{
		switch (state)
		{
			case LightState.Red:
				SetLight(redLightRenderer, redOnMaterial);
				SetLight(yellowLightRenderer, yellowOffMaterial);
				SetLight(greenLightRenderer, greenOffMaterial);
				break;
			case LightState.Yellow:
				SetLight(redLightRenderer, redOffMaterial);
				SetLight(yellowLightRenderer, yellowOnMaterial);
				SetLight(greenLightRenderer, greenOffMaterial);
				break;
			case LightState.Green:
				SetLight(redLightRenderer, redOffMaterial);
				SetLight(yellowLightRenderer, yellowOffMaterial);
				SetLight(greenLightRenderer, greenOnMaterial);
				break;
			case LightState.Off:
				SetLightsOff();
				break;
		}
	}

	private void SetLightsOff()
	{
		SetLight(redLightRenderer, redOffMaterial);
		SetLight(yellowLightRenderer, yellowOffMaterial);
		SetLight(greenLightRenderer, greenOffMaterial);
	}

	private void SetLight(MeshRenderer renderer, Material onMaterial)
	{
		if (renderer != null && onMaterial != null)
		{
			renderer.material = onMaterial;
		}
	}
}