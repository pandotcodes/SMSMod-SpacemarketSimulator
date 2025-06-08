using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using MyBox;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.SceneManagement;

namespace SpacemarketSimulator
{
    [BepInPlugin(PluginInfo.PLUGIN_GUID, PluginInfo.PLUGIN_NAME, PluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static Vector3 NPCArrivalPoint = new Vector3(6, -0.06f, 3);
        public static ConfigEntry<string> SpaceShader;
        public static ConfigEntry<bool> MoveSkyboxWithPlayer;
        public static ManualLogSource SLogger;
        private void Awake()
        {
            SLogger = Logger;
            Logger.LogInfo($"Plugin {PluginInfo.PLUGIN_GUID} is loaded! Applying patch...");
            Harmony harmony = new Harmony(PluginInfo.PLUGIN_GUID);
            harmony.PatchAll();

            foreach (var shader in Resources.FindObjectsOfTypeAll<Shader>())
            {
                Logger.LogInfo("Available shader: " + shader.name);
            }

            SpaceShader = Config.Bind("", "Space Shader", "Universal Render Pipeline/Particles/Unlit");
            MoveSkyboxWithPlayer = Config.Bind("", "Move Skybox with Player", true);

            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
        }
        private Dictionary<string, Texture2D> TextureCache { get; set; } = new();
        public static GameObject Player { get; set; }
        private Texture2D LoadTexture(string name)
        {
            if (TextureCache.ContainsKey(name)) return TextureCache[name];
            byte[] data = (byte[])Properties.Resources.ResourceManager.GetObject(name);
            Texture2D tex = new Texture2D(1024, 1024);
            tex.LoadImage(data);
            TextureCache[name] = tex;
            return tex;
        }
        private void ApplySkybox(Transform container)
        {
            float skyboxDistance = 500;
            float skyboxScale = 100;
            PrimitiveType type = PrimitiveType.Plane;

            var xmPlane = GameObject.CreatePrimitive(type);
            xmPlane.name = "xmPlane";
            var xpPlane = GameObject.CreatePrimitive(type);
            xpPlane.name = "xpPlane";

            var ymPlane = GameObject.CreatePrimitive(type);
            ymPlane.name = "ymPlane";
            var ypPlane = GameObject.CreatePrimitive(type);
            ypPlane.name = "ypPlane";

            var zmPlane = GameObject.CreatePrimitive(type);
            zmPlane.name = "zmPlane";
            var zpPlane = GameObject.CreatePrimitive(type);
            zpPlane.name = "zpPlane";

            var list = new List<(GameObject plane, string texture)> { (xpPlane, "front_galaxyfire1"), (xmPlane, "back_galaxyfire1"), (ypPlane, "down_galaxyfire1"), (ymPlane, "up_galaxyfire1"), (zpPlane, "left_galaxyfire1"), (zmPlane, "right_galaxyfire1") };
            list.ForEach(x =>
            {
                x.plane.transform.SetParent(container);
                var rend = x.plane.GetComponent<MeshRenderer>();
                rend.material.shader = Shader.Find(Plugin.SpaceShader.Value);
                rend.allowOcclusionWhenDynamic = false;
                rend.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
                rend.receiveShadows = false;
                SpaceShader.SettingChanged += (a, b) => { try { rend.material.shader = Shader.Find(SpaceShader.Value); } catch (Exception e) { Logger.LogError(e); } };
                rend.material.mainTexture = LoadTexture(x.texture);
            });

            var ymtd = TransformDefiner.AddToGameObject(ymPlane);
            ymtd.localScale = new Vector3(skyboxScale, 1, skyboxScale);
            ymtd.eulerAngles = new Vector3(0, 90, 0);
            ymtd.position = new Vector3(0, -skyboxDistance + 0.001f, 0);

            var yptd = TransformDefiner.AddToGameObject(ypPlane);
            yptd.position = new Vector3(0, skyboxDistance - 0.001f, 0);
            yptd.eulerAngles = new Vector3(180, -90, 0);
            yptd.localScale = new Vector3(-skyboxScale, 1, -skyboxScale);

            var xmtd = TransformDefiner.AddToGameObject(xmPlane);
            xmtd.position = new Vector3(-skyboxDistance + 0.001f, 0, 0);
            xmtd.eulerAngles = new Vector3(90, 0, 90);
            xmtd.localScale = new Vector3(skyboxScale, -1, -skyboxScale);

            var xptd = TransformDefiner.AddToGameObject(xpPlane);
            xptd.localScale = new Vector3(skyboxScale, 1, skyboxScale);
            xptd.position = new Vector3(skyboxDistance - 0.001f, 0, 0);
            xptd.eulerAngles = new Vector3(-90, 0, 90);

            var zmtd = TransformDefiner.AddToGameObject(zmPlane);
            zmtd.position = new Vector3(0, 0, -skyboxDistance + 0.001f);
            zmtd.eulerAngles = new Vector3(90, 180, 180);
            zmtd.localScale = new Vector3(-skyboxScale, 1, -skyboxScale);

            var zptd = TransformDefiner.AddToGameObject(zpPlane);
            zptd.localScale = new Vector3(skyboxScale, 1, skyboxScale);
            zptd.position = new Vector3(0, 0, skyboxDistance - 0.001f);
            zptd.eulerAngles = new Vector3(-90, 0, 0);
        }
        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Backspace))
            {
                Player.transform.position = new Vector3(4.52f, -0.06f, 4.85f);
            }
        }
        public static GameObject DisableStandardEnvironment()
        {
            string[] piecesToLeaveEnabled = new string[]
            {
                "Sidewalk_4x4_A_01 (1)",
                "Sidewalk_4x4_A_01 (3)",
                "Sidewalk_4x2_A_02 (3)",
                "Sidewalk_4x2_A_01 (2)",
                "Sidewalk_4x4_A_02 (3)",
                "Sidewalk_4x4_A_02 (1)",
                "Descent_Store (10)",
                "Street",
                "Showcase_Store_08",
                "Wall_2m_01",
                "Wall_4m_01",
                "NI_Wall_4m_01",
                "Store Status Sign",
                "Table Canvas",
                "Placement Obstacle",
            };
            var standardEnvironmentContainer = GameObject.Find("Environment v0.1.1");
            GameObject street = null;
            if (standardEnvironmentContainer == null) return null;
            for (int i = 0; i < standardEnvironmentContainer.transform.childCount; i++)
            {
                var child = standardEnvironmentContainer.transform.GetChild(i).gameObject;
                if (!piecesToLeaveEnabled.Contains(child.name)) child.SetActive(false);
                if (child.name == "Street") street = child;
            }
            for (int i = 0; i < street.transform.childCount; i++)
            {
                var child = street.transform.GetChild(i).gameObject;
                if (!piecesToLeaveEnabled.Contains(child.name)) child.SetActive(false);
            }
            for (int i = 2; i < 24; i++)
            {
                SLogger.LogInfo("Disabling Section " + i);
                var section = GameObject.Find("Section " + i);
                var tbd = section.transform.GetChild(0);
                for (int j = 0; j < tbd.childCount; j++)
                {
                    var child = tbd.GetChild(j).gameObject;
                    SLogger.LogInfo("Disabling " + child.name);
                    string name = child.name.Split(" (")[0];
                    if (!piecesToLeaveEnabled.Contains(name)) child.SetActive(false);
                }
            }

            var streetLightsContainer = GameObject.Find("Street Lights");
            streetLightsContainer.SetActive(false);

            return standardEnvironmentContainer;
        }
        private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            var standardEnvironmentContainer = DisableStandardEnvironment();
            if (standardEnvironmentContainer == null) return;

            Player = GameObject.Find("Player");

            var newContainer = new GameObject("Space Environment").transform;
            newContainer.SetParent(standardEnvironmentContainer.transform.parent);

            var skyboxContainer = new GameObject("Skybox").transform;
            skyboxContainer.SetParent(newContainer);

            //GameObject platform = GameObject.CreatePrimitive(PrimitiveType.Plane);
            //platform.transform.localScale = new Vector3(1, 1, 1);
            //platform.transform.position = new Vector3(6, 0, 6);
            //platform.transform.SetParent(newContainer);
            //var camera = GameObject.Find("Main Camera");
            //(camera.GetComponent<CinemachineBrain>().ActiveVirtualCamera as CinemachineVirtualCamera).;

            //var renderer = platform.GetComponent<MeshRenderer>();
            //renderer.material.shader = Shader.Find("Universal Render Pipeline/Lit");
            //renderer.material.mainTexture = LoadTexture("metalgrid3_basecolor");
            //renderer.material.SetTexture("_NORMALMAP", LoadTexture("metalgrid3_normal-ogl"));
            //renderer.material.SetTexture("_METALLICGLOSSMAP", LoadTexture("metalgrid3_metallic"));
            //renderer.material.DisableKeyword("_SPECULAR_SETUP");

            ApplySkybox(skyboxContainer);

            //RenderSettings.skybox.SetTexture("_BackTex", LoadTexture("back_galaxyfire"));
            //RenderSettings.skybox.SetTexture("_DownTex", LoadTexture("down_galaxyfire"));
            //RenderSettings.skybox.SetTexture("_FrontTex", LoadTexture("front_galaxyfire"));
            //RenderSettings.skybox.SetTexture("_LeftTex", LoadTexture("left_galaxyfire"));
            //RenderSettings.skybox.SetTexture("_RightTex", LoadTexture("right_galaxyfire"));
            //RenderSettings.skybox.SetTexture("_UpTex", LoadTexture("up_galaxyfire"));
        }
    }
}
