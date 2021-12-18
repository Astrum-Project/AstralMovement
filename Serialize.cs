using MelonLoader;
using System;
using System.Reflection;
using System.Runtime.InteropServices;
using UnhollowerBaseLib;

namespace Astrum
{
    partial class AstralMovement
    {
        public static class Serialize
        {
            private static bool state = false;
            [UIProperty<bool>("Movement", "Serialize")]
            public static bool State 
            {
                get => state; 
                set 
                {
                    if (state == value) return;
                    state = value;
                    cache = null;
                }
            }

            public static Il2CppSystem.Object cache { get; set; }

            public static unsafe void Initialize()
            {
                try
                {
                    var originalMethod = *(IntPtr*)(IntPtr)UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(AstralCore.Types.VRCNetworkingClient.m_OpRaiseEvent).GetValue(null);
                    MelonUtils.NativeHookAttach((IntPtr)(&originalMethod), typeof(Serialize).GetMethod(nameof(OpRaiseEventDetour), BindingFlags.Static | BindingFlags.NonPublic)!.MethodHandle.GetFunctionPointer());
                    opRaiseEventSetupDelegate = Marshal.GetDelegateForFunctionPointer<OpRaiseEventSetupDelegate>(originalMethod);
                }
                catch(Exception e) { Logger.Warn($"Failed to hook OpRaiseEvent:\n{e}"); }
            }

            private static IntPtr OpRaiseEventDetour(IntPtr instance, byte __0, IntPtr __1, IntPtr __2, IntPtr __3, IntPtr nativeMethodInfoPtr)
            {
                try
                {
                    if (state && __0 == 7)
                    {
                        Il2CppSystem.Object obj = UnhollowerSupport.Il2CppObjectPtrToIl2CppObject<Il2CppSystem.Object>(__1);
                        if (cache == null)
                            cache = obj;

                        else return opRaiseEventSetupDelegate(instance, __0, cache.Pointer, __2, __3, nativeMethodInfoPtr);
                    }
                }
                catch(Exception e) { Logger.Error($"An exception has occurred in hook OpRaiseEvent:\n{e}"); }
                return opRaiseEventSetupDelegate(instance, __0, __1, __2, __3, nativeMethodInfoPtr);
            }

            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate IntPtr OpRaiseEventSetupDelegate(IntPtr instance, byte __0, IntPtr __1, IntPtr __2, IntPtr __3, IntPtr nativeMethodInfoPtr);
            private static OpRaiseEventSetupDelegate opRaiseEventSetupDelegate;
        }
    }
}
