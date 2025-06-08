using HarmonyLib;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace SpacemarketSimulator.Patches
{
    [HarmonyPatch(typeof(MainMenuManager), "Awake")]
    public static class MainMenuManager_Awake_Patch
    {
        public static void Postfix(MainMenuManager __instance)
        {
            var bg = new Texture2D(1920, 1080);
            bg.LoadImage(Properties.Resources.bg);
            var image = __instance.gameObject.transform.GetChild(0).gameObject.GetComponent<Image>();
            Console.WriteLine(image == null);
            image.sprite = Sprite.Create(bg, new Rect(0, 0, bg.width, bg.height), new Vector2(0.5f, 0.5f)); ;
            __instance.gameObject.transform.GetChild(1).gameObject.SetActive(false);
        }
    }
}
