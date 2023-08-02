using AssetFactory.Legacy.UI;
using AssetFactory.UI;
using System;
using UnityEngine;

//Originally from AssetFactory
namespace AssetFactory.Legacy
{
    public class MainMenu : MenuSingleton<MainMenu>
    {
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }

        public void StartGame(string sceneName) => StartGame(sceneName, null);
        public void StartGame(string sceneName, Action callback)
        {
            SceneTransitioner.Inst.LoadScene(sceneName, () =>
            {
                ShowMainMenu(false);
                callback?.Invoke();
            });
        }
        public void ShowMainMenu(bool value)
        {
            if (lockCursorIfDisabled) LockCursor(!value);
            HUDManager.Inst.enabled = !value;
            PauseMenu.Inst.enabled = !value;
            //GameMenu.Inst.enabled = !value;
            enabled = value;
        }
        public void LeaveGame()
        {
            PauseMenu.Inst.Paused = false;
            SceneTransitioner.Inst.LoadScene("MainMenu", () => ShowMainMenu(true));
        }
        public void QuitGame() => Application.Quit();

        protected override void OnEnable()
        {
            base.OnEnable();
            ToMain();
        }
        protected override void OnDisable()
        {
            base.OnDisable();
            currentMenu.Display(false);
        }
    }
}
