using System;
using Beautify.Universal;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Unity.IL2CPP;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

namespace FXsettings
{
    [BepInPlugin("HC_FXsettings", "HC_FXsettings", "1.1.0")]
    public class FXsettings : BasePlugin
    {
        private static ConfigEntry<bool> AutoApply;
        private static ConfigEntry<float> RenderScale;
        private static ConfigEntry<bool> AllowDownsampling;
        private static ConfigEntry<bool> DownsamplingBilinear;
        private static ConfigEntry<float> DownsamplingScale;
        private static ConfigEntry<int> MainShadowResolution;
        private static ConfigEntry<int> ShadowCascadeCount;
        private static ConfigEntry<bool> SoftShadows;
        private static ConfigEntry<bool> AllowMSAA;
        private static ConfigEntry<int> MSAAQuality;
        private static ConfigEntry<swAntiAliasingMode> SoftwareAntiAliasing;
        private static ConfigEntry<swAntiAliasingQuality> SoftwareAntiAliasingQuality;
        private static ConfigEntry<bool> BeautifyFiltering;
        private static ConfigEntry<bool> EnableToneMapping;
        private static ConfigEntry<float> TonemapExposure;
        private static ConfigEntry<float> TonemapBrightness;
        private static ConfigEntry<float> Brightness;
        private static ConfigEntry<bool> Sharpening;
        private static ConfigEntry<float> SharpenIntensity;
        private static ConfigEntry<float> SharpenRelaxation;
        private static ConfigEntry<float> SharpenMotionSensibility;
        private static ConfigEntry<float> BloomIntensity;
        private static ConfigEntry<float> BloomThreshold;
        private static ConfigEntry<float> BloomMaxBrightness;
        private static ConfigEntry<float> AnamorphicFlaresIntensity;
        private static ConfigEntry<float> AnamorphicFlaresThreshold;
        private static ConfigEntry<bool> ColorTweaks;
        private static ConfigEntry<float> Saturation;
        private static ConfigEntry<float> Contrast;
        private static ConfigEntry<float> Daltonize;
        private static ConfigEntry<float> Sepia;
        private static ConfigEntry<float> Red;
        private static ConfigEntry<float> Green;
        private static ConfigEntry<float> Blue;
        private static ConfigEntry<float> ColorTemp;
        private static ConfigEntry<float> ColorTempBlend;

        public static Scene scene;
        public static Volume volume;
        public static Camera postProcessCamera;
        public static Beautify.Universal.Beautify beautifyInstance;

