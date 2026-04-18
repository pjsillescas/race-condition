using System.Collections.Generic;
using UnityEngine;

public interface IGhostLapData
{
    void AddNewData(float time, Transform transform);

    void GetDataAt(int sample, out GhostDataRow row);

    List<GhostDataRow> GetData();

    int GetNumSamples();

    void Reset();

    void SetData(List<GhostDataRow> newRows);

    void Assign(IGhostLapData data);

    float GetDuration();
}
