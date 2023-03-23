using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ServerAddress : MonoBehaviour
{
    public Button connectButton;
    [SerializeField]
    private TMP_Text serverAddressText;
    [SerializeField]
    private TMP_Text serverPortText;

    public System.Action<string, int> onServerSelected;

    public string serverAddress { get; private set; }
    public int serverPort { get; private set; }

    private void Start()
    {
        if(connectButton != null)
            connectButton.onClick.AddListener(OnButtonClicked);
    }

    public void SetServer(string address, int port)
    {
        serverAddress = address;
        serverPort = port;
        serverAddressText.text = address;
        serverPortText.text = port.ToString();
    }

    private void OnButtonClicked()
    {
        onServerSelected?.Invoke(serverAddress, serverPort);
    }
}
