using Blasphemous.DamageNumbersReborn.Components;
using Blasphemous.DamageNumbersReborn.Extensions;
using Framework.Inventory;
using Framework.Managers;
using Gameplay.GameControllers.Penitent;
using Gameplay.UI.Others.UIGameLogic;
using HarmonyLib;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Blasphemous.DamageNumbersReborn.Patches;

[HarmonyPatch(typeof(PlayerHealth))]
class PlayerHealth_FixBarLength_Patches
{
    [HarmonyPatch("CalculateHealthBarSize")]
    [HarmonyPrefix]
    public static bool FixBarLength(
        PlayerHealth __instance,
        float ___lastBarWidth,
        float ___backgroundStartSize,
        float ___endFillSize,
        RectTransform ___backgroundMid,
        RectTransform ___lossTransform,
        RectTransform ___healthTransform,
        RectTransform ___backgroundFillTransform)
    {
        if (!Main.DamageNumbersReborn.config.fixPenitentHealthBarLength)
            return true;

        Penitent penitent = Core.Logic.Penitent;
        if (penitent == null)
            return false;

        // use fixed health for length calculation
        float final = Main.DamageNumbersReborn.config.penitentHealthBarFixedLengthHealth;
        if (final != ___lastBarWidth)
        {
            ___lastBarWidth = final;
            TraverseUtils.SetValue(ref __instance, "lastBarWidth", ___lastBarWidth);
            float num = final - ___backgroundStartSize - ___endFillSize;
            num = ((num <= 0f) ? 0f : num);
            ___backgroundMid.sizeDelta = new Vector2(num, ___backgroundMid.sizeDelta.y);
            ___lossTransform.sizeDelta = new Vector2(final, ___lossTransform.sizeDelta.y);
            ___healthTransform.sizeDelta = new Vector2(final, ___healthTransform.sizeDelta.y);
            ___backgroundFillTransform.sizeDelta = new Vector2(final, ___healthTransform.sizeDelta.y);
        }

        return false;
    }
}

[HarmonyPatch(typeof(PlayerFervour))]
class PlayerFervour_FixBarLength_Patches
{
    [HarmonyPatch("CalculateBarSize")]
    [HarmonyPrefix]
    public static bool FixBarLength(
        PlayerFervour __instance,
        float ___lastBarWidth,
        float ___backgroundStartSize,
        float ___endFillSize,
        RectTransform ___backgroundMid,
        RectTransform ___fillExactTransform,
        RectTransform ___fillExactFullTransform,
        RectTransform ___fillAnimableTransform,
        RectTransform ___background,
        RectTransform ___fillNotEnoughTransform)
    {
        if (!Main.DamageNumbersReborn.config.fixPenitentFervourBarLength)
            return true;

        // use fixed health for length calculation
        float currentMaxWithoutFactor = Main.DamageNumbersReborn.config.penitentFervourBarFixedLengthFervour;
        if (currentMaxWithoutFactor != ___lastBarWidth)
        {
            ___lastBarWidth = currentMaxWithoutFactor;
            TraverseUtils.SetValue(ref __instance, "lastBarWidth", ___lastBarWidth);

            float num = currentMaxWithoutFactor - ___backgroundStartSize - ___endFillSize;
            num = ((num <= 0f) ? 0f : num);
            ___backgroundMid.sizeDelta = new Vector2(num, ___backgroundMid.sizeDelta.y);
            ___fillExactTransform.sizeDelta = new Vector2(currentMaxWithoutFactor, ___fillExactTransform.sizeDelta.y);
            ___fillExactFullTransform.sizeDelta = new Vector2(currentMaxWithoutFactor, ___fillExactFullTransform.sizeDelta.y);
            ___fillAnimableTransform.sizeDelta = new Vector2(currentMaxWithoutFactor, ___fillAnimableTransform.sizeDelta.y);
            ___background.sizeDelta = new Vector2(currentMaxWithoutFactor, ___background.sizeDelta.y);
            ___fillNotEnoughTransform.sizeDelta = new Vector2(currentMaxWithoutFactor, ___fillNotEnoughTransform.sizeDelta.y);
        }
        return false;
    }

