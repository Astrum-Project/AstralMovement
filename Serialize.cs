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
            [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
            private delegate IntPtr OpRaiseEventSetupDelegate(IntPtr instance, byte __0, IntPtr __1, IntPtr __2, IntPtr __3, IntPtr nativeMethodInfoPtr);
            private static OpRaiseEventSetupDelegate opRaiseEventSetupDelegate;
            public static Il2CppSystem.Object cache { get; set; }
            private static bool enabled = false;
            public static bool Enabled 
            {
                get => enabled; 
                set 
                {
                    if (enabled == value) return;
                    enabled = value;
                    cache = null;
                }
            }
            public static unsafe void Initialize()
            {
                try
                {
                    var originalMethod = *(IntPtr*)(IntPtr)UnhollowerUtils.GetIl2CppMethodInfoPointerFieldForGeneratedMethod(AstralCore.Types.VRCNetworkingClient.m_OpRaiseEvent).GetValue(null);
                    MelonUtils.NativeHookAttach((IntPtr)(&originalMethod), typeof(Serialize).GetMethod(nameof(OpRaiseEventDetour), BindingFlags.Static | BindingFlags.NonPublic)!.MethodHandle.GetFunctionPointer());
                    opRaiseEventSetupDelegate = Marshal.GetDelegateForFunctionPointer<OpRaiseEventSetupDelegate>(originalMethod);
                }
                catch(Exception e) { MelonLogger.Warning($"OpRaiseEvent Hook Failed!\n{e}"); }
            }
            private static IntPtr OpRaiseEventDetour(IntPtr instance, byte __0, IntPtr __1, IntPtr __2, IntPtr __3, IntPtr nativeMethodInfoPtr)
            {
                try
                {
                    if (Enabled && __0 == 7)
                    {
                        Il2CppSystem.Object obj = UnhollowerSupport.Il2CppObjectPtrToIl2CppObject<Il2CppSystem.Object>(__1);
                        if (cache == null)
                            cache = obj;

                        else return opRaiseEventSetupDelegate(instance, __0, cache.Pointer, __2, __3, nativeMethodInfoPtr);
                    }
                }
                catch(Exception e) { MelonLogger.Error($"OpRaiseEvent Error!\n{e}"); }
                return opRaiseEventSetupDelegate(instance, __0, __1, __2, __3, nativeMethodInfoPtr);
            }
        }
    }
}
