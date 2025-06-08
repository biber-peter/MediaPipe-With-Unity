using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Requit : MonoBehaviour
{

    public void OnRequitButtonClick()
    {
        SceneManager.LoadScene("UI");
    }
}
