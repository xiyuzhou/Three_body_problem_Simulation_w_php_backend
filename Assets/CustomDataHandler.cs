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

    public GameObject itemListParent;
    public List<GameObject> currentButtonAssets;
    public CustomDataInfo currentSelection;
    public void ConvertToJson()
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
        string json = JsonUtility.ToJson(myData);
        StartCoroutine(WebRequests.SetCustomData("1", DatasetName.text, json));//UserInfo.instance.UserID
    }
    private void Start()
    {
        
    }
    public void SaveInfo()
    {
        
    }
    public void getID()
    {
        Action<string> getDataId = (IDs) =>
        {
            Debug.Log(IDs);
            StartCoroutine(createItemsRoutine(IDs));
        };
        StartCoroutine(WebRequests.GetCustomDataIDs("1", getDataId));
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
                Debug.Log(dataInfo);
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
            });
        }
        yield return null;
    }
}
