using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Raymarcher
{
    public static class Time
    {
        public static double FixedDeltaTime
        {
            get
            {
                return (1 / 60D) * FixedScale;
            }
        }
        public static double DeltaTime { get; private set; }
        public static double FixedScale { get; set; } = 1.0D;
        public static double Scale { get; set; } = 1.0D;
        public static double TimeSinceStart
        {
            get
            {
                return _mainClock.Elapsed.TotalSeconds;
            }
        }
        private static DateTime _LastUpdateTime { get; set; } = DateTime.Now;

        private static Stopwatch _mainClock = new Stopwatch();


        [EngineInitializer(50)]
        public static void Initialize()
        {
            _mainClock.Start();
            Updater.OnUpdate += () => ComputeDeltaTime();
        }

        private static void ComputeDeltaTime()
        {
            DateTime now = DateTime.Now;
            DeltaTime = (now - _LastUpdateTime).TotalSeconds;
            _LastUpdateTime = now;
        }

        public static string CurrentDate(TimeFormat format)
        {
            DateTime now = DateTime.Now;

            switch(format)
            {
                case TimeFormat.FR:
                    return now.Day + "/" + now.Month + "/" + now.Year + " " + now.Hour + ":" + now.Minute + ":" + now.Second;

                case TimeFormat.ISO:
                    return now.Year + "-" + now.Month + "-" + now.Day + "_" + now.Hour + "-" + now.Minute + "-" + now.Second;

                default:
                    return CurrentDate(TimeFormat.ISO);
            }
        }
    }

    public enum TimeFormat
    {
        FR,
        ISO
    }
}