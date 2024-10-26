//#define ENABLE_THIS_FOR_EXAMPLE
// remove the comment above to enable the example config
#if ENABLE_THIS_FOR_EXAMPLE

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;
using Evryway.ADBC;

namespace ExampleApp
{
    public class ADBCConfigExample : ADBCConfig
    {
        public override void Setup(ADBCWindow window)
        {

            // by default, AdbControl will use the system path, and the Unity editor preferences,
            // to find the ADB executable location.
            // if this fails, or you need a specific version of ADB, you are able to point to the adb executable here.
            
            //ADBControl.SetExePathOverride("z:/path/to/adb.exe");

            // add a common command - for example, a button to press to process your builds.
            //window.AddCommonCommand("Example Build", () => BuildMyApp("Standard"));


            // add a utility command - a button to press to perform some action.
            //window.AddUtilityCommand("Example Utility", AdbUtilityFunc);

            // filter a specific string from all log output.
            //LogLine.AddFilter("exact_match");
        }

        // an example function for a build option.
        // this function takes a string parameter (see lambda above)

        void BuildMyApp(string build_id)
        {

            string build_root = System.IO.Path.Combine(Application.dataPath, "../../../Build");
            string apk_name = $"{PlayerSettings.applicationIdentifier}_{PlayerSettings.bundleVersion}_{build_id}.apk";
            string output_path = $"{build_root}/Android/{apk_name}";

            var bpo = new BuildPlayerOptions
            {
                //scenes = { },
                //extraScriptingDefines = ...,
                target = BuildTarget.Android,
                targetGroup = BuildTargetGroup.Android,
                options = BuildOptions.AllowDebugging,
                locationPathName = output_path,
            };

            BuildPipeline.BuildPlayer(bpo);
        }

        // an example function for a utility option.
        // this function takes no parameters, and can be passed directly to AddUtilityOption

        void AdbUtilityFunc()
        {
            string output;
            string err;
            ADBControl.Run(new List<string> { "devices" }, out output, out err);

            // c# 7.0 ...
            //ADBControl.Run(new List<string> { "devices" }, out string output, out string err);
            Debug.Log(output);
            Debug.Log(err);
        }
    }
}
#endif
