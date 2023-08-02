using AssetFactory.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

//Originally from AssetFactory
namespace AssetFactory.Legacy
{
    [RequireComponent(typeof(CanvasGroup))]
    public class HUDManager : MonoSingleton<HUDManager>
    {
        private CanvasGroup main;

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
            main = GetComponent<CanvasGroup>();
            enabled = false;
        }

        private void OnEnable()
        {
            main.Display(true, false);
        }
        private void OnDisable()
        {
            main.Display(false, false);
        }
    }
}
