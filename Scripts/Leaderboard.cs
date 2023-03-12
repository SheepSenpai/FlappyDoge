
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LootLocker.Requests;
using TMPro;

public class Leaderboard : MonoBehaviour
{
    public int scoreToUpload;
    readonly string leaderboardID = "12390";
    public bool canUploadScore;
    public TextMeshProUGUI namesText;
    public TextMeshProUGUI scoresText;

    public void SubmitScore()
    {
        if (canUploadScore == false)
        {
            return;
        }

        string playerID = PlayerPrefs.GetString("PlayerID") + GetAndIncrementScoreCharacters();
        string metadata = PlayerPrefs.GetString("PlayerName");

        LootLockerSDKManager.SubmitScore(playerID, scoreToUpload, leaderboardID, metadata, (response) =>
        {
            if (response.statusCode == 200)
            {
                Debug.Log("Successful");
                canUploadScore = false;
                StartCoroutine(FetchHighscoresCentered());
            }
            else
            {
                Debug.Log("failed: " + response.Error);
            }
        });

    }

    string GetAndIncrementScoreCharacters()
    {
        string incrementalScoreString = PlayerPrefs.GetString(nameof(incrementalScoreString), "a");
        char incrementalCharacter = PlayerPrefs.GetString(nameof(incrementalCharacter), "a")[0];

        if (incrementalScoreString[^1] == 'z')
        {
            incrementalScoreString += incrementalCharacter;
        }
        else
        {
            incrementalScoreString = incrementalScoreString[0..^1] + incrementalCharacter.ToString();
        }

        if ((int)incrementalCharacter < 122)
        {
            incrementalCharacter++;
        }
        else
        {
            incrementalCharacter = 'a';
        }

        PlayerPrefs.SetString(nameof(incrementalCharacter), incrementalCharacter.ToString());
        PlayerPrefs.SetString(nameof(incrementalScoreString), incrementalScoreString.ToString());

        return incrementalScoreString;
    }

    public IEnumerator FetchTopLeaderboardScores()
    {
        string playerNames = "Loading...";
        string playerScores = "";
        scoresText.text = playerScores;
        namesText.text = playerNames;
        int count = 10;
        int after = 0;

        bool done = false;
        LootLockerSDKManager.GetScoreList(leaderboardID, count, after, (response) =>
        {
            if (response.statusCode == 200)
            {
                Debug.Log("Successful");
                playerNames = "Names\n";
                playerScores = "Score\n";

                LootLockerLeaderboardMember[] members = response.items;
                for (int i = 0; i < members.Length; i++)
                {
                    playerNames += members[i].rank + ". " + members[i].metadata + "\n";
                    playerScores += members[i].score + "\n";
                }
                done = true;
            }
            else
            {
                Debug.Log("failed: " + response.Error);
                playerNames = "Error, could not retrieve leaderboard";
                done = true;
            }
        });

        yield return new WaitWhile(() => done == false);
        namesText.text = playerNames;
        scoresText.text = playerScores;
    }

    IEnumerator FetchHighscoresCentered()
    {
        bool done = false;
        string playerNames = "Loading...";
        string playerScores = "";
        scoresText.text = playerScores;
        namesText.text = playerNames;
        string latestPlayerID = PlayerPrefs.GetString("PlayerID") + GetAndIncrementScoreCharacters();



        LootLockerSDKManager.GetMemberRank(leaderboardID, latestPlayerID, (response) =>
        {
            if (response.statusCode == 200)
            {
                Debug.Log("Successful");
                int rank = response.rank;
                int count = 10;
                int after = rank < 6 ? 0 : rank - 5;

                LootLockerSDKManager.GetScoreList(leaderboardID, count, after, (response) =>
                {
                    if (response.statusCode == 200)
                    {
                        playerNames = "Names\n";
                        playerScores = "Score\n";
                        Debug.Log("Successful");
                        LootLockerLeaderboardMember[] members = response.items;

                        for (int i = 0; i < members.Length; i++)
                        {
                            if (members[i].rank == rank)
                            {
                                playerNames += "<color=#f4e063ff>" + members[i].rank + ". " + members[i].metadata + "\n";
                                playerScores += "<color=#f4e063ff>" + members[i].score + "\n";
                            }
                            else
                            {
                                playerNames += members[i].rank + ". " + members[i].metadata + "\n";
                                playerScores += members[i].score + "\n";
                            }
                        }
                        done = true;
                    }
                    else
                    {
                        Debug.Log("failed: " + response.Error);
                    }
                });
            }
            else
            {
                Debug.Log("failed: " + response.Error);
            }
        });

        yield return new WaitWhile(() => done == false);
        namesText.text = playerNames;
        scoresText.text = playerScores;
    }
}