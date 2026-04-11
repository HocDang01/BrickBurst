using System;
using UnityEngine;

namespace CoreDang
{
    /// <summary>
    /// Safe area implementation for notched mobile devices. Usage:
    ///  (1) Add this component to the top level of any GUI panel.
    ///  (2) If the panel uses a full screen background image, then create an immediate child and put the component on that instead, with all other elements childed below it.
    ///      This will allow the background image to stretch to the full extents of the screen behind the notch, which looks nicer.
    ///  (3) For other cases that use a mixture of full horizontal and vertical background stripes, use the Conform X & Y controls on separate elements as needed.
    /// </summary>
    public class SafeArea : MonoBehaviour
    {
        #region Simulations
        /// <summary>
        /// Simulation device that uses safe area due to a physical notch or software home bar. For use in Editor only.
        /// </summary>
        public enum SimDevice
        {
            /// <summary>
            /// Don't use a simulated safe area - GUI will be full screen as normal.
            /// </summary>
            None,
            /// <summary>
            /// Simulate the iPhone X and Xs (identical safe areas).
            /// </summary>
            iPhoneX,
            /// <summary>
            /// Simulate the iPhone Xs Max and XR (identical safe areas).
            /// </summary>
            iPhoneXsMax
        }

        /// <summary>
        /// Simulation mode for use in editor only. This can be edited at runtime to toggle between different safe areas.
        /// </summary>
        public SimDevice Sim = SimDevice.None;

        /// <summary>
        /// Normalised safe areas for iPhone X with Home indicator (ratios are identical to iPhone Xs). Absolute values:
        ///  PortraitU x=0, y=102, w=1125, h=2202 on full extents w=1125, h=2436;
        ///  PortraitD x=0, y=102, w=1125, h=2202 on full extents w=1125, h=2436 (not supported, remains in Portrait Up);
        ///  LandscapeL x=132, y=63, w=2172, h=1062 on full extents w=2436, h=1125;
        ///  LandscapeR x=132, y=63, w=2172, h=1062 on full extents w=2436, h=1125.
        ///  Aspect Ratio: ~19.5:9.
        /// </summary>
        Rect[] NSA_iPhoneX = new Rect[]
        {
            new Rect (0f, 102f / 2436f, 1f, 2202f / 2436f),  // Portrait
            new Rect (132f / 2436f, 63f / 1125f, 2172f / 2436f, 1062f / 1125f)  // Landscape
        };

        /// <summary>
        /// Normalised safe areas for iPhone Xs Max with Home indicator (ratios are identical to iPhone XR). Absolute values:
        ///  PortraitU x=0, y=102, w=1242, h=2454 on full extents w=1242, h=2688;
        ///  PortraitD x=0, y=102, w=1242, h=2454 on full extents w=1242, h=2688 (not supported, remains in Portrait Up);
        ///  LandscapeL x=132, y=63, w=2424, h=1179 on full extents w=2688, h=1242;
        ///  LandscapeR x=132, y=63, w=2424, h=1179 on full extents w=2688, h=1242.
        ///  Aspect Ratio: ~19.5:9.
        /// </summary>
        Rect[] NSA_iPhoneXsMax = new Rect[]
        {
            new Rect (0f, 102f / 2688f, 1f, 2454f / 2688f),  // Portrait
            new Rect (132f / 2688f, 63f / 1242f, 2424f / 2688f, 1179f / 1242f)  // Landscape
        };
        #endregion

        [SerializeField] bool conformX = true;  // Conform to screen safe area on X-axis (default true, disable to ignore)
        [SerializeField] bool conformY = true;  // Conform to screen safe area on Y-axis (default true, disable to ignore)
        [SerializeField] bool applyTop = true;
        [SerializeField] bool applyBottom = true;

        public Action onApplied;
        public RectTransform Panel { get; private set; }
        public bool ApplyingSafeArea { get; private set; }

        public bool ApplyTop { get { return applyTop; } set { applyTop = value; ForceRefresh(); } }
        public bool ApplyBottom { get { return applyBottom; } set { applyBottom = value; ForceRefresh(); } }

        Rect lastSafeArea = Rect.zero;

        void Awake()
        {
            Panel = GetComponent<RectTransform>();
            if(!Panel)
            {
                Debug.LogError("Cannot apply safe area - no RectTransform found on " + name);
                Destroy(gameObject);
            }

            Refresh();
        }

        void Update()
        {
            Refresh();
        }

        void Refresh()
        {
            Rect safeArea = GetSafeArea();
            if(safeArea != lastSafeArea
                && !float.IsNaN(safeArea.x) && !float.IsNaN(safeArea.y)
                && !float.IsNaN(safeArea.width) && !float.IsNaN(safeArea.height))
            {
                ApplySafeArea(safeArea);
                onApplied?.Invoke();
            }
        }

        public void ForceRefresh()
        {
            Rect safeArea = GetSafeArea();
            if(!float.IsNaN(safeArea.x) && !float.IsNaN(safeArea.y)
                && !float.IsNaN(safeArea.width) && !float.IsNaN(safeArea.height))
            {
                ApplySafeArea(safeArea);
                onApplied?.Invoke();
            }
        }

        public Rect GetSafeArea()
        {
#if !UNITY_EDITOR
            return Screen.safeArea;
#else
            Rect safeArea = Screen.safeArea;
            if(Sim != SimDevice.None)
            {
                Rect nsa = new Rect(0, 0, Screen.width, Screen.height);
                switch(Sim)
                {
                    case SimDevice.iPhoneX:
                    if(Screen.height > Screen.width)  // Portrait
                        nsa = NSA_iPhoneX[0];
                    else  // Landscape
                        nsa = NSA_iPhoneX[1];
                    break;
                    case SimDevice.iPhoneXsMax:
                    if(Screen.height > Screen.width)  // Portrait
                        nsa = NSA_iPhoneXsMax[0];
                    else  // Landscape
                        nsa = NSA_iPhoneXsMax[1];
                    break;
                    default:
                    break;
                }

                safeArea = new Rect(Screen.width * nsa.x, Screen.height * nsa.y, Screen.width * nsa.width, Screen.height * nsa.height);
            }
            return safeArea;
#endif
        }

        void ApplySafeArea(Rect safeArea)
        {
            lastSafeArea = safeArea;
            safeArea = UpdateConform(safeArea);

            // Convert safe area rectangle from absolute pixels to normalised anchor coordinates
            Vector2 anchorMin = safeArea.position;
            Vector2 anchorMax = safeArea.position + safeArea.size;
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            ApplyingSafeArea = !Mathf.Approximately(anchorMin.y, 0) || !Mathf.Approximately(anchorMax.y, 1);

            if(applyBottom) Panel.anchorMin = anchorMin;
            else Panel.anchorMin = Vector2.zero;

            if(applyTop) Panel.anchorMax = anchorMax;
            else Panel.anchorMax = Vector2.one;
        }

        public Rect UpdateConform(Rect safeArea)
        {
            // Ignore x-axis?
            if(!conformX)
            {
                safeArea.x = 0;
                safeArea.width = Screen.width;
            }

            // Ignore y-axis?
            if(!conformY)
            {
                safeArea.y = 0;
                safeArea.height = Screen.height;
            }

            return safeArea;
        }
    }
}
