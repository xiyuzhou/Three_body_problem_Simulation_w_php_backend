using UnityEngine;

public class UserInfo : MonoBehaviour
{
    public static UserInfo instance;
    
    public string UserID { get; private set; }
    string UserName;
    string UserPassword;

    private void Awake()
    {
        instance = this;
    }
    public void SetInfo(string name, string password)
    {
        UserName = name;
        UserPassword = password;
    }
    public void SetID(string id)
    {
        UserID = id;
    }

}
