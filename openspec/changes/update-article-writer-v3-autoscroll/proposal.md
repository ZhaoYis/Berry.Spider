# 变更：ArticleWriterAgentV2 AI 回答文本框支持自动滚动

## 为什么
当前 AIGenPlus 文章生成器 V2 (`ArticleWriterAgentV2`) 中，AI 回答展示使用的只读 `TextBox` 高度固定为 300，当 AI 生成内容超过该高度时，文本框不会自动滚动到底部，需要用户手动拖动滚动条查看最新回答，体验较差，且不符合长文本流式输出的常见交互预期。

## 变更内容
- 为 ArticleWriterAgentV2 页面中 AI 回答对应的 `TextBox` 增加“内容追加时自动滚动到底部”的行为（仅在用户未主动向上滚动查看历史时自动滚动）。
- 保持视图结构和绑定不变，仅通过控件属性或最小的代码隐藏/ViewModel 扩展实现自动滚动逻辑。
- 确保在流式输出（`AiResponseText += ...`）的过程中，界面持续滚动到最新内容，便于用户实时查看生成进度。

## 影响
- 受影响规范：桌面端 AIGenPlus 代理（ArticleWriterAgentV2）页面的 AI 回答展示交互。
- 受影响代码：
  - `desktop/Berry.Spider.AIGenPlus/Views/Pages/Agents/ArticleWriterAgentV2.axaml`
  - 如有需要，可能影响 `ArticleWriterAgentV2.axaml.cs` 或相关行为扩展（但优先考虑在 XAML 或通用行为中完成）。
