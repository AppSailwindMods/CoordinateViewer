using SailwindModdingHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CoordinateViewer.Scripts
{
    internal static class CoordinateRecording
    {
        private static float recordDelay;
        private static bool justStarted;
        public static int currentDay = -1;
        private static (double, double) lastPos;

        static float delay;
        static bool released = true;

        const double DECIMAL_PLACES = 1e10d;

        public static void StartRecording()
        {
            CoordinateRecorderUI.instance.UI.SetActive(true);
            justStarted = true;
            recordDelay = 0;
            lastPos = GetCurrentPlayerCoords();
        }

        public static void StopRecording()
        {
            CoordinateRecorderUI.instance.UI.SetActive(false);
            RecordCurrentCoords();
        }

        public static void ToggleRecording()
        {
            if (CoordinateRecorderUI.instance.UI.activeSelf)
            {
                StopRecording();
            }
            else
            {
                StartRecording();
            }
        }

        public static void RecordCoords()
        {
            if (!Utilities.GamePaused)
            {
                recordDelay += Time.unscaledDeltaTime;
                if (recordDelay >= CoordinateViewerMain.instance.recordTimer.Value || justStarted)
                {
                    justStarted = false;
                    recordDelay = 0;
                    RecordCurrentCoords();
                }
            }
        }

        public static void RecordCurrentCoords()
        {
            if (currentDay != GameState.day)
            {
                currentDay = GameState.day;
                File.AppendAllText(Path.Combine(CoordinateViewerMain.instance.Info.GetFolderLocation(), $"coords_{SaveSlots.currentSlot}.txt"), $"Day: {currentDay}" + Environment.NewLine);
            }
            var playerPos = GetCurrentPlayerCoords();
            float time = Sun.sun.globalTime;
            var currentWind = Wind.currentWind;
            File.AppendAllText(Path.Combine(CoordinateViewerMain.instance.Info.GetFolderLocation(), $"coords_{SaveSlots.currentSlot}.txt"), $"{playerPos.Item2} {playerPos.Item1} {time} {NormalizeAngle(Mathf.Atan2(currentWind.z, currentWind.x) * 180f / Mathf.PI)}" + Environment.NewLine);
            lastPos = playerPos;
        }

        private static (double, double) GetCurrentPlayerCoords()
        {
            Vector3 globePos = Utilities.GetPlayerGlobeCoords();
            double lat = Math.Round(globePos.z * DECIMAL_PLACES) / DECIMAL_PLACES;
            double longi = Math.Round(globePos.x * DECIMAL_PLACES) / DECIMAL_PLACES;
            return (longi, lat);
        }

        private static float NormalizeAngle(float angle)
        {
            while(angle < 0)
            {
                angle += 360;
            }

            while(angle >= 360)
            {
                angle -= 360;
            }

            return angle;
        }

        public static void PlayerInput()
        {
            var down = CoordinateViewerMain.instance.recordKey.Value.IsPressed();
            var up = CoordinateViewerMain.instance.recordKey.Value.IsUp();
            if (down && released)
            {
                delay += Time.deltaTime;
            }

            if (!down && released)
            {
                delay = 0;
            }

            if (up && !released)
            {
                released = true;
            }

            if (delay >= 2 && released)
            {
                delay = 0;
                released = false;
                CoordinateRecording.ToggleRecording();
            }

        }
    }
}
