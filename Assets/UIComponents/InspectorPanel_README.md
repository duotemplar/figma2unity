# Inspector Panel - Figma to Unity Implementation

基于 Figma 设计实现的 Unity UI Toolkit Inspector 面板组件，采用轻量化方案快速还原界面功能。

## 📋 项目概览

这是从 Figma 设计 (`XeMmqcpAwihR0MwgQX44rk`) 中提取的 Inspector 界面的 Unity UI Toolkit 实现。采用方案 B（轻量还原），使用 Unity 原生 UI Toolkit 控件结合现有的 `CustomButton` 组件。

### ✨ 已实现功能

- ✅ **深色主题界面**：匹配 Figma 设计的深灰背景和卡片样式
- ✅ **折叠面板**：Transform 和 Script 设置区域可折叠
- ✅ **Transform 控件**：Position、Rotation、Scale 的 Vector3 输入字段
- ✅ **Script 设置**：Toggle、Slider 等各种控件
- ✅ **自定义按钮**：复用现有 CustomButton 组件实现 Start BakeAsync / Cancel Bake 功能
- ✅ **响应式布局**：支持不同屏幕尺寸的适配
- ✅ **运行时 API**：完整的代码控制接口

## 📁 文件结构

```
Assets/DummyUI/
├── Scripts/
│   ├── InspectorPanel.cs           # 主要 Inspector 组件
│   ├── InspectorPanelExample.cs    # 使用示例和演示脚本
│   ├── CustomButton.cs             # 复用的按钮组件
│   └── CustomButtonElement.cs      # 按钮 UI 元素
├── UI/
│   ├── InspectorPanel.uxml         # Inspector UI 结构
│   ├── InspectorPanel.uss          # Inspector 深色主题样式
│   ├── CustomButton.uxml           # 按钮 UI 结构
│   └── CustomButton.uss            # 按钮样式
└── Resources/UI/
    ├── InspectorPanel.uxml         # 运行时资源（必需）
    └── CustomButton.uxml           # 运行时资源（必需）
```

## 🚀 使用方法

### 1. 基本设置

1. 在场景中创建一个空的 GameObject
2. 添加 `InspectorPanel` 组件
3. 运行场景即可看到 Inspector 面板

### 2. 代码控制示例

```csharp
// 获取 Inspector Panel 组件
InspectorPanel inspector = GetComponent<InspectorPanel>();

// 设置 Transform 值
inspector.SetPosition(new Vector3(1, 2, 3));
inspector.SetRotation(new Vector3(45, 90, 135));
inspector.SetScale(new Vector3(2, 2, 2));

// 获取当前值
Vector3 currentPos = inspector.GetPosition();
Vector3 currentRot = inspector.GetRotation();
Vector3 currentScale = inspector.GetScale();

// 绑定事件
inspector.OnStartBake.AddListener(() => {
    Debug.Log("开始烘焙处理...");
});

inspector.OnCancelBake.AddListener(() => {
    Debug.Log("取消烘焙操作");
});
```

### 3. 使用示例脚本

项目包含 `InspectorPanelExample.cs` 演示脚本，提供以下功能：

- **自动演示模式**：数值会自动变化，展示动态效果
- **事件响应**：演示按钮点击的处理逻辑
- **测试方法**：可通过 Inspector 右键菜单测试各种功能

## 🎨 视觉设计特点

### 基于 Figma 设计的还原：
- **深色主题**：深灰背景 (`rgb(45, 45, 45)`)，卡片式面板
- **折叠区域**：模拟 Unity Editor Inspector 的折叠行为
- **控件样式**：
  - Vector3 输入框：深色背景，浅色文本
  - Toggle：蓝色选中状态
  - Slider：蓝色滑块和进度条
  - 按钮：使用现有 CustomButton 组件
- **间距与边框**：精确匹配设计稿的间距和圆角

### 响应式适配：
- 小屏幕下自动调整为垂直布局
- 控件宽度自适应容器大小

## ⚙️ 技术实现

### UI Toolkit 原生控件使用：
- `Foldout`：实现可折叠区域
- `Vector3Field`：Transform 数值输入
- `Toggle`：开关控件
- `Slider`：滑动条控件
- `Label`：文本显示

### 与现有组件集成：
- 复用 `CustomButton` 实现操作按钮
- 保持与项目现有 UI 组件的一致性

### 事件系统：
- 使用 `UnityEvent` 提供类型安全的事件回调
- 支持 Inspector 中绑定和代码中动态添加

## 🔧 自定义与扩展

### 修改样式：
编辑 `InspectorPanel.uss` 文件来调整：
- 颜色主题
- 字体大小
- 间距和布局
- 控件外观

### 添加新控件：
1. 在 UXML 中添加新的 UI 元素
2. 在 `InspectorPanel.cs` 中添加对应的字段和逻辑
3. 在 USS 中添加样式定义

### 集成到项目：
- 可作为独立 UI 组件在其他场景中使用
- 支持继承 `InspectorPanel` 类实现自定义功能
- 提供完整的公开 API 供其他脚本调用

## 📝 与 Figma 设计的对比

### ✅ 已匹配的特性：
- 整体深色主题和布局结构
- 折叠面板的层次组织
- 控件类型和基本外观
- 按钮的功能和位置

### 🔄 轻量化实现的差异：
- 使用 Unity 原生控件样式（非完全自定义绘制）
- 简化了部分嵌套折叠的复杂度
- 图标使用简单色块代替（可后续替换真实图标）
- 字体使用 Unity 默认字体（可配置为 Inter 字体）

### 🚧 后续改进空间：
- 添加更精确的图标和字体
- 实现更复杂的嵌套设置面板
- 添加动画过渡效果
- 支持更多自定义主题

## 🎯 快速测试

1. 创建新场景
2. 添加空 GameObject，命名为 "InspectorPanel"
3. 添加 `InspectorPanel` 和 `InspectorPanelExample` 组件
4. 运行场景查看效果

**提示**：查看 Console 输出可以看到详细的交互日志和演示信息。

---

这个实现展示了如何将 Figma 设计快速转换为功能性的 Unity UI Toolkit 界面，为后续的精细化改进提供了solid基础。