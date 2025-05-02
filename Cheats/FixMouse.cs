using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using UnityEngine;
using Rewired;

namespace UnbeatableSongHack.Cheats
{
    public class FixMouse
    {
        [HarmonyPatch(typeof(JeffBezosController), "Update")]
        [HarmonyPostfix]
        public static void UnhideCursorUpdate()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

        static void OverrideButton(string actionName, ref bool __result, ref bool __runOriginal)
        {
            switch (actionName)
            {
                case "Interact" when Input.GetMouseButtonDown(0):
                    __runOriginal = false;
                    __result = false;
                    break;
                case "Vertical" when Input.mouseScrollDelta.magnitude > 0:
                case "Horizontal" when Input.mouseScrollDelta.magnitude > 0:
                    __runOriginal = false;
                    __result = false;
                    break;
            }
        }

        [HarmonyPatch(typeof(Player), "GetButtonDown", typeof(string))]
        [HarmonyPrefix]
        public static void OverrideGetButtonDown(string actionName, ref bool __result, ref bool __runOriginal)
        {
            OverrideButton(actionName, ref __result, ref __runOriginal);
        }
        [HarmonyPatch(typeof(Player), "GetNegativeButtonDown", typeof(string))]
        [HarmonyPrefix]
        public static void OverrideGetNegativeButtonDown(string actionName, ref bool __result, ref bool __runOriginal)
        {
            OverrideButton(actionName, ref __result, ref __runOriginal);
        }

    }
}
