namespace NVConso
{
    public interface INvmlManager
    {
        bool Initialize();
        void Shutdown();
        uint GetCurrentPowerLimit();
        uint GetPowerLimit(GpuPowerMode mode);
        bool SetPowerLimit(uint targetMilliwatt);
    }
}
