using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace AssetFactory.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class Menu : MonoBehaviour
    {
        [SerializeField] private GameObject _firstSelection;
        [SerializeField] private UnityEvent _onDisplay;
        [SerializeField] private UnityEvent _onHide;

        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _canvasGroup.Display(false);
        }

        /// <returns>First selection</returns>
        public GameObject Display(bool show)
        {
            _canvasGroup.interactable = _canvasGroup.blocksRaycasts = show;
            if (show)
            {
                _canvasGroup.alpha = 1f;
                _onDisplay.Invoke();
                return _firstSelection;
            }
            else
            {
                _canvasGroup.alpha = 0f;
                _onHide.Invoke();
                return null;
            }
        }
    }
}
