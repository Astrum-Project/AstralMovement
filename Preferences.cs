using MelonLoader;
using System;

namespace Astrum
{
    partial class AstralMovement
    {
        public static class Prefs
        {
            public static MelonPreferences_Entry<bool> highStep;
            public static MelonPreferences_Entry<float> highStepHeight;

            public static MelonPreferences_Entry<bool> infJump;

            public static MelonPreferences_Entry<float> flightSpeed;
            public static MelonPreferences_Entry<bool> flightNoClip;
            public static MelonPreferences_Entry<bool> serialize;

            public static void Initalize()
            {
                MelonPreferences_Category category = MelonPreferences.CreateCategory("Astrum-AstralMovement", "Astral Movement");

                category.Create(ref highStep, nameof(highStep), "High Step", false, (_, value) => HighStep.Enabled = value);
                category.Create(ref highStepHeight, nameof(highStepHeight), "High Step Height", 2.0f, (_, value) => HighStep.Height = value);

                category.Create(ref infJump, nameof(infJump), "Infinite Jump", false, (_, value) => Jumping.State = value);

                // there seems to be no way to make a MelonPref unsaved
                // as a result, im keeping flight core only
                if (hasCore)
                {
                    category.Create(ref flightSpeed, nameof(flightSpeed), "Flight Speed", 8.0f, (_, value) => Flight.speed = value);
                    category.Create(ref flightNoClip, nameof(flightNoClip), "Flight No Clip", false, (_, value) => Flight.NoClip = value);
                    category.Create(ref serialize, nameof(serialize), "Serialize", false, (_, value) => Serialize.Enabled = value);
                }
            }
        }
    }

    internal static class Extensions
    {
        internal static void Create<T>(this MelonPreferences_Category category, ref MelonPreferences_Entry<T> entry, string id, string name, T def, Action<T, T> OnValueChanged)
        {
            (entry = category.CreateEntry(id, def, name)).OnValueChanged += OnValueChanged;
            if (!def.Equals(entry.Value)) // since ML can't do it on its own
                OnValueChanged(def, entry.Value);
        }
    }
}
