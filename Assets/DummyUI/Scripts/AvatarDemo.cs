using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Networking;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DummyUI
{
    /// <summary>
    /// Demo script showing different Avatar component variants
    /// Demonstrates: Circle, Rounded, and Stacked avatars with image loading
    /// </summary>
    public class AvatarDemo : MonoBehaviour
    {
        [Header("UI Settings")]
        [SerializeField] private UIDocument uiDocument;
        
    [Header("Avatar Image URLs")]
    [SerializeField] private string avatar1Url = "https://avatars.githubusercontent.com/u/123456789";
    [SerializeField] private string avatar2Url = "https://avatars.githubusercontent.com/u/123456789";
    [SerializeField] private string avatar3Url = "https://avatars.githubusercontent.com/u/123456789";
        
        private VisualElement root;
        private List<AvatarElement> avatarElements = new List<AvatarElement>();
        
        private void Start()
        {
            if (uiDocument == null)
            {
                uiDocument = GetComponent<UIDocument>();
            }
            
            if (uiDocument == null)
            {
                Debug.LogError("[AvatarDemo] UIDocument not found!");
                return;
            }
            
            root = uiDocument.rootVisualElement;
            
            // Load USS styles
            LoadAvatarStyles();
            
            CreateAvatarDemo();
            
            // Load images for all avatars
            StartCoroutine(LoadAllAvatarImages());
        }
        
        private void LoadAvatarStyles()
        {
            // Load Avatar USS from Resources
            var uss = Resources.Load<StyleSheet>("UI/Avatar");
            if (uss != null)
            {
                root.styleSheets.Add(uss);
                Debug.Log("[AvatarDemo] Avatar.uss loaded successfully");
            }
            else
            {
                Debug.LogWarning("[AvatarDemo] Failed to load Avatar.uss from Resources/UI/Avatar");
            }
        }
        
        private void CreateAvatarDemo()
        {
            // Clear existing content
            root.Clear();
            avatarElements.Clear();
            
            // Create main container
            var container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;
            container.style.flexWrap = Wrap.Wrap;
            container.style.alignItems = Align.Center;
            container.style.paddingTop = 20;
            container.style.paddingLeft = 20;
            container.style.paddingRight = 20;
            container.style.paddingBottom = 20;
            root.Add(container);
            
            // 1. Default Circle Avatar (will load image from URL)
            var avatar1 = CreateAvatar(
                fallbackText: "CN",
                shape: Avatar.AvatarShape.Circle,
                size: Avatar.AvatarSize.Medium,
                colorVariant: "avatar-primary",
                imageUrl: avatar1Url
            );
            container.Add(avatar1);
            avatarElements.Add(avatar1);
            
            AddSpacer(container, 48);
            
            // 2. Rounded Square Avatar (will load image from URL)
            var avatar2 = CreateAvatar(
                fallbackText: "ER",
                shape: Avatar.AvatarShape.RoundedSquare,
                size: Avatar.AvatarSize.Medium,
                colorVariant: "avatar-secondary",
                imageUrl: avatar2Url
            );
            container.Add(avatar2);
            avatarElements.Add(avatar2);
            
            AddSpacer(container, 48);
            
            // 3. Stacked Avatars Group (will load images from URLs)
            var stackedGroup = CreateStackedAvatarGroup();
            container.Add(stackedGroup);
            
            Debug.Log("[AvatarDemo] Avatar demo created successfully!");
        }
        
        private AvatarElement CreateAvatar(
            string fallbackText, 
            Avatar.AvatarShape shape, 
            Avatar.AvatarSize size,
            string colorVariant = "avatar-primary",
            string imageUrl = null)
        {
            var avatarElement = new AvatarElement();
            avatarElement.SetFallbackText(fallbackText);
            avatarElement.SetShape(shape);
            avatarElement.SetSize(size);
            avatarElement.AddToClassList(colorVariant);
            
            // Store the image URL for later loading
            avatarElement.userData = imageUrl;
            
            return avatarElement;
        }
        
        private IEnumerator LoadAllAvatarImages()
        {
            Debug.Log("[AvatarDemo] Starting to load avatar images...");
            
            int loadedCount = 0;
            int failedCount = 0;
            
            foreach (var avatarElement in avatarElements)
            {
                string url = avatarElement.userData as string;
                if (!string.IsNullOrEmpty(url))
                {
                    yield return StartCoroutine(LoadAvatarImage(avatarElement, url));
                    
                    if (avatarElement.userData is Texture2D)
                    {
                        loadedCount++;
                    }
                    else
                    {
                        failedCount++;
                    }
                }
            }
            
            Debug.Log($"[AvatarDemo] Image loading complete. Loaded: {loadedCount}, Failed: {failedCount}");
        }
        
        private IEnumerator LoadAvatarImage(AvatarElement avatarElement, string url)
        {
            Debug.Log($"[AvatarDemo] ========================================");
            Debug.Log($"[AvatarDemo] Starting to load image from: {url}");
            Debug.Log($"[AvatarDemo] Fallback text: {avatarElement.userData ?? "N/A"}");
            
            using (UnityWebRequest request = AvatarRequestUtility.CreateTextureRequest(url))
            {
                // Send request
                var operation = request.SendWebRequest();
                
                // Wait for completion
                yield return operation;
                
                Debug.Log($"[AvatarDemo] Request completed.");
                Debug.Log($"[AvatarDemo] Result: {request.result}");
                Debug.Log($"[AvatarDemo] Response Code: {request.responseCode}");
                Debug.Log($"[AvatarDemo] Error: {request.error ?? "None"}");
                
                if (request.result == UnityWebRequest.Result.Success)
                {
                    var data = request.downloadHandler?.data;
                    TryApplyTextureData(data, avatarElement, url, "[AvatarDemo]");
                }
                else
                {
                    Debug.LogError($"[AvatarDemo] ✗ Failed to load image!");
                    Debug.LogError($"[AvatarDemo]   URL: {url}");
                    Debug.LogError($"[AvatarDemo]   Result: {request.result}");
                    Debug.LogError($"[AvatarDemo]   Error: {request.error}");
                    Debug.LogError($"[AvatarDemo]   Response Code: {request.responseCode}");
                    
                    // Try to get more info
                    if (request.downloadHandler != null)
                    {
                        Debug.LogError($"[AvatarDemo]   Downloaded bytes: {request.downloadHandler.data?.Length ?? 0}");
                    }

                    if (ShouldFallback(request))
                    {
                        yield return TryFallbackDownload(avatarElement, url);
                    }
                    
                    // Fallback text will be shown automatically
                }
            }
            
            Debug.Log($"[AvatarDemo] ========================================");
        }

        private bool TryApplyTextureData(byte[] data, AvatarElement avatarElement, string url, string logPrefix)
        {
            if (data == null || data.Length == 0)
            {
                Debug.LogError($"{logPrefix} ✗ Request succeeded but no data received! ({url})");
                return false;
            }

            Texture2D texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (texture.LoadImage(data))
            {
                Debug.Log($"{logPrefix} ✓ Texture loaded successfully!");
                Debug.Log($"{logPrefix}   Size: {texture.width}x{texture.height}");
                Debug.Log($"{logPrefix}   Format: {texture.format}");

                avatarElement.SetImage(texture);
                avatarElement.userData = texture;
                return true;
            }

            Debug.LogError($"{logPrefix} ✗ Failed to decode texture data! ({url})");
            Debug.LogError($"{logPrefix}   Data Length: {data.Length}");
            return false;
        }

        private static bool ShouldFallback(UnityWebRequest request)
        {
            return request.result == UnityWebRequest.Result.ConnectionError || request.responseCode == 0;
        }

        private IEnumerator TryFallbackDownload(AvatarElement avatarElement, string url)
        {
            Debug.Log("[AvatarDemo][Fallback] Attempting HttpClient download...");

            Task<byte[]> task = AvatarRequestUtility.DownloadWithHttpClientAsync(url);
            while (!task.IsCompleted)
            {
                yield return null;
            }

            if (task.IsCanceled)
            {
                Debug.LogWarning("[AvatarDemo][Fallback] HttpClient task canceled.");
                yield break;
            }

            if (task.IsFaulted)
            {
                Debug.LogError($"[AvatarDemo][Fallback] HttpClient task faulted: {task.Exception?.GetBaseException().Message}");
                yield break;
            }

            var data = task.Result;
            if (!TryApplyTextureData(data, avatarElement, url, "[AvatarDemo][Fallback]"))
            {
                Debug.LogError("[AvatarDemo][Fallback] ✗ Failed to decode data from HttpClient.");
            }
        }
        private VisualElement CreateStackedAvatarGroup()
        {
            var group = new VisualElement();
            group.AddToClassList("avatar-group");
            group.style.flexDirection = FlexDirection.Row;
            group.style.alignItems = Align.Center;
            
            // Avatar 1 - CN (will load image)
            var avatar1 = new AvatarElement();
            avatar1.SetFallbackText("CN");
            avatar1.SetShape(Avatar.AvatarShape.Circle);
            avatar1.SetSize(Avatar.AvatarSize.Medium);
            avatar1.SetGrayscale(true);
            avatar1.SetRing(true, Color.white);
            avatar1.AddToClassList("avatar-primary");
            avatar1.style.marginLeft = 0;
            avatar1.userData = avatar1Url;
            group.Add(avatar1);
            avatarElements.Add(avatar1);
            
            // Avatar 2 - LR (will load image)
            var avatar2 = new AvatarElement();
            avatar2.SetFallbackText("LR");
            avatar2.SetShape(Avatar.AvatarShape.Circle);
            avatar2.SetSize(Avatar.AvatarSize.Medium);
            avatar2.SetGrayscale(true);
            avatar2.SetRing(true, Color.white);
            avatar2.AddToClassList("avatar-success");
            avatar2.style.marginLeft = -8; // Overlap
            avatar2.userData = avatar3Url; // maxleiter
            group.Add(avatar2);
            avatarElements.Add(avatar2);
            
            // Avatar 3 - ER (will load image)
            var avatar3 = new AvatarElement();
            avatar3.SetFallbackText("ER");
            avatar3.SetShape(Avatar.AvatarShape.Circle);
            avatar3.SetSize(Avatar.AvatarSize.Medium);
            avatar3.SetGrayscale(true);
            avatar3.SetRing(true, Color.white);
            avatar3.AddToClassList("avatar-danger");
            avatar3.style.marginLeft = -8; // Overlap
            avatar3.userData = avatar2Url; // evilrabbit
            group.Add(avatar3);
            avatarElements.Add(avatar3);
            
            return group;
        }
        
        private void AddSpacer(VisualElement parent, float width)
        {
            var spacer = new VisualElement();
            spacer.style.width = width;
            spacer.style.height = 1;
            parent.Add(spacer);
        }
        
        // Context menu test methods
        [ContextMenu("Test Avatar Variants")]
        public void TestAvatarVariants()
        {
            Debug.Log("=== Avatar Variants Test ===");
            Debug.Log("1. Circle Avatar (Default)");
            Debug.Log("2. Rounded Square Avatar");
            Debug.Log("3. Stacked Avatars with Grayscale + Ring");
        }
        
        [ContextMenu("Reload Demo")]
        public void ReloadDemo()
        {
            if (root != null)
            {
                CreateAvatarDemo();
                Debug.Log("[AvatarDemo] Demo reloaded!");
            }
        }
    }
}