    [HarmonyPatch("CalculateMarks")]
    [HarmonyPrefix]
    public static bool CalculateMarksOnFixedLengthBar(
        PlayerFervour __instance,
        Image ___fillExactFull,
        float ___epsilonToShowLastBar,
        int ___currentMarks,
        int ___currentMarksSeparation,
        float ___currentSegmentsFilled,
        float ___currentAnimElapsed,
        float ___currentAnimPosition,
        List<RectTransform> ___anims,
        int ___barAnimEndPosition,
        Transform ___marksParent,
        string ___barAnimChildName,
        string ___barMaskChildName,
        string ___barBarChildName,
        float ___barAnimUpdatedElapsed,
        float ___barAnimMovementPerElapsed)
    {
        if (!Main.DamageNumbersReborn.config.fixPenitentFervourBarLength)
            return true;

        // calculate fervour bar scale factor based on fixed-length bar
        float fixedBarLength = Main.DamageNumbersReborn.config.penitentFervourBarFixedLengthFervour;
        float barLengthScaleFactor = fixedBarLength / Core.Logic.Penitent.Stats.Fervour.CurrentMaxWithoutFactor;

        int totalSegments = 0;
        int fervourfilledSegments = 0;

        // determine prayer cost
        Prayer prayerInSlot = Core.InventoryManager.GetPrayerInSlot(0);
        int prayerCost = (!prayerInSlot) ? 0 : (prayerInSlot.fervourNeeded + (int)Core.Logic.Penitent.Stats.PrayerCostAddition.Final);

        // calculate light-blue color length
        if (prayerCost > 0)
        {
            // calculate segments count
            totalSegments = Mathf.FloorToInt(Core.Logic.Penitent.Stats.Fervour.CurrentMax / prayerCost);
            fervourfilledSegments = Mathf.FloorToInt(Core.Logic.Penitent.Stats.Fervour.Current / prayerCost);

            ___fillExactFull.fillAmount = (float)(fervourfilledSegments * prayerCost) / Core.Logic.Penitent.Stats.Fervour.CurrentMaxWithoutFactor;
        }
        else
        {
            ___fillExactFull.fillAmount = 0f;
        }

        bool showPartialLastSegment = Core.Logic.Penitent.Stats.Fervour.CurrentMax - (float)(prayerCost * totalSegments) > ___epsilonToShowLastBar;
        bool shouldResetAnimationThisFrame = false;
        float animationResetPosition = ((-(float)prayerCost) + 1f) * barLengthScaleFactor;

        // rebuild UI
        if ((totalSegments != ___currentMarks)
            || (prayerCost != ___currentMarksSeparation)
            || (!Mathf.Approximately(fervourfilledSegments, ___currentSegmentsFilled)))
        {
            if (totalSegments == 0)
            {
                ___currentAnimPosition = animationResetPosition;
                TraverseUtils.SetValue(ref __instance, "currentAnimPosition", ___currentAnimPosition);
                ___currentAnimElapsed = 0f;
                TraverseUtils.SetValue(ref __instance, "currentAnimElapsed", ___currentAnimElapsed);
                shouldResetAnimationThisFrame = true;
            }
            ___anims.Clear();

            // reset animation position
            if (___currentAnimPosition > (float)___barAnimEndPosition * barLengthScaleFactor)
            {
                ___currentAnimPosition = animationResetPosition;
                TraverseUtils.SetValue(ref __instance, "currentAnimPosition", ___currentAnimPosition);
            }

            // process segment markings
            // calculate marks' position based on scaled length
            ___currentMarks = totalSegments;
            TraverseUtils.SetValue(ref __instance, "currentMarks", ___currentMarks);
            ___currentMarksSeparation = prayerCost;
            TraverseUtils.SetValue(ref __instance, "currentMarksSeparation", ___currentMarksSeparation);
            ___currentSegmentsFilled = (float)fervourfilledSegments;
            TraverseUtils.SetValue(ref __instance, "currentSegmentsFilled", ___currentSegmentsFilled);
            float currentXOffset = 0f;
            for (int i = 0; i < ___marksParent.childCount; i++)
            {
                RectTransform markRectTransform = (RectTransform)___marksParent.GetChild(i);
                bool segmentShouldBeVisible = i < ___currentMarks;
                markRectTransform.gameObject.SetActive(segmentShouldBeVisible);
                if (segmentShouldBeVisible)
                {
                    markRectTransform.sizeDelta = new Vector2((float)prayerCost * barLengthScaleFactor, markRectTransform.sizeDelta.y);
                    markRectTransform.localPosition = new Vector3(currentXOffset, 0f, 0f);
                    currentXOffset += (float)___currentMarksSeparation * barLengthScaleFactor;
                    RectTransform markMaskRectTransform = (RectTransform)markRectTransform.Find(___barMaskChildName);
                    markMaskRectTransform.sizeDelta = new Vector2((float)(prayerCost - 1f) * barLengthScaleFactor, markMaskRectTransform.sizeDelta.y);
                    RectTransform markBarRectTransform = (RectTransform)markRectTransform.Find(___barBarChildName);
                    markBarRectTransform.gameObject.SetActive(showPartialLastSegment || i != ___currentMarks - 1);
                    bool isSegmentFilled = (float)i < ___currentSegmentsFilled;
                    RectTransform markAnimRectTransform = (RectTransform)markMaskRectTransform.Find(___barAnimChildName);
                    markAnimRectTransform.gameObject.SetActive(isSegmentFilled);
                    if (isSegmentFilled)
                    {
                        markAnimRectTransform.localPosition = new Vector3(___currentAnimPosition, markAnimRectTransform.localPosition.y);
                        ___anims.Add(markAnimRectTransform);
                    }
                }
            }
        }

        // check if shining animation on light blue bars should be reset
        if (!shouldResetAnimationThisFrame && totalSegments > 0)
        {
            ___currentAnimElapsed += Time.deltaTime;
            TraverseUtils.SetValue(ref __instance, "currentAnimElapsed", ___currentAnimElapsed);
            if (___currentAnimElapsed >= ___barAnimUpdatedElapsed)
            {
                ___currentAnimElapsed = 0f;
                TraverseUtils.SetValue(ref __instance, "currentAnimElapsed", ___currentAnimElapsed);
                ___currentAnimPosition += ___barAnimMovementPerElapsed * barLengthScaleFactor;
                TraverseUtils.SetValue(ref __instance, "currentAnimPosition", ___currentAnimPosition);
                if (___currentAnimPosition > (float)___barAnimEndPosition * barLengthScaleFactor)
                {
                    ___currentAnimPosition = animationResetPosition;
                    TraverseUtils.SetValue(ref __instance, "currentAnimPosition", ___currentAnimPosition);
                }
                ___anims.ForEach(delegate (RectTransform anim)
                {
                    anim.localPosition = new Vector3(___currentAnimPosition, anim.localPosition.y);
                });
            }
        }
        return false;
    }

