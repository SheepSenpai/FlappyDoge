using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class GameController : MonoBehaviour
{
    public static GameController instance;
    public TextMeshProUGUI menuText;
    private float countDownTimer;
    private int countdownInt;
    public ObstacleController obsController;
    public CharacterController charController;
    private int score;
    public TextMeshProUGUI scoreText;
    public PlayerManager playerManager;
    public Leaderboard leaderboard;
    public GameObject submitScoreButton;
    public GameObject leaderboardObject;
    public GameObject playerNameInputFieldObject;
    public enum State {Loading, Menu, Countdown, Playing, Dying, Dead };
    public State curState = State.Menu;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        StartCoroutine(SetupRoutine());
    }

    IEnumerator ClearInfoText(float timeToClear)
    {
        yield return new WaitForSeconds(timeToClear);
        menuText.text = "";
    }

    void SetMenuText()
    {
        menuText.text = "Press spacebar to play";
    }

    void Update()
    {
        switch (curState)
        {
            case State.Menu:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    ChangeState(State.Countdown);
                }
                break;

            case State.Countdown:
                countDownTimer -= Time.deltaTime;
                if (Mathf.CeilToInt(countDownTimer) != countdownInt)
                {
                    countdownInt = Mathf.CeilToInt(countDownTimer);
                    if (countdownInt == 0)
                    {
                        menuText.text = "GO!\nPRESS SPACEBAR";
                        StartCoroutine(ClearInfoText(0.5f));
                        ChangeState(State.Playing);
                    }
                    else
                    {
                        menuText.text = countdownInt.ToString();
                    }
                }
                break;

            case State.Playing:
                break;

            case State.Dead:
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    ChangeState(State.Menu);
                }
                break;
        }
    }

    public void ChangeState(State newState)
    {
        switch (newState)
        {
            case State.Menu:
                charController.ResetCharacter();
                obsController.ResetObstacles();
                Score = 0;
                scoreText.text = "";
                SetMenuText();
                leaderboardObject.SetActive(true);
                submitScoreButton.SetActive(false);
                playerNameInputFieldObject.SetActive(false);
                break;

            case State.Countdown:
                leaderboardObject.SetActive(false);
                countDownTimer = 3.5f;
                countdownInt = Mathf.CeilToInt(countDownTimer);
                menuText.text = "GET READY";
                break;

            case State.Playing:
                break;

            case State.Dying:
                StartCoroutine(DeathRoutine());
                break;

            case State.Dead:
                playerNameInputFieldObject.SetActive(true);
                leaderboardObject.SetActive(true);
                submitScoreButton.SetActive(true);
                break;
        }
        curState = newState;
    }

    IEnumerator DeathRoutine()
    {
        menuText.text = "GAME OVER";
        leaderboard.scoreToUpload = score;
        leaderboard.canUploadScore = true;
        yield return new WaitForSeconds(2f);
        ChangeState(State.Dead);
        menuText.text = "PRESS SPACEBAR TO RESTART";
    }

    public int Score
    {
        get { return score; }
        set
        {
            scoreText.text = value.ToString();
            score = value;
        }
    }

    IEnumerator SetupRoutine()
    {
        menuText.text = "Logging in...";
        yield return playerManager.LoginRoutine();

        if (playerManager.loggedIn == false)
        {
            float loginCountdown = 4;
            float timer = loginCountdown;
            while (timer >= -1f)
            {
                timer -= Time.deltaTime;

                if (Mathf.CeilToInt(timer) != Mathf.CeilToInt(loginCountdown))
                {
                    menuText.text = "Failed to login retrying in " + Mathf.CeilToInt(timer).ToString();
                    loginCountdown -= 1f;
                }
                yield return null;
            }
            StartCoroutine(SetupRoutine());
            yield break;
        }

        yield return leaderboard.FetchTopLeaderboardScores();

        SetMenuText();
        ChangeState(State.Menu);

        yield return null;
    }
}