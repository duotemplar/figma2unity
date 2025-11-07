# Inspector Panel - Panel Settings 配置指南

## 问题说明

如果你在使用 `InspectorPanel` 时遇到以下问题：
- ✗ 字体大小异常（过大或过小）
- ✗ UI 元素布局错乱、重叠
- ✗ 间距不一致
- ✗ 控件尺寸不正确

这通常是因为 **Panel Settings 配置不当** 导致的缩放问题。

## 自动配置（推荐）

从当前版本开始，`InspectorPanel` 已经包含自动配置代码，会在运行时自动设置 Panel Settings：

```csharp
// InspectorPanel.cs 中的 ConfigurePanelSettings() 方法会自动：
// 1. 检查是否有 Panel Settings
// 2. 如果没有，尝试从 Resources 加载或创建临时设置
// 3. 设置缩放模式为 Constant Pixel Size
// 4. 设置 scale = 1.0
```

**大多数情况下，你不需要手动配置**，组件会自动处理。

## 手动配置（推荐方法）

如果需要手动控制 Panel Settings，按以下步骤操作：

### 步骤 1: 创建 Panel Settings Asset

1. 在 Unity 编辑器中，右键点击 `Assets/DummyUI/UI/` 文件夹
2. 选择 `Create > UI Toolkit > Panel Settings Asset`
3. 命名为 `InspectorPanelSettings`

### 步骤 2: 配置 Panel Settings

在 Inspector 窗口中设置以下参数：

```
Panel Settings Inspector:
├── Scale Mode: ★ Constant Pixel Size (重要!)
├── Scale: 1.0
├── Reference Resolution: 1920 x 1080
├── Screen Match Mode: Match Width Or Height
├── Match: 0.5
├── Sort Order: 0
└── Target Texture: None
```

**关键设置说明：**

| 参数 | 推荐值 | 说明 |
|------|--------|------|
| **Scale Mode** | `Constant Pixel Size` | **最重要！** 确保 UI 不随屏幕缩放 |
| **Scale** | `1.0` | 保持 1:1 像素比例 |
| **Reference Resolution** | `1920 x 1080` | 参考分辨率（可选） |

### 步骤 3: 分配 Panel Settings

有两种方式分配：

#### 方法 A: 在编辑器中分配（推荐）

1. 选择包含 `UIDocument` 的 GameObject
2. 在 Inspector 中找到 `UIDocument` 组件
3. 将 `InspectorPanelSettings` 拖到 `Panel Settings` 字段

#### 方法 B: 放在 Resources 文件夹

1. 将 `InspectorPanelSettings.asset` 放在 `Assets/Resources/UI/` 文件夹
2. 组件会自动加载（无需手动分配）

## 常见问题排查

### Q: UI 还是太大/太小怎么办？

**A:** 检查以下几点：

1. **Scale Mode 必须是 `Constant Pixel Size`**
   ```
   ✓ Constant Pixel Size  ← 正确
   ✗ Scale With Screen Size  ← 会导致缩放问题
   ✗ Constant Physical Size  ← 会导致缩放问题
   ```

2. **Scale 值应该是 1.0**
   - 如果 UI 太大，可以尝试 0.8 或 0.9
   - 如果 UI 太小，可以尝试 1.1 或 1.2

3. **检查 USS 文件是否正确加载**
   ```bash
   # 确认文件存在
   Assets/DummyUI/UI/InspectorPanel.uss
   Assets/Resources/UI/InspectorPanel.uxml
   ```

### Q: 为什么不同分辨率下 UI 大小不一致？

**A:** 这是正常的！`Constant Pixel Size` 模式下，UI 使用固定像素大小：
- ✓ 在高分辨率屏幕上，UI 相对更小（正确行为）
- ✓ 在低分辨率屏幕上，UI 相对更大（正确行为）

如果需要自适应缩放，可以改用 `Scale With Screen Size` 模式，但需要调整 USS：
```css
/* 使用百分比或相对单位 */
.inspector-root {
    width: 25%;  /* 屏幕宽度的 25% */
}
```

### Q: 控制台显示 "No Panel Settings assigned" 警告

**A:** 这个警告不影响功能。组件会自动创建临时 Panel Settings。

要消除警告，按照上面的 **步骤 1-3** 手动创建并分配 Panel Settings。

### Q: 运行时修改 Panel Settings 不生效

**A:** Panel Settings 的某些参数在运行时修改后，需要重新初始化 UIDocument：

