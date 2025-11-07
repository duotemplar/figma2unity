# Avatar Component - UI Toolkit 实现

基于 shadcn/ui Avatar 组件的 Unity UI Toolkit 实现。

## 📦 组件文件

```
Assets/DummyUI/
├── Scripts/
│   ├── Avatar.cs              # 主组件 (MonoBehaviour)
│   ├── AvatarElement.cs       # 自定义 VisualElement
│   └── AvatarDemo.cs          # 演示脚本
└── UI/
    ├── Avatar.uxml            # UI 结构模板
    └── Avatar.uss             # 样式定义
```

## ✨ 功能特性

### 核心功能
- ✅ **图片加载**：支持从 URL 加载头像图片
- ✅ **Fallback 文字**：图片加载失败时显示备用文字
- ✅ **多种形状**：圆形 (Circle) 和圆角矩形 (RoundedSquare)
- ✅ **多种尺寸**：Small (32px), Medium (40px), Large (48px)
- ✅ **灰度效果**：支持灰度滤镜
- ✅ **边框环绕**：支持自定义颜色的边框环
- ✅ **堆叠布局**：支持负间距重叠显示

### 样式变体
- 🎨 **颜色方案**：Primary, Secondary, Success, Danger, Warning
- 🎨 **状态指示器**：Online, Offline, Busy, Away（可选）
- 🎨 **悬停/聚焦状态**：内置交互样式

## 🚀 快速开始

### 方法 1: 使用 Avatar 组件

```csharp
// 1. 创建 GameObject
GameObject avatarObj = new GameObject("Avatar");

// 2. 添加 Avatar 组件
Avatar avatar = avatarObj.AddComponent<Avatar>();

// 3. 配置属性（通过 Inspector 或代码）
avatar.SetImageUrl("https://github.com/shadcn.png");
avatar.SetFallbackText("CN");
avatar.SetShape(Avatar.AvatarShape.Circle);
avatar.SetSize(Avatar.AvatarSize.Medium);
```

### 方法 2: 直接使用 AvatarElement

```csharp
// 在 UIDocument 中创建
var avatarElement = new AvatarElement();
avatarElement.SetFallbackText("CN");
avatarElement.SetShape(Avatar.AvatarShape.Circle);
avatarElement.SetSize(Avatar.AvatarSize.Medium);

// 添加到 UI
uiDocument.rootVisualElement.Add(avatarElement);
```

### 方法 3: 使用演示脚本

```csharp
// 1. 创建空 GameObject
// 2. 添加 UIDocument 组件
// 3. 添加 AvatarDemo 组件
// 4. 运行场景查看三种 Avatar 样式
```

## 📖 API 文档

### Avatar.cs (MonoBehaviour)

#### 属性

| 属性 | 类型 | 默认值 | 描述 |
|------|------|--------|------|
| `imageUrl` | string | "" | 头像图片 URL |
| `fallbackText` | string | "CN" | 备用显示文字 |
| `shape` | AvatarShape | Circle | 头像形状 |
| `size` | AvatarSize | Medium | 头像尺寸 |
| `useGrayscale` | bool | false | 是否使用灰度效果 |
| `showRing` | bool | false | 是否显示边框环 |
| `ringColor` | Color | White | 边框环颜色 |

#### 公共方法

```csharp
// 设置图片 URL（自动加载）
void SetImageUrl(string url)

// 设置备用文字
void SetFallbackText(string text)

// 设置形状
void SetShape(AvatarShape shape)
// AvatarShape: Circle, RoundedSquare

// 设置尺寸
void SetSize(AvatarSize size)
// AvatarSize: Small (32px), Medium (40px), Large (48px)

// 设置灰度效果
void SetGrayscale(bool enabled)

// 设置边框环
void SetRing(bool enabled, Color color)
```

### AvatarElement.cs (VisualElement)

#### 公共方法

```csharp
// 设置图片纹理
void SetImage(Texture2D texture)

// 设置备用文字
void SetFallbackText(string text)

// 设置形状
void SetShape(Avatar.AvatarShape shape)

// 设置尺寸
void SetSize(Avatar.AvatarSize size)

// 设置灰度效果
void SetGrayscale(bool enabled)

// 设置边框环
void SetRing(bool enabled, Color color)
```

## 🎨 样式自定义

### USS 类名

