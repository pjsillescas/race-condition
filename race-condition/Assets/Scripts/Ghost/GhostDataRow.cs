using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct GhostDataRow
{
	[SerializeField]
	public float time;
	
	[SerializeField]
	public Vector3 position;
	
	[SerializeField]
	public Quaternion rotation;

	public GhostDataRow(float time,Vector3 position,Quaternion rotation)
	{
		this.time = time;
		this.position = position;
		this.rotation = rotation;
	}
}
