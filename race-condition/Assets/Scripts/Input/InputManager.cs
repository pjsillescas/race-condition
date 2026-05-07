using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
	public event EventHandler<Vector2> OnMove;
	public event EventHandler OnInteract;

	private InputActions actions;

	private void Awake()
	{
		actions = new InputActions();
	}

	private void OnEnable()
	{
		actions.Enable();
	}

	private void OnDisable()
	{
		actions.Disable();
	}

	public Vector2 GetMoveVector()
	{
		return actions.Player.Move.ReadValue<Vector2>();
	}

	public bool GetNewRound()
	{
		return actions.Player.Jump.WasPerformedThisFrame();
	}

	// Start is called once before the first execution of Update after the MonoBehaviour is created
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{
		if(actions.Player.Move.WasPressedThisFrame())
		{
			OnMove?.Invoke(this, GetMoveVector());
		}
		
		if (actions.Player.Jump.WasPressedThisFrame())
		{
			OnInteract?.Invoke(this, EventArgs.Empty);
		}
	}
}
