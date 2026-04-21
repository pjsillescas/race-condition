using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class LogToFile : MonoBehaviour
{
	private List<string> messages;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Application.logMessageReceived += OnLog;
		messages = new();
    }

	private void OnLog(string condition, string stackTrace, LogType type)
	{
		messages.Add(condition);
	}

	private void OnDestroy()
	{
		Application.logMessageReceived -= OnLog;

		using (StreamWriter writetext = new ("log.txt"))
		{
			writetext.WriteLine(string.Join("\n", messages.ToArray()));
		}
	}
}
