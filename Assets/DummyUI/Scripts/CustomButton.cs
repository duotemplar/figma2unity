using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

namespace DummyUI
{
    public class CustomButton : MonoBehaviour
    {
        [System.Serializable]
        public class ButtonClickEvent : UnityEvent { }
        
        [Header("Button Settings")]
        [SerializeField] private string buttonText = "Button";
        [SerializeField] private Texture2D buttonIcon;
        [SerializeField] private Color normalColor = Color.white;
        [SerializeField] private Color hoverColor = Color.gray;
        [SerializeField] private Color pressedColor = new Color(0.4f, 0.4f, 0.4f, 1f);
        [SerializeField] private Color disabledColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        [SerializeField] private bool interactable = true;
        
        [Header("Events")]
        [SerializeField] private ButtonClickEvent onClick = new ButtonClickEvent();
        
        private CustomButtonElement buttonElement;
        private UIDocument uiDocument;
        
        // 公开属性用于代码访问
        public string ButtonText
        {
            get => buttonText;
            set
            {
                buttonText = value;
                if (buttonElement != null)
                    buttonElement.SetText(buttonText);
            }
        }
        
        public Texture2D ButtonIcon
        {
            get => buttonIcon;
            set
            {
                buttonIcon = value;
                if (buttonElement != null)
                    buttonElement.SetIcon(buttonIcon);
            }
        }
        
        public Color NormalColor
        {
            get => normalColor;
            set
            {
                normalColor = value;
                if (buttonElement != null)
                    buttonElement.SetNormalColor(normalColor);
            }
        }
        
        public Color HoverColor
        {
            get => hoverColor;
            set
            {
                hoverColor = value;
                if (buttonElement != null)
                    buttonElement.SetHoverColor(hoverColor);
            }
        }
        
        public Color PressedColor
        {
            get => pressedColor;
            set
            {
                pressedColor = value;
                if (buttonElement != null)
                    buttonElement.SetPressedColor(pressedColor);
            }
        }
        
        public Color DisabledColor
        {
            get => disabledColor;
            set
            {
                disabledColor = value;
                if (buttonElement != null)
                    buttonElement.SetDisabledColor(disabledColor);
            }
        }
        
        public bool Interactable
        {
            get => interactable;
            set
            {
                interactable = value;
                if (buttonElement != null)
                    buttonElement.SetInteractable(interactable);
            }
        }
        
        public ButtonClickEvent OnClick => onClick;
        
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
            InitializeButton();
        }
        
        private void InitializeButton()
        {
            if (uiDocument.visualTreeAsset == null)
            {
                // 尝试加载 UXML 资源
                var uxml = Resources.Load<VisualTreeAsset>("UI/CustomButton");
                if (uxml == null)
                {
                    Debug.LogError("CustomButton UXML not found in Resources/UI/CustomButton");
                    return;
                }
                uiDocument.visualTreeAsset = uxml;
            }
            
            // 创建自定义按钮元素
            buttonElement = new CustomButtonElement();
            buttonElement.Initialize(buttonText, buttonIcon, normalColor, hoverColor, pressedColor, disabledColor, interactable);
            
            // 绑定点击事件
            buttonElement.OnButtonClick += () => onClick?.Invoke();
            
            // 添加到 UI Document 的根元素
            var root = uiDocument.rootVisualElement;
            root.Clear();
            root.Add(buttonElement);
        }
        
        private void OnValidate()
        {
            if (Application.isPlaying && buttonElement != null)
            {
                buttonElement.SetText(buttonText);
                buttonElement.SetIcon(buttonIcon);
                buttonElement.SetNormalColor(normalColor);
                buttonElement.SetHoverColor(hoverColor);
                buttonElement.SetPressedColor(pressedColor);
                buttonElement.SetDisabledColor(disabledColor);
                buttonElement.SetInteractable(interactable);
            }
        }
    }
}