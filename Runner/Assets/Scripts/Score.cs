using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    [SerializeField] private Transform player;
    [SerializeField] public Text scoreText;
    private int totalscore;

    public int scoreMultiplier;
    private void FixedUpdate()
    {
        totalscore += scoreMultiplier;
        scoreText.text = totalscore.ToString();
    }
}
