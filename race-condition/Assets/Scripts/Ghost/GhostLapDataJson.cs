using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[Serializable]
public class GhostLapDataJson : MonoBehaviour, IGhostLapData
{
    [Serializable]
    public class Data
	{
        public List<GhostDataRow> Rows;
    }

    const string RESOURCES_DIR = "/Resources/";
    
    public string FileName;

    [SerializeField]
    //private List<GhostDataRow> Rows;
    private Data LapData;

	private void Awake()
	{
        LapData = null;
        LoadFromFile();
    }

    private void LoadFromFile()
    {
        try
        {
            if (FileName != null && FileName != "")
            {
                var jsonTextFile = Resources.Load<TextAsset>(FileName.Replace(".json", ""));
                LapData = (jsonTextFile != null) ? JsonUtility.FromJson<Data>(jsonTextFile.text) : null;
            }
        }
        catch(Exception ex)
		{
            Debug.Log("loadfromfile: " + ex.Message);
		}

        if (LapData == null)
        {
            LapData = new Data() { Rows = new() };
        }
    }

    private void WriteToFile()
    {
        try
        {
            string appPath;

#if UNITY_EDITOR
            appPath = Application.dataPath;
#else
            appPath = Application.persistentDataPath;
#endif
            var jsonText = JsonUtility.ToJson(LapData);
            string filePath = appPath + RESOURCES_DIR + FileName;
            StreamWriter outStream = System.IO.File.CreateText(filePath);
            outStream.WriteLine(jsonText);
            outStream.Close();
        }
        catch(Exception ex)
		{
            Debug.Log("write to file exception: " + ex.Message);
		}
    }

    public void AddNewData(float time, Transform transform)
    {
        if (LapData != null && LapData.Rows != null)
        {
            LapData.Rows.Add(new GhostDataRow(time, transform.position, transform.rotation));
            WriteToFile();
        }
    }

    public void GetDataAt(int sample, out GhostDataRow row)
    {
        row = (LapData != null && LapData.Rows != null) ? LapData.Rows[sample] : new GhostDataRow(0,Vector3.zero,Quaternion.identity);
    }
    
    public List<GhostDataRow> GetData()
    {
        return (LapData != null) ? LapData.Rows : new();
    }

    public int GetNumSamples()
    {
        return (LapData != null && LapData.Rows != null) ? LapData.Rows.Count : 0;
    }

    public void Reset()
    {
        if (LapData != null && LapData.Rows != null)
        {
            LapData.Rows.Clear();
            WriteToFile();
        }
    }

    public void SetData(List<GhostDataRow> newRows)
    {
        LapData.Rows = new(newRows);
        WriteToFile();
    }

    public void Assign(IGhostLapData data)
    {
        LapData.Rows = new(data.GetData());
        WriteToFile();
    }

    public float GetDuration()
    {
        return (LapData != null && LapData.Rows != null && LapData.Rows.Count == 0) ? 0 : LapData.Rows[LapData.Rows.Count - 1].time;
    }

}
