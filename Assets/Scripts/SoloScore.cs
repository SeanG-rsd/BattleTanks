using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SoloScore : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    private int score;

    private int position;
    [SerializeField] private Image image;
    [SerializeField] private Sprite[] rankSprites;

    [SerializeField] private Image backGroundImage;

    public void SetScore(int s)
    {
        score = s;
        scoreText.text = score.ToString();
    }

    public void SetImage(int position, Color color)
    {
        this.position = position;
        image.sprite = rankSprites[position];
        backGroundImage.color = color;
    }
}
