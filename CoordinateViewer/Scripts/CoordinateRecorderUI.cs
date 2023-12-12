using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CoordinateViewer.Scripts
{
    internal class CoordinateRecorderUI : MonoBehaviour
    {
        public static CoordinateRecorderUI instance;
        public GameObject UI;
        private TextMesh text;
        private bool reverse;
        private float timer;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            UI.SetActive(false);
            text = UI.GetComponentInChildren<TextMesh>();
            text.color = new Color(0, 0.8f, 0);
        }

        private void Update()
        {
            if (UI.activeSelf)
            {
                CoordinateRecording.RecordCoords();
                if (timer <= 0)
                {
                    timer = 0.5f;
                    reverse = !reverse;
                }

                if (!reverse)
                {
                    text.color = Color.Lerp(text.color, Color.red, Time.unscaledDeltaTime * 20f);
                }
                else
                {
                    text.color = Color.Lerp(text.color, new Color(0, 0.8f, 0), Time.unscaledDeltaTime * 20f);
                }
                timer -= Time.unscaledDeltaTime;
            }
        }
    }
}
