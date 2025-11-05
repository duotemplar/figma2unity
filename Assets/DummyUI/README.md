# Custom UI Toolkit Button

这是一个基于 Unity UI Toolkit 的自定义 Button 组件，支持运行时使用，提供完整的状态管理和事件系统。

## 功能特性

- ✅ **点击事件**: 支持 UnityEvent 回调系统
- ✅ **状态管理**: Normal、Hover、Pressed、Disabled 四种状态
- ✅ **动态属性**: 支持运行时修改文本、图标、颜色
- ✅ **UXML/USS**: 基于 UI Toolkit 的现代 UI 方案
- ✅ **独立组件**: 可在任何场景中独立使用
- ✅ **样式系统**: 丰富的 CSS 样式支持，包括大小变体和主题色彩

## 文件结构

```
Assets/DummyUI/
├── Scripts/
│   ├── CustomButton.cs           # 主要组件脚本
│   ├── CustomButtonElement.cs    # UI Toolkit 自定义元素
│   └── CustomButtonExample.cs    # 使用示例脚本
├── UI/
│   ├── CustomButton.uxml         # UI 结构定义
│   └── CustomButton.uss          # 样式定义
└── Resources/UI/
    └── CustomButton.uxml         # 运行时资源（必需）
```

## 使用方法

### 1. 基本设置

1. 在场景中创建一个 GameObject
2. 添加 `CustomButton` 组件
3. 添加 `UIDocument` 组件（如果没有会自动添加）
4. 运行游戏即可看到按钮

### 2. Inspector 配置

在 Inspector 中可以配置：

- **Button Text**: 按钮显示文本
- **Button Icon**: 按钮图标（Texture2D）
- **颜色设置**: Normal/Hover/Pressed/Disabled 状态的颜色
- **Interactable**: 是否可交互
- **OnClick**: 点击事件回调

### 3. 代码控制

```csharp
// 获取按钮组件
CustomButton button = GetComponent<CustomButton>();

// 修改属性
button.ButtonText = "新文本";
button.ButtonIcon = myTexture2D;
button.NormalColor = Color.blue;
button.Interactable = false;

// 绑定点击事件
button.OnClick.AddListener(() => {
    Debug.Log("按钮被点击了！");
});
```

### 4. 样式定制

#### 基本样式类：
- `.custom-button`: 主按钮样式
- `.custom-button-background`: 背景样式
- `.custom-button-content`: 内容容器
- `.custom-button-icon`: 图标样式
- `.custom-button-text`: 文本样式

#### 状态样式类：
- `.custom-button--normal`: 正常状态
- `.custom-button--hover`: 悬停状态
- `.custom-button--pressed`: 按下状态
- `.custom-button--disabled`: 禁用状态

#### 大小变体：
- `.custom-button--small`: 小尺寸按钮
- `.custom-button--large`: 大尺寸按钮

#### 主题色彩：
- `.custom-button--primary`: 主要按钮（蓝色）
- `.custom-button--success`: 成功按钮（绿色）
- `.custom-button--warning`: 警告按钮（黄色）
- `.custom-button--danger`: 危险按钮（红色）

## 示例代码

查看 `CustomButtonExample.cs` 获取完整的使用示例，包括：

- 动态修改按钮属性
- 事件绑定
- 状态控制
- 运行时演示

## 技术实现

### 架构设计
- `CustomButton`: MonoBehaviour 组件，提供 Inspector 界面和公开 API
- `CustomButtonElement`: 继承 VisualElement，实现具体的 UI 逻辑
- UXML: 定义 UI 结构
- USS: 定义样式和状态

### 状态管理
按钮状态通过 CSS 类切换实现，支持平滑的视觉过渡效果。

### 事件系统
使用 Unity 的 UnityEvent 系统，支持在 Inspector 中绑定和代码中动态添加监听器。

## 注意事项

1. UXML 文件必须放在 `Resources/UI/` 目录下才能在运行时加载
2. 如果要修改样式，建议复制 USS 文件并创建自定义版本
3. 图标纹理建议使用 2D 纹理，并设置适当的导入设置
4. 组件需要 UIDocument 才能正常工作

## 扩展建议

- 添加音效支持
- 实现按钮动画系统
- 支持更多的交互手势
- 添加可访问性支持
- 实现按钮组功能