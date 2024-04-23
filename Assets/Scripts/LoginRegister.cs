using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.UI;

public class LoginRegister : MonoBehaviour
{
    public TMP_InputField usernameInput;
    public TMP_InputField passwordInput;

    public TMP_InputField SignupName;
    public TMP_InputField SignupPassword1;
    public TMP_InputField SignupPassword2;

    public TextMeshProUGUI displayText;
    public TextMeshProUGUI displayText2;
    public UnityEvent onLoggedIn;

    public static LoginRegister instance;

    public GameObject LoginPage;
    public GameObject RegisterPage;

    public MenuInteraction menuInteraction;

    public CustomDataHandler handler;
    public Button[] buttons;
    void Awake()
    {
        instance = this;
    }

    public void OnRegister()
    {
        if (SignupPassword1.text == "")
        {
            SetDisplayText(displayText2, "Empty password", Color.red);
            Debug.Log("Empty password");
            return;
        }
        if (SignupPassword1.text != SignupPassword2.text || SignupPassword1.text == "")
        {
            SetDisplayText(displayText2, "Password do not match", Color.red);
            return;
        }
        StartCoroutine(WebRequests.RegisterUser(SignupName.text, SignupPassword1.text, HandleRegisterResponse));
    }

    public void OnLoginButton()
    {
        if (usernameInput.text == "")
        {
            SetDisplayText(displayText, "Empty Username", Color.red);
            return;
        }
        if (passwordInput.text == "")
        {
            SetDisplayText(displayText, "Empty password", Color.red);
            return;
        }
        StartCoroutine(WebRequests.Login(usernameInput.text, passwordInput.text, HandleLoginResponse));
    }

    void SetDisplayText(TextMeshProUGUI TmpText, string text, Color color)
    {
        TmpText.text = text;
        TmpText.color = color;
    }

    private void HandleLoginResponse(WebRequests.ApiResponse response)
    {
        if (response.success)
        {
            SetDisplayText(displayText, response.message, Color.green);
            Debug.Log(response.message);
            onLoggedIn.Invoke();
            foreach (Button button in buttons)
            {
                button.interactable = true;
            }
            handler.getID();
        }
        else
        {
            SetDisplayText(displayText, response.message, Color.red);
        }
    }

    private void HandleRegisterResponse(WebRequests.ApiResponse response)
    {
        if (response.success)
        {
            SetDisplayText(displayText2, response.message + '\n' + "Leading to sign in page...", Color.green);
            Invoke("SigninWithInfo", 2f);
            Debug.Log(response.message);
        }
        else
        {
            SetDisplayText(displayText2, response.message, Color.red);
        }
    }

    public void OnCreateAccountBT()
    {
        LoginPage.SetActive(false);
        RegisterPage.SetActive(true);
        menuInteraction.CurPageIndex = false;
    }

    public void OnSignInBT()
    {
        LoginPage.SetActive(true);
        RegisterPage.SetActive(false);
        menuInteraction.CurPageIndex = true;
    }

    public void SigninWithInfo()
    {
        LoginPage.SetActive(true);
        RegisterPage.SetActive(false);
        menuInteraction.CurPageIndex = true;
        usernameInput.text = SignupName.text;
        passwordInput.text = SignupPassword1.text;
    }

    // ... (other methods)
}
