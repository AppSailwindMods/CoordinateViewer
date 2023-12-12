using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using CoordinateViewer.Scripts;
using HarmonyLib;
using SailwindModdingHelper;
using System.Collections;
using System.Reflection;
using UnityEngine;

namespace CoordinateViewer
{
    [BepInPlugin(GUID, NAME, VERSION)]
    [BepInDependency(SailwindModdingHelperMain.GUID, "2.0.0")]
    public class CoordinateViewerMain : BaseUnityPlugin
    {
        public const string GUID = "com.app24.coordinateviewer";
        public const string NAME = "Coordinate Viewer";
        public const string VERSION = "1.3.2";

        internal static ManualLogSource logSource;

        private ConfigEntry<KeyboardShortcut> coordinateKey;
        internal ConfigEntry<KeyboardShortcut> recordKey;

        internal ConfigEntry<int> decimalPrecision;
        internal ConfigEntry<float> recordTimer;

        internal static CoordinateViewerMain instance;

        private void Awake()
        {
            instance = this;
            logSource = Logger;
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), GUID);

            coordinateKey = Config.Bind("Hotkeys", "Coordinate Key", new KeyboardShortcut(KeyCode.R, KeyCode.LeftAlt));
            recordKey = Config.Bind("Hotkeys", "Record Key", new KeyboardShortcut(KeyCode.N));

            decimalPrecision = Config.Bind("Values", "Decimal Precision", 3);
            recordTimer = Config.Bind("Values", "Record Timer", 30f);

            GameEvents.OnGameStart += (_, __) =>
            {
                GameObject notificationUIParent = GameObject.FindObjectOfType<NotificationUi>().gameObject;
                GameObject notificationUI = notificationUIParent.transform.GetChild(0).gameObject;

                GameObject placeholder = new GameObject();
                placeholder.transform.parent = GameCanvas.Transform;
                placeholder.transform.localPosition = notificationUIParent.transform.localPosition;
                placeholder.transform.localRotation = notificationUIParent.transform.localRotation;
                placeholder.transform.localScale = notificationUIParent.transform.localScale;

                GameObject ui = GameObject.Instantiate(notificationUI);
                ui.transform.parent = placeholder.transform;
                ui.transform.localPosition = notificationUI.transform.localPosition;
                ui.transform.localRotation = notificationUI.transform.localRotation;
                ui.transform.localScale = Vector3.one;
                placeholder.AddComponent<CoordinateNotificationUI>().UI = ui;
            };

            GameEvents.OnGameStart += (_, __) =>
            {
                GameObject notificationUIParent = GameObject.FindObjectOfType<NotificationUi>().gameObject;
                GameObject notificationUI = notificationUIParent.transform.GetChild(0).gameObject;

                GameObject placeholder = new GameObject();
                placeholder.transform.parent = GameCanvas.Transform;
                placeholder.transform.localPosition = notificationUIParent.transform.localPosition;
                placeholder.transform.localRotation = notificationUIParent.transform.localRotation;
                placeholder.transform.localScale = notificationUIParent.transform.localScale;

                GameObject ui = new GameObject();
                ui.transform.parent = placeholder.transform;
                ui.transform.localPosition = new Vector3(-1.7f, 3.3f, 1.042f);
                ui.transform.localRotation = Quaternion.identity;
                ui.transform.localScale = Vector3.one;
                placeholder.AddComponent<CoordinateRecorderUI>().UI = ui;

                GameObject textGameObject = new GameObject();
                textGameObject.transform.parent = ui.transform;
                textGameObject.transform.localPosition = new Vector3(-0.001f, 0f, -0.03f);
                textGameObject.transform.localRotation = Quaternion.Euler(0, -2.732076e-05f, 0);
                textGameObject.transform.localScale = new Vector3(0.01399471f, 0.01399469f, 0.01399471f);
                TextMesh text = textGameObject.AddComponent<TextMesh>();
                text.alignment = TextAlignment.Center;
                text.anchor = TextAnchor.MiddleCenter;
                text.font = GameAssets.ArealFont;
                text.fontSize = 60;
                text.text = "Recording...";
            };

            GameEvents.OnPlayerInput += (_, __) =>
            {
                if (coordinateKey.Value.IsDown())
                {
                    CoordinateNotificationUI.instance.Toggle();
                }

                CoordinateRecording.PlayerInput();
            };

            GameEvents.OnNewGame += (_, __) =>
            {
                CoordinateRecording.currentDay = -1;
            };

            GameEvents.OnSaveLoad += (_, __) =>
            {
                StartCoroutine(GetCurrentDay());
            };
        }

        private static IEnumerator GetCurrentDay()
        {
            yield return new WaitForEndOfFrame();
            while (GameState.currentlyLoading)
            {
                yield return null;
            }
            CoordinateRecording.currentDay = GameState.day;
        }
    }
}
