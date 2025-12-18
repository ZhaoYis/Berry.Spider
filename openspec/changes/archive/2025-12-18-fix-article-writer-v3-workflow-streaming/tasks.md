## 1. 分析与设计
- [x] 1.1 梳理 ArticleWriterAgentV3 工作流链路：确认 `WorkflowBuilder(summarizeWriterExecutor)` 所构建工作流的输入类型由 `SummarizeWriterExecutor : IMessageHandler<string, SummarizeOutput>` 决定为 `string`。
- [x] 1.2 对比当前 `InProcessExecution.StreamAsync(workflow, messages)` 调用与 V2 及官方示例，明确 V3 中使用 `List<ChatMessage>` 作为输入与工作流实际输入类型不匹配，是导致无事件流的主要原因。

## 2. 实施
- [x] 2.1 将 `ArticleWriterAgentV3ViewModel.GeneratingAsync` 中的 `messages` 输入调整为与工作流输入类型一致的 `string`，例如：直接传入 `this.UserInput` 或构造后的 prompt 字符串。
- [x] 2.2 根据需要，评估是否保留 `List<ChatMessage>` 形式：若确有需求，则应相应调整 `SummarizeWriterExecutor` 的泛型签名为 `IMessageHandler<ChatMessage, SummarizeOutput>` 并在执行器内部从 `ChatMessage` 提取文本内容；二者二选一，确保类型统一。（本次选择前者，保持执行器签名不变）
- [x] 2.3 为工作流执行增加最小诊断：
  - 在 `WatchStreamAsync` 循环中统计事件数量，执行结束后如为 `0`，输出一条诊断日志提示“未收到任何 WorkflowEvent，可能存在输入类型不匹配或配置问题”；
  - 对 `WorkflowErrorEvent` 等错误事件至少写 Debug/日志，避免静默失败。

## 3. 验证
- [ ] 3.1 在桌面应用中手动测试：
  - 输入正常的 UserInput 触发 V3 生成；
  - 确认可以在 Debug 输出中看到各类 WorkflowEvent 被触发（例如自定义的 `SummarizeWriterFinishedEvent`、`MainWriterFinishedEvent`、`ReviewerFinishedEvent` 或 `WorkflowOutputEvent`），并且 UI `AiResponseText` 出现合乎预期的更新。
- [ ] 3.2 故意传入异常输入（如空字符串或格式错误的 JSON），观察是否触发错误事件或异常日志，确认不会出现“无事件、无日志”的静默失败场景。
- [ ] 3.3 回归检查：确保 V2 的工作流行为未受影响，且 V3 不会因类型调整引入新的运行时异常。