```csharp
// 如果需要运行时修改
uiDocument.panelSettings.scale = 1.2f;
uiDocument.visualTreeAsset = null;  // 重置
uiDocument.visualTreeAsset = uxml;  // 重新加载
```

## USS 优化说明

当前 USS 已经过优化，使用固定像素单位：

```css
/* 已优化的关键样式 */
.inspector-root {
    width: 320px;           /* 固定宽度 */
    max-width: 320px;       /* 最大宽度限制 */
    min-width: 300px;       /* 最小宽度限制 */
}

.inspector-title {
    font-size: 14px;        /* 固定字体大小 */
    height: 18px;           /* 固定高度 */
}

.vector3-input .unity-text-field__input {
    font-size: 11px;        /* 固定字体 */
    height: 18px;           /* 固定高度 */
}
```

这些固定尺寸配合 `Constant Pixel Size` 模式，确保 UI 显示一致。

## 推荐的项目设置

### 单个 Inspector Panel
```
GameObject "Inspector"
├── InspectorPanel (Component)
├── InspectorPanelExample (Component, optional)
└── UIDocument (自动添加)
    ├── Panel Settings: InspectorPanelSettings
    ├── Source Asset: InspectorPanel.uxml (自动加载)
    └── USS: InspectorPanel.uss (通过 UXML 引用)
```

### 多个 Inspector Panel（共享配置）
```
# 所有 Inspector Panel 可以共享同一个 Panel Settings

Inspector1 GameObject → InspectorPanelSettings
Inspector2 GameObject → InspectorPanelSettings (same)
Inspector3 GameObject → InspectorPanelSettings (same)
```

## 验证清单

配置完成后，检查以下内容：

- [ ] Panel Settings Asset 已创建
- [ ] Scale Mode = `Constant Pixel Size`
- [ ] Scale = `1.0`
- [ ] Panel Settings 已分配给 UIDocument
- [ ] USS 文件路径正确
- [ ] UXML 文件在 Resources/UI/ 目录
- [ ] 运行时无错误日志
- [ ] UI 显示大小正常
- [ ] 字体清晰可读
- [ ] 控件间距一致

## 技术原理

### Panel Settings 缩放模式对比

| 模式 | 行为 | 适用场景 |
|------|------|----------|
| **Constant Pixel Size** | UI 使用固定像素大小 | 固定大小的工具面板、Inspector 面板 |
| **Scale With Screen Size** | UI 随屏幕分辨率缩放 | 全屏游戏 UI、响应式界面 |
| **Constant Physical Size** | UI 根据 DPI 保持物理尺寸 | 跨设备一致性（手机、平板） |

### 为什么选择 Constant Pixel Size？

Inspector Panel 是一个 **固定大小的工具面板**，类似 Unity Editor 自身的 Inspector：
- ✓ 保持一致的像素精度
- ✓ 字体和图标清晰
- ✓ 布局稳定可预测
- ✓ 不受分辨率变化影响

## 进阶配置

### 支持 4K 和高 DPI 屏幕

如果在 4K 显示器上 UI 太小，可以增加 scale：

```csharp
// 在 InspectorPanel.cs 的 ConfigurePanelSettings() 中修改
settings.scale = Screen.dpi > 150 ? 1.5f : 1.0f;
```

### 动态调整 UI 大小

添加用户可调整的缩放设置：

```csharp
[Header("UI Scale")]
[Range(0.5f, 2.0f)]
public float uiScale = 1.0f;

private void ConfigurePanelSettings()
{
    // ...existing code...
    settings.scale = uiScale;
}
```

### 响应式布局（可选）

如果需要响应式设计，修改 USS 使用百分比：

```css
.inspector-root {
    width: 320px;
    max-width: 25%;  /* 屏幕宽度的 25% */
}

@media (max-width: 1280px) {
    .inspector-root {
        width: 280px;
    }
}
```

## 总结

✅ **最简单的方法**：不做任何配置，让组件自动处理  
✅ **推荐方法**：创建 Panel Settings Asset 并设置为 `Constant Pixel Size`  
✅ **高级方法**：根据 DPI 动态调整 scale

配置正确后，Inspector Panel 将在所有分辨率下保持一致的外观和布局。

---

**更新日期**: 2025年11月5日  
**适用版本**: Unity 2021.3+  
**相关文件**: 
- `InspectorPanel.cs` - 包含自动配置代码
- `InspectorPanel.uss` - 优化的固定尺寸样式
- `InspectorPanel_README.md` - 组件使用文档
