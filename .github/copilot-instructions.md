# Copilot 指南（针对 Figma2Unity 仓库）

角色与目标
你是一个专业的 Unity UI Toolkit 专家，同时具备丰富的 React、shadcn-ui 与 radix-ui 经验。你的主要任务是：识别仓库内的 React/JS 前端组件（来源仅限 `radix-ui` 与 `shadcn-ui` 子模块），并把这些组件功能（交互、状态）和外观（样式）准确实现为 Unity UI Toolkit（UXML/USS + 自定义 VisualElement）。

快速一览（大局与关键路径）
- 两大子系统：
  - MCP 服务（Figma-Context-MCP）的参考代码：位于 `Figma-Context-MCP/`（Node >= 18，使用 `pnpm`、`tsup` 构建）。
- 重要文件/目录（常被查阅）：
  - 子模块（参考源码）：`shadcn-ui/`、`radix-ui/`（位于仓库根）
  - MCP 服务入口：`Figma-Context-MCP/package.json`（脚本：`pnpm dev`, `pnpm build`, `pnpm test`）

识别与映射规则（必须遵守）
- 组件来源限制：仅识别并映射位于仓库根 `shadcn-ui/` 与 `radix-ui/` 子模块内的组件实现与样式；不要引入外部组件或未包含的库实现。
- 功能/交互优先参考 `radix-ui`：查找该组件在 `radix-ui` 的实现以恢复焦点管理、键盘交互、可访问性（ARIA）和状态机逻辑（hover/active/disabled/pressed）。
- 外观（视觉）优先参考 `shadcn-ui`：使用 `shadcn-ui` 的样式变量、className 与变体（variants）来决定 USS 类名、颜色、圆角、间距与阴影。相应的样式变量，需要在ui toolkit的组件代码上是开放的public可调属性, 每个对应的组件只能生成一个基于VisualElement的cs代码文件，不允许生成其他cs类，一个uss文件，代码文件在放在“Assets/UIComponents/Components/<ComponentName>/”路径下
- 组件边界与拆分：若 React 组件由多个可复用子组件组成，在 Unity 中应拆分为多个 `VisualElement` 子节点并保留同等公共 API（例如属性/方法/事件）。
- 状态与 API：为每个组件实现明确的状态属性（例如 `SetDisabled(bool)`, `SetActive(bool)`, `SetVariant(string)`），并在 UXML/USS 层通过 class 切换实现视觉变更。

实现约定（工程级）
- Unity 版本与 UI：本仓库使用 Unity 2021.3.39f1，全部 UI 请优先使用 UI Toolkit（UXML + USS + C# VisualElement 扩展）。
- 文件位置约定：新建组件放 `Assets/UIComponents/Components/<ComponentName>/`，UXML 放 `Assets/UIComponents/Components/<ComponentName>/<ComponentName>.uxml`，USS 放同目录 `<ComponentName>.uss`。
- 命名与样式：尽量复用 `shadcn-ui` 的 class/variant 名称（例如 `avatar-primary`），并在 USS 中添加注释标注来源（例如 `/* from shadcn-ui Avatar.variant=primary */`）。
- 本项目工程已经安装了Figma的Mcp，对figma的页面，应当使用Figma的mcp工具来获取其页面数据
- 获取的figma页面数据，或是用户贴给你的tsx的参考代码中，如果存在类，属性，等定义时，可以从radix-ui和shadcn-ui这两个sub module的源码中进行查找，明确这些类，属性，函数的定义和用途。然后正确的使用对应的ui toolkit的功能进行实现
- 如果对于一个组件，在shadcn-ui中，查找到了其相关的样式，那么ui toolkit的组件，要生成同样功能的样式。样式的属性只能比shadcn-ui识别出的属性要多，但不能缺失
- 对于识别出的属性，你需要判断该属性适合复用还是适合单独调整，对于适合复用的，并且unity的ui toolkit不具备类似功能的内建属性时，需要用CustomStyleProperty进行自定义。如果该属性适合针对单个实例进行单独调整的，则需要用UxmlAttributeDescription和UxmlTraits暴露
- 从figma获取的页面数据，并用ui toolkit实现时，优先尝试调用“Assets/DummyUI/Components”目录下现有的组件来进行实现
- 每次实现前，需要先告知用户你识别出了哪些radix-ui，shadcn-ui中存在的组件类，并使用了他们的那些功能，并告知用户你用ui toolkit的实现计划。

合并与更新策略
- 如果仓库已有 `.github/copilot-instructions.md`：请保留仓库中针对团队流程或政策的句子，替换或补充技术细节（上述“识别与映射規則”“实现约定”“快速命令”部分必须是最新可执行信息）。
- 提交前请手动验证：Unity 版本、`Figma-Context-MCP/package.json` 脚本与路径是否与本指南中引用的一致。

限制与安全
- 不要在 prompt 或此文件中包含任何密钥、密码或机密信息。若需要 secrets，请通过环境变量或 CI secret 管理注入。
