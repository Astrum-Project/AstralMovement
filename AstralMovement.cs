global using Astrum.AstralCore;
global using Astrum.AstralCore.UI.Attributes;
using MelonLoader;
using System;
using System.Linq;
using VRC.SDKBase;

[assembly: MelonInfo(typeof(Astrum.AstralMovement), nameof(Astrum.AstralMovement), "0.6.0", downloadLink: "github.com/Astrum-Project/" + nameof(Astrum.AstralMovement))]
[assembly: MelonGame("VRChat", "VRChat")]
[assembly: MelonColor(ConsoleColor.DarkMagenta)]
[assembly: MelonOptionalDependencies("ActionMenuApi")]

namespace Astrum
{
    public partial class AstralMovement : MelonMod
    {
        public override void OnApplicationStart()
        {
            HighStep.Initialize();
            Serialize.Initialize();

            if (AppDomain.CurrentDomain.GetAssemblies().Any(x => x.GetName().Name == "ActionMenuApi"))
                Extern.InitializeActionMenu();
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

        [UIButton("Movement", "Teleport")]
        public static void Teleport()
        {
            VRCPlayerApi player;
            if (AstralCore.Managers.SelectionManager.SelectedPlayer is null)
                player = Networking.LocalPlayer;
            else player = VRCPlayerApi.AllPlayers.Find(
                UnhollowerRuntimeLib.DelegateSupport.ConvertDelegate<Il2CppSystem.Predicate<VRCPlayerApi>>(
                    new Predicate<VRCPlayerApi>(x => x.displayName == AstralCore.Managers.SelectionManager.SelectedPlayer.displayName)
                )
            );
        }

        internal class Extern
        {
            public static void InitializeActionMenu()
            {
                ActionMenuApi.Api.VRCActionMenuPage.AddSubMenu(ActionMenuApi.Api.ActionMenuPage.Main, "Movement", () =>
                {
                    ActionMenuApi.Api.CustomSubMenu.AddToggle("Flight", Flight.State, state => Flight.State = state);
                    ActionMenuApi.Api.CustomSubMenu.AddToggle("Noclip", Flight.NoClip, state => Flight.NoClip = state);
                    ActionMenuApi.Api.CustomSubMenu.AddToggle("Serialize", Serialize.State, state => Serialize.State = state);
                });
            }
        }
    }
}
