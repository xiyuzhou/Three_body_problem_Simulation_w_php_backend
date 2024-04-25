using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class MenuInteraction : MonoBehaviour
{

    public bool CurPageIndex;

    public TMP_InputField[] SignInPage;
    public TMP_InputField[] SignupPage;

    // Update is called once per frame
    void Awake()
    {
        CurPageIndex = true;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
            TabSelectNext();
        if (Input.GetKeyDown(KeyCode.Return))
            EnterConfirm();
    }


    private void TabSelectNext()
    {
        var InputFieldList = CurPageIndex ? SignInPage : SignupPage;
        for (int i = 0; i < InputFieldList.Length; i++)
        {
            if (InputFieldList[i].isFocused)
            {
                int nextIndex = (i + 1) % InputFieldList.Length;
                InputFieldList[nextIndex].ActivateInputField();
                return;
            }
        }
        InputFieldList[0].ActivateInputField();
    }

    private void EnterConfirm()
    {
        if (CurPageIndex)
        {
            LoginRegister.instance.OnLoginButton();
        }
        else
        {
            LoginRegister.instance.OnRegister();
        }
    }
}
