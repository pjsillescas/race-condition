using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Reflection;

public class CircuitEditorWindow : EditorWindow
{
	private enum ToolMode
	{
		Select,
		AddPoint
	}

	private ToolMode currentMode = ToolMode.AddPoint;
	private SplineTrack currentSpline;
	private RoadMeshGenerator roadGenerator;
	private int selectedPointIndex = -1;
	private bool isDragging;
	private float roadWidth = 8f;
	private float segmentLength = 0.5f;
	private float uvScale = 1f;
	private Vector2 uvTiling = new(1f, 10f);
	private bool generateShoulders = true;

	[SerializeField]
	private GameObject carPrefab;

	[SerializeField]
	private int carCount = 3;

	private AICarSpawner carSpawner;

	private const float handleSize = 0.5f;
	private const float pickSize = 1.5f;

	[MenuItem("Tools/Circuit Editor")]
	public static void ShowWindow()
	{
		GetWindow<CircuitEditorWindow>("Circuit Editor");
	}

	private void OnEnable()
	{
		SceneView.duringSceneGui += OnSceneGUI;
	}

	private void OnDisable()
	{
		SceneView.duringSceneGui -= OnSceneGUI;
	}

	private void OnGUI()
	{
		GUILayout.Label("Circuit Editor", EditorStyles.boldLabel);

		EditorGUILayout.Space();
		GUILayout.Label("Spline Settings", EditorStyles.boldLabel);
		currentSpline = (SplineTrack)EditorGUILayout.ObjectField("Spline", currentSpline, typeof(SplineTrack), true);

		if (currentSpline == null)
		{
			if (GUILayout.Button("Create New Spline"))
			{
				CreateNewSpline();
			}
		}
		else
		{
			EditorGUILayout.Space();
			GUILayout.Label("Tools", EditorStyles.boldLabel);
			currentMode = (ToolMode)GUILayout.Toolbar((int)currentMode, new string[] { "Select", "Add Point" });

			EditorGUILayout.Space();
			GUILayout.Label("Road Settings", EditorStyles.boldLabel);
			roadWidth = EditorGUILayout.FloatField("Road Width", roadWidth);
			segmentLength = EditorGUILayout.FloatField("Segment Length", segmentLength);
			uvScale = EditorGUILayout.FloatField("UV Scale", uvScale);
			uvTiling = EditorGUILayout.Vector2Field("UV Tiling", uvTiling);
			generateShoulders = EditorGUILayout.Toggle("Generate Shoulders", generateShoulders);

			EditorGUILayout.Space();
			GUILayout.Label("Actions", EditorStyles.boldLabel);
			if (GUILayout.Button("Generate Road Mesh"))
			{
				GenerateRoad();
			}

			EditorGUILayout.Space();
			GUILayout.Label("AI Cars", EditorStyles.boldLabel);
			carPrefab = (GameObject)EditorGUILayout.ObjectField("Car Prefab", carPrefab, typeof(GameObject), false);
			carCount = EditorGUILayout.IntSlider("Car Count", carCount, 2, 10);

			if (carPrefab != null && currentSpline != null)
			{
				if (GUILayout.Button("Spawn AI Cars"))
				{
					SpawnAICars();
				}

				if (carSpawner != null && GUILayout.Button("Clear AI Cars"))
				{
					ClearAICars();
				}
			}

			EditorGUILayout.Space();
			if (GUILayout.Button("Close Spline"))
			{
				currentSpline.SetClosed(!currentSpline.Closed);
			}

			if (GUILayout.Button("Clear All Points"))
			{
				if (EditorUtility.DisplayDialog("Clear All Points", "Are you sure you want to clear all points?", "Clear", "Cancel"))
				{
					currentSpline.Clear();
					selectedPointIndex = -1;
				}
			}

			EditorGUILayout.Space();
			GUILayout.Label($"Points: {currentSpline.Points.Count}, Segments: {currentSpline.SegmentCount}", EditorStyles.miniLabel);
		}
	}

	private void OnSceneGUI(SceneView sceneView)
	{
		if (currentSpline == null)
		{
			return;
		}

		Event e = Event.current;

		if (e.type == EventType.MouseDown)
		{
			HandleMouseDown(e, sceneView);
		}
		else if (e.type == EventType.MouseDrag)
		{
			HandleMouseDrag(e, sceneView);
		}
		else if (e.type == EventType.MouseUp)
		{
			HandleMouseUp(e);
		}

		DrawSplinePreview();
		DrawControlPoints();

		sceneView.Repaint();
	}

