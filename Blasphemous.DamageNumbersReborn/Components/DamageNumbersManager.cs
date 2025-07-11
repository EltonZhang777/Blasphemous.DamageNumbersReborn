using Framework.Managers;
using Gameplay.GameControllers.Entities;
using System.Collections.Generic;
using UnityEngine;

namespace Blasphemous.DamageNumbersReborn.Components;

internal class DamageNumbersManager : MonoBehaviour
{
    private List<DamageNumberObject> Hits;
    private float labelOffset;

    private Camera GameCamera => Core.Screen.GameCamera;

    private float ScreenWidthScale => (float)Screen.width / 640f;

    private float ScreenHeightScale => (float)Screen.height / 360f;

    private float GuiScale => (this.ScreenHeightScale > this.ScreenWidthScale) ? this.ScreenWidthScale : this.ScreenHeightScale;

    public static DamageNumbersManager instance { get; private set; }

    private void Awake()
    {
        DamageNumbersManager.instance = this;
        this.Hits = new List<DamageNumberObject>(40);
    }

    public void AddHit(Hit hit, Entity entity)
    {
        float postMitigationDamage = Mathf.Max(entity.GetReducedDamage(hit) - entity.Stats.Defense.Final, 0f);
        Vector3 position = entity.transform.position;
        float num = UnityEngine.Random.Range(-1f, 1f);
        DamageNumberObject item = new()
        {
            hit = hit,
            postMitigationDamage = postMitigationDamage,
            originalPosition = new Vector2(position.x + num, position.y)
        };
        this.Hits.Add(item);
    }

    private void OnGUI()
    {
        if (Event.current.type != EventType.Repaint || this.Hits.Count == 0)
        {
            return;
        }

        // initialize font
        int fontSize = (int)(16f * this.GuiScale);
        float rectSize = (float)fontSize * 2f;
        GUI.skin.label.fontStyle = FontStyle.Bold;
        GUI.skin.label.fontSize = fontSize;

        // process damage numbers
        for (int i = this.Hits.Count - 1; i >= 0; i--)
        {
            DamageNumberObject DamageNumberObject = this.Hits[i];
            if (DamageNumberObject.timePassed > 1.2f)
            {
                this.Hits.RemoveAt(i);
            }
            else
            {
                // calculate screen position of the damage number
                DamageNumberObject.screenY += 0.2f;
                Vector3 screenPosition = this.GameCamera.WorldToScreenPoint(new Vector3(DamageNumberObject.originalPosition.x, DamageNumberObject.originalPosition.y, 0f));
                float xPosition = screenPosition.x * this.ScreenWidthScale;
                float yPosition = (float)Screen.height - screenPosition.y * this.ScreenHeightScale - DamageNumberObject.screenY;
                yPosition += this.labelOffset;

                // calculate current alpha with ease-in quintic curve
                double currentCurveValue = 1.0 - (double)(1f / (1.2f / (1.2f - DamageNumberObject.timePassed)));
                float currentAlpha = this.EaseInQuint(1f, 0f, (float)currentCurveValue);

                // display damage number
                GUI.color = new Color(0f, 0f, 0f, currentAlpha);
                GUI.Label(new Rect(xPosition + 1f, yPosition + 1f, rectSize, rectSize), DamageNumberObject.postMitigationDamage.ToString());

                // display a black shadow of the damage number to its bottom right
                GUI.color = new Color(1f, 1f, 1f, currentAlpha);
                GUI.Label(new Rect(xPosition, yPosition, rectSize, rectSize), DamageNumberObject.postMitigationDamage.ToString());

                DamageNumberObject.timePassed += Time.deltaTime;
            }
        }
    }

    private void Update()
    {
        // Toggle label offset with keys (down, up, and 0)
        if (Input.GetKeyDown(KeyCode.Equals))
        {
            this.labelOffset += 20f;
        }
        if (Input.GetKeyDown(KeyCode.Minus))
        {
            this.labelOffset -= 20f;
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            this.labelOffset = 0f;
        }
    }

    private float EaseInQuint(float start, float end, float value)
    {
        end -= start;
        return end * value * value * value * value * value + start;
    }
}
