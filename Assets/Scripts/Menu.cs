using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public TMPro.TMP_Text playerName;
    public TMPro.TMP_InputField inputField;

    private void Start()
    {
        playerName.text = PlayerPrefs.GetString("playerName", "nobody");
    }
    public void StartGame()
    {
        SceneManager.LoadScene("Basics");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    public void SavePlayerName()
    {
        playerName.text = inputField.text;
        PlayerPrefs.SetString("playerName", inputField.text);
    }
}
