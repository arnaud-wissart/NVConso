using Microsoft.VisualBasic;
using System;

namespace NVConso
{
    public class MockNvmlManager : INvmlManager
    {
        private readonly uint _min;
        private readonly uint _max;
        private uint _current;

        public MockNvmlManager(uint minLimit, uint maxLimit)
        {
            _min = minLimit;
            _max = maxLimit;
            _current = maxLimit;
        }

        public bool Initialize()
        {
            return true;
        }

        public void Shutdown()
        {
            // no-op
        }

        public uint GetCurrentPowerLimit()
        {
            return _current;
        }

        public uint GetEcoLimit()
        {
            return (uint)(_min + (_max - _min) * Constants.EcoPercentage / 100);
        }

        public uint GetPerformanceLimit()
        {
            return _max;
        }

        public uint GetPowerLimit(GpuPowerMode mode)
        {
            return mode switch
            {
                GpuPowerMode.Eco => GetEcoLimit(),
                GpuPowerMode.Performance => GetPerformanceLimit(),
                _ => throw new ArgumentOutOfRangeException(nameof(mode), "Mode inconnu")
            };
        }

        public bool SetPowerLimit(uint targetMilliwatt)
        {
            _current = Math.Clamp(targetMilliwatt, _min, _max);
            return true;
        }
    }
}
