using Unity.Cinemachine;
using UnityEngine;

public class CameraManager : MonoBehaviour
{
    private CinemachineCamera activeCamera;

	private void Awake()
	{
		activeCamera = null;
	}

	public void SetCamera(CinemachineCamera camera)
    {
		if (activeCamera != null)
		{
			activeCamera.gameObject.SetActive(false);
		}

		activeCamera = camera;
		camera.gameObject.SetActive(true);
    }
}
