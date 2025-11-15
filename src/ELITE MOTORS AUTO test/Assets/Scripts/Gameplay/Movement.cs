using Common.GlobalContext;
using Input;
using UnityEngine;

namespace Gameplay
{
	public sealed class Movement : MonoBehaviour
	{
		const float _groundCheckRadius = 0.2f;

		[SerializeField] private Rigidbody2D _rb;
		[SerializeField] private Transform _center;
		[SerializeField] private Transform _groundCheck;
		[SerializeField] private LayerMask _groundLayer;
		[SerializeField] private float _speed = 8f;
		[SerializeField] private float _jumpingPower = 16f;
		[SerializeField] private float _gravityPower = 5f;
		[SerializeField] private float _rotateFactor = 1f;
		[SerializeField] private bool _isJumping;
		[SerializeField] private bool _inAir;
		[SerializeField] private bool _isGrounded;

		private RaycastHit2D[] _hits = new RaycastHit2D[8];

		private float Horizontal => Input.Horizontal;
		private IInputService Input => G.InputService;

		private void Start()
		{
			Input.Jump += OnJump;
		}

		private void OnDestroy()
		{
			Input.Jump -= OnJump;
		}

		private void FixedUpdate()
		{
			SetGroundState();
			SetAirState();

			var yVelocity = CalculateVerticalVelocity();
			var xVelocity = CalculateLocalHorizontalVelocity();

			_rb.linearVelocity = xVelocity + yVelocity;

			ApplyRotation();

			ResetJumpingState();
		}

		private Vector3 CalculateVerticalVelocity()
		{
			var yVelocity = LocalVerticalVelocity();
			yVelocity = ApplyJumpVelocity(yVelocity);
			yVelocity = ApplyGravitation(yVelocity);
			return yVelocity;
		}

		private void ApplyRotation()
		{
			if (TryGetGroundNormal(out var normal))
			{
				Quaternion endRot = Quaternion.FromToRotation(transform.up, normal) * transform.rotation;
				transform.rotation = Quaternion.Slerp(transform.rotation, endRot, Time.deltaTime * _rotateFactor);
			}
		}

		private bool TryGetGroundNormal(out Vector3 normal)
		{
			if (CastLineToCenter(out var hit) == false)
			{
				normal = default;
				return false;
			}

			normal = hit.normal;
			return true;
		}

		private bool CastLineToCenter(out RaycastHit2D result)
		{
			var count = Physics2D.LinecastNonAlloc(transform.position, _center.position, _hits, _groundLayer);
			if (count == 0)
			{
				result = default;
				return false;
			}

			result = _hits[0];
			var distance = Vector2.Distance(result.point, _center.position);
			for (var i = 0; i < count; i++)
			{
				var hit = _hits[i];
				var newDistance = Vector2.Distance(hit.point, _center.position);
				if (newDistance < distance)
				{
					result = hit;
					distance = newDistance;
				}
			}

			return true;
		}

		private void OnJump()
		{
			if (_isJumping == false)
			{
				_isJumping = true;
			}
		}

		private Vector3 ApplyGravitation(Vector3 yVelocity)
		{
			if (!_inAir) return yVelocity;

			return yVelocity += -transform.up * _gravityPower * Time.fixedDeltaTime;
		}

		private Vector3 ApplyJumpVelocity(Vector3 yVelocity)
		{
			if (!_inAir && _isJumping && _isGrounded)
			{
				yVelocity += transform.up * _jumpingPower;
			}

			return yVelocity;
		}

		private Vector3 CalculateLocalHorizontalVelocity() =>
			transform.right * Horizontal * _speed;

		private Vector3 LocalVerticalVelocity() =>
			Vector3.Project(_rb.linearVelocity, transform.up);

		private void ResetJumpingState() =>
			_isJumping = false;

		private void SetAirState() =>
			_inAir = !_isGrounded;

		private void SetGroundState() =>
			_isGrounded = Physics2D.OverlapCircle(_groundCheck.position, _groundCheckRadius, _groundLayer);

		private void OnDrawGizmos()
		{
			Gizmos.color = _isGrounded ? Color.red : Color.blue;
			Gizmos.DrawWireSphere(_groundCheck.position, _groundCheckRadius);
		}
	}
}