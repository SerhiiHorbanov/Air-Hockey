static class FrameTiming
{
    public const int TargetFps = 60;
    private const long TicksBetweenFrames = TimeSpan.TicksPerSecond / TargetFps;
    public const float TargetDeltaSeconds = 1.0f / TargetFps;
    
    public static float DeltaSeconds;
    private static long _lastTimingTick;

    public static void UpdateLastTimingTick()
        => _lastTimingTick = DateTime.Now.Ticks;
    
    public static void Timing()
    {
        long ticksToSleep = TicksBetweenFrames - (DateTime.Now.Ticks - _lastTimingTick);

        DeltaSeconds = (float)(DateTime.Now.Ticks - _lastTimingTick) / TimeSpan.TicksPerSecond;

        UpdateLastTimingTick();
        if (ticksToSleep > 0)
            Thread.Sleep((int)(ticksToSleep / TimeSpan.TicksPerMillisecond));
    }
}