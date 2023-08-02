using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace AssetFactory.Legacy
{
    public class CommandPrompt : MonoSingleton<CommandPrompt>
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void StartUp()
        {
#if !UNITY_SERVER
            new GameObject("Command Prompt").AddComponent<CommandPrompt>();
#endif
        }

        public static Dictionary<string, Command> commands;

        public bool Activated { get; set; }
        private string commandText;


        public void ExecuteCommand(string command)
        {
            if (string.IsNullOrEmpty(command)) return;

            string[] args = command.Split(' ');
            if (commands.TryGetValue(args[0], out Command c))
            {
                if (c.TryExecute(args))
                {
                    commandText = string.Empty;
                }
            }
        }
        public static void AddCommand(Command command)
        {
            if (commands.ContainsKey(command.Key)) return;

            commands.Add(command.Key, command);
        }

        private void Update()
        {
            if (Keyboard.current.leftCtrlKey.isPressed)
            {
                if (Keyboard.current.f3Key.wasPressedThisFrame)
                    Activated = !Activated;
            }
            if (Activated && Keyboard.current.enterKey.wasPressedThisFrame)
            {
                ExecuteCommand(commandText);
            }
        }

        private const int FIELD_HEIGHT = 25;
        private const int BUTTON_WIDTH = 100;
        private void OnGUI()
        {
            if (!Activated) return;

            Rect fieldRect = new Rect(0, Screen.height - FIELD_HEIGHT, Screen.width - BUTTON_WIDTH, FIELD_HEIGHT);
            Rect buttonRect = fieldRect;
            buttonRect.x = fieldRect.width;
            buttonRect.width = BUTTON_WIDTH;

            commandText = GUI.TextField(fieldRect, commandText);
            if (GUI.Button(buttonRect, "Execute"))
            {
                ExecuteCommand(commandText);
                Cursor.visible = true;
            }
        }
        protected override void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}
