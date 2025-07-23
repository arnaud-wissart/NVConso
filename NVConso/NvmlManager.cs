using Microsoft.Extensions.Logging;
using System.Runtime.InteropServices;

namespace NVConso
{
    public class NvmlManager(ILogger<NvmlManager> logger) : INvmlManager
    {
        private const int EcoPercentage = 10;

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
        private static extern int nvmlDeviceGetCount(out int deviceCount);

        [DllImport("nvml.dll", CallingConvention = CallingConvention.Cdecl)]
        private static extern int nvmlDeviceGetPowerManagementMode(IntPtr device, out uint mode);

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

        public bool Initialize()
        {
            if (nvmlInit_v2() != 0) return false;
            if (nvmlDeviceGetHandleByIndex(0, out _device) != 0) return false;
            if (nvmlDeviceGetPowerManagementLimitConstraints(_device, out _minLimit, out _maxLimit) != 0) return false;
            return true;
        }

        public void Shutdown() => _ = nvmlShutdown();

        public uint GetCurrentPowerLimit()
        {
            _ = nvmlDeviceGetPowerManagementLimit(_device, out uint currentLimit);
            return currentLimit;
        }

        public bool CheckCompatibility(out string message)
        {
            message = string.Empty;

            int result = nvmlInit_v2();
            if (result != 0)
            {
                message = $"Initialisation NVML impossible : {GetNvmlError(result)}";
                return false;
            }

            result = nvmlDeviceGetHandleByIndex(0, out var device);
            if (result != 0)
            {
                message = "Aucun GPU NVIDIA compatible n'a été trouvé.";
                _ = nvmlShutdown();
                return false;
            }

            result = nvmlDeviceGetPowerManagementLimitConstraints(device, out uint min, out uint max);
            if (result != 0)
            {
                message = "La carte ne prend pas en charge la modification du Power Limit.";
                _ = nvmlShutdown();
                return false;
            }

            result = nvmlDeviceGetPowerManagementLimit(device, out uint current);
            if (result == 0)
                result = nvmlDeviceSetPowerManagementLimit(device, current);

            _ = nvmlShutdown();

            if (result != 0)
            {
                message = "Permission insuffisante pour modifier la limite de puissance (exécutez en mode administrateur).";
                return false;
            }

            return true;
        }

        public uint GetPowerLimit(GpuPowerMode mode)
        {
            return mode switch
            {
                GpuPowerMode.Eco => (uint)(_minLimit + (_maxLimit - _minLimit) * EcoPercentage / 100),
                GpuPowerMode.Performance => _maxLimit,
                _ => _maxLimit
            };
        }

        public bool SetPowerLimit(uint targetMilliwatt)
        {
            targetMilliwatt = Math.Clamp(targetMilliwatt, _minLimit, _maxLimit);
            int result = nvmlDeviceSetPowerManagementLimit(_device, targetMilliwatt);

            if (result != 0)
            {
                logger.LogInformation($"[NVML] Erreur : {GetNvmlError(result)} (code {result})");
                return false;
            }

            logger.LogInformation($"[NVML] Limite fixée à {targetMilliwatt} mW");
            return true;
        }
    }
}
