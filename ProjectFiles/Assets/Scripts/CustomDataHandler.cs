using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using SimpleJSON;
using System;
using UnityEngine.UI;

public class CustomDataHandler : MonoBehaviour
{
    public TMP_InputField[] star1Info;
    public TMP_InputField[] star2Info;
    public TMP_InputField[] star3Info;

    public TMP_InputField DatasetName;
    public TMP_InputField GravitationalConst;
    public TMP_InputField TimeStep;
    public TMP_InputField Size;
    public TMP_InputField duration;

    public TextMeshProUGUI DebugText;
    public TextMeshProUGUI displayText;

    public GameObject itemListParent;
    public List<GameObject> currentButtonAssets;
    public CustomDataInfo currentSelection;
    public void ConvertToJson()
    {
        DataObject myData = GetCurrentData();
       
        string json = JsonUtility.ToJson(myData);

        StartCoroutine(WebRequests.SetCustomData(UserInfo.instance.UserID, DatasetName.text, json, showDebugText));
    }
    public DataObject GetCurrentData()
    {
        DataObject myData = new DataObject();
        myData.name = DatasetName.text;
        for (int i = 0; i < 7; i++)
        {
            myData.star1Info[i] = float.Parse(star1Info[i].text);
            myData.star2Info[i] = float.Parse(star2Info[i].text);
            myData.star3Info[i] = float.Parse(star3Info[i].text);
        }
        myData.GravitationalConst = float.Parse(GravitationalConst.text);
        myData.TimeStep = float.Parse(TimeStep.text);
        myData.Size = float.Parse(Size.text);
        myData.duration = float.Parse(duration.text);
        return myData;
    }
    public void Reset()
    {
        for (int i = 0; i < 7; i++)
        {
            star1Info[i].text = i == 0 ? "1" : "0";
            star2Info[i].text = i == 0 ? "1" : "0";
            star3Info[i].text = i == 0 ? "1" : "0";
        }
        GravitationalConst.text = "5";
        TimeStep.text = "1";
        Size.text = "1";
        duration.text = "5";
        showDebugText("reset to default");
        CameraController.instance.OnStart();
    }
    private void showDebugText(string log)
    {
        StartCoroutine(showDebugTextRoutine(log));
    }
    IEnumerator showDebugTextRoutine(string debugLog)
    {
        DebugText.text = debugLog;
        yield return new WaitForSeconds(3);
        if (DebugText.text == debugLog)
            DebugText.text = "";

    }
    public void getID()
    {
        Action<string> getDataId = (IDs) =>
        {
            StartCoroutine(createItemsRoutine(IDs));
        };
        StartCoroutine(WebRequests.GetCustomDataIDs(UserInfo.instance.UserID, getDataId));
    }
    IEnumerator createItemsRoutine(string jsonArrayString)
    {
        if (jsonArrayString == "0")
            yield break;
        JSONArray jsonArray = JSON.Parse(jsonArrayString) as JSONArray;
        for (int i = 0; i < jsonArray.Count; i++)
        {
            bool isDone = false;
            bool isNew = false;
            string id = jsonArray[i].AsObject["id"];
            foreach(GameObject obj in currentButtonAssets){
                if (obj.GetComponent<CustomDataInfo>().id == id)
                {
                    isNew = true;
                    break;
                }
            }
            if (isNew)
                continue;
            DataObject data1 = new DataObject();
            Action<string> getDataInfo = (dataInfo) =>
            {
                //Debug.Log(dataInfo);
                isDone = true;
                data1 = JsonUtility.FromJson<DataObject>(dataInfo);
            };
            StartCoroutine(WebRequests.GetCustomData(id, getDataInfo));
            yield return new WaitUntil(() => isDone == true);

            GameObject assetButton = Instantiate(Resources.Load("Prefab/DataAssetButton") as GameObject);
            CustomDataInfo DataAsset = assetButton.AddComponent<CustomDataInfo>();
            currentButtonAssets.Add(assetButton);
            DataAsset.id = id;
            assetButton.transform.SetParent(itemListParent.transform, false);
            DataAsset.data = data1;
            assetButton.transform.Find("ButtonText").GetComponent<TextMeshProUGUI>().text = DataAsset.data.name;

            assetButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                currentSelection = DataAsset;
                displayText.text = DataAsset.data.name;
            });
        }
        yield return null;
    }

    public void LoadSelection()
    {
        if (currentSelection == null)
            return;
        DataObject myData = currentSelection.data;
        DatasetName.text = myData.name;
        for (int i = 0; i < 7; i++)
        {
            star1Info[i].text = myData.star1Info[i].ToString();
            star2Info[i].text = myData.star2Info[i].ToString();
            star3Info[i].text = myData.star3Info[i].ToString();
        }
        GravitationalConst.text = myData.GravitationalConst.ToString();
        TimeStep.text = myData.TimeStep.ToString();
        Size.text = myData.Size.ToString();
        duration.text = myData.duration.ToString();
        showDebugText(myData.name + " loaded");
    }
    public void DeleteSelection()
    {
        if (currentSelection == null) return;

        Action<string> DeleteItemCallback = (message) =>
        {
            showDebugText(message);
            if (!message.StartsWith("Error"))
            {
                currentButtonAssets.Remove(currentSelection.gameObject);
                Destroy(currentSelection.gameObject);
                currentSelection = null;
            }
        };
        StartCoroutine(WebRequests.DeleteCustomData(currentSelection.id, DeleteItemCallback));
    }
}