    [HarmonyPatch("ShowSpark")]
    [HarmonyPrefix]
    public static bool ShowSparkOnScaledLocation(
        PlayerFervour __instance,
        Image ___fillExact,
        GameObject ___fervourSpark)
    {
        if (!Main.DamageNumbersReborn.config.fixPenitentFervourBarLength)
            return true;

        __instance.StartCoroutineSafe(ShowSparkCoroutine());
        return false;

        IEnumerator ShowSparkCoroutine()
        {
            yield return null;
            ___fervourSpark.SetActive(true);
            float securityTimeLeft = 2f;
            float barTarget = TraverseUtils.GetValue<float>(__instance, "BarTarget", TraverseUtils.TraverseAccessType.Property);
            while (!Mathf.Approximately(___fillExact.fillAmount, barTarget) && securityTimeLeft > 0f)
            {
                securityTimeLeft -= Time.deltaTime;

                // use scaled position
                float fixedBarLength = Main.DamageNumbersReborn.config.penitentFervourBarFixedLengthFervour;
                float barLengthScaleFactor = fixedBarLength / Core.Logic.Penitent.Stats.Fervour.CurrentMaxWithoutFactor;
                float posx = (float)((int)Core.Logic.Penitent.Stats.Fervour.CurrentMaxWithoutFactor) * ___fillExact.fillAmount - 1f;
                posx *= barLengthScaleFactor;
                ___fervourSpark.transform.localPosition = new Vector3(posx, ___fervourSpark.transform.localPosition.y);
                yield return null;
            }
            ___fervourSpark.SetActive(false);
            yield break;
        }
    }
}