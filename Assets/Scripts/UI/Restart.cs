using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Restart : MonoBehaviour
{

    public void OnRestartButtonClick()
    {
        SceneManager.LoadScene("PostureScene");
    }
}
