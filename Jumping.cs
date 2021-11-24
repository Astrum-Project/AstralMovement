using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using VRC.Animation;

namespace Astrum
{
    partial class AstralMovement
    {
        public static class Jumping 
        {
            private static bool state;
            public static bool State
            {
                get => state;
                set
                {
                    if (state == value) return;
                    state = value;
                    Toggle(state);
                }
            }

            private static VRCMotionState motion;

            private static void Toggle(bool state)
            {
                if (state) AstralMovement.Update += Update;
                else AstralMovement.Update -= Update;
            }

            private static void Update()
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    if (motion == null)
                        motion = FetchMotion();

                    motion.field_Private_Boolean_0 = true;
                }
            }

            private static VRCMotionState FetchMotion() =>VRC.SDKBase.Networking.LocalPlayer.gameObject.GetComponent<VRCMotionState>();
        }
    }
}