        public override void Load()
        {
            AutoApply = Config.Bind("Apply on Startup (has to be enabled or effects will reset on map change)", "Apply on Startup", true, "Apply graphics settings when you start the game or a new scene is loaded");
            AutoApply.SettingChanged += (sender, args) => ApplySettings();
            //Resolution
            RenderScale = Config.Bind("Resolution", "Render scale", 1f, new ConfigDescription("Set render scale. Changing this mid scene will make the main light dissapear until scene change", new AcceptableValueRange<float>(0.1f, 4f)));
            RenderScale.SettingChanged += (sender, args) => ApplyUnitySettings();
            AllowDownsampling = Config.Bind("Resolution", "Downsampling", false, "Toggle downsampling");
            AllowDownsampling.SettingChanged += (sender, args) => ApplySettings();
            DownsamplingBilinear = Config.Bind("Resolution", "Bilinear downsampling", false, "Toggle bilinear downsampling");
            DownsamplingBilinear.SettingChanged += (sender, args) => ApplySettings();
            DownsamplingScale = Config.Bind("Resolution", "Downsampling scale", 1f, new ConfigDescription("Set downsampling scale", new AcceptableValueRange<float>(1f, 32f)));
            DownsamplingScale.SettingChanged += (sender, args) => ApplySettings();
            //Shadows
            MainShadowResolution = Config.Bind("Shadows", "Main shadow resolution", 8192, new ConfigDescription("Set main shadow resolution", new AcceptableValueList<int>(128, 256, 512, 1024, 2048, 4096, 6144, 8192, 12288, 16384)));
            MainShadowResolution.SettingChanged += (sender, args) => ApplyUnitySettings();
            ShadowCascadeCount = Config.Bind("Shadows", "Shadow cascade count", 4, new ConfigDescription("Set shadow cascade count", new AcceptableValueList<int>(1, 2, 3, 4)));
            ShadowCascadeCount.SettingChanged += (sender, args) => ApplyUnitySettings();
            SoftShadows = Config.Bind("Shadows", "Soft shadows", true, "Toggle soft shadows");
            SoftShadows.SettingChanged += (sender, args) => ApplyUnitySettings();
            //Antialiasing
            AllowMSAA = Config.Bind("Anti aliasing", "MSAA", true, "Enable MSAA");
            AllowMSAA.SettingChanged += (sender, args) => ApplyUnitySettings();
            MSAAQuality = Config.Bind("Anti aliasing", "MSAA quality", 0, new ConfigDescription("Set MSAA quality", new AcceptableValueList<int>(0, 2, 4, 8, 16)));
            MSAAQuality.SettingChanged += (sender, args) => ApplyUnitySettings();
            SoftwareAntiAliasing = Config.Bind("Anti Aliasing", "Postprocess antialiasing", swAntiAliasingMode.None, "Set postprocess antialiasing mode");
            SoftwareAntiAliasing.SettingChanged += (sender, args) => ApplyUnitySettings();
            SoftwareAntiAliasingQuality = Config.Bind("Anti Aliasing", "SMAA quality", swAntiAliasingQuality.High, "Set postprocess antialiasing quality");
            SoftwareAntiAliasingQuality.SettingChanged += (sender, args) => ApplyUnitySettings();
            //Postprocessing effects
            BeautifyFiltering = Config.Bind("Beautify filtering", "Toggle all Beautify filtering", true, "Toggle all Beautify filtering");
            BeautifyFiltering.SettingChanged += (sender, args) => ApplySettings();
            BloomIntensity = Config.Bind("Bloom and flares", "Bloom intensity", 0.3f, new ConfigDescription("Set bloom intensity", new AcceptableValueRange<float>(0f, 10f)));
            BloomIntensity.SettingChanged += (sender, args) => ApplySettings();
            BloomThreshold = Config.Bind("Bloom and flares", "Bloom threshold", 0.85f, new ConfigDescription("Set bloom threshold", new AcceptableValueRange<float>(0f, 5f)));
            BloomThreshold.SettingChanged += (sender, args) => ApplySettings();
            BloomMaxBrightness = Config.Bind("Bloom and flares", "Bloom max brightness", 5f, new ConfigDescription("Set bloom max brightness", new AcceptableValueRange<float>(0f, 10f)));
            BloomMaxBrightness.SettingChanged += (sender, args) => ApplySettings();
            AnamorphicFlaresIntensity = Config.Bind("Bloom and flares", "Anamorphic flares intensity", 0.1f, new ConfigDescription("Set anamorphic flares intensity", new AcceptableValueRange<float>(0f, 10f)));
            AnamorphicFlaresIntensity.SettingChanged += (sender, args) => ApplySettings();
            AnamorphicFlaresThreshold = Config.Bind("Bloom and flares", "Anamorphic flares threshold", 1f, new ConfigDescription("Set anamorphic flares threshold", new AcceptableValueRange<float>(0f, 5f)));
            AnamorphicFlaresThreshold.SettingChanged += (sender, args) => ApplySettings();
            Sharpening = Config.Bind("Sharpening", "Sharpening", true, "Toggle sharpening");
            Sharpening.SettingChanged += (sender, args) => ApplySettings();
            SharpenIntensity = Config.Bind("Sharpening", "Sharpen intensity", 6f, new ConfigDescription("Set sharpen intensity", new AcceptableValueRange<float>(0f, 25f)));
            SharpenIntensity.SettingChanged += (sender, args) => ApplySettings();
            SharpenRelaxation = Config.Bind("Sharpening", "Sharpening relaxation", 0.05f, new ConfigDescription("Relaxation: sharpen is subtler on high contrasted areas", new AcceptableValueRange<float>(0f, 0.2f)));
            SharpenRelaxation.SettingChanged += (sender, args) => ApplySettings();
            SharpenMotionSensibility = Config.Bind("Sharpening", "Sharpening motion sensibility", 0.05f, new ConfigDescription("Motion Sensibility: reduces sharpen effect while camera moves/rotates", new AcceptableValueRange<float>(0f, 1f)));
            SharpenMotionSensibility.SettingChanged += (sender, args) => ApplySettings();
            //Color and image adjustments
            Contrast = Config.Bind("Image adjustments", "Contrast", 1f, new ConfigDescription("Set contrast", new AcceptableValueRange<float>(0.5f, 1.5f)));
            Contrast.SettingChanged += (sender, args) => ApplySettings();
            Sepia = Config.Bind("Image adjustments", "Sepia", 0f, new ConfigDescription("Set sepia", new AcceptableValueRange<float>(0f, 2f)));
            Sepia.SettingChanged += (sender, args) => ApplySettings();
            EnableToneMapping = Config.Bind("Image adjustments", "Tone mapping", true, "Toggle tone mapping");
            EnableToneMapping.SettingChanged += (sender, args) => ApplySettings();
            TonemapExposure = Config.Bind("Image adjustments", "Tone mapping exposure", 2.6f, new ConfigDescription("Set tone mapping exposure", new AcceptableValueRange<float>(0f, 10f)));
            TonemapExposure.SettingChanged += (sender, args) => ApplySettings();
            TonemapBrightness = Config.Bind("Image adjustments", "Tone mapping brightness", 1.1f, new ConfigDescription("Set tone mapping brightness", new AcceptableValueRange<float>(0f, 10f)));
            TonemapBrightness.SettingChanged += (sender, args) => ApplySettings();
            Brightness = Config.Bind("Image adjustments", "Brightness", 1f, new ConfigDescription("Set brightness", new AcceptableValueRange<float>(0f, 2f)));
            Brightness.SettingChanged += (sender, args) => ApplySettings();
            ColorTweaks = Config.Bind("Color", "Color tweaks", true, "Toggle color tweaks");
            ColorTweaks.SettingChanged += (sender, args) => ApplySettings();
            Saturation = Config.Bind("Color", "Saturation", 1f, new ConfigDescription("Set saturation", new AcceptableValueRange<float>(-2f, 3f)));
            Saturation.SettingChanged += (sender, args) => ApplySettings();
            ColorTemp = Config.Bind("Color", "Color temperature", 6192f, new ConfigDescription("Set color temperature", new AcceptableValueRange<float>(1000f, 40000f)));
            ColorTemp.SettingChanged += (sender, args) => ApplySettings();
            ColorTempBlend = Config.Bind("Color", "Color temperature blend", 1f, new ConfigDescription("Set color temperature blend", new AcceptableValueRange<float>(0f, 1f)));
            ColorTempBlend.SettingChanged += (sender, args) => ApplySettings();
            Red = Config.Bind("Color", "Red", 1f, new ConfigDescription("Set red", new AcceptableValueRange<float>(0f, 1f)));
            Red.SettingChanged += (sender, args) => ApplySettings();
            Green = Config.Bind("Color", "Green", 1f, new ConfigDescription("Set green", new AcceptableValueRange<float>(0f, 1f)));
            Green.SettingChanged += (sender, args) => ApplySettings();
            Blue = Config.Bind("Color", "Blue", 1f, new ConfigDescription("Set blue", new AcceptableValueRange<float>(0f, 1f)));
            Blue.SettingChanged += (sender, args) => ApplySettings();
            Daltonize = Config.Bind("Image adjustments", "Daltonize", 0.3f, new ConfigDescription("Set daltonize", new AcceptableValueRange<float>(0f, 2f)));
            Daltonize.SettingChanged += (sender, args) => ApplySettings();

            //on new scene
            SceneManager.add_sceneLoaded(new Action<Scene, LoadSceneMode>((s, lsm) =>
            {
                //Check if map/charamake gameobject has been loaded
                var gameObject = s.GetRootGameObjects()[0];
                if (AutoApply.Value && (gameObject.name == "Map") || (gameObject.name == "CustomScene"))
                {
                    //Check if main game or charamake and get volume + camera
                    if (gameObject.name == "Map")
                    {
                        volume = BeautifySettings.FindBeautifyVolume();
                        volume.profile.TryGet(out beautifyInstance);
                        postProcessCamera = GameObject.Find("Manager(Clone)").GetComponentInChildren<Camera>();
                    }
                    else
                    {
                        var charaCustomComponent = gameObject.GetComponent<CharacterCreation.HumanCustom>();
                        volume = charaCustomComponent._cameraEffect._volume;
                        volume.profile.TryGet(out beautifyInstance);
                        postProcessCamera = charaCustomComponent._mainCamera;
                    }
                    ApplySettings();
                    ApplyUnitySettings();
                }
            }));
        }

