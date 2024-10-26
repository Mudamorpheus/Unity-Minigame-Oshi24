using UnityEngine;
using UnityEngine.UI;


    /// <summary>
    /// Safe area implementation for notched mobile devices. Usage:
    ///  (1) Add this component to the top level of any GUI panel. 
    ///  (2) If the panel uses a full screen background image, then create an immediate child and put the component on that instead, with all other elements childed below it.
    ///      This will allow the background image to stretch to the full extents of the screen behind the notch, which looks nicer.
    ///  (3) For other cases that use a mixture of full horizontal and vertical background stripes, use the Conform X & Y controls on separate elements as needed.
    /// </summary>
    public class SafeArea : MonoBehaviour {
        #region Simulations

        /// <summary>
        /// Simulation device that uses safe area due to a physical notch or software home bar. For use in Editor only.
        /// </summary>
        public enum SimDevice {
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
        public static SimDevice Sim = SimDevice.None;

        /// <summary>
        /// Normalised safe areas for iPhone X with Home indicator (ratios are identical to iPhone Xs). Absolute values:
        ///  PortraitU x=0, y=102, w=1125, h=2202 on full extents w=1125, h=2436;
        ///  PortraitD x=0, y=102, w=1125, h=2202 on full extents w=1125, h=2436 (not supported, remains in Portrait Up);
        ///  LandscapeL x=132, y=63, w=2172, h=1062 on full extents w=2436, h=1125;
        ///  LandscapeR x=132, y=63, w=2172, h=1062 on full extents w=2436, h=1125.
        ///  Aspect Ratio: ~19.5:9.
        /// </summary>
        Rect[] NSA_iPhoneX = new Rect[] {
            new Rect(0f, 102f / 2436f, 1f, 2202f / 2436f), // Portrait
            new Rect(132f / 2436f, 63f / 1125f, 2172f / 2436f, 1062f / 1125f) // Landscape
        };

        /// <summary>
        /// Normalised safe areas for iPhone Xs Max with Home indicator (ratios are identical to iPhone XR). Absolute values:
        ///  PortraitU x=0, y=102, w=1242, h=2454 on full extents w=1242, h=2688;
        ///  PortraitD x=0, y=102, w=1242, h=2454 on full extents w=1242, h=2688 (not supported, remains in Portrait Up);
        ///  LandscapeL x=132, y=63, w=2424, h=1179 on full extents w=2688, h=1242;
        ///  LandscapeR x=132, y=63, w=2424, h=1179 on full extents w=2688, h=1242.
        ///  Aspect Ratio: ~19.5:9.
        /// </summary>
        Rect[] NSA_iPhoneXsMax = new Rect[] {
            new Rect(0f, 102f / 2688f, 1f, 2454f / 2688f), // Portrait
            new Rect(132f / 2688f, 63f / 1242f, 2424f / 2688f, 1179f / 1242f) // Landscape
        };

        #endregion

        public GameObject BottomBlackGo,TopBlackGo, LeftBlackGo,RightBlackGo;
        RectTransform BottomBlack, LeftBlack,RightBlack,TopBlack;
        public RectTransform[] Panels;
        Rect LastSafeArea = new Rect(0, 0, 0, 0);
        [SerializeField] private bool X, XR, NONE;
        [SerializeField]
        bool ConformX = true; // Conform to screen safe area on X-axis (default true, disable to ignore)
        [SerializeField]
        bool ConformY = true; // Conform to screen safe area on Y-axis (default true, disable to ignore)

        void Awake() {
            TopBlack = TopBlackGo.GetComponent<RectTransform>();
            BottomBlack = BottomBlackGo.GetComponent<RectTransform>();
            LeftBlack = LeftBlackGo.GetComponent<RectTransform>();
            RightBlack = RightBlackGo.GetComponent<RectTransform>();
            Refresh();
            ApplyIphoneSafeArea();
   

        }

        public void ApplyIphoneSafeArea() {


#if UNITY_EDITOR
            if (X)
                Sim = SimDevice.iPhoneX;
            if (XR)
                Sim = SimDevice.iPhoneXsMax;
#endif

#if UNITY_IOS || UNITY_IPHONE
            if (UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhoneX ||
                UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhoneXS ||
                UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhoneXSMax || 
                UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone11 ||
                UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone11Pro ||
                UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone12 ||
                UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone12Pro ||
                UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone12ProMax ||
                UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone13 ||
                UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone13Pro ||
                UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone13ProMax ||
                UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone14 ||
                UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone14Pro ||
                UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone14ProMax ||
                UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhone11ProMax || X) {
                Sim = SimDevice.iPhoneX;
                SetBlackOffset("X");
            }
            else if (UnityEngine.iOS.Device.generation == UnityEngine.iOS.DeviceGeneration.iPhoneXR || XR) {
                Sim = SimDevice.iPhoneXsMax;
                SetBlackOffset("XR");

            }
            else {
                Sim = SimDevice.None;
            }


            #elif UNITY_ANDROID || UNITY_EDITOR
                if (Screen.width == 2246 && Screen.height == 1080) Sim = SimDevice.iPhoneX;
                else Sim = SimDevice.None; //deviceIsIphoneX ? SimDevice.iPhoneX : SimDevice.None;
            #endif
        }

        void Update() {
            Refresh();
            ApplyIphoneSafeArea();

        }

        void Refresh() {
            var safeArea = GetSafeArea();

            if (safeArea != LastSafeArea)
                ApplySafeArea(safeArea);
        }

       public Rect GetSafeArea() {
            var safeArea = Screen.safeArea;

            if (Application.isEditor && Sim != SimDevice.None) {
                var nsa = new Rect(0, 0, Screen.width, Screen.height);

                switch (Sim) {
                    case SimDevice.iPhoneX:
                        if (Screen.height > Screen.width) // Portrait
                            nsa = NSA_iPhoneX[0];
                        else // Landscape
                            nsa = NSA_iPhoneX[1];
                        break;

                    case SimDevice.iPhoneXsMax:
                        if (Screen.height > Screen.width) // Portrait
                            nsa = NSA_iPhoneXsMax[0];
                        else // Landscape
                            nsa = NSA_iPhoneXsMax[1];
                        break;
                    default:
                        break;
                }

                safeArea = new Rect(Screen.width * nsa.x, Screen.height * nsa.y, Screen.width * nsa.width,
                    Screen.height * nsa.height);
            }

            return safeArea;
        }

        void ApplySafeArea(Rect r) {
            LastSafeArea = r;

            // Ignore x-axis?
            if (!ConformX) {
                r.x = 0;
                r.width = Screen.width;
            }

            // Ignore y-axis?
            if (!ConformY) {
                r.y = 0;
                r.height = Screen.height;
            }

            // Convert safe area rectangle from absolute pixels to normalised anchor coordinates
            Vector2 anchorMin = r.position;
            Vector2 anchorMax = r.position + r.size;
            anchorMin.x /= Screen.width;
            anchorMin.y /= Screen.height;
            anchorMax.x /= Screen.width;
            anchorMax.y /= Screen.height;

            foreach (var Panels in Panels) {
                Panels.anchorMin = anchorMin;
                Panels.anchorMax = anchorMax;
                
            }

          
            //XR OFFSET
            //68 bottom 
            //88 top
            //X OFFSET
            // 102 top
            //132 bottom

            Debug.LogFormat("New safe area applied to {0}: x={1}, y={2}, w={3}, h={4} on full extents w={5}, h={6}",
                name, r.x, r.y, r.width, r.height, Screen.width, Screen.height);

        }

        private void SetBlackOffset(string Iphone) {
            
            var top = Screen.height - GetSafeArea().height - GetSafeArea().y;
            var bottom = Screen.height - GetSafeArea().height - top;
            var left = Screen.width - GetSafeArea().width - GetSafeArea().x;
            var right = Screen.width - GetSafeArea().width - left;
            switch (Iphone) {
                case "X": case "XR" :{
                    BottomBlackGo.SetActive(true);
                    LeftBlackGo.SetActive(true);
                    RightBlackGo.SetActive(true);
                    TopBlackGo.SetActive(true);
                    BottomBlack.sizeDelta = new Vector2(Screen.width, bottom);
                    TopBlack.sizeDelta = new Vector2(Screen.width,top);
                    LeftBlack.sizeDelta = new Vector2(left, Screen.height);
                    RightBlack.sizeDelta = new Vector2(right,Screen.height);
                    break;
                }
            }
        }
    }
