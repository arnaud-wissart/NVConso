using NvAPIWrapper.GPU;
using NvAPIWrapper;
using NvAPIWrapper.Native;
using System.Runtime.InteropServices;

namespace NVConso
{
    public static class NvmlManager
    {
        private static IntPtr _device;
        private static uint _minLimit;
        private static uint _maxLimit;

        [DllImport("nvml.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int nvmlInit_v2();

        [DllImport("nvml.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int nvmlShutdown();

        [DllImport("nvml.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int nvmlDeviceGetHandleByIndex(int index, out IntPtr device);

        [DllImport("nvml.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int nvmlDeviceGetPowerManagementLimitConstraints(IntPtr device, out uint minLimit, out uint maxLimit);

        [DllImport("nvml.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int nvmlDeviceSetPowerManagementLimit(IntPtr device, uint limit);

        [DllImport("nvml.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int nvmlDeviceGetPowerManagementLimit(IntPtr device, out uint currentLimit);

        [DllImport("nvml.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr nvmlErrorString(int result);

        private static string GetNvmlError(int code)
        {
            IntPtr ptr = nvmlErrorString(code);
            return Marshal.PtrToStringAnsi(ptr) ?? $"Code inconnu {code}";
        }

        public static bool Initialize()
        {
            if (nvmlInit_v2() != 0)
                return false;

            if (nvmlDeviceGetHandleByIndex(0, out _device) != 0)
                return false;

            if (nvmlDeviceGetPowerManagementLimitConstraints(_device, out _minLimit, out _maxLimit) != 0)
                return false;

            return true;
        }

        public static void Shutdown()
        {
            nvmlShutdown();
        }

        public static uint GetCurrentPowerLimit()
        {
            nvmlDeviceGetPowerManagementLimit(_device, out uint currentLimit);
            return currentLimit;
        }

        public static uint GetEcoLimit()
        {
            return (uint)(_minLimit + (_maxLimit - _minLimit) * 10 / 100); // 10% du range
        }

        public static uint GetPerformanceLimit()
        {
            return _maxLimit;
        }

        public static bool SetPowerLimit(uint targetMilliwatt)
        {
            targetMilliwatt = Math.Clamp(targetMilliwatt, _minLimit, _maxLimit);
            int result = nvmlDeviceSetPowerManagementLimit(_device, targetMilliwatt);

            if (result != 0)
            {
                Console.WriteLine($"[NVML] Erreur : {GetNvmlError(result)} (code {result})");
                return false;
            }

            Console.WriteLine($"[NVML] Limite fixée à {targetMilliwatt} mW");
            return true;
        }
    }
}