```css
/* 基础类 */
.avatar                    /* 根容器 */
.avatar-container         /* 头像容器 */
.avatar-image             /* 图片元素 */
.avatar-fallback          /* 备用文字 */

/* 形状变体 */
.avatar-circle            /* 圆形头像 */
.avatar-rounded           /* 圆角矩形 */

/* 尺寸变体 */
.avatar-small             /* 32x32 */
.avatar-medium            /* 40x40 */
.avatar-large             /* 48x48 */

/* 效果类 */
.avatar-grayscale         /* 灰度滤镜 */
.avatar-ring              /* 边框环 */

/* 颜色变体 */
.avatar-primary           /* 蓝色背景 */
.avatar-secondary         /* 灰色背景 */
.avatar-success           /* 绿色背景 */
.avatar-danger            /* 红色背景 */
.avatar-warning           /* 橙色背景 */

/* 状态指示器（可选） */
.avatar-status            /* 状态点 */
.avatar-status-online     /* 在线 - 绿色 */
.avatar-status-offline    /* 离线 - 灰色 */
.avatar-status-busy       /* 忙碌 - 红色 */
.avatar-status-away       /* 离开 - 橙色 */

/* 分组布局 */
.avatar-group             /* 头像组容器 */
.avatar-stacked           /* 堆叠样式 */
```

### 自定义样式示例

```css
/* 修改默认尺寸 */
.avatar-medium .avatar-container {
    width: 50px;
    height: 50px;
}

/* 自定义颜色方案 */
.avatar-custom .avatar-fallback {
    background-color: rgb(123, 31, 162); /* 紫色 */
}

/* 修改边框环样式 */
.avatar-ring .avatar-container {
    border-width: 3px;
    border-color: rgb(255, 215, 0); /* 金色 */
}

/* 添加阴影效果 */
.avatar:hover .avatar-container {
    box-shadow: 0 4px 8px rgba(0, 0, 0, 0.3);
}
```

## 💡 使用示例

### 示例 1: 基础圆形头像

```csharp
var avatar = new AvatarElement();
avatar.SetFallbackText("CN");
avatar.SetShape(Avatar.AvatarShape.Circle);
avatar.SetSize(Avatar.AvatarSize.Medium);
avatar.AddToClassList("avatar-primary");
```

### 示例 2: 圆角矩形头像

```csharp
var avatar = new AvatarElement();
avatar.SetFallbackText("ER");
avatar.SetShape(Avatar.AvatarShape.RoundedSquare);
avatar.SetSize(Avatar.AvatarSize.Large);
avatar.AddToClassList("avatar-secondary");
```

### 示例 3: 堆叠头像组

```csharp
var group = new VisualElement();
group.AddToClassList("avatar-group");
group.style.flexDirection = FlexDirection.Row;

// 创建 3 个头像
for (int i = 0; i < 3; i++)
{
    var avatar = new AvatarElement();
    avatar.SetFallbackText(new string[] { "CN", "LR", "ER" }[i]);
    avatar.SetGrayscale(true);
    avatar.SetRing(true, Color.white);
    
    // 第一个头像不需要负间距
    if (i > 0)
    {
        avatar.style.marginLeft = -8;
    }
    
    group.Add(avatar);
}
```

### 示例 4: 带状态指示器

```csharp
var container = new VisualElement();
container.AddToClassList("avatar");
container.style.position = Position.Relative;

var avatarElement = new AvatarElement();
avatarElement.SetFallbackText("CN");
container.Add(avatarElement);

// 添加在线状态指示器
var status = new VisualElement();
status.AddToClassList("avatar-status");
status.AddToClassList("avatar-status-online");
container.Add(status);
```

### 示例 5: 从 URL 加载图片

```csharp
// 使用 Avatar MonoBehaviour
var avatarComponent = gameObject.AddComponent<Avatar>();
avatarComponent.SetImageUrl("https://github.com/shadcn.png");
avatarComponent.SetFallbackText("CN");

// 图片加载失败时自动显示 "CN"
```

## 🔧 高级用法

### 动态更新头像

```csharp
public class UserProfileUI : MonoBehaviour
{
    private AvatarElement avatar;
    
    void Start()
    {
        avatar = new AvatarElement();
        // 初始化...
    }
    
    public void UpdateUserAvatar(string username, Texture2D profilePic)
    {
        if (profilePic != null)
        {
            avatar.SetImage(profilePic);
        }
        else
        {
            // 使用用户名首字母作为 fallback
            string initials = GetInitials(username);
            avatar.SetFallbackText(initials);
        }
    }
    
    private string GetInitials(string name)
    {
        var parts = name.Split(' ');
        if (parts.Length >= 2)
        {
            return $"{parts[0][0]}{parts[1][0]}".ToUpper();
        }
        return name.Substring(0, Math.Min(2, name.Length)).ToUpper();
    }
}
```

