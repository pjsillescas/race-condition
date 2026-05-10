using UnityEngine;

public class TrackInitializer : MonoBehaviour
{
	public void Initialize()
	{
		var generator = GetComponentInChildren<RoadMeshGenerator>();
		if (generator != null)
		{
			generator.Generate();
		}
	}
}
