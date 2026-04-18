using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class GhostLapData : ScriptableObject, IGhostLapData
{
    [SerializeField]
    List<GhostDataRow> rows;

    public GhostLapData()
	{
        rows = new();
	}

    public void AddNewData(float time, Transform transform)
    {
        rows.Add(new GhostDataRow(time,transform.position,transform.rotation));
    }

    public void GetDataAt(int sample, out GhostDataRow row)
    {
        row = rows[sample];
    }

    public List<GhostDataRow> GetData()
	{
        return rows;
	}

    public int GetNumSamples()
	{
        return rows.Count;
	}

    public void Reset()
    {
        rows.Clear();
    }

    public void SetData(List<GhostDataRow> newRows)
	{
        rows = new(newRows);
	}

    public void Assign(IGhostLapData data)
	{
        rows = new (data.GetData());
	}

    public float GetDuration()
	{
        return (rows.Count == 0) ? 0 : rows[rows.Count - 1].time;
	}
}
