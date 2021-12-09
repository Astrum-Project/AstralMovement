using Astrum.AstralCore.Managers;
using MelonLoader;
using System;
using System.Linq;
using VRC.SDKBase;

[assembly: MelonInfo(typeof(Astrum.AstralMovement), nameof(Astrum.AstralMovement), "0.3.0", downloadLink: "github.com/Astrum-Project/" + nameof(Astrum.AstralMovement))]
[assembly: MelonGame("VRChat", "VRChat")]
[assembly: MelonColor(ConsoleColor.DarkMagenta)]
[assembly: MelonOptionalDependencies("AstralCore")]

namespace Astrum
{
    public partial class AstralMovement : MelonMod
    {
        public static bool hasCore = false;

        public override void OnApplicationStart()
        {
            hasCore = AppDomain.CurrentDomain.GetAssemblies().Any(x => x.GetName().Name == "AstralCore");

            HighStep.Initialize();
            Prefs.Initalize();
            if (hasCore)
            {
                Serialize.Initialize();
                Extern.SetupCommands();
            }
            else MelonLogger.Warning("AstralCore is missing, running at reduced functionality");
        }

        public override void OnSceneWasLoaded(int index, string _)
        {
            if (index != -1) return;
            Prefs.serialize.Value = false;
            MelonCoroutines.Start(WaitForLocalLoad());
        }

        private static System.Collections.IEnumerator WaitForLocalLoad()
        {
            while (Networking.LocalPlayer?.gameObject is null) yield return null;

            if (HighStep.Enabled && HighStep.m_maxStepHeight != null)
                HighStep.Set(HighStep.Height);
        }

        internal static Action Update = new Action(() => { });
        public override void OnUpdate() => Update();

        internal static class Extern
        {
            public static void SetupCommands()
            {
                ModuleManager.Module module = new ModuleManager.Module("Movement");

                module.Register(new CommandManager.ConVar<bool>(new Action<bool>(state => Prefs.highStep.Value = state), false), "HighStep");
                module.Register(new CommandManager.ConVar<float>(new Action<float>(value => Prefs.highStepHeight.Value = value)), "HighStep.Height");

                module.Register(new CommandManager.ConVar<bool>(new Action<bool>(state => Prefs.infJump.Value = state)), "Infinite Jump");

                module.Register(new CommandManager.ConVar<bool>(new Action<bool>(state => Flight.State = state)), "Flight");
                module.Register(new CommandManager.ConVar<float>(new Action<float>(value => Prefs.flightSpeed.Value = value)), "Flight.Speed");
                module.Register(new CommandManager.ConVar<bool>(new Action<bool>(state => Prefs.flightNoClip.Value = state)), "Flight.NoClip");

                module.Register(new CommandManager.ConVar<bool>(new Action<bool>(state => Prefs.serialize.Value = state)), "Serialize");
            }
        }
    }
}
