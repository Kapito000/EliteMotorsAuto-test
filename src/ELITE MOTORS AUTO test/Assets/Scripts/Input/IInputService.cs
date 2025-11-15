using System;

namespace Input
{
	public interface IInputService
	{
		event Action Jump;
		float Horizontal { get; }
		void CallJump();
		void CallHorizontal(float horizontal);
	}
}