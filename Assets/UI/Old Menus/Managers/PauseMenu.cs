using AssetFactory.Legacy.UI;
using AssetFactory.UI;
using System.Collections;
using UnityEngine;

//Originally from AssetFactory
namespace AssetFactory.Legacy
{
    public class PauseMenu : MenuSingleton<PauseMenu>
    {
        [Delayed]
        [SerializeField] private float timeScale = 1f;

        private bool inPauseMenu;
        private bool timeStopped;

        public static bool IsPaused => Exists && Inst.TimeStopped;
        public float TimeScale
        {
            get => timeScale;
            set
            {
                timeScale = Mathf.Max(0f, value);
                TimeStopped = TimeStopped;
            }
        }
        public bool Paused
        {
            get => inPauseMenu;
            set
            {
                inPauseMenu = value;
                if (value)
                {
                    ToMain();
                }
                else
                {
                    if (currentMenu != null)
                        currentMenu.Display(false);
                }
                if (lockCursorIfDisabled)
                    LockCursor(!value);
                TimeStopped = value;
            }
        }
        public bool TimeStopped
        {
            get => timeStopped;
            set
            {
                timeStopped = value;
                Time.timeScale = value ? 0f : timeScale;
            }
        }


        public void TogglePauseMenu()
        {
            if (!isActiveAndEnabled)
                return;
            //Unpause only if in main screen
            if (Paused && navigationStack.Count > 0)
                return;

            Paused = !Paused;
        }
        public override void Back()
        {
            //Delay to avoid unpausing instead of backing out
            StartCoroutine(BackCoroutine());
        }
        IEnumerator BackCoroutine()
        {
            yield return null;
            base.Back();
        }
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }
        private void OnValidate()
        {
            TimeScale = timeScale;
        }

        //protected override void OnEnable()
        //{
        //	pauseAction.performed += _ => TogglePause();
        //	base.OnEnable();
        //}
        //protected override void OnDisable()
        //{
        //	if (CurrentMenu != null)
        //		CurrentMenu.Display(false);
        //	pauseAction.performed -= _ => TogglePause();
        //	base.OnDisable();
        //}
    }
}
