using System;
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
        private List<Callback> _callbacks = new();

        private InputSystemUIInputModule _inputModule;

        private void Start()
        {
            _inputModule = GetComponent<InputSystemUIInputModule>();
            _inputModule.cancel.action.performed += Cancel;
        }

        private void Cancel(InputContext ctx)
        {
            for (int i = 0; i < _callbacks.Count; i++)
            {
                if (_callbacks[i].callback.TryExecute())
                    return;
            }
        }

        private void OnDestroy()
        {
            _inputModule.cancel.action.performed -= Cancel;
        }

        public void AddAction(PredicateAction callback, int priority)
        {
            for (int i = 0; i < _callbacks.Count; i++)
            {
                if (_callbacks[i].priority < priority)
                {
                    _callbacks.Insert(i, new Callback(priority, callback));
                    return;
                }
            }
            _callbacks.Add(new Callback(priority, callback));
        }

        private class Callback
        {
            public int priority;
            public PredicateAction callback;

            public Callback(int priority, PredicateAction callback)
            {
                this.priority = priority;
                this.callback = callback;
            }
        }
    }
}
