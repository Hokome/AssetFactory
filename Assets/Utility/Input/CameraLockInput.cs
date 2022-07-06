#if CINEMACHINE
using Cinemachine;
#endif
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

//Originally from AssetFactory
namespace AssetFactory
{
#if CINEMACHINE
	public class CameraLockInput : MonoBehaviour, AxisState.IInputAxisProvider
	{
		[SerializeField] private InputActionReference XYAxis = default;

		private InputAction[] cachedActions;

		public bool Locked { get; set; }

		public float GetAxisValue(int axis)
		{
			if (Locked)
				return 0f;
            if (axis > 1 || axis < 0)
                return 0f;
            
			return axis switch
			{
				0 => XYAxis.action.ReadValue<Vector2>().x,
				1 => XYAxis.action.ReadValue<Vector2>().y,
				_ => 0f,
			};
		}
    }
#else
	public class CameraLockInput : MonoBehaviour
	{

	}
#endif
}
