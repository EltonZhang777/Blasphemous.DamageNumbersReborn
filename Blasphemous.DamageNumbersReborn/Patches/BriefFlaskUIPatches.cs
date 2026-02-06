using Framework.FrameworkCore.Attributes;
using Framework.Inventory;
using Framework.Managers;
using Gameplay.UI.Others.UIGameLogic;
using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Blasphemous.DamageNumbersReborn.Patches;

[HarmonyPatch(typeof(PlayerFlask))]
class PlayerFlask_ShowBriefFlaskUI_Patches
{
    [HarmonyPatch("RefreshFlask")]
    [HarmonyPrefix]
    public static bool ShowBriefFlaskUI(
        PlayerFlask __instance,
        Sword ___swordHeart06,
        bool ___currentFlaskIsFervour,
        List<Image> ___flasks,
        float ___currentFlaskNumber,
        float ___currentFlaskFull,
        float ___currentFlaskLevel)
    {
        if (!Main.DamageNumbersReborn.config.briefFlasksDisplay)
            return true;

        if (Core.Logic?.Penitent == null)
            return false;

        Flask flask = Core.Logic.Penitent.Stats.Flask;
        int flaskLevel = (int)(Core.Logic.Penitent.Stats.FlaskHealth.PermanetBonus / Core.Logic.Penitent.Stats.FlaskHealthUpgrade);
        flaskLevel = Mathf.Min(flaskLevel, __instance.flasksEmpty.Count - 1);

        ___swordHeart06 ??= Core.InventoryManager.GetSword("HE06");
        if (___swordHeart06?.IsEquiped ?? false)
        {
            flask.Current = 0f;
        }

        if (___currentFlaskNumber == flask.Final
            && ___currentFlaskFull == flask.Current
            && ___currentFlaskLevel == (float)flaskLevel
            && ___flasks[0].gameObject.activeInHierarchy
            && ___currentFlaskIsFervour == Core.PenitenceManager.UseFervourFlasks)
        {
            return false;
        }

        ___currentFlaskIsFervour = Core.PenitenceManager.UseFervourFlasks;
        ___currentFlaskNumber = flask.Final;
        ___currentFlaskFull = flask.Current;
        ___currentFlaskLevel = (float)flaskLevel;

        // in brief display mode for flasks, only show 1 flask regardless of flask count
        // display first flask
        if (___currentFlaskFull == 0)
        {
            ___flasks[0].sprite = __instance.flasksEmpty[flaskLevel];
        }
        else if (Core.PenitenceManager.UseFervourFlasks)
        {
            ___flasks[0].sprite = __instance.flasksFullFervour[flaskLevel];
        }
        else
        {
            ___flasks[0].sprite = __instance.flasksFull[flaskLevel];
        }
        ___flasks[0].gameObject.SetActive(true);
        // disable remaining flasks
        for (int j = 1; j < ___flasks.Count; j++)
        {
            ___flasks[j].gameObject.SetActive(false);
        }
        return false;
    }
}