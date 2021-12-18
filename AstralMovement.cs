global using Astrum.AstralCore;
global using Astrum.AstralCore.UI.Attributes;
using MelonLoader;
using System;
using System.Linq;
using VRC.SDKBase;

[assembly: MelonInfo(typeof(Astrum.AstralMovement), nameof(Astrum.AstralMovement), "0.5.1", downloadLink: "github.com/Astrum-Project/" + nameof(Astrum.AstralMovement))]
[assembly: MelonGame("VRChat", "VRChat")]
[assembly: MelonColor(ConsoleColor.DarkMagenta)]
[assembly: MelonOptionalDependencies("AstralCore")]

namespace Astrum
{
    public partial class AstralMovement : MelonMod
    {
        public override void OnApplicationStart()
        {
            HighStep.Initialize();
            if (AppDomain.CurrentDomain.GetAssemblies().Any(x => x.GetName().Name == "AstralCore"))
                Serialize.Initialize();
            else LoggerInstance.Warning("AstralCore is missing, running at reduced functionality");
        }

        public override void OnSceneWasLoaded(int index, string _)
        {
            if (index != -1) return;

            MelonCoroutines.Start(WaitForLocalLoad());
        }

        public override void OnSceneWasUnloaded(int index, string _)
        {
            if (index != -1) return;

            Serialize.State = false;
        }

        private static System.Collections.IEnumerator WaitForLocalLoad()
        {
            while (Networking.LocalPlayer?.gameObject is null) yield return null;

            if (HighStep.Enabled && HighStep.m_maxStepHeight != null)
                HighStep.Set(HighStep.Height);
        }

        internal static Action Update = new(() => { });
        public override void OnUpdate() => Update();
    }
}
