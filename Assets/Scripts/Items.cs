using SimpleJSON;
using System;
using System.Collections;

using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Items : MonoBehaviour
{
    Action<String> _createItemsCallback;
    void Start()
    {
        /*_createItemsCallback = (JSONArrayString) =>
        {
            StartCoroutine(createItemsRoutine(JSONArrayString));
        };
        CreatItems();*/
    }

    public void CreatItems()
    {
        //StartCoroutine(WebRequests.GetItemsIDs(UserInfo.instance.UserID, _createItemsCallback));
    }

    IEnumerator createItemsRoutine(string jsonArrayString)
    {
        if (jsonArrayString == "0")
            yield break;
        JSONArray jsonArray = JSON.Parse(jsonArrayString) as JSONArray;

        for (int i = 0; i < jsonArray.Count; i++)
        {
            bool isDone = false;
            string itemID = jsonArray[i].AsObject["itemID"];
            string id = jsonArray[i].AsObject["ID"];

            JSONObject itemInfoJson = new JSONObject();

            Action<string> getItemInfoCallback = (itemInfo) =>
            {
                isDone = true;
                JSONArray tempArray = JSON.Parse(itemInfo) as JSONArray;
                itemInfoJson = tempArray[0].AsObject;
            };

            //StartCoroutine(WebRequests.GetItem(itemID, getItemInfoCallback));

            yield return new WaitUntil(() => isDone == true);

            GameObject item = Instantiate(Resources.Load("Prefab/Item") as GameObject);
            ItemInfo thisItem = item.AddComponent<ItemInfo>();
            thisItem.ID = id;
            thisItem.ItemID = itemID;


            item.transform.SetParent(this.transform);
            item.transform.localScale = Vector3.one;
            item.transform.localPosition = Vector3.zero;

            item.transform.Find("Name").GetComponent<TextMeshProUGUI>().text = itemInfoJson["name"];
            item.transform.Find("Price").GetComponent<TextMeshProUGUI>().text = itemInfoJson["price"];
            item.transform.Find("Description").GetComponent<TextMeshProUGUI>().text = itemInfoJson["description"];
            Action<Sprite> getItemIconCallback = (downloadedSprite) =>
            {
                item.transform.Find("Icon").GetComponent<Image>().sprite = downloadedSprite;
            };

            //StartCoroutine(WebRequests.GetItemIcon(itemID, getItemIconCallback));
            //yield return new WaitUntil(() => IconDownload == true);

            item.transform.Find("SellButton").GetComponent<Button>().onClick.AddListener(() =>
            {
                string iID = itemID;
                string idUnique = id;
                string uID = UserInfo.instance.UserID;
                Debug.Log(iID +  idUnique+ uID);
                //StartCoroutine(WebRequests.SellItem(id, iID, uID));
            });

        }


        yield return null;
    }

}
