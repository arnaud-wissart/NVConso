namespace NVConso
{
    public class MockNvmlManager(uint minLimit, uint maxLimit) : INvmlManager
    {
        public bool Initialize() => true;

        public uint GetCurrentPowerLimit() => maxLimit;

        public uint GetEcoLimit() => minLimit + (maxLimit - minLimit) * Constants.EcoPercentage / 100;

        public uint GetPerformanceLimit() => maxLimit;

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
            maxLimit = Math.Clamp(targetMilliwatt, minLimit, maxLimit);
            return true;
        }

        public void Shutdown()
        {
            // no-op
        }
    }
}