### 创建头像选择器

```csharp
public class AvatarSelector : MonoBehaviour
{
    private VisualElement container;
    private AvatarElement selectedAvatar;
    
    void CreateAvatarGrid()
    {
        container = new VisualElement();
        container.style.flexDirection = FlexDirection.Row;
        container.style.flexWrap = Wrap.Wrap;
        
        string[] users = { "CN", "ER", "LR", "MX", "JD" };
        
        foreach (var user in users)
        {
            var avatar = CreateSelectableAvatar(user);
            container.Add(avatar);
        }
    }
    
    private AvatarElement CreateSelectableAvatar(string initials)
    {
        var avatar = new AvatarElement();
        avatar.SetFallbackText(initials);
        avatar.SetSize(Avatar.AvatarSize.Large);
        
        // 添加点击事件
        avatar.RegisterCallback<ClickEvent>(evt => {
            SelectAvatar(avatar);
        });
        
        return avatar;
    }
    
    private void SelectAvatar(AvatarElement avatar)
    {
        // 移除之前选中的样式
        selectedAvatar?.RemoveFromClassList("avatar-ring");
        
        // 添加选中样式
        avatar.AddToClassList("avatar-ring");
        avatar.SetRing(true, new Color(0.23f, 0.51f, 0.96f)); // 蓝色边框
        
        selectedAvatar = avatar;
    }
}
```

## 📝 注意事项

### 1. 图片加载
- 图片 URL 需要网络权限
- 加载失败时自动显示 Fallback 文字
- 支持本地文件路径（file:///）

### 2. 性能优化
- 避免频繁创建/销毁 Avatar 元素
- 大量头像时考虑使用对象池
- 图片尺寸建议不超过 512x512

### 3. 圆形遮罩限制
- Unity UI Toolkit 的 `border-radius: 50%` 在某些平台可能不完美
- 如需完美圆形，可使用 Mask 纹理

### 4. 灰度效果
- 使用 `-unity-background-image-tint-color` 实现
- 不是真正的灰度滤镜，只是降低饱和度

## 🆚 与 shadcn/ui 的对比

| 特性 | shadcn/ui (React) | UI Toolkit 实现 | 说明 |
|------|------------------|----------------|------|
| 图片加载 | ✅ | ✅ | 使用 UnityWebRequest |
| Fallback 文字 | ✅ | ✅ | 自动切换显示 |
| 圆形/圆角 | ✅ | ✅ | CSS border-radius |
| 灰度效果 | ✅ | ⚠️ | 使用 tint-color 近似 |
| 堆叠布局 | ✅ | ✅ | 负 margin 实现 |
| 响应式 | ✅ | ⚠️ | 需要手动适配 |
| 动画过渡 | ✅ | ❌ | UI Toolkit 动画有限 |

## 🐛 故障排除

### 问题: 头像不显示

**检查清单：**
1. UIDocument 组件已添加
2. Panel Settings 已配置
3. USS 文件已加载
4. Console 中无错误日志

### 问题: 图片加载失败

**可能原因：**
1. URL 无效或网络连接失败
2. Unity 没有网络权限（Build Settings）
3. CORS 策略阻止（Web 平台）

**解决方案：**
```csharp
// 检查网络请求结果
if (request.result != UnityWebRequest.Result.Success)
{
    Debug.LogError($"Failed to load image: {request.error}");
}
```

### 问题: 圆形不完美

**解决方案：**
- 增加 Avatar 尺寸
- 使用偶数尺寸（40px, 48px）
- 或使用圆形 Mask 纹理

## 📚 相关资源

- [UI Toolkit 官方文档](https://docs.unity3d.com/Manual/UIElements.html)
- [shadcn/ui Avatar 组件](https://ui.shadcn.com/docs/components/avatar)
- [UnityWebRequest 文档](https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequest.html)

## 🎯 未来改进

- [ ] 支持 GIF 动画头像
- [ ] 添加加载进度指示器
- [ ] 支持本地相册选择
- [ ] 添加头像编辑功能（裁剪、滤镜）
- [ ] 支持视频头像
- [ ] 优化圆形遮罩渲染

---

**创建日期**: 2025年11月5日  
**Unity 版本**: 2021.3+  
**UI Toolkit 版本**: 1.0+