        private enum swAntiAliasingMode
        {
            None,
            FXAA,
            SMAA
        }

        private enum swAntiAliasingQuality
        {
            Low,
            Medium,
            High
        }

        private void ApplySettings()
        {
            if (beautifyInstance != null)
            {
                //Apply beautify settings
                beautifyInstance.disabled.Override(!BeautifyFiltering.Value);
                beautifyInstance.downsampling.Override(AllowDownsampling.Value);
                beautifyInstance.downsamplingMultiplier.Override(DownsamplingScale.Value);
                beautifyInstance.downsamplingBilinear.Override(DownsamplingBilinear.Value);
                beautifyInstance.stripBeautifySharpen.Override(!Sharpening.Value);
                beautifyInstance.sharpenIntensity.Override(SharpenIntensity.Value);
                beautifyInstance.sharpenRelaxation.Override(SharpenRelaxation.Value);
                beautifyInstance.sharpenMotionSensibility.Override(SharpenMotionSensibility.Value);
                beautifyInstance.stripBeautifyTonemapping.Override(!EnableToneMapping.Value);
                beautifyInstance.tonemapExposurePre.Override(TonemapExposure.Value);
                beautifyInstance.tonemapBrightnessPost.Override(TonemapBrightness.Value);
                beautifyInstance.brightness.Override(Brightness.Value);
                beautifyInstance.bloomIntensity.Override(BloomIntensity.Value);
                beautifyInstance.bloomThreshold.Override(BloomThreshold.Value);
                beautifyInstance.bloomMaxBrightness.Override(BloomMaxBrightness.Value);
                beautifyInstance.anamorphicFlaresIntensity.Override(AnamorphicFlaresIntensity.Value);
                beautifyInstance.anamorphicFlaresThreshold.Override(AnamorphicFlaresThreshold.Value);
                beautifyInstance.stripBeautifyColorTweaks.Override(!ColorTweaks.Value);
                beautifyInstance.saturate.Override(Saturation.Value);
                beautifyInstance.contrast.Override(Contrast.Value);
                beautifyInstance.daltonize.Override(Daltonize.Value);
                beautifyInstance.sepia.Override(Sepia.Value);
                beautifyInstance.tintColor.Override(new Color(Red.Value, Green.Value, Blue.Value, 1f));
                beautifyInstance.colorTemp.Override(ColorTemp.Value);
                beautifyInstance.colorTempBlend.Override(ColorTempBlend.Value);
            }
            else
                Log.LogError("Beautify instance not found");
        }

