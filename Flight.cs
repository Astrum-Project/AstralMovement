using System.Runtime.CompilerServices;
using UnityEngine;
using VRC.SDKBase;

namespace Astrum
{
    partial class AstralMovement
    {
        public static class Flight
        {
            [UIField<float>("Movement", "Flight.Speed")]
            public static float speed = 8f;

            private static bool state;
            [UIProperty<bool>("Movement", "Flight")]
            public static bool State
            {
                get => state;
                set
                {
                    if (state == value) return;
                    state = value;
                    if (state)
                        Enable();
                    else Disable();
                }
            }

            private static Collider collider;

            private static bool noClip;
            [UIProperty<bool>("Movement", "NoClip")]
            public static bool NoClip
            {
                get => noClip;
                set
                {
                    if (collider == null)
                    {
                        if (Networking.LocalPlayer?.gameObject == null) return;

                        collider = Networking.LocalPlayer.gameObject.GetComponent<Collider>();
                    }

                    collider.enabled = !(noClip = value);
                }
            }
            
            private static bool stored = false;
            private static Vector3 oGrav = new(0, -9.8f, 0);

            public static void Enable()
            {
                if (!stored)
                {
                    stored = true;
                    oGrav = Physics.gravity;
                }

                Physics.gravity = Vector3.zero;

                if (UnityEngine.XR.XRDevice.isPresent)
                    Update += OnUpdateVR;
                else Update += OnUpdateDesktop;

            }

            public static void Disable()
            {
                stored = false;

                Physics.gravity = oGrav;

                if (UnityEngine.XR.XRDevice.isPresent)
                    Update -= OnUpdateVR;
                else Update -= OnUpdateDesktop;
            }

            public static void OnUpdateDesktop()
            {
                if (Networking.LocalPlayer is null) return;

                unsafe
                {
                    // the point of this is to not branch at all (fast)
                    // also, this code is so convoluted i'll know if you copied it
                    // you can't inline the ToByte method, the JIT compiler has to
                    byte shift = ToByte(Input.GetKey(KeyCode.LeftShift));

                    Networking.LocalPlayer.gameObject.transform.position +=
                        Networking.LocalPlayer.gameObject.transform.forward * speed * Time.deltaTime *
                            (ToByte(Input.GetKey(KeyCode.W)) + ~ToByte(Input.GetKey(KeyCode.S)) + 1) * (shift * 8 + 1)
                        + Networking.LocalPlayer.gameObject.transform.right * speed * Time.deltaTime *
                            (ToByte(Input.GetKey(KeyCode.D)) + ~ToByte(Input.GetKey(KeyCode.A)) + 1) * (shift * 8 + 1)
                        + Networking.LocalPlayer.gameObject.transform.up * speed * Time.deltaTime *
                            (ToByte(Input.GetKey(KeyCode.E)) + ~ToByte(Input.GetKey(KeyCode.Q)) + 1) * (shift * 8 + 1);
                }

                CheckGravity();

                Networking.LocalPlayer.SetVelocity(new Vector3(0f, 0f, 0f));
            }

            public static void OnUpdateVR()
            {
                if (Networking.LocalPlayer is null) return;

                Networking.LocalPlayer.gameObject.transform.position +=
                    Networking.LocalPlayer.gameObject.transform.forward * speed * Time.deltaTime * Input.GetAxis("Vertical")
                    + Networking.LocalPlayer.gameObject.transform.right * speed * Time.deltaTime * Input.GetAxis("Horizontal")
                    + Networking.LocalPlayer.gameObject.transform.up * speed * Time.deltaTime * Input.GetAxis("Oculus_CrossPlatform_SecondaryThumbstickVertical");

                CheckGravity();

                Networking.LocalPlayer.SetVelocity(new Vector3(0f, 0f, 0f));
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static void CheckGravity()
            {
                if (Physics.gravity.y != 0)
                {
                    oGrav = Physics.gravity;
                    Physics.gravity = Vector3.zero;
                }
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private static unsafe byte ToByte(bool from) => *(byte*)&from;
        }
    }
}
