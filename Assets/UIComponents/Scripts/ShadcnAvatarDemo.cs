using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace DummyUI
{
    /// <summary>
    /// Demonstrates shadcn-ui Avatar variations using inspector-assigned textures.
    /// Mirrors the React example with primary, rounded, and stacked avatars.
    /// </summary>
    public class ShadcnAvatarDemo : MonoBehaviour
    {
        [Header("UI Document")]
        [SerializeField] private UIDocument uiDocument;
        
        [Header("Primary Avatar")]
        [SerializeField] private Texture2D primaryTexture;
        [SerializeField] private string primaryFallback = "CN";
        
        [Header("Rounded Avatar")]
        [SerializeField] private Texture2D roundedTexture;
        [SerializeField] private string roundedFallback = "ER";
        
        [Header("Stacked Avatars")]
        [SerializeField] private Texture2D stackedFirstTexture;
        [SerializeField] private string stackedFirstFallback = "CN";
        [SerializeField] private Texture2D stackedSecondTexture;
        [SerializeField] private string stackedSecondFallback = "LR";
        [SerializeField] private Texture2D stackedThirdTexture;
        [SerializeField] private string stackedThirdFallback = "ER";
        [SerializeField] private Color stackedRingColor = Color.white;
        
        private VisualElement root;
        
        private void Start()
        {
            if (uiDocument == null)
            {
                uiDocument = GetComponent<UIDocument>();
            }
            
            if (uiDocument == null)
            {
                Debug.LogError("[ShadcnAvatarDemo] UIDocument not found on GameObject.");
                return;
            }
            
            root = uiDocument.rootVisualElement;
            if (root == null)
            {
                Debug.LogError("[ShadcnAvatarDemo] UIDocument rootVisualElement is null.");
                return;
            }
            
            LoadAvatarStyles();
            CreateDemoLayout();
        }
        
        private void LoadAvatarStyles()
        {
            var avatarStyle = Resources.Load<StyleSheet>("UI/Avatar");
            if (avatarStyle != null)
            {
                root.styleSheets.Add(avatarStyle);
            }
            else
            {
                Debug.LogWarning("[ShadcnAvatarDemo] Failed to load Avatar.uss from Resources/UI/Avatar");
            }
        }
        
        private void CreateDemoLayout()
        {
            root.Clear();
            
            var container = new VisualElement();
            container.style.flexDirection = FlexDirection.Row;
            container.style.flexWrap = Wrap.Wrap;
            container.style.alignItems = Align.Center;
            container.style.paddingLeft = 20;
            container.style.paddingRight = 20;
            container.style.paddingTop = 20;
            container.style.paddingBottom = 20;
            root.Add(container);
            
            // Primary circular avatar
            var primaryAvatar = CreateAvatar(primaryTexture, primaryFallback, AvatarElement.Shape.Circle, "avatar-primary");
            container.Add(primaryAvatar);
            AddSpacer(container, 48);
            
            // Rounded square avatar
            var roundedAvatar = CreateAvatar(roundedTexture, roundedFallback, AvatarElement.Shape.RoundedSquare, "avatar-secondary");
            container.Add(roundedAvatar);
            AddSpacer(container, 48);
            
            // Stacked avatar group
            var stackedGroup = CreateStackedAvatars();
            container.Add(stackedGroup);
        }
        
    private AvatarElement CreateAvatar(Texture2D texture, string fallback, AvatarElement.Shape shape, string colorClass)
        {
            var element = new AvatarElement();
            element.SetFallbackText(fallback);
            element.SetShape(shape);
            element.SetSize(AvatarElement.Size.Medium);
            element.AddToClassList(colorClass);
            
            if (texture != null)
            {
                element.SetImage(texture);
            }
            else
            {
                element.SetImage(null);
            }
            
            return element;
        }
        
        private VisualElement CreateStackedAvatars()
        {
            var group = new VisualElement();
            group.AddToClassList("avatar-group");
            group.style.flexDirection = FlexDirection.Row;
            group.style.alignItems = Align.Center;
            
            var first = CreateStackedAvatar(stackedFirstTexture, stackedFirstFallback, "avatar-primary", 0);
            group.Add(first);
            
            var second = CreateStackedAvatar(stackedSecondTexture, stackedSecondFallback, "avatar-success", -8);
            group.Add(second);
            
            var third = CreateStackedAvatar(stackedThirdTexture, stackedThirdFallback, "avatar-danger", -8);
            group.Add(third);
            
            return group;
        }
        
        private AvatarElement CreateStackedAvatar(Texture2D texture, string fallback, string colorClass, float marginLeft)
        {
            var element = new AvatarElement();
            element.SetFallbackText(fallback);
            element.SetShape(AvatarElement.Shape.Circle);
            element.SetSize(AvatarElement.Size.Medium);
            element.SetGrayscale(true);
            element.SetRing(true, stackedRingColor);
            element.AddToClassList(colorClass);
            
            if (texture != null)
            {
                element.SetImage(texture);
            }
            else
            {
                element.SetImage(null);
            }
            
            element.style.marginLeft = marginLeft;
            return element;
        }
        
        private void AddSpacer(VisualElement parent, float width)
        {
            var spacer = new VisualElement();
            spacer.style.width = width;
            spacer.style.height = 1;
            parent.Add(spacer);
        }
    }
}
