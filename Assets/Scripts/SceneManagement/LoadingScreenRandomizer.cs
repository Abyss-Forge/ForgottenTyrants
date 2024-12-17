using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Linq;
using TMPro;

namespace Systems.SceneManagement
{
    public class LoadingScreenRandomizer : MonoBehaviour
    {
        [SerializeField] private Image _backgroundImage;
        [SerializeField] private TMP_Text _messageText;

        [SerializeField] private TextAsset _messagesTxt;
        [SerializeField] private FolderReference _backgroundsFolder;

        void OnEnable()
        {
            SetRandomBackground();
            SetRandomMessage();
        }

        private void SetRandomMessage()
        {
            string[] messages = _messagesTxt.text.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            if (messages.Length == 0) return;

            string randomMessage = messages[UnityEngine.Random.Range(0, messages.Length)];
            _messageText.text = randomMessage;
        }

        private void SetRandomBackground()
        {
            string[] files = Directory.GetFiles(_backgroundsFolder.FolderPath, "*.*")
                                 .Where(file => file.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase) ||
                                                file.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase) ||
                                                file.EndsWith(".png", StringComparison.OrdinalIgnoreCase))
                                 .ToArray();


            if (files.Length == 0) return;

            string randomFile = files[UnityEngine.Random.Range(0, files.Length)];

            byte[] fileData = File.ReadAllBytes(randomFile);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(fileData);

            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));

            _backgroundImage.sprite = sprite;
        }

    }
}