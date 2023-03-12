using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LootLocker.Requests;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    public bool loggedIn;
    private int incrementalIDInt;
    public TMP_InputField playerInputField;

    public IEnumerator LoginRoutine()
    {
        incrementalIDInt = PlayerPrefs.GetInt(nameof(incrementalIDInt));
        bool done = false;
        LootLockerSDKManager.StartGuestSession((response) =>
        {
            if (response.success)
            {
                Debug.Log("Successfully started LootLocker session");
                loggedIn = true;
                done = true;
                PlayerPrefs.SetString("PlayerID", response.player_id.ToString());
                PlayerPrefs.SetString("PlayerName", PlayerPrefs.GetString("PlayerName", "#Guest" + PlayerPrefs.GetString("PlayerID")));
            }
            else
            {
                Debug.Log("Error starting LootLocker session");
                done = true;
            }
        });

        yield return new WaitWhile(() => done == false);

        UpdatePlayerInputField();
    }

    public void UpdatePlayerName()
    {
        PlayerPrefs.SetString("PlayerName", playerInputField.text);
    }

    public void UpdatePlayerInputField()
    {
        playerInputField.text = PlayerPrefs.GetString("PlayerName");
        TextMeshProUGUI placeHolderText = playerInputField.placeholder as TextMeshProUGUI;
        placeHolderText.text = PlayerPrefs.GetString("PlayerName");
    }
}