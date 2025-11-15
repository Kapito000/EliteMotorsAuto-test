using Common.GlobalContext;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Input
{
	public sealed class HorizontalActionButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
	{
		[SerializeField] private bool Negative;

		private float Value => Negative ? -1 : 1;

		public void OnPointerDown(PointerEventData eventData)
		{
			G.InputService.CallHorizontal(Value);
		}

		public void OnPointerUp(PointerEventData eventData)
		{
			G.InputService.CallHorizontal(0);
		}
	}
}