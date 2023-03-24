using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using shared;

public class ImageSendButton : MonoBehaviour
{
    public Image image;
    public Button sendButton;
    public LobbyState lobbyState;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SendImage()
    {
        if (image.sprite != null)
        {
            lobbyState.SendImage(image.sprite.texture);
        }
    }
}