	private void HandleMouseDown(Event e, SceneView sceneView)
	{
		if (e.button != 0)
		{
			return;
		}

		int controlID = GUIUtility.GetControlID(FocusType.Passive);
		Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);

		if (currentMode == ToolMode.AddPoint)
		{
			Vector3 hitPoint = GetGroundHitPoint(ray, sceneView);
			if (hitPoint != Vector3.zero)
			{
				Undo.RecordObject(currentSpline, "Add Point");
				currentSpline.AddPoint(hitPoint);
				selectedPointIndex = currentSpline.Points.Count - 1;
				e.Use();
			}
		}
		else if (currentMode == ToolMode.Select)
		{
			int hitIndex = PickControlPoint(ray);
			if (hitIndex >= 0)
			{
				selectedPointIndex = hitIndex;
				isDragging = true;
				GUIUtility.hotControl = controlID;
				Undo.RecordObject(currentSpline, "Move Point");
				e.Use();
			}
			else
			{
				selectedPointIndex = -1;
			}
		}
	}

	private void HandleMouseDrag(Event e, SceneView sceneView)
	{
		if (!isDragging || selectedPointIndex < 0 || currentMode != ToolMode.Select) return;

		Ray ray = HandleUtility.GUIPointToWorldRay(e.mousePosition);
		Vector3 hitPoint = GetGroundHitPoint(ray, sceneView);

		if (hitPoint != Vector3.zero)
		{
			currentSpline.Points[selectedPointIndex].position = hitPoint;
			currentSpline.UpdateLineRenderer();
			e.Use();
		}
	}

	private void HandleMouseUp(Event e)
	{
		isDragging = false;
		GUIUtility.hotControl = 0;
	}

	private Vector3 GetGroundHitPoint(Ray ray, SceneView sceneView)
	{
		Plane ground = new(Vector3.up, Vector3.zero);
		float distance;
		if (ground.Raycast(ray, out distance))
		{
			return ray.GetPoint(distance);
		}

		GameObject go = Selection.activeGameObject;
		if (go != null)
		{
			if (Physics.Raycast(ray, out RaycastHit hit, 100f, LayerMask.GetMask("Default")))
			{
				return hit.point;
			}
		}

		Vector3 cameraPos = sceneView.camera.transform.position;
		float planeY = currentSpline.transform.position.y;
		if (Mathf.Abs(ray.direction.y) > 0.001f)
		{
			float t = (planeY - cameraPos.y) / ray.direction.y;
			if (t > 0)
			{
				return cameraPos + ray.direction * t;
			}
		}

		return Vector3.zero;
	}

	private int PickControlPoint(Ray ray)
	{
		List<SplinePoint> points = currentSpline.Points;
		int closest = -1;
		float closestDist = pickSize;

		for (int i = 0; i < points.Count; i++)
		{
			Vector3 worldPos = currentSpline.transform.TransformPoint(points[i].position);
			float dist = HandleUtility.DistanceToCircle(worldPos, handleSize);
			if (dist < closestDist)
			{
				closestDist = dist;
				closest = i;
			}
		}

		return closest;
	}

	private void DrawSplinePreview()
	{
		if (currentSpline.SegmentCount == 0)
		{
			return;
		}

		int segments = 50;
		Vector3[] positions = new Vector3[segments + 1];
		float step = 1f / segments;

		for (int i = 0; i <= segments; i++)
		{
			float t = (float)i / segments;
			positions[i] = currentSpline.transform.TransformPoint(currentSpline.GetPoint(t));
		}

		Handles.DrawPolyLine(positions);
	}

	private void DrawControlPoints()
	{
		List<SplinePoint> points = currentSpline.Points;

		for (int i = 0; i < points.Count; i++)
		{
			SplinePoint p = points[i];
			Vector3 pointWorld = currentSpline.transform.TransformPoint(p.position);

			Color pointColor = (i == selectedPointIndex) ? Color.green : Color.white;
			float size = (i == selectedPointIndex) ? handleSize * 1.5f : handleSize;

			Handles.color = pointColor;
			Handles.DrawSolidDisc(pointWorld, Vector3.up, size);

			Handles.Label(pointWorld + Vector3.up * 2, i.ToString());
		}
	}

	private void CreateNewSpline()
	{
		GameObject splineObj = new GameObject("CircuitSpline");
		splineObj.transform.position = Vector3.zero;

		SplineTrack spline = splineObj.AddComponent<SplineTrack>();
		LineRenderer lr = splineObj.AddComponent<LineRenderer>();
		lr.material = new Material(Shader.Find("Sprites/Default"));
		lr.startWidth = 0.5f;
		lr.endWidth = 0.5f;
		lr.positionCount = 0;

		GameObject roadObj = new GameObject("RoadMesh");
		roadObj.transform.position = Vector3.zero;

		RoadMeshGenerator generator = roadObj.AddComponent<RoadMeshGenerator>();
		MeshRenderer mr = roadObj.AddComponent<MeshRenderer>();

		Material asphaltMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/RoadAsphalt.mat");
		if (asphaltMat != null)
		{
			generator.SetRoadMaterial(asphaltMat);
		}
		else
		{
			Debug.LogWarning("RoadAsphalt.mat not found, using default material");
			mr.material = new Material(Shader.Find("Standard"));
		}

		generator.SetSpline(spline);
		generator.SetRoadWidth(roadWidth);
		generator.SetUVScale(uvScale);
		generator.SetUVTiling(uvTiling);
		generator.SetGenerateShoulders(generateShoulders);

		if (spline.Points.Count >= 2)
		{
			generator.Generate();
		}

		currentSpline = spline;
		roadGenerator = generator;

		Selection.activeGameObject = splineObj;
	}

	private void GenerateRoad()
	{
		if (currentSpline == null)
		{
			return;
		}

		if (roadGenerator == null)
		{
			SplineTrack[] splines = FindObjectsByType<SplineTrack>();
			for (int i = 0; i < splines.Length; i++)
			{
				if (splines[i] == currentSpline)
				{
					RoadMeshGenerator[] generators = FindObjectsByType<RoadMeshGenerator>();
					for (int j = 0; j < generators.Length; j++)
					{
						if (generators[j].GetComponent<MeshFilter>().sharedMesh == null ||
							generators[j].GetComponent<MeshFilter>().sharedMesh.vertexCount == 0)
						{
							roadGenerator = generators[j];
							break;
						}
					}
					break;
				}
			}
		}

		if (roadGenerator == null)
		{
			GameObject roadObj = new GameObject("RoadMesh");
			roadObj.transform.position = currentSpline.transform.position;
			roadGenerator = roadObj.AddComponent<RoadMeshGenerator>();
			MeshRenderer mr = roadObj.AddComponent<MeshRenderer>();

			Material asphaltMat = AssetDatabase.LoadAssetAtPath<Material>("Assets/Materials/RoadAsphalt.mat");
			if (asphaltMat != null)
			{
				roadGenerator.SetRoadMaterial(asphaltMat);
			}
			else
			{
				Debug.LogWarning("RoadAsphalt.mat not found");
				mr.material = new Material(Shader.Find("Standard"));
			}
		}

		if (currentSpline.Points.Count < 2)
		{
			EditorUtility.DisplayDialog("Error", "Add at least 2 points to generate the road.", "OK");
			return;
		}

		if (roadGenerator == null)
		{
			EditorUtility.DisplayDialog("Error", "No road generator found. Create a new spline first.", "OK");
			return;
		}

		roadGenerator.SetSpline(currentSpline);
		roadGenerator.SetRoadWidth(roadWidth);
		roadGenerator.SetUVScale(uvScale);
		roadGenerator.SetUVTiling(uvTiling);
		roadGenerator.SetGenerateShoulders(generateShoulders);

		try
		{
			roadGenerator.Generate();
			EditorUtility.DisplayDialog("Road Generated", "Road mesh has been generated successfully!", "OK");
		}
		catch (System.Exception ex)
		{
			EditorUtility.DisplayDialog("Error", "Failed to generate road: " + ex.Message, "OK");
		}
	}

	private void SpawnAICars()
	{
		if (carPrefab == null || currentSpline == null) return;

		ClearAICars();

		GameObject spawnerObj = new GameObject("AICarSpawner");
		AICarSpawner spawner = spawnerObj.AddComponent<AICarSpawner>();
		spawner.SetCircuit(currentSpline);
		spawner.SetCarPrefab(carPrefab);

		carSpawner = spawner;

		FieldInfo countField = typeof(AICarSpawner).GetField("carCount", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
		if (countField != null)
		{
			countField.SetValue(spawner, carCount);
		}

		EditorUtility.DisplayDialog("AI Cars Spawned", $"Spawned {carCount} AI cars on the circuit!", "OK");
	}

	private void ClearAICars()
	{
		if (carSpawner != null)
		{
			foreach (GameObject car in carSpawner.GetSpawnedCars())
			{
				if (car != null) DestroyImmediate(car);
			}
			DestroyImmediate(carSpawner.gameObject);
			carSpawner = null;
		}
	}
}