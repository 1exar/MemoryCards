using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

namespace MemoryCards.Scripts.Services
{
    public class ImageService
    {
        private const string JsonUrl = "https://drive.usercontent.google.com/download?id=1YFYTH6V33YI3lGEKr1RLP-jJlaj0xaFZ&export=download&authuser=0";

        [System.Serializable]
        public class CardData { public int id; public string url; }
        [System.Serializable]
        public class CardDataList { public List<CardData> cards; }

        public async Task<List<Sprite>> LoadSpritesAsync()
        {
            using (var request = UnityWebRequest.Get(JsonUrl))
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