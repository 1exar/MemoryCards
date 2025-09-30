using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MemoryCards.Scripts.Configs;
using UnityEngine;
using UnityEngine.Networking;
using Zenject;

namespace MemoryCards.Scripts.Services
{
    public class ImageService
    {
        private readonly string _jsonUrl;

        [Inject]
        public ImageService(GameConfig config)
        {
            _jsonUrl = config.JsonUrl;
        }

        [Serializable]
        public class CardData { public int id; public string url; }
        
        [Serializable]
        public class CardDataList { public List<CardData> cards; }

        public event Action OnSpritesLoaded;

        public async Task<List<Sprite>> LoadSpritesAsync()
        {
            using (var request = UnityWebRequest.Get(_jsonUrl))
            {
                await request.SendWebRequest().ToTask();

                if (request.result != UnityWebRequest.Result.Success)
                {
                    Debug.LogError("Failed to load JSON: " + request.error);
                    return null;
                }

                var json = request.downloadHandler.text;
                
                Debug.Log(json);
                
                var data = JsonUtility.FromJson<CardDataList>(json);

                var sprites = new List<Sprite>();

                foreach (var card in data.cards)
                {
                    using (var texReq = UnityWebRequestTexture.GetTexture(card.url))
                    {
                        await texReq.SendWebRequest().ToTask();
                        if (texReq.result != UnityWebRequest.Result.Success)
                        {
                            Debug.LogError("Failed to load sprite: " + texReq.error);
                            continue;
                        }

                        var tex = DownloadHandlerTexture.GetContent(texReq);
                        var sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height),
                            new Vector2(0.5f, 0.5f));
                        sprites.Add(sprite);
                    }
                }

                OnSpritesLoaded?.Invoke();
                return sprites;
            }
        }
    }

    public static class UnityWebRequestExtensions
    {
        public static Task ToTask(this UnityWebRequestAsyncOperation operation)
        {
            var tcs = new TaskCompletionSource<bool>();
            operation.completed += _ => tcs.SetResult(true);
            return tcs.Task;
        }
    }
}