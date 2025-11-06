using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Threading.Tasks;

namespace DummyUI
{
    /// <summary>
    /// Avatar component for displaying user avatars with image and fallback text
    /// Similar to shadcn/ui Avatar component
    /// </summary>
    public class Avatar : MonoBehaviour
    {
    [Header("Avatar Settings")]
    [SerializeField] private string imageUrl = "https://avatars.githubusercontent.com/u/123456789";
        [SerializeField] private string fallbackText = "CN";
        [SerializeField] private AvatarShape shape = AvatarShape.Circle;
        [SerializeField] private AvatarSize size = AvatarSize.Medium;
        
        [Header("Style Settings")]
        [SerializeField] private bool useGrayscale = false;
        [SerializeField] private bool showRing = false;
        [SerializeField] private Color ringColor = Color.white;
        
        private UIDocument uiDocument;
        private AvatarElement avatarElement;
        
        public enum AvatarShape
        {
            Circle,
            RoundedSquare
        }
        
        public enum AvatarSize
        {
            Small,      // 32px
            Medium,     // 40px
            Large       // 48px
        }
        
        private void Awake()
        {
            uiDocument = GetComponent<UIDocument>();
            if (uiDocument == null)
            {
                uiDocument = gameObject.AddComponent<UIDocument>();
            }
        }
        
        private void Start()
        {
            InitializeAvatar();
            LoadImage();
        }
        
        private void InitializeAvatar()
        {
            // Create avatar element
            avatarElement = new AvatarElement();
            avatarElement.SetFallbackText(fallbackText);
            avatarElement.SetShape(shape);
            avatarElement.SetSize(size);
            avatarElement.SetGrayscale(useGrayscale);
            avatarElement.SetRing(showRing, ringColor);
            
            if (uiDocument.rootVisualElement != null)
            {
                uiDocument.rootVisualElement.Add(avatarElement);
            }
        }
        
        private void LoadImage()
        {
            if (!string.IsNullOrEmpty(imageUrl))
            {
                StartCoroutine(LoadImageFromUrl(imageUrl));
            }
        }
        
        private IEnumerator LoadImageFromUrl(string url)
        {
            using (UnityWebRequest request = AvatarRequestUtility.CreateTextureRequest(url))
            {
                yield return request.SendWebRequest();
                
                if (request.result == UnityWebRequest.Result.Success)
                {
                    var data = request.downloadHandler?.data;
                    TryApplyTextureData(data, url, "[Avatar]");
                }
                else
                {
                    Debug.LogWarning($"[Avatar] Failed to load image from {url}: {request.error}");
                    Debug.LogWarning($"[Avatar] Response Code: {request.responseCode}");
                    // Fallback text will be shown automatically

                    if (ShouldFallback(request))
                    {
                        yield return TryFallbackDownload(url);
                    }
                }
            }
        }

        private bool TryApplyTextureData(byte[] data, string url, string logPrefix)
        {
            if (data == null || data.Length == 0)
            {
                Debug.LogWarning($"{logPrefix} Request succeeded but returned no data for {url}");
                return false;
            }

            Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (texture.LoadImage(data))
            {
                avatarElement.SetImage(texture);
                Debug.Log($"{logPrefix} Successfully loaded image from {url}");
                return true;
            }

            Debug.LogWarning($"{logPrefix} Failed to decode texture from {url}");
            return false;
        }

        private static bool ShouldFallback(UnityWebRequest request)
        {
            return request.result == UnityWebRequest.Result.ConnectionError || request.responseCode == 0;
        }

        private IEnumerator TryFallbackDownload(string url)
        {
            Debug.Log("[Avatar][Fallback] Attempting HttpClient download...");

            Task<byte[]> task = AvatarRequestUtility.DownloadWithHttpClientAsync(url);
            while (!task.IsCompleted)
            {
                yield return null;
            }

            if (task.IsCanceled)
            {
                Debug.LogWarning("[Avatar][Fallback] HttpClient task canceled.");
                yield break;
            }

            if (task.IsFaulted)
            {
                Debug.LogWarning($"[Avatar][Fallback] HttpClient task faulted: {task.Exception?.GetBaseException().Message}");
                yield break;
            }

            var data = task.Result;
            if (!TryApplyTextureData(data, url, "[Avatar][Fallback]"))
            {
                Debug.LogWarning("[Avatar][Fallback] Failed to decode HttpClient data.");
            }
        }
        
        // Public API
        public void SetImageUrl(string url)
        {
            imageUrl = url;
            if (avatarElement != null)
            {
                StartCoroutine(LoadImageFromUrl(url));
            }
        }
        
        public void SetFallbackText(string text)
        {
            fallbackText = text;
            if (avatarElement != null)
            {
                avatarElement.SetFallbackText(text);
            }
        }
        
        public void SetShape(AvatarShape newShape)
        {
            shape = newShape;
            if (avatarElement != null)
            {
                avatarElement.SetShape(newShape);
            }
        }
        
        public void SetSize(AvatarSize newSize)
        {
            size = newSize;
            if (avatarElement != null)
            {
                avatarElement.SetSize(newSize);
            }
        }
        
        public void SetGrayscale(bool enabled)
        {
            useGrayscale = enabled;
            if (avatarElement != null)
            {
                avatarElement.SetGrayscale(enabled);
            }
        }
        
        public void SetRing(bool enabled, Color color)
        {
            showRing = enabled;
            ringColor = color;
            if (avatarElement != null)
            {
                avatarElement.SetRing(enabled, color);
            }
        }
    }
}
