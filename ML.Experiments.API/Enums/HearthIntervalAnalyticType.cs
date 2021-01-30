namespace ML.Experiments.API.Enums
{
    /*
        enum class AnalyticTypes(val type: Byte) 
        {
            Unknown(0), Steady(1), PhysicalStress(2),
            LowPhysical(3), MediumPhysical(4), SleepingLight(7), Sleeping(8)
        }
     */
    public enum HearthIntervalAnalyticType
    {
        Unknown = 0, 
        Steady = 1, // Minimal activity (< 30 steps per minute)
        PhysicalStress = 2, // Ultra high activity ( > 110 steps per minute)
        LowPhysical = 3, // Low activity (< 80 steps per minute)
        MediumPhysical = 4, // Medium activity (< 110 steps per minute)
        SleepingLight = 7, // Quick sleeping phase
        Sleeping = 8, // Deep sleeping phase
    }
}