        private void ApplyUnitySettings()
        {
            if (postProcessCamera != null)
            {
                //Apply unity settings
                postProcessCamera.allowMSAA = AllowMSAA.Value;
                UniversalRenderPipeline.asset.msaaSampleCount = MSAAQuality.Value;
                UniversalRenderPipeline.asset.renderScale = RenderScale.Value;
                UniversalRenderPipeline.asset.mainLightShadowmapResolution = MainShadowResolution.Value;
                UniversalRenderPipeline.asset.supportsSoftShadows = SoftShadows.Value;
                UniversalRenderPipeline.asset.shadowCascadeCount = ShadowCascadeCount.Value;

                //Apply software antialiasing mode
                if (SoftwareAntiAliasing.Value == swAntiAliasingMode.None)
                    postProcessCamera.GetUniversalAdditionalCameraData().antialiasing = AntialiasingMode.None;
                else if (SoftwareAntiAliasing.Value == swAntiAliasingMode.SMAA)
                    postProcessCamera.GetUniversalAdditionalCameraData().antialiasing = AntialiasingMode.SubpixelMorphologicalAntiAliasing;
                else if (SoftwareAntiAliasing.Value == swAntiAliasingMode.FXAA)
                    postProcessCamera.GetUniversalAdditionalCameraData().antialiasing = AntialiasingMode.FastApproximateAntialiasing;

                //Apply software antialiasing quality
                if (SoftwareAntiAliasingQuality.Value == swAntiAliasingQuality.High)
                    postProcessCamera.GetUniversalAdditionalCameraData().antialiasingQuality = AntialiasingQuality.High;
                else if (SoftwareAntiAliasingQuality.Value == swAntiAliasingQuality.Medium)
                    postProcessCamera.GetUniversalAdditionalCameraData().antialiasingQuality = AntialiasingQuality.Medium;
                else if (SoftwareAntiAliasingQuality.Value == swAntiAliasingQuality.Low)
                    postProcessCamera.GetUniversalAdditionalCameraData().antialiasingQuality = AntialiasingQuality.Low;
            }
            else
                Log.LogError("Post processing camera not found");
        }
    }
}
