using System.Collections.Generic;
using UnityEngine;

public class GameData : MonoBehaviour
{
	[SerializeField]
	private List<PlayerDataSO> PlayersData;
	[SerializeField]
	private List<PlayerDataSO> AIData;

	public List<PlayerDataSO> GetPlayersData() => PlayersData;
	public void SetPlayersData(List<PlayerDataSO> data)
	{
		PlayersData = data;
	}

	public List<PlayerDataSO> GetAIData() => AIData;
	public void SetAIData(List<PlayerDataSO> data)
	{
		AIData = data;
	}
}
