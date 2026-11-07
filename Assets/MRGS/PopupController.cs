using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class PopupController : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private List<KeyValuePair<string, string>> awaitingList = new List<KeyValuePair<string, string>>();

    public void showPopup(string header, string body)
    {

        if (Popup.activeInHierarchy)
        {
            //Уже на экране, надо дождаться закрытия и сменить текст
            awaitingList.Add(new KeyValuePair<string, string>(header, body));
        }
        else
        {
            Header.text = header;
            Body.text = body;
            Popup.SetActive(true);
        }
    }

    public Text Header;
    public Text Body;
    public GameObject Popup;

    public void closePopup()
    {
        if (awaitingList.Count > 0)
        {
            int lastElementIndex = awaitingList.Count - 1;
            var savedParams = awaitingList[lastElementIndex];
            Header.text = savedParams.Key;
            Body.text = savedParams.Value;
            awaitingList.RemoveAt(lastElementIndex);
        }
        else
        {
            Popup.SetActive(false);
        }

    }


}
