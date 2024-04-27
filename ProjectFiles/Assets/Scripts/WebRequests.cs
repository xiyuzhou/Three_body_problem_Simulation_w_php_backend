using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class WebRequests : MonoBehaviour
{
    [System.Serializable]
    public class ApiResponse
    {
        public bool success;
        public string message;
        public string UserID;
    }
    public static string url = "https://appendicular-conjun.000webhostapp.com/unity_simulation/";
    public static IEnumerator Login(string username, string password, Action<ApiResponse> callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("loginUser", username);
        form.AddField("loginPass", password);

        using (UnityWebRequest www = UnityWebRequest.Post(url + "Login.php", form))
        {
            www.timeout = 2;
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                callback(new ApiResponse { success = false, message = www.error });
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                ApiResponse response = JsonUtility.FromJson<ApiResponse>(www.downloadHandler.text);
                UserInfo.instance.SetInfo(username, password);
                UserInfo.instance.SetID(response.UserID);
                callback(response);
            }
        }
    }

    public static IEnumerator RegisterUser(string username, string password, Action<ApiResponse> callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("loginUser", username);
        form.AddField("loginPass", password);

        using (UnityWebRequest www = UnityWebRequest.Post(url+"RegisterUser.php", form))
        {
            www.timeout = 2;
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                callback(new ApiResponse { success = false, message = www.error });
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                ApiResponse response = JsonUtility.FromJson<ApiResponse>(www.downloadHandler.text);
                callback(response);
            }
        }
    }
    public static IEnumerator SetCustomData(string userID, string dataName, string jsonData, Action<String> callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("userID", userID);
        form.AddField("Data_name", dataName);
        form.AddField("custom_json", jsonData);

        using (UnityWebRequest www = UnityWebRequest.Post(url + "SetCustomData.php", form))
        {
            www.timeout = 2;
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                callback(www.error);
            }
            else
            {
                callback(www.downloadHandler.text);
            }
        }
    }

    public static IEnumerator GetCustomDataIDs(string userID, Action<String> callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("userID", userID);

        using (UnityWebRequest www = UnityWebRequest.Post(url + "GetCustomDataID.php", form))
        {
            www.timeout = 2;
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                callback(www.error);
            }
            else
            {
                string response = www.downloadHandler.text;
                callback(response);
            }
        }
    }
    public static IEnumerator GetCustomData(string id, Action<String> callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", id);

        using (UnityWebRequest www = UnityWebRequest.Post(url + "GetCustomDataset.php", form))
        {
            www.timeout = 2;
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                callback(www.error);
            }
            else
            {
                //ApiResponse response = JsonUtility.FromJson<ApiResponse>(www.downloadHandler.text);
                string response = www.downloadHandler.text;
                callback(response);
            }
        }
    }
    public static IEnumerator DeleteCustomData(string id, Action<String> callback)
    {
        WWWForm form = new WWWForm();
        form.AddField("id", id);

        using (UnityWebRequest www = UnityWebRequest.Post(url + "DeleteCustomData.php", form))
        {
            www.timeout = 2;
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                callback(www.error);
            }
            else
            {
                callback(www.downloadHandler.text);
            }
        }
    }
}
