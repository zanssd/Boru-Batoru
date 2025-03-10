﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class UIManager : MonoBehaviour
{
    [SerializeField]
    private TMP_Text timerText;
    [SerializeField]
    private TMP_Text countdownText;
    [SerializeField]
    private GameObject countdownObj;
    public TMP_Text resultMatchText;
    public TMP_Text resultEnemyScoreText;
    public TMP_Text resultPlayerScoreText;
    public GameObject resultObj;
    public GameObject nextMatchBtnObj;
    public GameObject endGameTxt;


    [Header("Enemy UI Ref")]
    public TMP_Text enemyTitle;
    public TMP_Text enemyScore;

    [Header("Enemy UI Ref")]
    public TMP_Text playerTitle;
    public TMP_Text playerScore;

    [Header("Penalty Reference")]
    public GameObject penaltyBtn;

    public TMP_Text logTxt;
    private void Start()
    {
        //DefineTitle();
        if(!GameManager.Instance.isAR) StartCoroutine(CountdownRoutine());
    }

    public void DefineTitle()
    {
        enemyTitle.text = !GameManager.Instance.isPlayerAttacking ? "Enemy - AI Attacker" : "Enemy - AI Defender";
        playerTitle.text = !GameManager.Instance.isPlayerAttacking ? "Player (Defender)" : "Player (Attacker)";

    }
    // Update is called once per frame
    void Update()
    {
        if (!GameManager.Instance.isStart) return;
        GameManager.Instance.timer -= Time.deltaTime;
        timerText.text = GameManager.Instance.timer.ToString("F1") + "s";

        if (GameManager.Instance.timer <= 0)
        {
           
            if (GameManager.Instance.isPenalty)
            {
                GameManager.Instance.MatchEnd("defender");
            }
            else
            {
                GameManager.Instance.MatchEnd("draw");
            }
        }
    }

    public IEnumerator CountdownRoutine()
    {
        for (int i = 3; i >= 0; i--)
        {
            countdownText.text = i > 0 ? i.ToString() : "GO!";
            yield return new WaitForSeconds(1f);
        }

        GameManager.Instance.MatchBegin();
        countdownObj.SetActive(false);
    }

    public void ShowResult(string result)
    {
        GameManager.Instance.isStart = false;
        resultObj.SetActive(true);
        resultEnemyScoreText.text = GameManager.Instance.enemyWins.ToString();
        resultPlayerScoreText.text = GameManager.Instance.playerWins.ToString();
        enemyScore.text = resultEnemyScoreText.text;
        playerScore.text = resultPlayerScoreText.text;
        switch (result)
        {
            case "attacker":
                resultMatchText.text = "Attacker Won";
                break;
            case "defender":
                resultMatchText.text = "Defender Won";
                break;
            case "draw":
                resultMatchText.text = "Draw";
                break;
        }

        if (GameManager.Instance.roundMatch == 5)
        {
            nextMatchBtnObj.SetActive(false);
            resultMatchText.text = (GameManager.Instance.playerWins == GameManager.Instance.enemyWins)
                ? "DRAW"
                : (GameManager.Instance.playerWins > GameManager.Instance.enemyWins) ? "PLAYER WINNER" : "ENEMY WINNER";
            endGameTxt.SetActive(GameManager.Instance.playerWins != GameManager.Instance.enemyWins);
            penaltyBtn.SetActive(GameManager.Instance.playerWins == GameManager.Instance.enemyWins);
            if (GameManager.Instance.isPenalty)
            {
                endGameTxt.GetComponent<TMP_Text>().text = "GAME OVER";
            }
        }
    }


    public void MoveScene(string scene)
    {
        ScenesManager.instance.MoveScene(scene);
    }

}
