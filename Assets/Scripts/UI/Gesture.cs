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
    public TMP_InputField PasswordInput;//�����ֶ����͵ı���
        public void OnGestureButtonClick()
    {
        string username = UsernameInput.text;
        string password = PasswordInput.text;//��ȡ UsernameInput �� PasswordInput �е��ı����ݣ��ֱ�ֵ�� username �� password �ַ���������
        if (username == "ykh" && password == "111")

        {
            SceneManager.LoadScene("GestureScene");
        }
    }
}
