using UnityEngine;

namespace AssetFactory.Legacy
{
    public abstract class Command
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Setup()
        {
            if (CommandPrompt.commands != null) return;
            Command[] list = new Command[]
            {
                new LogCommand(),
                new TimeScaleCommand()
				//new ResetCommand(),
				//new FlyCommand()
			};
            CommandPrompt.commands = new System.Collections.Generic.Dictionary<string, Command>(list.Length);
            foreach (var c in list)
            {
                CommandPrompt.AddCommand(c);
            }
        }

        public abstract string Key { get; }
        protected abstract int MinArgs { get; }
        public bool TryExecute(string[] args)
        {
            if (args.Length - 1 < MinArgs)
            {
                Debug.LogWarning($"The command '{Key}' requires at least {MinArgs} arguments.");
                return false;
            }
            try
            {
                Execute(args);
                return true;
            }
            catch (System.Exception e)
            {
                Debug.LogError(e);
                return false;
            }
        }
        protected abstract void Execute(string[] args);
    }
    public class LogCommand : Command
    {
        public override string Key => "log";
        protected override int MinArgs => 1;
        protected override void Execute(string[] args)
        {
            Debug.Log(args[1]);
        }
    }
    //public class ResetCommand : Command
    //{
    //	public override string Key => "reset";

    //	protected override int MinArgs => 0;

    //	protected override void Execute(string[] args)
    //	{
    //		if (Player.Exists)
    //			Player.Inst.Respawn();
    //	}
    //}

    //public class FlyCommand : Command
    //{
    //	public override string Key => "fly";

    //	protected override int MinArgs => 0;

    //	protected override void Execute(string[] args)
    //	{
    //		PlatformerPlayer.Inst.FlyMode = !PlatformerPlayer.Inst.FlyMode;
    //	}
    //}
    public class TimeScaleCommand : Command
    {
        public override string Key => "timescale";
        protected override int MinArgs => 1;

        protected override void Execute(string[] args)
        {
            if (float.TryParse(args[1], out float scale))
            {
                PauseMenu.Inst.TimeScale = scale;
            }
        }
    }
}
