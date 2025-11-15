using Common.GlobalContext;
using UnityEngine;

namespace Input
{
	public sealed class JumpActionButton : MonoBehaviour
	{
		public void OnJump()
		{
			G.InputService.CallJump();
		}
	}
}