using UnityEngine;
using System.Collections;
using System;

public class ScoreManager : MonoBehaviour
{
    private int Score;
    private GameManager gameManager;
    private TextComponent component;

    void Start()
    {
        this.gameManager = GameManager.GetGameManager();
        this.component = this.gameManager.ScorePartObject.transform.FindChild(Constants.GameObjects.ScoreTextComponent).GetComponent<TextComponent>();
    }

    public void IncreaseScore(uint amount)
    {
        this.Score += (int)amount;
        this.component.Text = this.Score.ToString().PadLeft(7, '0');
    }

    public void ResetScore()
    {
        this.Score = 0;
        this.component.Text = this.Score.ToString().PadLeft(7, '0');
    }
}