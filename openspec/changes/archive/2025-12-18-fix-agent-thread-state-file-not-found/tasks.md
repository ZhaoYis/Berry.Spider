## 1. 分析与设计
- [x] 1.1 明确 `AgentServiceBase.ResumePreviousConversationAsync` 在文件不存在、内容为空或内容无效时的当前行为（包括异常类型与调用栈）。
- [x] 1.2 设计期望行为：在首次执行或状态损坏时，优先保证 Agent 能从新的线程正常启动，同时通过日志保留足够的排查信息。

## 2. 实施
- [x] 2.1 在 `ResumePreviousConversationAsync` 中增加文件存在性检查：
  - 若 `AgentThreads/{taskId}.json` 文件不存在，直接返回 `null`；
  - 若存在但内容为空或反序列化失败，捕获异常、记录日志并返回 `null`。
- [x] 2.2 在 `SaveThreadStateAsync` 中在写入前确保目录存在（必要时调用 `Directory.CreateDirectory`）。
- [x] 2.3 根据项目现有日志/异常处理约定（如 `BusinessException` 或 Debug 输出），为上述异常/降级路径添加简洁、可读的日志信息。

## 3. 验证
- [ ] 3.1 手动删除指定 `taskId` 的线程状态文件，首次运行 Agent，确认：
  - 不会抛出文件不存在异常；
  - 能正常创建新对话并完成执行；
  - 执行结束后生成对应的 `AgentThreads/{taskId}.json` 文件。
- [ ] 3.2 故意写入一个损坏/非 JSON 的状态文件，确认：
  - 不会导致整个 Agent 执行失败；
  - 日志中能看到反序列化失败的提示；
  - Agent 以新线程方式继续执行。
- [ ] 3.3 回归已有多轮对话场景，确认在状态文件正常存在且内容有效时，仍然可以成功恢复之前的对话状态。
