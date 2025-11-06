using UnityEngine;
using UnityEngine.UIElements;

namespace DummyUI
{
    /// <summary>
    /// Custom VisualElement for Avatar UI
    /// Handles image display and fallback text rendering
    /// </summary>
    public class AvatarElement : VisualElement
    {
        private VisualElement imageContainer;
        private VisualElement imageElement;
        private Label fallbackLabel;
        
        private Texture2D currentTexture;
        private string fallbackText = "??";
        private bool imageLoaded = false;
        
        public new class UxmlFactory : UxmlFactory<AvatarElement, UxmlTraits> { }
        
        public AvatarElement()
        {
            // Add base avatar class
            AddToClassList("avatar");
            
            // Create image container
            imageContainer = new VisualElement();
            imageContainer.AddToClassList("avatar-container");
            Add(imageContainer);
            
            // Create image element
            imageElement = new VisualElement();
            imageElement.AddToClassList("avatar-image");
            imageElement.style.display = DisplayStyle.None; // Hidden until image loads
            imageContainer.Add(imageElement);
            
            // Create fallback label
            fallbackLabel = new Label(fallbackText);
            fallbackLabel.AddToClassList("avatar-fallback");
            imageContainer.Add(fallbackLabel);
        }
        
        public void SetImage(Texture2D texture)
        {
            if (texture == null)
            {
                Debug.LogWarning("[AvatarElement] SetImage called with null texture");
                imageLoaded = false;
                UpdateDisplay();
                return;
            }
            
            Debug.Log($"[AvatarElement] SetImage called with texture: {texture.name}, size: {texture.width}x{texture.height}");
            currentTexture = texture;
            imageElement.style.backgroundImage = new StyleBackground(texture);
            imageLoaded = true;
            UpdateDisplay();
            
            Debug.Log($"[AvatarElement] Image display updated. imageLoaded={imageLoaded}, imageElement.display={imageElement.style.display.value}");
        }
        
        public void SetFallbackText(string text)
        {
            fallbackText = string.IsNullOrEmpty(text) ? "??" : text;
            fallbackLabel.text = fallbackText;
        }
        
        public void SetShape(Avatar.AvatarShape shape)
        {
            // Remove existing shape classes
            RemoveFromClassList("avatar-circle");
            RemoveFromClassList("avatar-rounded");
            
            // Add new shape class
            switch (shape)
            {
                case Avatar.AvatarShape.Circle:
                    AddToClassList("avatar-circle");
                    break;
                case Avatar.AvatarShape.RoundedSquare:
                    AddToClassList("avatar-rounded");
                    break;
            }
        }
        
        public void SetSize(Avatar.AvatarSize size)
        {
            // Remove existing size classes
            RemoveFromClassList("avatar-small");
            RemoveFromClassList("avatar-medium");
            RemoveFromClassList("avatar-large");
            
            // Add new size class
            switch (size)
            {
                case Avatar.AvatarSize.Small:
                    AddToClassList("avatar-small");
                    break;
                case Avatar.AvatarSize.Medium:
                    AddToClassList("avatar-medium");
                    break;
                case Avatar.AvatarSize.Large:
                    AddToClassList("avatar-large");
                    break;
            }
        }
        
        public void SetGrayscale(bool enabled)
        {
            if (enabled)
            {
                imageElement.AddToClassList("avatar-grayscale");
            }
            else
            {
                imageElement.RemoveFromClassList("avatar-grayscale");
            }
        }
        
        public void SetRing(bool enabled, Color color)
        {
            if (enabled)
            {
                AddToClassList("avatar-ring");
                style.borderTopColor = new StyleColor(color);
                style.borderBottomColor = new StyleColor(color);
                style.borderLeftColor = new StyleColor(color);
                style.borderRightColor = new StyleColor(color);
            }
            else
            {
                RemoveFromClassList("avatar-ring");
            }
        }
        
        private void UpdateDisplay()
        {
            if (imageLoaded)
            {
                // Show image, hide fallback
                imageElement.style.display = DisplayStyle.Flex;
                fallbackLabel.style.display = DisplayStyle.None;
                Debug.Log($"[AvatarElement] Displaying image for '{fallbackText}'");
            }
            else
            {
                // Show fallback, hide image
                imageElement.style.display = DisplayStyle.None;
                fallbackLabel.style.display = DisplayStyle.Flex;
                Debug.Log($"[AvatarElement] Displaying fallback text '{fallbackText}'");
            }
        }
    }
}
