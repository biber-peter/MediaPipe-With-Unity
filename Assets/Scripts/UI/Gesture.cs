using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class Gesture : MonoBehaviour
{
    // Start is called before the first frame update
    public TMP_InputField UsernameInput;
    public TMP_InputField PasswordInput;//输入字段类型的变量
        public void OnGestureButtonClick()
    {
        string username = UsernameInput.text;
        string password = PasswordInput.text;//获取 UsernameInput 和 PasswordInput 中的文本内容，分别赋值给 username 和 password 字符串变量。
        if (username == "ykh" && password == "111")

        {
            SceneManager.LoadScene("GestureScene");
        }
    }
}
