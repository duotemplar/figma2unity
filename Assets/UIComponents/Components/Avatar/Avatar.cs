using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace DummyUI
{
    /// <summary>
    /// 基于 UI Toolkit 的头像组件，融合了外观与状态控制。
    /// </summary>
    public class AvatarElement : VisualElement
    {
        // ========== Stylesheet loading & caching ==========
        private static readonly Dictionary<string, StyleSheet> s_StyleCache = new Dictionary<string, StyleSheet>();
        private StyleSheet _attachedSheet;

        private static StyleSheet GetStyleFromResources(string key)
        {
            if (string.IsNullOrEmpty(key)) key = "Avatar";

            if (s_StyleCache.TryGetValue(key, out var cached) && cached != null)
                return cached;

            // Resources path convention: Assets/Resources/UI/{key}.uss
            var sheet = Resources.Load<StyleSheet>("UI/" + key);
            if (sheet != null)
            {
                s_StyleCache[key] = sheet;
            }
            return sheet;
        }

        private void AttachStyle(string key)
        {
            var sheet = GetStyleFromResources(key);
            if (sheet == null)
                return;

            // Remove previous sheet if any
            if (_attachedSheet != null)
            {
                try { styleSheets.Remove(_attachedSheet); } catch { /* ignore */ }
            }

            _attachedSheet = sheet;
            try { styleSheets.Add(sheet); } catch { /* ignore */ }
        }

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

        private readonly Image avatarImage;

        private bool ringEnabled;
        private Color ringColor = Color.white;
    // 当通过 SetRingColor 显式设置过颜色后，优先使用该颜色，不被样式变量覆盖
    private bool _ringColorExplicit = false;
        private bool hasResolvedRingColor;
        private Color resolvedRingColor;
        // 当通过 SetCornerRadius 显式设置过圆角后，优先使用该值，不被 shape/USS 覆盖
        private bool _cornerRadiusExplicit = false;

        private static readonly CustomStyleProperty<Color> s_RingColor = new CustomStyleProperty<Color>("--avatar-ring-color");

        public new class UxmlFactory : UxmlFactory<AvatarElement, UxmlTraits> { }

        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            private readonly Image.UxmlTraits imageTraits = new Image.UxmlTraits();

            private readonly UxmlEnumAttributeDescription<Shape> m_Shape =
                new UxmlEnumAttributeDescription<Shape> { name = "shape", defaultValue = Shape.Circle };

            private readonly UxmlEnumAttributeDescription<Size> m_Size =
                new UxmlEnumAttributeDescription<Size> { name = "size", defaultValue = Size.Medium };

            private readonly UxmlBoolAttributeDescription m_Grayscale =
                new UxmlBoolAttributeDescription { name = "grayscale", defaultValue = false };

            private readonly UxmlBoolAttributeDescription m_Ring =
                new UxmlBoolAttributeDescription { name = "ring", defaultValue = false };

            private readonly UxmlColorAttributeDescription m_RingColor =
                new UxmlColorAttributeDescription { name = "ring-color", defaultValue = Color.white };

            private readonly UxmlFloatAttributeDescription m_BorderRadius =
                new UxmlFloatAttributeDescription { name = "border-radius", defaultValue = -1f };

            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                Debug.LogWarning("Component init");
                base.Init(ve, bag, cc);

                if (ve is not AvatarElement avatar)
                {
                    return;
                }

                imageTraits.Init(avatar.avatarImage, bag, cc);

                avatar.SetShape(m_Shape.GetValueFromBag(bag, cc));
                avatar.SetSize(m_Size.GetValueFromBag(bag, cc));
                avatar.SetGrayscale(m_Grayscale.GetValueFromBag(bag, cc));

                // 先设置颜色，再启用 ring，保证 inspector 修改 ring-color 时即时生效
                var ringColorValue = m_RingColor.GetValueFromBag(bag, cc);
                avatar.SetRingColor(ringColorValue);
                avatar.SetRing(m_Ring.GetValueFromBag(bag, cc));

                // 可选：显式圆角（像素）。当 >= 0 时，覆盖 shape 的 USS 圆角
                var radius = m_BorderRadius.GetValueFromBag(bag, cc);
                if (radius >= 0f)
                {
                    //avatar.SetCornerRadius(radius);
                }
            }
        }

        public AvatarElement()
        {
            AddToClassList("avatar");

            // 自动附加默认样式，UI Builder 拖入即可生效
            AttachStyle("Avatar");

            avatarImage = new Image
            {
                scaleMode = ScaleMode.ScaleAndCrop
            };
            avatarImage.AddToClassList("avatar-image");
            avatarImage.StretchToParentSize();
            hierarchy.Add(avatarImage);

            SetShape(Shape.Circle);
            SetSize(Size.Medium);

            RegisterCallback<CustomStyleResolvedEvent>(OnCustomStyleResolvedCallback);
        }

        private void OnCustomStyleResolvedCallback(CustomStyleResolvedEvent evt)
        {
            Debug.LogWarning("Resovled");
            if (evt.customStyle.TryGetValue(s_RingColor, out var customRingColor))
            {
                hasResolvedRingColor = true;
                resolvedRingColor = customRingColor;

                // 只有当没有显式设色时，才使用样式变量驱动颜色
                if (ringEnabled && !_ringColorExplicit)
                {
                    ApplyRingColor(customRingColor);
                }
            }
            else
            {
                hasResolvedRingColor = false;
            }
        }

        public void SetImage(Texture2D texture)
        {
            avatarImage.image = texture;
            avatarImage.MarkDirtyRepaint();
        }

        public void ClearImage()
        {
            SetImage(null);
        }

        public void SetShape(Shape shape)
        {
            RemoveFromClassList("avatar-circle");
            RemoveFromClassList("avatar-rounded");

            AddToClassList(shape == Shape.Circle ? "avatar-circle" : "avatar-rounded");
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
                    //style.width = 32;
                    //style.height = 32;
                    break;
                case Size.Medium:
                    AddToClassList("avatar-medium");
                    //style.width = 40;
                    //style.height = 40;
                    break;
                case Size.Large:
                    AddToClassList("avatar-large");
                    //style.width = 64;
                    //style.height = 64;
                    break;
            }
        }

        public void SetGrayscale(bool enabled)
        {
            if (enabled)
            {
                avatarImage.AddToClassList("avatar-image-grayscale");
            }
            else
            {
                avatarImage.RemoveFromClassList("avatar-image-grayscale");
            }
        }

        public void SetRing(bool enabled)
        {
            SetRing(enabled, ringColor);
        }

        public void SetRing(bool enabled, Color color)
        {
            ringEnabled = enabled;
            ringColor = color;

            if (enabled)
            {
                AddToClassList("avatar-ring");
                // 颜色优先级：显式设色 > 样式变量 > 传入值
                var effectiveColor = _ringColorExplicit
                    ? ringColor
                    : (hasResolvedRingColor ? resolvedRingColor : color);
                ApplyRingColor(effectiveColor);
            }
            else
            {
                RemoveFromClassList("avatar-ring");
                style.borderTopWidth = 0f;
                style.borderBottomWidth = 0f;
                style.borderLeftWidth = 0f;
                style.borderRightWidth = 0f;

                style.borderTopColor = StyleKeyword.Null;
                style.borderBottomColor = StyleKeyword.Null;
                style.borderLeftColor = StyleKeyword.Null;
                style.borderRightColor = StyleKeyword.Null;
            }
        }

        /// <summary>
        /// 单独更新环的颜色（不影响开启/关闭状态）。在 inspector 调整 ring-color 时调用。
        /// </summary>
        public void SetRingColor(Color color)
        {
            ringColor = color;
            _ringColorExplicit = true;
            if (ringEnabled)
            {
                ApplyRingColor(color);
            }
        }

        private void ApplyRingColor(Color color)
        {
            style.borderTopColor = color;
            style.borderBottomColor = color;
            style.borderLeftColor = color;
            style.borderRightColor = color;

            style.borderTopWidth = 2f;
            style.borderBottomWidth = 2f;
            style.borderLeftWidth = 2f;
            style.borderRightWidth = 2f;
        }

        /// <summary>
        /// 显式设置四个角的圆角（像素），覆盖 USS/shape 规则。
        /// </summary>
        public void SetCornerRadius(float pixels)
        {
            if (pixels < 0f)
            {
                ClearCornerRadius();
                return;
            }

            _cornerRadiusExplicit = true;
            var length = new Length(pixels, LengthUnit.Pixel);
            style.borderTopLeftRadius = length;
            style.borderTopRightRadius = length;
            style.borderBottomLeftRadius = length;
            style.borderBottomRightRadius = length;
        }

        /// <summary>
        /// 清除显式圆角，回退到 shape/USS 控制。
        /// </summary>
        public void ClearCornerRadius()
        {
            _cornerRadiusExplicit = false;
            style.borderTopLeftRadius = StyleKeyword.Null;
            style.borderTopRightRadius = StyleKeyword.Null;
            style.borderBottomLeftRadius = StyleKeyword.Null;
            style.borderBottomRightRadius = StyleKeyword.Null;
        }

        /// <summary>
        /// 切换样式变体。资源路径约定：Assets/Resources/UI/Avatar_{variant}.uss
        /// 例如 SetStyleVariant("Dark") 将加载 UI/Avatar_Dark。
        /// 传入 null/空字符串 时回退到默认 UI/Avatar。
        /// </summary>
        public void SetStyleVariant(string variant)
        {
            if (string.IsNullOrEmpty(variant))
            {
                AttachStyle("Avatar");
            }
            else
            {
                AttachStyle("Avatar_" + variant);
            }
        }
    }
}
