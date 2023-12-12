using SailwindModdingHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CoordinateViewer.Scripts
{
    internal class CoordinateNotificationUI : MonoBehaviour
    {
        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            text = UI.GetComponentInChildren<TextMesh>();
            text.GetComponent<Renderer>().material.renderQueue = 3002;
        }

        private void ShowCoords()
        {
            if (!UI.activeSelf) return;
            Vector3 globePos = Utilities.GetPlayerGlobeCoords();
            text.text = $"Longitude: {globePos.x.ToString($"n{CoordinateViewerMain.instance.decimalPrecision.Value}")}\nLatitude: {globePos.z.ToString($"n{CoordinateViewerMain.instance.decimalPrecision.Value}")}";
        }

        public void Toggle()
        {
            UI.SetActive(!UI.activeSelf);
            UISoundPlayer.instance.PlayParchmentSound();
        }

        private void Update()
        {
            ShowCoords();
            if (UI.activeSelf)
            {
                UI.transform.localScale = Vector3.Lerp(UI.transform.localScale, Vector3.one, Time.deltaTime * 5f);
            }
            else
            {
                UI.transform.localScale = new Vector3(0f, 1f, 1f);
            }
        }

        public static CoordinateNotificationUI instance;

        public GameObject UI;
        private TextMesh text;
    }
}
