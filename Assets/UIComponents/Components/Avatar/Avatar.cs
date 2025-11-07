using UnityEngine;
using UnityEngine.UIElements;

namespace DummyUI
{
    /// <summary>
    /// 基于 UI Toolkit 的头像组件，融合了外观与状态控制。
    /// </summary>
    public class AvatarElement : VisualElement
    {
        public enum Shape
        {
            Circle,
            RoundedSquare
        }

        public enum Size
        {
            Small,
            Medium,
            Large
        }

        private readonly VisualElement imageContainer;
        private readonly VisualElement imageElement;
        private readonly Label fallbackLabel;

        private Texture2D currentTexture;
        private string fallbackText = "??";
        private bool imageLoaded;

        // 使用 USS 变量 (CustomStyleProperty) 来控制某些可样式化属性。
        // 这样可以在样式表（.uss）中定义变量，例如:
        // .avatar { --avatar-ring-color: #ffffff; --avatar-fallback-bg: #646464; }

        private static readonly CustomStyleProperty<Color> s_RingColor = new CustomStyleProperty<Color>("--avatar-ring-color");
        private static readonly CustomStyleProperty<Color> s_FallbackBg = new CustomStyleProperty<Color>("--avatar-fallback-bg");
        private static readonly CustomStyleProperty<Color> s_FallbackColor = new CustomStyleProperty<Color>("--avatar-fallback-color");

        // Register a callback to read custom style variables when they are resolved.
        private void OnCustomStyleResolvedCallback(CustomStyleResolvedEvent evt)
        {
            var customStyle = evt.customStyle;

            if (customStyle.TryGetValue(s_RingColor, out var ringColor))
            {
                style.borderTopColor = ringColor;
                style.borderBottomColor = ringColor;
                style.borderLeftColor = ringColor;
                style.borderRightColor = ringColor;
            }

            if (customStyle.TryGetValue(s_FallbackBg, out var fbBg))
            {
                fallbackLabel.style.backgroundColor = fbBg;
            }

            if (customStyle.TryGetValue(s_FallbackColor, out var fbColor))
            {
                fallbackLabel.style.color = fbColor;
            }
        }

        // UXML 属性：允许在 UI Builder 的 Attributes 面板中编辑并序列化 ring-color 到 UXML。
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly UxmlColorAttributeDescription m_RingColor =
                new UxmlColorAttributeDescription { name = "ring-color", defaultValue = UnityEngine.Color.white };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                if (ve is AvatarElement avatar)
                {
                    var ringColor = m_RingColor.GetValueFromBag(bag, cc);
                    // 将从 UXML 获得的 ring-color 应用为内联边框颜色（这会覆盖样式表变量）
                    avatar.style.borderTopColor = ringColor;
                    avatar.style.borderBottomColor = ringColor;
                    avatar.style.borderLeftColor = ringColor;
                    avatar.style.borderRightColor = ringColor;
                }
            }
        }

        public new class UxmlFactory : UxmlFactory<AvatarElement, UxmlTraits> { }

        public AvatarElement()
        {
            AddToClassList("avatar");

            imageContainer = new VisualElement();
            imageContainer.AddToClassList("avatar-container");
            Add(imageContainer);

            imageElement = new VisualElement();
            imageElement.AddToClassList("avatar-image");
            imageElement.style.display = DisplayStyle.None;
            imageContainer.Add(imageElement);

            fallbackLabel = new Label(fallbackText);
            fallbackLabel.AddToClassList("avatar-fallback");
            imageContainer.Add(fallbackLabel);

            // 监听自定义样式解析完成事件，用于读取 USS 中的 --var 变量
            RegisterCallback<CustomStyleResolvedEvent>(OnCustomStyleResolvedCallback);
        }

        public void SetImage(Texture2D texture)
        {
            currentTexture = texture;
            imageLoaded = texture != null;

            if (imageLoaded)
            {
                imageElement.style.backgroundImage = new StyleBackground(texture);
            }

            UpdateDisplay();
        }

        public void ClearImage()
        {
            currentTexture = null;
            imageLoaded = false;
            imageElement.style.backgroundImage = StyleKeyword.None;
            UpdateDisplay();
        }

        public void SetFallbackText(string text)
        {
            fallbackText = string.IsNullOrEmpty(text) ? "??" : text;
            fallbackLabel.text = fallbackText;
        }

        public void SetShape(Shape shape)
        {
            RemoveFromClassList("avatar-circle");
            RemoveFromClassList("avatar-rounded");

            switch (shape)
            {
                case Shape.Circle:
                    AddToClassList("avatar-circle");
                    break;
                case Shape.RoundedSquare:
                    AddToClassList("avatar-rounded");
                    break;
            }
        }

        public void SetSize(Size size)
        {
            RemoveFromClassList("avatar-small");
            RemoveFromClassList("avatar-medium");
            RemoveFromClassList("avatar-large");

            switch (size)
            {
                case Size.Small:
                    AddToClassList("avatar-small");
                    break;
                case Size.Medium:
                    AddToClassList("avatar-medium");
                    break;
                case Size.Large:
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
                style.borderTopColor = color;
                style.borderBottomColor = color;
                style.borderLeftColor = color;
                style.borderRightColor = color;
            }
            else
            {
                RemoveFromClassList("avatar-ring");
            }
        }

        private void UpdateDisplay()
        {
            if (imageLoaded && currentTexture != null)
            {
                imageElement.style.display = DisplayStyle.Flex;
                fallbackLabel.style.display = DisplayStyle.None;
            }
            else
            {
                imageElement.style.display = DisplayStyle.None;
                fallbackLabel.style.display = DisplayStyle.Flex;
            }
        }
    }
}
