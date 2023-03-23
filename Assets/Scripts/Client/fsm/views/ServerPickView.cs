using UnityEngine.UI;
using TMPro;
using UnityEngine;
using System.Collections.Generic;

/**
 * Wraps all elements and functionality required for the LoginView.
 */
public class ServerPickView : View
{      
    [SerializeField] private TextMeshProUGUI  textConnectResults = null;
    [SerializeField] private GameObject       goConnectResultsRow = null;

    [SerializeField] private GameObject serverListRoot;
    [SerializeField] private ServerAddress serverListItemPrefab;

    public System.Action<string, int> OnServerConnectRequest;
    
    public string TextConnectResults
    {
        set
        {
            textConnectResults.text = value;
            //enable the row with status info based on whether we actually have status info
            goConnectResultsRow.SetActive(value != null && value.Length > 0);
        }
    }
    
    public ServerAddress AddServer(string address, int port)
    {
        ServerAddress serverAddress = Instantiate(serverListItemPrefab, serverListRoot.transform);
        serverAddress.SetServer(address, port);
        serverAddress.onServerSelected += (a, b) => OnServerConnectRequest?.Invoke(a, b);
        return serverAddress;
    }

    public void ClearServers()
    {
        foreach (Transform child in serverListRoot.transform)
        {
            Destroy(child.gameObject);
        }
    }
}
