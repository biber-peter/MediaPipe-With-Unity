using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Start : MonoBehaviour
{
    
    public void OnStartButtonClick()
    {
            SceneManager.LoadScene("GestureScene");
    }
}
