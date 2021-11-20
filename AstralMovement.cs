using Astrum.AstralCore.Managers;
using MelonLoader;
using System;
using System.Linq;
using VRC.SDKBase;

[assembly: MelonInfo(typeof(Astrum.AstralMovement), nameof(Astrum.AstralMovement), "0.1.0", downloadLink: "github.com/Astrum-Project/" + nameof(Astrum.AstralMovement))]
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
            HighStep.Initialize();

            RegisterPrefs();

            if (AppDomain.CurrentDomain.GetAssemblies().Any(x => x.GetName().Name == "AstralCore"))
            {
                hasCore = true;
                Extern.SetupCommands();
            }
            else MelonLogger.Warning("AstralCore is missing, running at reduced functionality");
        }

        public override void OnSceneWasLoaded(int index, string _)
        {
            if (index != -1) return;

            MelonCoroutines.Start(WaitForLocalLoad());
        }

        private static System.Collections.IEnumerator WaitForLocalLoad()
        {
            while (Networking.LocalPlayer?.gameObject is null) yield return null;

            if (HighStep.Enabled && HighStep.m_maxStepHeight != null)
                HighStep.Set(null);
        }

        internal static Action Update = new Action(() => { });
        public override void OnUpdate() => Update();

        internal static class Extern
        {
            public static void SetupCommands()
            {
                ModuleManager.Module module = new ModuleManager.Module("Movement");

                module.Register(new CommandManager.ConVar<bool>(new Action<bool>(state =>
                {
                    Flight.Toggle(state);
                    AstralCore.Logger.Notif("Flight " + (state ? "on" : "off"));
                })), "Flight", "Fly");

                module.Register("Flight.Speed", new CommandManager.ConVar<float>(new Action<float>(value => Flight.speed = value)));
            }
        }
    }
}
