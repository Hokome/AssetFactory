using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

using InputContext = UnityEngine.InputSystem.InputAction.CallbackContext;

namespace AssetFactory
{
    public class CancelManager : MonoSingleton<CancelManager>
    {
        private InputSystemUIInputModule _inputModule;

        private void Start()
        {
            _inputModule = GetComponent<InputSystemUIInputModule>();
            _inputModule.cancel.action.performed += Cancel;
        }

        private void Cancel(InputContext ctx)
        {

        }

        private void OnDestroy()
        {
            _inputModule.cancel.action.performed -= Cancel;
        }
    }
}
