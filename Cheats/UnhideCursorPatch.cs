using System;
using System.Collections.Generic;
using System.Text;
using HarmonyLib;
using UnityEngine;

namespace UnbeatableSongHack.Cheats
{
    public class UnhideCursorPatch
    {
        [HarmonyPatch(typeof(JeffBezosController), "Update")]
        [HarmonyPostfix]
        public static void UnhideCursorUpdate()
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }

    }
}
