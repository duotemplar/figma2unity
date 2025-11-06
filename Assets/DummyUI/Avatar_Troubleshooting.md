# Avatar 图片加载问题排查指南

## 当前状态

根据截图，Avatar 只显示了 Fallback 文字（CN、ER、CNLRER），图片未加载。

## 已完成的修复

### 1. 添加了详细的调试日志
- `AvatarElement.SetImage()` 会输出纹理信息
- `UpdateDisplay()` 会显示当前显示状态
- `LoadAvatarImage()` 会记录加载过程

### 2. 确保 USS 样式正确加载
- 在 `AvatarDemo.Start()` 中添加了 `LoadAvatarStyles()`
- 从 Resources/UI/Avatar.uss 加载样式表

### 3. 修复 CSS 定位问题
- 为 `.avatar-image` 和 `.avatar-fallback` 添加了明确的定位属性
- 设置 `top: 0; left: 0; right: 0; bottom: 0;`

## 排查步骤

### 步骤 1: 检查 Console 日志

运行场景后，查看 Console 中的日志：

**期望看到的日志：**
```
[AvatarDemo] Avatar.uss loaded successfully
[AvatarDemo] Avatar demo created successfully!
[AvatarDemo] Starting to load avatar images...
[AvatarDemo] Loading image from: https://github.com/shadcn.png
[AvatarElement] SetImage called with texture: ..., size: ...
[AvatarElement] Image display updated. imageLoaded=True
[AvatarElement] Displaying image for 'CN'
[AvatarDemo] Successfully loaded image from: ...
```

**如果看到以下错误：**

#### A. "Failed to load Avatar.uss"
```
[AvatarDemo] Failed to load Avatar.uss from Resources/UI/Avatar
```
**解决方案：**
- 检查文件是否存在：`Assets/Resources/UI/Avatar.uss`
- 重新导入 Assets（右键 Assets 文件夹 → Reimport）

#### B. "Failed to load image"
```
[AvatarDemo] Failed to load image from https://...: ...
```
**解决方案：**
1. 检查网络连接
2. 确认 Unity Player Settings 中启用了 Internet Access
   - Edit → Project Settings → Player
   - Other Settings → Internet Access = Required

#### C. 没有任何图片加载日志
- 检查 Coroutine 是否正确启动
- 确认 `avatarElements` 列表不为空

### 步骤 2: 手动验证文件

**在终端运行：**
```bash
# 检查 Resources 文件夹
ls -la /Users/duoshen/AIGC/Figma2Unity/Assets/Resources/UI/

# 应该看到：
# Avatar.uss
# Avatar.uss.meta
```

### 步骤 3: 检查 Unity 编辑器设置

1. **UIDocument 组件配置**
   - Panel Settings: 必须设置（使用 InspectorPanelSettings）
   - Source Asset: 可以为空（代码创建 UI）

2. **AvatarDemo 组件配置**
   - UI Settings → Ui Document: 应该指向同一个 GameObject 的 UIDocument
   - Avatar Image URLs: 确认 URL 正确

### 步骤 4: 测试图片 URL

在浏览器中打开以下 URL，确认可以访问：
- https://github.com/shadcn.png
- https://github.com/evilrabbit.png
- https://github.com/maxleiter.png

如果无法访问，替换为其他可用的图片 URL：
```
https://i.pravatar.cc/150?img=1
https://i.pravatar.cc/150?img=2
https://i.pravatar.cc/150?img=3
```

### 步骤 5: 简化测试

创建一个最小测试场景：

```csharp
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Networking;
using System.Collections;

public class AvatarTest : MonoBehaviour
{
    void Start()
    {
        var doc = GetComponent<UIDocument>();
        var root = doc.rootVisualElement;
        
        // 加载样式
        var uss = Resources.Load<StyleSheet>("UI/Avatar");
        if (uss != null)
        {
            root.styleSheets.Add(uss);
            Debug.Log("USS loaded!");
        }
        else
        {
            Debug.LogError("Failed to load USS!");
        }
        
        // 创建头像
        var avatar = new DummyUI.AvatarElement();
        avatar.SetFallbackText("TEST");
        avatar.SetShape(DummyUI.Avatar.AvatarShape.Circle);
        avatar.SetSize(DummyUI.Avatar.AvatarSize.Large);
        root.Add(avatar);
        
        // 加载图片
        StartCoroutine(LoadTest(avatar));
    }
    
    IEnumerator LoadTest(DummyUI.AvatarElement avatar)
    {
        Debug.Log("Starting load test...");
        
        using (var request = UnityWebRequestTexture.GetTexture("https://via.placeholder.com/150"))
        {
            yield return request.SendWebRequest();
            
            if (request.result == UnityWebRequest.Result.Success)
            {
                var texture = DownloadHandlerTexture.GetContent(request);
                Debug.Log($"Loaded texture: {texture.width}x{texture.height}");
                avatar.SetImage(texture);
            }
            else
            {
                Debug.LogError($"Load failed: {request.error}");
            }
        }
    }
}
```

## 已知问题和限制

### 1. border-radius: 50% 可能不完美
Unity UI Toolkit 的圆形遮罩在某些平台可能不是完美的圆形。

**临时解决方案：**
- 使用偶数尺寸（40px, 48px）
- 或者增大头像尺寸

### 2. 图片加载需要时间
首次加载可能需要 2-5 秒，取决于网络速度。

### 3. CORS 限制（WebGL 构建）
如果是 WebGL 构建，某些图片 URL 可能因为 CORS 策略而无法加载。

**解决方案：**
- 使用支持 CORS 的图片服务器
- 或将图片放在本地 StreamingAssets 文件夹

## 快速修复清单

- [ ] 检查 Console 是否有错误日志
- [ ] 确认 Resources/UI/Avatar.uss 文件存在
- [ ] 验证图片 URL 在浏览器中可访问
- [ ] 确认 Unity Player Settings 启用了 Internet Access
- [ ] 重新导入 Assets 文件夹（Reimport）
- [ ] 停止播放再重新运行场景
- [ ] 尝试使用不同的图片 URL（如 placeholder 服务）

## 下一步

1. **运行场景并检查 Console 日志**
2. **将 Console 日志内容发给我**，我可以帮你分析具体问题
3. 如果还是不显示，我们可以尝试：
   - 使用本地图片资源替代网络图片
   - 创建一个更简单的测试场景
   - 检查 Unity 版本兼容性

---

**最后更新**: 2025年11月5日  
**测试环境**: Unity 2021.3.39f1
