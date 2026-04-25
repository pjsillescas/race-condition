using UnityEngine;

[CreateAssetMenu(fileName = "PlayerDataSO", menuName = "Scriptable Objects/PlayerDataSO")]
public class PlayerDataSO : ScriptableObject
{
	public string playerName = "player";
	public bool isIA = false;
	public CarDataSO carData = null;
	public Material scoreMaterial;
}
