using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering;

namespace CoreDang
{
    public class FPSDisplay : MonoBehaviour
    {
        float deltaTime = 0.0f;
        int line = 2;

        void OnGUI()
        {
            float msec = deltaTime * 1000.0f;
            float fps = 1.0f / deltaTime;
            int w = Screen.width, h = Screen.height;
            float textH = h * 2 / 140;
            var rect = new Rect(0, 0, w, textH);

            var boxRect = rect;
            boxRect.x = w - 350;
            boxRect.width = 350;
            boxRect.height = textH * line;
            GUI.Box(boxRect, "");
            var style = new GUIStyle { alignment = TextAnchor.UpperRight, fontSize = (int)textH };
            style.normal.textColor = new Color(1.0f, 1.0f, 0.0f, 1.0f);
            GUI.Label(rect, string.Format("{0:0.0} ms {1:0.}/{2:0.} fps", msec, fps, Application.targetFrameRate), style);

            rect.y += textH;
            GUI.Label(rect, string.Format("({0},{1})|{2} DPI",
                Screen.currentResolution.width,
                Screen.currentResolution.height,
                Screen.dpi
                ), style);
        }

        void Update()
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
        }
    }
}
