using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Quit : MonoBehaviour
{

    public void OnQuitButtonClick()
    {
        SceneManager.LoadScene("UI");
    }
}
