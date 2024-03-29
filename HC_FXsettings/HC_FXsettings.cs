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
    [BepInProcess("HoneyCome")]
    [BepInProcess("HoneyComeccp")]
    [BepInPlugin(GUID, PluginName, PluginVersion)]
    public class FXsettings : BasePlugin
    {
        public const string PluginName = "HC_FXsettings";
        public const string GUID = "HC_FXsettings";
        public const string PluginVersion = "1.3.2";
        //Beautify and Unity settings
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
        private static ConfigEntry<float> TonemapExposureOffset;
        private static ConfigEntry<float> TonemapBrightnessOffset;
        private static ConfigEntry<float> Brightness;
        private static ConfigEntry<bool> Sharpening;
        private static ConfigEntry<float> SharpenIntensity;
        private static ConfigEntry<float> SharpenRelaxation;
        private static ConfigEntry<float> SharpenMotionSensibility;
        private static ConfigEntry<float> sharpenDepthThreshold;
        private static ConfigEntry<float> sharpenMinDepth;
        private static ConfigEntry<float> sharpenMaxDepth;
        private static ConfigEntry<float> sharpenMinMaxDepthFallOff;
        private static ConfigEntry<float> sharpenClamp;
        private static ConfigEntry<float> BloomIntensity;
        private static ConfigEntry<float> BloomThreshold;
        private static ConfigEntry<float> BloomMaxBrightness;
        private static ConfigEntry<float> bloomDepthAtten;
        private static ConfigEntry<float> bloomNearAtten;
        private static ConfigEntry<float> AnamorphicFlaresIntensity;
        private static ConfigEntry<float> AnamorphicFlaresThreshold;
        private static ConfigEntry<bool> AnamorphicFlaresVertical;
        private static ConfigEntry<float> AnamorphicFlaresSpread;
        private static ConfigEntry<float> AnamorphicFlaresDepthAtten;
        private static ConfigEntry<float> AnamorphicFlaresNearAtten;
        private static ConfigEntry<bool> AnamorphicFlaresAntiflicker;
        private static ConfigEntry<float> AnamorphicFlaresR;
        private static ConfigEntry<float> AnamorphicFlaresG;
        private static ConfigEntry<float> AnamorphicFlaresB;
        private static ConfigEntry<float> AnamorphicFlaresA;
        private static ConfigEntry<float> sunFlaresIntensity;
        private static ConfigEntry<bool> ColorTweaks;
        private static ConfigEntry<float> Saturation;
        private static ConfigEntry<bool> chromaticAbberation;
        private static ConfigEntry<float> chromaticAberrationIntensity;
        private static ConfigEntry<float> chromaticAberrationSmoothing;
        private static ConfigEntry<bool> vignette;
        private static ConfigEntry<float> vignettingOuterRing;
        private static ConfigEntry<float> vignettingInnerRing;
        private static ConfigEntry<float> vignettingFade;
        private static ConfigEntry<bool> vignettingCircularShape;
        private static ConfigEntry<float> vignettingAspectRatio;
        private static ConfigEntry<float> Contrast;
        private static ConfigEntry<float> Daltonize;
        private static ConfigEntry<float> Sepia;
        private static ConfigEntry<float> Red;
        private static ConfigEntry<float> Green;
        private static ConfigEntry<float> Blue;
        private static ConfigEntry<float> ColorTemp;
        private static ConfigEntry<float> ColorTempBlend;
        private static ConfigEntry<bool> depthOfField;
        private static ConfigEntry<float> depthOfFieldDistance;
        private static ConfigEntry<float> depthOfFieldFocalLength;
        private static ConfigEntry<float> depthOfFieldAperture;
        private static ConfigEntry<bool> depthOfFieldForegroundBlur;
        private static ConfigEntry<bool> depthOfFieldForegroundBlurHQ;
        private static ConfigEntry<float> depthOfFieldForegroundBlurHQSpread;
        private static ConfigEntry<float> depthOfFieldForegroundDistance;
        private static ConfigEntry<bool> depthOfFieldBokeh;
        private static ConfigEntry<float> depthOfFieldBokehThreshold;
        private static ConfigEntry<float> depthOfFieldBokehIntensity;
        private static ConfigEntry<float> blurIntensity;
        //Charamaker specific settings
        private static ConfigEntry<bool> enableTonemappingCharamaker;
        private static ConfigEntry<float> TonemapExposureOffsetCharamaker;
        private static ConfigEntry<float> TonemapBrightnessOffsetCharamaker;
        private static ConfigEntry<float> BloomIntensityCharamaker;
        private static ConfigEntry<float> BloomThresholdCharamaker;
        private static ConfigEntry<float> SaturationCharamaker;
        //Variables
        private static GameObject gameObject;
        private static Scene scene;
        private static Volume volume;
        private static Camera postProcessCamera;
        private static Beautify.Universal.Beautify beautifyInstance;
        private static float originalTonemapExposure;
        private static float originalTonemapBrightness;

        public override void Load()
        {
            AutoApply = Config.Bind("Apply on Startup (has to be enabled or effects will reset on map change)", "Apply on Startup", true, "Apply graphics settings when you start the game or a new scene is loaded");
            AutoApply.SettingChanged += (sender, args) => ApplySettings();
            //Resolution
            RenderScale = Config.Bind("Resolution", "Render scale", 1f, new ConfigDescription("Set render scale. Changing this mid scene will make the main light dissapear until scene change", new AcceptableValueRange<float>(0.1f, 4f)));
            RenderScale.SettingChanged += (sender, args) => ApplyUnitySettings();
            AllowDownsampling = Config.Bind("Resolution", "Downsampling", false, "Toggle downsampling");
            AllowDownsampling.SettingChanged += (sender, args) => ApplySettings();
            DownsamplingBilinear = Config.Bind("Resolution", "Downsampling bilinear", false, "Toggle bilinear downsampling");
            DownsamplingBilinear.SettingChanged += (sender, args) => ApplySettings();
            DownsamplingScale = Config.Bind("Resolution", "Downsampling scale", 1f, new ConfigDescription("Set downsampling scale", new AcceptableValueRange<float>(1f, 32f)));
            DownsamplingScale.SettingChanged += (sender, args) => ApplySettings();
            //Shadows
            MainShadowResolution = Config.Bind("Shadows", "Main shadow resolution", 4096, new ConfigDescription("Set main shadow resolution", new AcceptableValueList<int>(128, 256, 512, 1024, 2048, 4096, 6144, 8192, 12288, 16384)));
            MainShadowResolution.SettingChanged += (sender, args) => ApplyUnitySettings();
            ShadowCascadeCount = Config.Bind("Shadows", "Shadow cascade count", 4, new ConfigDescription("Set shadow cascade count", new AcceptableValueList<int>(1, 2, 3, 4)));
            ShadowCascadeCount.SettingChanged += (sender, args) => ApplyUnitySettings();
            SoftShadows = Config.Bind("Shadows", "Soft shadows", true, "Toggle soft shadows");
            SoftShadows.SettingChanged += (sender, args) => ApplyUnitySettings();
            //Antialiasing
            AllowMSAA = Config.Bind("Antialiasing", "MSAA", true, "Enable MSAA");
            AllowMSAA.SettingChanged += (sender, args) => ApplyUnitySettings();
            MSAAQuality = Config.Bind("Antialiasing", "MSAA quality", 2, new ConfigDescription("Set MSAA quality", new AcceptableValueList<int>(0, 2, 4, 8, 16)));
            MSAAQuality.SettingChanged += (sender, args) => ApplyUnitySettings();
            SoftwareAntiAliasing = Config.Bind("Antialiasing", "Postprocess antialiasing", swAntiAliasingMode.None, "Set postprocess antialiasing mode");
            SoftwareAntiAliasing.SettingChanged += (sender, args) => ApplyUnitySettings();
            SoftwareAntiAliasingQuality = Config.Bind("Antialiasing", "SMAA quality", swAntiAliasingQuality.High, "Set postprocess antialiasing quality");
            SoftwareAntiAliasingQuality.SettingChanged += (sender, args) => ApplyUnitySettings();
            BeautifyFiltering = Config.Bind("Beautify filtering", "Toggle all Beautify filtering", true, "Toggle all Beautify filtering");
            BeautifyFiltering.SettingChanged += (sender, args) => ApplySettings();
            //Bloom and flares
            BloomIntensity = Config.Bind("Bloom and flares", "Bloom intensity", 0.3f, new ConfigDescription("Set bloom intensity", new AcceptableValueRange<float>(0f, 10f)));
            BloomIntensity.SettingChanged += (sender, args) => ApplySettings();
            BloomThreshold = Config.Bind("Bloom and flares", "Bloom threshold", 0.85f, new ConfigDescription("Set bloom threshold", new AcceptableValueRange<float>(0f, 5f)));
            BloomThreshold.SettingChanged += (sender, args) => ApplySettings();
            BloomMaxBrightness = Config.Bind("Bloom and flares", "Bloom max brightness", 5f, new ConfigDescription("Set bloom max brightness", new AcceptableValueRange<float>(0f, 10f)));
            BloomMaxBrightness.SettingChanged += (sender, args) => ApplySettings();
            bloomDepthAtten = Config.Bind("Bloom and flares", "Bloom attenuation depth", 0.3f, new ConfigDescription("Set bloom depth attenuation", new AcceptableValueRange<float>(0f, 1f)));
            bloomDepthAtten.SettingChanged += (sender, args) => ApplySettings();
            bloomNearAtten = Config.Bind("Bloom and flares", "Bloom attenuation near", 0.5f, new ConfigDescription("Set bloom near attenuation", new AcceptableValueRange<float>(0f, 24f)));
            bloomNearAtten.SettingChanged += (sender, args) => ApplySettings();
            AnamorphicFlaresIntensity = Config.Bind("Bloom and flares", "Anamorphic flares intensity", 0.1f, new ConfigDescription("Set anamorphic flares intensity", new AcceptableValueRange<float>(0f, 10f)));
            AnamorphicFlaresIntensity.SettingChanged += (sender, args) => ApplySettings();
            AnamorphicFlaresThreshold = Config.Bind("Bloom and flares", "Anamorphic flares threshold", 1f, new ConfigDescription("Set anamorphic flares threshold", new AcceptableValueRange<float>(0f, 5f)));
            AnamorphicFlaresThreshold.SettingChanged += (sender, args) => ApplySettings();
            AnamorphicFlaresVertical = Config.Bind("Bloom and flares", "Anamorphic flares vertical", true, "Toggle anamorphic flares vertical");
            AnamorphicFlaresVertical.SettingChanged += (sender, args) => ApplySettings();
            AnamorphicFlaresSpread = Config.Bind("Bloom and flares", "Anamorphic flares spread", 0.2f, new ConfigDescription("Set anamorphic flares spread", new AcceptableValueRange<float>(0.1f, 2f)));
            AnamorphicFlaresSpread.SettingChanged += (sender, args) => ApplySettings();
            AnamorphicFlaresDepthAtten = Config.Bind("Bloom and flares", "Anamorphic flares attenuation depth", 0.97f, new ConfigDescription("Set anamorphic flares depth attenuation", new AcceptableValueRange<float>(0f, 1f)));
            AnamorphicFlaresDepthAtten.SettingChanged += (sender, args) => ApplySettings();
            AnamorphicFlaresNearAtten = Config.Bind("Bloom and flares", "Anamorphic flares attenuation near", 0.97f, new ConfigDescription("Set anamorphic flares near attenuation", new AcceptableValueRange<float>(0f, 24f)));
            AnamorphicFlaresNearAtten.SettingChanged += (sender, args) => ApplySettings();
            AnamorphicFlaresAntiflicker = Config.Bind("Bloom and flares", "Anamorphic flares antiflicker", false, "Toggle anamorphic flares antiflicker");
            AnamorphicFlaresAntiflicker.SettingChanged += (sender, args) => ApplySettings();
            AnamorphicFlaresR = Config.Bind("Bloom and flares", "Anamorphic flares (R)", 1f, new ConfigDescription("Set anamorphic flares red", new AcceptableValueRange<float>(0f, 1f)));
            AnamorphicFlaresR.SettingChanged += (sender, args) => ApplySettings();
            AnamorphicFlaresG = Config.Bind("Bloom and flares", "Anamorphic flares (G)", 1f, new ConfigDescription("Set anamorphic flares green", new AcceptableValueRange<float>(0f, 1f)));
            AnamorphicFlaresG.SettingChanged += (sender, args) => ApplySettings();
            AnamorphicFlaresB = Config.Bind("Bloom and flares", "Anamorphic flares (B)", 1f, new ConfigDescription("Set anamorphic flares blue", new AcceptableValueRange<float>(0f, 1f)));
            AnamorphicFlaresB.SettingChanged += (sender, args) => ApplySettings();
            AnamorphicFlaresA = Config.Bind("Bloom and flares", "Anamorphic flares (A)", 0.25f, new ConfigDescription("Set anamorphic flares alpha", new AcceptableValueRange<float>(0f, 1f)));
            AnamorphicFlaresA.SettingChanged += (sender, args) => ApplySettings();
            //Sharpening
            Sharpening = Config.Bind("Sharpening", "Sharpening", true, "Toggle sharpening");
            Sharpening.SettingChanged += (sender, args) => ApplySettings();
            SharpenIntensity = Config.Bind("Sharpening", "Sharpen intensity", 6f, new ConfigDescription("Set sharpen intensity", new AcceptableValueRange<float>(0f, 25f)));
            SharpenIntensity.SettingChanged += (sender, args) => ApplySettings();
            SharpenRelaxation = Config.Bind("Sharpening", "Sharpening relaxation", 0.05f, new ConfigDescription("Sharpen is subtler on high contrasted areas", new AcceptableValueRange<float>(0f, 0.2f)));
            SharpenRelaxation.SettingChanged += (sender, args) => ApplySettings();
            SharpenMotionSensibility = Config.Bind("Sharpening", "Sharpening motion sensibility", 0.5f, new ConfigDescription("Reduces sharpen effect while camera moves/rotates", new AcceptableValueRange<float>(0f, 1f)));
            SharpenMotionSensibility.SettingChanged += (sender, args) => ApplySettings();
            sharpenDepthThreshold = Config.Bind("Sharpening", "Sharpen depth threshold", 0.97f, new ConfigDescription("Will compute depth difference around pixels to detect edges\nThis will protect thin objects like standalone wires or lines", new AcceptableValueRange<float>(0f, 1f)));
            sharpenDepthThreshold.SettingChanged += (sender, args) => ApplySettings();
            sharpenMinDepth = Config.Bind("Sharpening", "Sharpen min depth", 0.28f, new ConfigDescription("Set sharpen min depth", new AcceptableValueRange<float>(0f, 1.1f)));
            sharpenMinDepth.SettingChanged += (sender, args) => ApplySettings();
            sharpenMaxDepth = Config.Bind("Sharpening", "Sharpen max depth", 0f, new ConfigDescription("Set sharpen max depth", new AcceptableValueRange<float>(0f, 1.1f)));
            sharpenMaxDepth.SettingChanged += (sender, args) => ApplySettings();
            sharpenMinMaxDepthFallOff = Config.Bind("Sharpening", "Sharpen min/max depth falloff", 0.97f, new ConfigDescription("Set sharpen min/max depth falloff", new AcceptableValueRange<float>(0f, 1f)));
            sharpenMinMaxDepthFallOff.SettingChanged += (sender, args) => ApplySettings();
            sharpenClamp = Config.Bind("Sharpening", "Sharpen clamp", 0.97f, new ConfigDescription("Maximum effect applied over a single pixel", new AcceptableValueRange<float>(0f, 1f)));
            sharpenClamp.SettingChanged += (sender, args) => ApplySettings();
            //Color
            ColorTweaks = Config.Bind("Color", "Color tweaks", true, "Toggle color tweaks");
            ColorTweaks.SettingChanged += (sender, args) => ApplySettings();
            Saturation = Config.Bind("Color", "Saturation", 1f, new ConfigDescription("Set saturation", new AcceptableValueRange<float>(-2f, 3f)));
            Saturation.SettingChanged += (sender, args) => ApplySettings();
            ColorTemp = Config.Bind("Color", "Color temperature", 6192f, new ConfigDescription("Set color temperature", new AcceptableValueRange<float>(1000f, 40000f)));
            ColorTemp.SettingChanged += (sender, args) => ApplySettings();
            ColorTempBlend = Config.Bind("Color", "Color temperature blend", 1f, new ConfigDescription("Set color temperature blend", new AcceptableValueRange<float>(0f, 1f)));
            ColorTempBlend.SettingChanged += (sender, args) => ApplySettings();
            Red = Config.Bind("Color", "RGB (R)", 1f, new ConfigDescription("Set red", new AcceptableValueRange<float>(0f, 1f)));
            Red.SettingChanged += (sender, args) => ApplySettings();
            Green = Config.Bind("Color", "RGB (G)", 1f, new ConfigDescription("Set green", new AcceptableValueRange<float>(0f, 1f)));
            Green.SettingChanged += (sender, args) => ApplySettings();
            Blue = Config.Bind("Color", "RGB (B)", 1f, new ConfigDescription("Set blue", new AcceptableValueRange<float>(0f, 1f)));
            Blue.SettingChanged += (sender, args) => ApplySettings();
            //Image adjustments
            Contrast = Config.Bind("Image adjustments", "Contrast", 1f, new ConfigDescription("Set contrast", new AcceptableValueRange<float>(0.5f, 1.5f)));
            Contrast.SettingChanged += (sender, args) => ApplySettings();
            Sepia = Config.Bind("Image adjustments", "Sepia", 0f, new ConfigDescription("Set sepia", new AcceptableValueRange<float>(0f, 2f)));
            Sepia.SettingChanged += (sender, args) => ApplySettings();
            EnableToneMapping = Config.Bind("Image adjustments", "Tone mapping", true, "Toggle tone mapping");
            EnableToneMapping.SettingChanged += (sender, args) => ApplySettings();
            TonemapExposureOffset = Config.Bind("Image adjustments", "Tone mapping exposure offset", 0f, new ConfigDescription("Set tone mapping exposure offset", new AcceptableValueRange<float>(-3f, 6f)));
            TonemapExposureOffset.SettingChanged += (sender, args) => ApplySettings();
            TonemapBrightnessOffset = Config.Bind("Image adjustments", "Tone mapping brightness offset", 0f, new ConfigDescription("Set tone mapping brightness offset", new AcceptableValueRange<float>(-2f, 4f)));
            TonemapBrightnessOffset.SettingChanged += (sender, args) => ApplySettings();
            Brightness = Config.Bind("Image adjustments", "Brightness", 1f, new ConfigDescription("Set brightness", new AcceptableValueRange<float>(0f, 2f)));
            Brightness.SettingChanged += (sender, args) => ApplySettings();
            Daltonize = Config.Bind("Image adjustments", "Daltonize", 0.3f, new ConfigDescription("Set daltonize", new AcceptableValueRange<float>(0f, 2f)));
            Daltonize.SettingChanged += (sender, args) => ApplySettings();
            //Effects
            chromaticAbberation = Config.Bind("Image adjustments", "Chromatic abberation", false, "Toggle chromatic abberation");
            chromaticAbberation.SettingChanged += (sender, args) => ApplySettings();
            chromaticAberrationIntensity = Config.Bind("Image adjustments", "Chromatic abberation intensity", 0.00f, new ConfigDescription("Set chromatic abberation intensity", new AcceptableValueRange<float>(0f, 2f)));
            chromaticAberrationIntensity.SettingChanged += (sender, args) => ApplySettings();
            chromaticAberrationSmoothing = Config.Bind("Image adjustments", "Chromatic abberation smoothing", 32f, new ConfigDescription("Set chromatic abberation smoothing", new AcceptableValueRange<float>(0f, 256f)));
            chromaticAberrationSmoothing.SettingChanged += (sender, args) => ApplySettings();
            depthOfField = Config.Bind("Depth of field", "Depth of field", true, "Toggle depth of field");
            depthOfField.SettingChanged += (sender, args) => ApplySettings();
            depthOfFieldDistance = Config.Bind("Depth of field", "Depth of field distance", 32f, new ConfigDescription("Set depth of field distance", new AcceptableValueRange<float>(0f, 64f)));
            depthOfFieldDistance.SettingChanged += (sender, args) => ApplySettings();
            depthOfFieldFocalLength = Config.Bind("Depth of field", "Depth of field focal length", 0.25f, new ConfigDescription("Set depth of field focal length", new AcceptableValueRange<float>(0.005f, 0.5f)));
            depthOfFieldFocalLength.SettingChanged += (sender, args) => ApplySettings();
            depthOfFieldAperture = Config.Bind("Depth of field", "Depth of field aperture", 1.3f, new ConfigDescription("Set depth of field aperture", new AcceptableValueRange<float>(-0f, 20f)));
            depthOfFieldAperture.SettingChanged += (sender, args) => ApplySettings();
            depthOfFieldForegroundBlur = Config.Bind("Depth of field", "Depth of field foreground blur", false, "Toggle depth of field foreground blur");
            depthOfFieldForegroundBlur.SettingChanged += (sender, args) => ApplySettings();
            depthOfFieldForegroundBlurHQ = Config.Bind("Depth of field", "Depth of field foreground blur HQ", false, "Toggle depth of field foreground blur HQ");
            depthOfFieldForegroundBlurHQ.SettingChanged += (sender, args) => ApplySettings();
            depthOfFieldForegroundBlurHQSpread = Config.Bind("Depth of field", "Depth of field foreground blur HQ spread", 3f, new ConfigDescription("Set depth of field foreground blur HQ spread", new AcceptableValueRange<float>(0f, 32f)));
            depthOfFieldForegroundBlurHQSpread.SettingChanged += (sender, args) => ApplySettings();
            depthOfFieldForegroundDistance = Config.Bind("Depth of field", "Depth of field foreground distance", 3f, new ConfigDescription("Set depth of field foreground distance", new AcceptableValueRange<float>(0f, 10f)));
            depthOfFieldForegroundDistance.SettingChanged += (sender, args) => ApplySettings();
            depthOfFieldBokeh = Config.Bind("Depth of field", "Depth of field bokeh", true, "Toggle depth of field bokeh");
            depthOfFieldBokeh.SettingChanged += (sender, args) => ApplySettings();
            depthOfFieldBokehThreshold = Config.Bind("Depth of field", "Depth of field bokeh threshold", 1.5f, new ConfigDescription("Set depth of field bokeh threshold", new AcceptableValueRange<float>(0f, 3f)));
            depthOfFieldBokehThreshold.SettingChanged += (sender, args) => ApplySettings();
            depthOfFieldBokehIntensity = Config.Bind("Depth of field", "Depth of field bokeh intensity", 1.5f, new ConfigDescription("Set depth of field bokeh intensity", new AcceptableValueRange<float>(0f, 8f)));
            depthOfFieldBokehIntensity.SettingChanged += (sender, args) => ApplySettings();
            blurIntensity = Config.Bind("Image adjustments", "Blur intensity", 0f, new ConfigDescription("Set blur intensity", new AcceptableValueRange<float>(0f, 4f)));
            blurIntensity.SettingChanged += (sender, args) => ApplySettings();
            vignette = Config.Bind("Vignette", "Vignetting", true, "Toggle vignetting");
            vignette.SettingChanged += (sender, args) => ApplySettings();
            vignettingOuterRing = Config.Bind("Vignette", "Vignetting outer ring", 0.3f, new ConfigDescription("Set vignetting outer ring", new AcceptableValueRange<float>(0f, 1f)));
            vignettingOuterRing.SettingChanged += (sender, args) => ApplySettings();
            vignettingInnerRing = Config.Bind("Vignette", "Vignetting inner ring (enable vignette in game settings)", 1f, new ConfigDescription("Set vignetting inner ring", new AcceptableValueRange<float>(0f, 1f)));
            vignettingInnerRing.SettingChanged += (sender, args) => ApplySettings();
            vignettingFade = Config.Bind("Vignette", "Vignetting fade", 0f, new ConfigDescription("Set vignetting fade", new AcceptableValueRange<float>(0f, 1f)));
            vignettingFade.SettingChanged += (sender, args) => ApplySettings();
            vignettingCircularShape = Config.Bind("Vignette", "Vignetting circular shape", false, "Toggle vignetting circular shape");
            vignettingCircularShape.SettingChanged += (sender, args) => ApplySettings();
            vignettingAspectRatio = Config.Bind("Vignette", "Vignetting aspect ratio", 1f, new ConfigDescription("Set vignetting aspect ratio", new AcceptableValueRange<float>(0f, 2f)));
            vignettingAspectRatio.SettingChanged += (sender, args) => ApplySettings();
            //Charamaker specific settings
            enableTonemappingCharamaker = Config.Bind("Charamaker settings", "Enable tonemapping charamaker", true, "Toggle tonemapping for charamaker");
            enableTonemappingCharamaker.SettingChanged += (sender, args) => ApplySettings();
            TonemapBrightnessOffsetCharamaker = Config.Bind("Charamaker settings", "Tone mapping brightness offset charamaker", 0f, new ConfigDescription("Set tone mapping brightness offset for charamaker", new AcceptableValueRange<float>(-2f, 4f)));
            TonemapBrightnessOffsetCharamaker.SettingChanged += (sender, args) => ApplySettings();
            TonemapExposureOffsetCharamaker = Config.Bind("Charamaker settings", "Tone mapping exposure offset charamaker", 0f, new ConfigDescription("Set tone mapping exposure offset for charamaker", new AcceptableValueRange<float>(-3f, 6f)));
            TonemapExposureOffsetCharamaker.SettingChanged += (sender, args) => ApplySettings();
            BloomIntensityCharamaker = Config.Bind("Charamaker settings", "Bloom intensity charamaker", 1f, new ConfigDescription("Set bloom intensity for charamaker", new AcceptableValueRange<float>(0f, 10f)));
            BloomIntensityCharamaker.SettingChanged += (sender, args) => ApplySettings();
            BloomThresholdCharamaker = Config.Bind("Charamaker settings", "Bloom threshold charamaker", 0.98f, new ConfigDescription("Set bloom threshold for charamaker", new AcceptableValueRange<float>(0f, 5f)));
            BloomThresholdCharamaker.SettingChanged += (sender, args) => ApplySettings();
            SaturationCharamaker = Config.Bind("Charamaker settings", "Saturation charamaker", 1.5f, new ConfigDescription("Set saturation for charamaker", new AcceptableValueRange<float>(-2f, 3f)));
            SaturationCharamaker.SettingChanged += (sender, args) => ApplySettings();

            //on new scene
            SceneManager.add_sceneLoaded(new Action<Scene, LoadSceneMode>((s, lsm) =>
            {
                //Check if map/charamaker gameobject has been loaded
                gameObject = s.GetRootGameObjects()[0];
                if (AutoApply.Value && (gameObject.name == "Map") || (gameObject.name == "CustomScene"))
                {
                    //Check if main game or charamaker and get volume + camera
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
                    GetOriginalValueForOffsets();
                    ApplySettings();
                    ApplyUnitySettings();
                }
            }));
        }

        private enum swAntiAliasingMode
        {
            None = 0,
            FXAA = 1,
            SMAA = 2
        }

        private enum swAntiAliasingQuality
        {
            Low = 0,
            Medium = 1,
            High = 2
        }

        private void GetOriginalValueForOffsets()
        {
            originalTonemapExposure = beautifyInstance.tonemapExposurePre.GetValue<float>();
            originalTonemapBrightness = beautifyInstance.tonemapBrightnessPost.GetValue<float>();
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
                beautifyInstance.brightness.Override(Brightness.Value);
                beautifyInstance.bloomMaxBrightness.Override(BloomMaxBrightness.Value);
                beautifyInstance.anamorphicFlaresIntensity.Override(AnamorphicFlaresIntensity.Value);
                beautifyInstance.anamorphicFlaresThreshold.Override(AnamorphicFlaresThreshold.Value);
                beautifyInstance.anamorphicFlaresVertical.Override(AnamorphicFlaresVertical.Value);
                beautifyInstance.anamorphicFlaresSpread.Override(AnamorphicFlaresSpread.Value);
                beautifyInstance.anamorphicFlaresAntiflicker.Override(AnamorphicFlaresAntiflicker.Value);
                beautifyInstance.anamorphicFlaresDepthAtten.Override(AnamorphicFlaresDepthAtten.Value);
                beautifyInstance.anamorphicFlaresNearAtten.Override(AnamorphicFlaresNearAtten.Value);
                beautifyInstance.anamorphicFlaresTint.Override(new Color(AnamorphicFlaresR.Value, AnamorphicFlaresG.Value, AnamorphicFlaresB.Value, AnamorphicFlaresA.Value));
                beautifyInstance.stripBeautifyColorTweaks.Override(!ColorTweaks.Value);
                beautifyInstance.contrast.Override(Contrast.Value);
                beautifyInstance.daltonize.Override(Daltonize.Value);
                beautifyInstance.sepia.Override(Sepia.Value);
                beautifyInstance.tintColor.Override(new Color(Red.Value, Green.Value, Blue.Value, 1f));
                beautifyInstance.colorTemp.Override(ColorTemp.Value);
                beautifyInstance.colorTempBlend.Override(ColorTempBlend.Value);
                beautifyInstance.bloomIntensity.Override(BloomIntensity.Value);
                beautifyInstance.bloomThreshold.Override(BloomThreshold.Value);
                beautifyInstance.saturate.Override(Saturation.Value);
                beautifyInstance.stripBeautifyChromaticAberration.Override(!chromaticAbberation.Value);
                beautifyInstance.stripUnityChromaticAberration.Override(!chromaticAbberation.Value);
                beautifyInstance.chromaticAberrationIntensity.Override(chromaticAberrationIntensity.Value);
                beautifyInstance.chromaticAberrationSmoothing.Override(chromaticAberrationSmoothing.Value);
                beautifyInstance.depthOfField.Override(depthOfField.Value);
                beautifyInstance.depthOfFieldDistance.Override(depthOfFieldDistance.Value);
                beautifyInstance.depthOfFieldFocalLength.Override(depthOfFieldFocalLength.Value);
                beautifyInstance.depthOfFieldAperture.Override(depthOfFieldAperture.Value);
                beautifyInstance.depthOfFieldForegroundBlur.Override(depthOfFieldForegroundBlur.Value);
                beautifyInstance.depthOfFieldForegroundBlurHQ.Override(depthOfFieldForegroundBlurHQ.Value);
                beautifyInstance.depthOfFieldForegroundBlurHQSpread.Override(depthOfFieldForegroundBlurHQSpread.Value);
                beautifyInstance.depthOfFieldBokeh.Override(depthOfFieldBokeh.Value);
                beautifyInstance.depthOfFieldBokehThreshold.Override(depthOfFieldBokehThreshold.Value);
                beautifyInstance.depthOfFieldBokehIntensity.Override(depthOfFieldBokehIntensity.Value);
                beautifyInstance.blurIntensity.Override(blurIntensity.Value);
                beautifyInstance.sharpenDepthThreshold.Override(sharpenDepthThreshold.Value);
                beautifyInstance.sharpenMinMaxDepth.Override(new Vector2(sharpenMinDepth.Value, sharpenMaxDepth.Value));
                beautifyInstance.sharpenMinMaxDepthFallOff.Override(sharpenMinMaxDepthFallOff.Value);
                beautifyInstance.sharpenClamp.Override(sharpenClamp.Value);
                beautifyInstance.bloomDepthAtten.Override(bloomDepthAtten.Value);
                beautifyInstance.bloomNearAtten.Override(bloomNearAtten.Value);
                beautifyInstance.stripBeautifyVignetting.Override(!vignette.Value);
                beautifyInstance.vignettingOuterRing.Override(vignettingOuterRing.Value);
                beautifyInstance.vignettingInnerRing.Override(vignettingInnerRing.Value);
                beautifyInstance.vignettingFade.Override(vignettingFade.Value);
                beautifyInstance.vignettingCircularShape.Override(vignettingCircularShape.Value);
                beautifyInstance.vignettingAspectRatio.Override(vignettingAspectRatio.Value);
                //If main game apply normal settings, else apply charamaker specific settings
                if (gameObject.name == "Map")
                {
                    beautifyInstance.stripBeautifyTonemapping.Override(!EnableToneMapping.Value);
                    beautifyInstance.tonemapExposurePre.Override(Mathf.Clamp(originalTonemapExposure + TonemapExposureOffset.Value, 0.1f, 6f));
                    beautifyInstance.tonemapBrightnessPost.Override(Mathf.Clamp(originalTonemapBrightness + TonemapBrightnessOffset.Value, 0.02f, 5f));
                    beautifyInstance.bloomIntensity.Override(BloomIntensity.Value);
                    beautifyInstance.bloomThreshold.Override(BloomThreshold.Value);
                    beautifyInstance.saturate.Override(Saturation.Value);
                }
                else
                {
                    beautifyInstance.stripBeautifyTonemapping.Override(!enableTonemappingCharamaker.Value);
                    beautifyInstance.tonemapExposurePre.Override(Mathf.Clamp(originalTonemapExposure + TonemapExposureOffsetCharamaker.Value, 0.1f, 6f));
                    beautifyInstance.tonemapBrightnessPost.Override(Mathf.Clamp(originalTonemapBrightness + TonemapBrightnessOffsetCharamaker.Value, 0.02f, 5f));
                    beautifyInstance.bloomIntensity.Override(BloomIntensityCharamaker.Value);
                    beautifyInstance.bloomThreshold.Override(BloomThresholdCharamaker.Value);
                    beautifyInstance.saturate.Override(SaturationCharamaker.Value);
                }
            }
            else
                Log.LogError("Beautify instance not found");
        }

        private void ApplyUnitySettings()
        {
            if (postProcessCamera != null)
            {
                postProcessCamera.allowMSAA = AllowMSAA.Value;
                UniversalRenderPipeline.asset.msaaSampleCount = MSAAQuality.Value;
                UniversalRenderPipeline.asset.renderScale = RenderScale.Value;
                UniversalRenderPipeline.asset.mainLightShadowmapResolution = MainShadowResolution.Value;
                UniversalRenderPipeline.asset.supportsSoftShadows = SoftShadows.Value;
                UniversalRenderPipeline.asset.shadowCascadeCount = ShadowCascadeCount.Value;
                //Apply software antialiasing mode and quality
                postProcessCamera.GetUniversalAdditionalCameraData().antialiasing = (AntialiasingMode)SoftwareAntiAliasing.Value;
                postProcessCamera.GetUniversalAdditionalCameraData().antialiasingQuality = (AntialiasingQuality)SoftwareAntiAliasingQuality.Value;
            }
            else
                Log.LogError("Post processing camera not found");
        }
    }
}
