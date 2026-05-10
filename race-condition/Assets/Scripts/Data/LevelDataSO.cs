using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDataSO", menuName = "Scriptable Objects/LevelDataSO")]

public class LevelDataSO : ScriptableObject
{
	public string LevelName = "Level";
	public string LevelSceneName = "Track";
	public GameObject LevelPrefab = null;
	public int numIARacers = 2;
}
