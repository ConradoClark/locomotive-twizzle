using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

public class MapHealth : MonoBehaviour
{
    public int Health;
    private int currentHealth;

    public Vector2 HealthBarOffset;

    public GameObject HealthBarPrefab;
    public GameObject HealthTickPrefab;
    public GameObject HealthTickSplitterPrefab;

    public Color HealthyColor;
    public Color WarningColor;
    public Color DangerColor;

    [Range(1, 100)]
    public int WarningPercentage;

    [Range(1, 100)]
    public int DangerPercentage;

    private GameObject HealthBar;
    private List<SpriteRenderer> HealthTicks;
    private List<SpriteRenderer> HealthTickSplitters;

    public bool IsAlive
    {
        get
        {
            return this.Health > 0 && currentHealth > 0;
        }
    }

    void Start()
    {
        this.HealthTicks = new List<SpriteRenderer>();
        this.HealthTickSplitters = new List<SpriteRenderer>();
        this.currentHealth = this.Health;
        if (Health > 0 && HealthBarPrefab != null && HealthTickPrefab != null && HealthTickSplitterPrefab != null)
        {
            this.HealthBar = Instantiate(HealthBarPrefab);
            RectTransform rectTransform = this.HealthBar.GetComponent<RectTransform>();

            this.HealthBar.transform.SetParent(this.transform, false);
            this.HealthBar.transform.localPosition += new Vector3(HealthBarOffset.x, HealthBarOffset.y);

            float tickSize = (rectTransform.rect.width) / this.Health;

            for (int i = 0; i < this.Health; i++)
            {
                var tick = Instantiate(HealthTickPrefab);
                var tickRect = tick.GetComponent<RectTransform>();
                tick.transform.SetParent(this.HealthBar.transform, false);
                tick.transform.localScale = new Vector3(tickSize / tickRect.rect.width, tick.transform.localScale.y, tick.transform.localScale.z);
                tick.transform.localPosition += new Vector3(i * tickSize, 0, 0);

                var sprRenderer = tick.GetComponent<SpriteRenderer>();
                sprRenderer.color = this.HealthyColor;

                this.HealthTicks.Add(sprRenderer);

                if (i < this.Health - 1)
                {
                    var tickSplitter = Instantiate(HealthTickSplitterPrefab);
                    tickSplitter.transform.SetParent(this.HealthBar.transform, false);
                    tickSplitter.transform.localPosition += new Vector3((i + 1) * tickSize, 0, 0);
                    this.HealthTickSplitters.Add(tickSplitter.GetComponent<SpriteRenderer>());
                }
            }
        }
    }

    public int GetCurrentHealth()
    {
        return this.currentHealth;
    }

    public void Damage(uint amount)
    {
        this.currentHealth = (int)Math.Max(this.currentHealth - amount, 0);
        if (this.HealthTicks.Count > 0)
        {
            foreach (var tick in this.HealthTicks.Skip(this.currentHealth))
            {
                tick.enabled = false;
            }
        }
        this.AdjustHealthColorByPercentage();
    }

    public void Heal(uint amount)
    {
        this.currentHealth += Math.Max((int)amount, this.Health - this.currentHealth);
        if (this.HealthTicks.Count > 0)
        {
            foreach (var tick in this.HealthTicks.Skip(this.currentHealth))
            {
                tick.enabled = false;
            }
        }
        this.AdjustHealthColorByPercentage();
    }

    private void AdjustHealthColorByPercentage()
    {
        Color color;
        if (currentHealth < this.Health * (float) this.DangerPercentage / 100)
        {
            color = this.DangerColor;
        }
        else if (currentHealth < this.Health * (float) this.WarningPercentage / 100)
        {
            color = this.WarningColor;
        }
        else
        {
            color = this.HealthyColor;
        }

        foreach (var tick in this.HealthTicks)
        {
            tick.color = color;
        }
    }
}
