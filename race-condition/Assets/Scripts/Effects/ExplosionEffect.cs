using System.Collections;
using UnityEngine;

[ExecuteInEditMode]
public class ExplosionEffect : MonoBehaviour
{
	[Header("Explosion Settings")]
	[SerializeField]
	private int particleCount = 100;
	[SerializeField]
	private float explosionForce = 10f;
	[SerializeField]
	private float explosionRadius = 5f;
	[SerializeField]
	private float lifeTime = 1f;

	[Header("Particle Visual")]
	[SerializeField]
	private Color startColor = new(1f, 0.5f, 0f, 1f);
	[SerializeField]
	private Color endColor = new(1f, 0f, 0f, 0f);
	[SerializeField]
	private float startSize = 0.5f;
	[SerializeField]
	private float endSize = 0f;
	[SerializeField]
	private Material material;

	private ParticleSystem ps;

	void Awake()
	{
		ps = gameObject.AddComponent<ParticleSystem>();
		ConfigureParticleSystem();

		StartCoroutine(Explode());

	}

	private void ConfigureParticleSystem()
	{
		var main = ps.main;
		main.loop = false;
		main.playOnAwake = false;
		main.startLifetime = lifeTime;
		main.startSpeed = explosionForce;
		main.startSize = startSize;
		main.startColor = startColor;
		main.gravityModifier = 0.5f;
		main.simulationSpace = ParticleSystemSimulationSpace.World;

		var emission = ps.emission;
		emission.rateOverTime = 0;
		emission.SetBursts(new ParticleSystem.Burst[] { new(0f, particleCount) });

		var shape = ps.shape;
		shape.enabled = false;

		var colorOverLifetime = ps.colorOverLifetime;
		colorOverLifetime.enabled = true;
		Gradient grad = new();
		grad.SetKeys(
			new GradientColorKey[] { new(startColor, 0f), new GradientColorKey(endColor, 1f) },
			new GradientAlphaKey[] { new(1f, 0f), new GradientAlphaKey(0f, 1f) }
		);
		colorOverLifetime.color = grad;

		var sizeOverLifetime = ps.sizeOverLifetime;
		sizeOverLifetime.enabled = true;
		AnimationCurve curve = new();
		curve.AddKey(0f, startSize);
		curve.AddKey(1f, endSize);
		sizeOverLifetime.size = new ParticleSystem.MinMaxCurve(1f, curve);

		var renderer = ps.GetComponent<ParticleSystemRenderer>();
		renderer.material = material; // new Material(Shader.Find("Particles/Standard Unlit"));
		renderer.renderMode = ParticleSystemRenderMode.Billboard;
	}

	public void TriggerExplosion()
	{
		if (ps != null)
		{
			ps.Emit(particleCount);
		}
	}

	public void TriggerExplosion(Vector3 position)
	{
		transform.position = position;
		TriggerExplosion();
	}

	private IEnumerator Explode()
	{
		while (true)
		{
			Debug.Log("explode");
			TriggerExplosion();

			yield return new WaitForSeconds(2f);
		}
	}
}