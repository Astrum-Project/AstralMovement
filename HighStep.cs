#pragma warning disable CS0618 // Type or member is obsolete

using MelonLoader;
using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using VRC.SDKBase;

namespace Astrum
{
    partial class AstralMovement
    {
        public static class HighStep
        {
            public static PropertyInfo m_maxStepHeight;
            public static MethodInfo m_GetComponent;

            private static bool enabled = false;
            [UIProperty<bool>("Movement", "HighStep")]
            public static bool Enabled
            {
                get => enabled;
                set {
                    enabled = value;
                    if (value)
                        Set(height);
                    else Set(0.5f);
                }
            }

            private static float height = 0;
            [UIProperty<float>("Movement", "HighStep.Height")]
            public static float Height
            {
                get => height;
                set {
                    height = value;
                    if (enabled) 
                        Set(value);
                }
            }

            public static void Initialize()
            {
                m_maxStepHeight = AppDomain.CurrentDomain.GetAssemblies()
                    .First(x => x.GetName().Name == "Assembly-CSharp")
                    .GetExportedTypes()
                    .Where(x => x.BaseType == typeof(MonoBehaviour))
                    .SelectMany(x => x.GetProperties())
                    .Where(x => x.PropertyType == typeof(float))
                    .FirstOrDefault(x => x.Name == "maxStepHeight");

                if (m_maxStepHeight is null)
                    MelonLogger.Warning("Failed to find LocomotionInputController");
                else
                {
                    MelonLogger.Msg("LocomotionInputController is " + m_maxStepHeight.DeclaringType.Name);

                    m_GetComponent = typeof(GameObject).GetMethod(nameof(GameObject.GetComponent), new Type[0] { }).MakeGenericMethod(m_maxStepHeight.DeclaringType);
                }
            }

            public static void Set(float height)
            {
                if (Networking.LocalPlayer?.gameObject is null) return;

                object inputController = m_GetComponent.Invoke(Networking.LocalPlayer.gameObject, new object[0] { });
                m_maxStepHeight.SetValue(inputController, height);
            }
        }
    }
}
