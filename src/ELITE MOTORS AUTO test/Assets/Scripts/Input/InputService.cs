using System;
using Common.GlobalContext;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Input
{
	public sealed class InputService : MonoBehaviour, IInputService
	{
		[SerializeField] float _horizontal;

		private InputSystem_Actions _actions;

		public event Action Jump;
		public float Horizontal => _horizontal;

		private void Awake()
		{
			_actions = new InputSystem_Actions();
			_actions.Player.Jump.performed += OnJump;
			_actions.Player.Move.performed += OnMovePerformed;
			_actions.Player.Move.canceled += OnMoveCanceled;
			
			G.InputService = this;
		}

		public void CallJump() => 
			Jump?.Invoke();

		public void CallHorizontal(float horizontal)
		{
			_horizontal = horizontal;
		}
		
		private void OnMovePerformed(InputAction.CallbackContext context) => 
			CallHorizontal(context.ReadValue<Vector2>().x);

		private void OnMoveCanceled(InputAction.CallbackContext obj) => 
			CallHorizontal(0);

		private void OnEnable()
		{
			_actions.Enable();
		}

		private void OnDisable()
		{
			_actions.Disable();
		}

		private void OnJump(InputAction.CallbackContext context)
		{
			CallJump();
		}
	}
}