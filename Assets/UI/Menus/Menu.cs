using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AssetFactory.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Menu : MonoBehaviour
    {
        [SerializeField] private UnityEvent _onDisplay;
        [SerializeField] private UnityEvent _onHide;

        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.Display(false);
        }

        public void Display(bool show)
        {
            _canvasGroup.interactable = _canvasGroup.blocksRaycasts = show;
            if (show)
            {
                _canvasGroup.alpha = 1f;
                _onDisplay.Invoke();
            }
            else
            {
                _canvasGroup.alpha = 0f;
                _onHide.Invoke();
            }
        }
    }
}
