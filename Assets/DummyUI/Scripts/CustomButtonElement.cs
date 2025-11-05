using System;
using UnityEngine;
using UnityEngine.UIElements;

namespace DummyUI
{
    public class CustomButtonElement : VisualElement
    {
        public enum ButtonState
        {
            Normal,
            Hover,
            Pressed,
            Disabled
        }
        
        public event Action OnButtonClick;
        
        private Label textLabel;
        private VisualElement iconElement;
        private VisualElement backgroundElement;
        
        private ButtonState currentState = ButtonState.Normal;
        private bool isInteractable = true;
        
        // 颜色设置
        private Color normalColor = Color.white;
        private Color hoverColor = Color.gray;
        private Color pressedColor = new Color(0.4f, 0.4f, 0.4f, 1f);
        private Color disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        
        public new class UxmlFactory : UxmlFactory<CustomButtonElement, UxmlTraits> { }
        
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlStringAttributeDescription m_Text = new UxmlStringAttributeDescription { name = "text", defaultValue = "Button" };
            UxmlBoolAttributeDescription m_Interactable = new UxmlBoolAttributeDescription { name = "interactable", defaultValue = true };
            
            public override void Init(VisualElement ve, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(ve, bag, cc);
                var button = ve as CustomButtonElement;
                button?.SetText(m_Text.GetValueFromBag(bag, cc));
                button?.SetInteractable(m_Interactable.GetValueFromBag(bag, cc));
            }
        }
        
        public CustomButtonElement()
        {
            // 设置基本样式类
            AddToClassList("custom-button");
            
            // 创建背景元素
            backgroundElement = new VisualElement();
            backgroundElement.AddToClassList("custom-button-background");
            Add(backgroundElement);
            
            // 创建内容容器
            var contentContainer = new VisualElement();
            contentContainer.AddToClassList("custom-button-content");
            backgroundElement.Add(contentContainer);
            
            // 创建图标元素
            iconElement = new VisualElement();
            iconElement.AddToClassList("custom-button-icon");
            iconElement.style.display = DisplayStyle.None; // 默认隐藏
            contentContainer.Add(iconElement);
            
            // 创建文本标签
            textLabel = new Label("Button");
            textLabel.AddToClassList("custom-button-text");
            contentContainer.Add(textLabel);
            
            // 注册事件
            RegisterCallbacks();
            
            // 设置初始状态
            UpdateVisualState();
        }
        
        public void Initialize(string text, Texture2D icon, Color normal, Color hover, Color pressed, Color disabled, bool interactable)
        {
            SetText(text);
            SetIcon(icon);
            SetNormalColor(normal);
            SetHoverColor(hover);
            SetPressedColor(pressed);
            SetDisabledColor(disabled);
            SetInteractable(interactable);
        }
        
        private void RegisterCallbacks()
        {
            RegisterCallback<ClickEvent>(OnClick);
            RegisterCallback<MouseEnterEvent>(OnMouseEnter);
            RegisterCallback<MouseLeaveEvent>(OnMouseLeave);
            RegisterCallback<MouseDownEvent>(OnMouseDown);
            RegisterCallback<MouseUpEvent>(OnMouseUp);
        }
        
        private void OnClick(ClickEvent evt)
        {
            if (!isInteractable) return;
            OnButtonClick?.Invoke();
        }
        
        private void OnMouseEnter(MouseEnterEvent evt)
        {
            if (!isInteractable) return;
            if (currentState == ButtonState.Normal)
            {
                SetState(ButtonState.Hover);
            }
        }
        
        private void OnMouseLeave(MouseLeaveEvent evt)
        {
            if (!isInteractable) return;
            if (currentState == ButtonState.Hover || currentState == ButtonState.Pressed)
            {
                SetState(ButtonState.Normal);
            }
        }
        
        private void OnMouseDown(MouseDownEvent evt)
        {
            if (!isInteractable) return;
            SetState(ButtonState.Pressed);
        }
        
        private void OnMouseUp(MouseUpEvent evt)
        {
            if (!isInteractable) return;
            SetState(ButtonState.Hover);
        }
        
        private void SetState(ButtonState newState)
        {
            if (currentState == newState) return;
            
            // 移除旧状态样式
            RemoveFromClassList($"custom-button--{currentState.ToString().ToLower()}");
            
            currentState = newState;
            
            // 添加新状态样式
            AddToClassList($"custom-button--{currentState.ToString().ToLower()}");
            
            UpdateVisualState();
        }
        
        private void UpdateVisualState()
        {
            Color targetColor = currentState switch
            {
                ButtonState.Normal => normalColor,
                ButtonState.Hover => hoverColor,
                ButtonState.Pressed => pressedColor,
                ButtonState.Disabled => disabledColor,
                _ => normalColor
            };
            
            backgroundElement.style.backgroundColor = targetColor;
        }
        
        public void SetText(string text)
        {
            if (textLabel != null)
            {
                textLabel.text = text;
                textLabel.style.display = string.IsNullOrEmpty(text) ? DisplayStyle.None : DisplayStyle.Flex;
            }
        }
        
        public void SetIcon(Texture2D icon)
        {
            if (iconElement != null)
            {
                if (icon != null)
                {
                    iconElement.style.backgroundImage = new StyleBackground(icon);
                    iconElement.style.display = DisplayStyle.Flex;
                }
                else
                {
                    iconElement.style.backgroundImage = StyleKeyword.None;
                    iconElement.style.display = DisplayStyle.None;
                }
            }
        }
        
        public void SetNormalColor(Color color)
        {
            normalColor = color;
            if (currentState == ButtonState.Normal)
                UpdateVisualState();
        }
        
        public void SetHoverColor(Color color)
        {
            hoverColor = color;
            if (currentState == ButtonState.Hover)
                UpdateVisualState();
        }
        
        public void SetPressedColor(Color color)
        {
            pressedColor = color;
            if (currentState == ButtonState.Pressed)
                UpdateVisualState();
        }
        
        public void SetDisabledColor(Color color)
        {
            disabledColor = color;
            if (currentState == ButtonState.Disabled)
                UpdateVisualState();
        }
        
        public void SetInteractable(bool interactable)
        {
            isInteractable = interactable;
            
            if (!interactable)
            {
                SetState(ButtonState.Disabled);
            }
            else if (currentState == ButtonState.Disabled)
            {
                SetState(ButtonState.Normal);
            }
            
            SetEnabled(interactable);
        }
    }
}