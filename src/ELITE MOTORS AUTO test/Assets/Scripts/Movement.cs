using System.Collections;
using UnityEngine;

public sealed class Movement : MonoBehaviour
{
	[SerializeField] private Rigidbody2D _rb;
	[SerializeField] private Transform _center;
	[SerializeField] private Transform _groundCheck;
	[SerializeField] private MoveController _controller;
	[SerializeField] private LayerMask _groundLayer;
	[SerializeField] private float _speed = 8f;
	[SerializeField] private float _jumpingPower = 16f;
	[SerializeField] private float _gravityPower = 5f;
	[SerializeField] private float _rotateFactor = 1f;
	[SerializeField] private bool _isJumping;
	[SerializeField] private bool _inAir;

	private float Horizontal => _controller.Horizontal;

	private void Start()
	{
		_controller.Jump += OnJump;
	}

	private void OnDestroy()
	{
		_controller.Jump -= OnJump;
	}

	private void FixedUpdate()
	{
		var isGrounded = IsGrounded();
		_inAir = !isGrounded;

		var yVelocity = Vector3.Project(_rb.linearVelocity, transform.up);

		if (!_inAir && _isJumping && isGrounded)
		{
			yVelocity += transform.up * _jumpingPower;
			_isJumping = false;
		}

		if (_inAir)
		{
			yVelocity += -transform.up * _gravityPower * Time.fixedDeltaTime;
		}

		var xVelocity = transform.right * Horizontal * _speed;

		_rb.linearVelocity = xVelocity + yVelocity;

		// Rotate.
		if (GroundNormal(out var normal))
		{
			Quaternion endRot = Quaternion.FromToRotation(transform.up, normal) * transform.rotation;
			transform.rotation = Quaternion.Slerp(transform.rotation, endRot, Time.deltaTime * _rotateFactor);
		}
	}

	private bool GroundNormal(out Vector3 normal)
	{
		if (CastToCenter(out var hit) == false)
		{
			normal = default;
			return false;
		}

		normal = hit.normal;
		return true;
	}

	private bool CastToCenter(out RaycastHit2D result)
	{
		var hits = Physics2D.LinecastAll(transform.position, _center.position, _groundLayer);
		if (hits.Length == 0)
		{
			result = default;
			return false;
		}

		result = hits[0];
		var distance = Vector2.Distance(result.point, _center.position);
		foreach (var hit in hits)
		{
			var d = Vector2.Distance(hit.point, _center.position);
			if (d < distance)
			{
				result = hit;
				distance = distance;
			}
		}

		return true;
	}

	private bool IsGrounded()
	{
		const float groundCheckRadius = 0.2f;
		return Physics2D.OverlapCircle(_groundCheck.position, groundCheckRadius, _groundLayer);
	}

	private void OnJump()
	{
		if (_isJumping == false)
		{
			_isJumping = true;
		}
	}
}