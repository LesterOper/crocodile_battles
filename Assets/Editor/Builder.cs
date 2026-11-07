using System;
using UnityEditor;
using UnityEditor.Build.Reporting;
using static  UnityEditor.BuildPipeline;


    public static class Builder
    {
        [MenuItem("Build/📱Android")]
        public static void BuildAndroid()
        {
            PlayerSettings.Android.keystorePass = "Prokorim8";
            PlayerSettings.Android.keyaliasName = "crocodile";
            PlayerSettings.Android.keyaliasPass =  "Prokorim8";
         var report =   BuildPipeline.BuildPlayer(
                new BuildPlayerOptions
                {
                    target = BuildTarget.Android,
                    locationPathName = "../../artifacts/Game.apk",
                    scenes = new[] {"Assets/Scenes/Common/Initial.unity", "Assets/Scenes/Common/Main.unity", "Assets/Scenes/Common/BattleScene.unity"},

                });

         if (report.summary.result != BuildResult.Succeeded)
         {
             throw  new Exception("Failed to build Android package. See log for details");
         }
        }
    }
