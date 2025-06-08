using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Posture : MonoBehaviour
{
    // Start is called before the first frame update
    public TMP_InputField UsernameInput;
    public TMP_InputField PasswordInput;
        public void OnPostureButtonClick()
    {
        string username = UsernameInput.text;
        string password = PasswordInput.text;
        if (username == "ykh" && password == "111")

        {
            SceneManager.LoadScene("PostureScene");
        }
    }
}
