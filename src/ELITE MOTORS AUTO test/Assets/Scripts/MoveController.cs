using System;
using UnityEngine;
using UnityEngine.InputSystem;

public sealed class MoveController : MonoBehaviour
{
	[SerializeField] float _horizontal;

	public event Action Jump;
	public float Horizontal => _horizontal;

	private void OnMove(InputValue value)
	{
		_horizontal = value.Get<Vector2>().x;
	}

	private void OnJump()
	{
		Jump?.Invoke();
	}
}