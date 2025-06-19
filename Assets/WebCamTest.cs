using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WebCamTest : MonoBehaviour
{
    public WebCamTexture webcamTexture;
    public Image image;
    public Button button;
    public LobbyState lobby;

    // Start is called before the first frame update
    void Start()
    {
        button.onClick.AddListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        //if we dont have webcam running, start it
        if (webcamTexture == null || !webcamTexture.isPlaying)
        {
            // Initialize the webcam texture with the first available device
            webcamTexture = new WebCamTexture(WebCamTexture.devices[0].name);
            image.material.mainTexture = webcamTexture;
            webcamTexture.Play();
            var mat = image.material;
            image.material = null;
            image.material = mat;
        }
        else
        {
            //if we have webcam running, send its image to server
            if (webcamTexture.isPlaying)
            {
                // Convert the webcam texture to a Texture2D
                Texture2D texture = new Texture2D(webcamTexture.width, webcamTexture.height);
                texture.SetPixels(webcamTexture.GetPixels());
                texture.Apply();
                // Send the texture to the server
                lobby.SendImage(texture);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
