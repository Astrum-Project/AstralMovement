using MelonLoader;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Astrum
{
    partial class AstralMovement
    {
        public void RegisterPrefs()
        {
            MelonPreferences_Category category = MelonPreferences.CreateCategory("Astrum-AstralMovement", "Astral Movement");

            category.CreateEntry("highStep", false, "High Step");
            category.CreateEntry("highStepHeight", 2.0f, "High Step Height");

            OnPreferencesLoaded();
        }

        public override void OnPreferencesSaved() => OnPreferencesLoaded();
        public override void OnPreferencesLoaded()
        {
            MelonPreferences_Category category = MelonPreferences.GetCategory("Astrum-AstralMovement");

            HighStep.Enabled = category.GetEntry<bool>("highStep").Value;
            HighStep.Height = category.GetEntry<float>("highStepHeight").Value;
        }
    }
}
