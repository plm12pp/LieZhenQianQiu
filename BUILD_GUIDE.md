# 列阵千秋 Unity 项目打包指导书

## 一、项目概述

**项目名称**: 列阵千秋 (LieZhenQianQiu)
**项目类型**: Unity回合制策略游戏
**Unity版本**: 推荐 Unity 2021.3 LTS 或 Unity 2022.3 LTS
**支持平台**: Windows / macOS / Android / iOS

---

## 二、环境准备

### 1. 安装Unity
- 从 [Unity官网](https://unity.com/download) 下载Unity Hub
- 通过Unity Hub安装Unity编辑器（推荐2021.3或2022.3 LTS版本）
- 安装时勾选以下模块：
  - Windows Build Support (IL2CPP)
  - Android Build Support (如需Android版)
  - iOS Build Support (如需iOS版)

### 2. 项目导入
1. 打开Unity Hub
2. 点击 "Open" 或 "Add"
3. 选择项目文件夹: `/列阵千秋`
4. 等待Unity导入资源（首次可能需要几分钟）

---

## 三、打包前检查清单

### 1. 资源检查
- [ ] 所有JSON数据文件位于 `Assets/Resources/Data/` 目录
- [ ] 图片资源已正确放入对应目录
- [ ] 音效和音乐文件已导入

### 2. 场景设置
确保以下场景已添加到Build Settings:
- MainMenu场景
- Battle场景
- Formation场景（如有独立场景）

### 3. 代码检查
- [ ] 所有脚本无编译错误
- [ ] 单例Manager正确初始化
- [ ] JSON数据正确加载

---

## 四、Windows平台打包步骤

### 步骤1: 打开Build Settings
1. 菜单栏: `File` → `Build Settings...`
2. 或快捷键: `Ctrl + Shift + B`

### 步骤2: 配置平台
1. 在Platform列表选择 **Windows**
2. 点击 "Switch Platform"（如当前非Windows）

### 步骤3: 添加场景
1. 打开所有需要的场景
2. 在Build Settings窗口点击 "Add Open Scenes"
3. 确保场景列表顺序正确（主菜单应为第一个）

### 步骤4: 设置参数
- **Target Platform**: Windows
- **Architecture**: x86_64 (64位)
- **Server Build**: 不勾选
- **Development Build**: 可勾选用于测试
- **Compression Method**: LZ4 (推荐)

### 步骤5: 打包
1. 点击 "Player Settings..." 检查以下设置：
   - Product Name: 列阵千秋
   - Company Name: 你的公司/团队名
   - Version: 1.0.0
   - Default Screen Width: 1920
   - Default Screen Height: 1080
   - Fullscreen Mode: Windowed

2. 回到Build Settings，点击 **Build**
3. 选择输出目录（如: `Builds/Windows/`)
4. 等待编译完成

### 步骤6: 运行测试
- 进入输出目录
- 运行 `列阵千秋.exe`
- 测试所有功能是否正常

---

## 五、Android平台打包步骤

### 步骤1: 安装Android模块
- Unity Hub → Installs → 选择Unity版本 → Add Modules
- 勾选 Android Build Support

### 步骤2: 配置Android SDK
- 设置Android SDK路径
- 菜单: `Edit` → `Preferences` → `External Tools`
- 填写Android SDK、NDK、JDK路径

### 步骤3: 配置打包参数
在Player Settings中设置:
- **Other Settings**:
  - Package Name: com.yourcompany.lzqq
  - Version: 1.0.0
  - Minimum API Level: Android 6.0 (API 23)
  - Target API Level: Automatic
  - Scripting Backend: IL2CPP
  - Target Architecture: ARM64

### 步骤4: 图标和屏幕适配
- 设置应用图标
- 配置屏幕分辨率和UI适配

### 步骤5: 生成APK/AAB
1. Build Settings → 选择 Android → Switch Platform
2. 选择 **Build App Bundle (Google Play)** 或 **Build APK**
3. 点击 **Build**
4. 输出文件: `列阵千秋.apk` 或 `列阵千秋.aab`

---

## 六、iOS平台打包步骤（需Mac）

### 步骤1: 准备Mac环境
- 必须在Mac上执行iOS打包
- 安装Xcode（最新版本）

### 步骤2: 配置iOS设置
在Player Settings中设置:
- **Other Settings**:
  - Bundle Identifier: com.yourcompany.lzqq
  - Version: 1.0.0
  - Target Device: iPhone + iPad
  - Target Resolution: High

### 步骤3: 生成Xcode项目
1. Build Settings → 选择 iOS → Switch Platform
2. 点击 **Build** 或 **Build and Run**
3. 输出Xcode项目文件

### 步骤4: 使用Xcode打包
1. 打开生成的Xcode项目
2. 配置签名证书
3. 选择目标设备进行Archive
4. 导出IPA文件

---

## 七、常见问题解决

### 问题1: 编译错误
- 检查所有脚本是否有语法错误
- 确保所有引用的组件存在
- 清理Library文件夹后重新打开项目

### 问题2: JSON数据加载失败
- 确认JSON文件位于 `Resources/Data/` 目录
- 检查JSON格式是否正确（无语法错误）
- 验证数据类结构是否匹配JSON字段

### 问题3: UI显示异常
- 检查Canvas设置是否正确
- 验证UI Scale Mode设置为 Scale With Screen Size
- 调整Reference Resolution为1920x1080

### 问题4: 内存占用过高
- 压缩纹理资源
- 使用Sprite Atlas合并小图片
- 优化音频资源大小

---

## 八、优化建议

### 1. 资源优化
- 纹理压缩格式: ASTC (Android) / PVRTC (iOS)
- 音频压缩: Vorbis / AAC
- 网格优化: 启用Mesh Compression

### 2. 代码优化
- 使用对象池减少GC
- 异步加载大型资源
- 减少每帧Update调用

### 3. 打包体积优化
- 剔除未使用资源
- 压缩AssetBundle
- 使用IL2CPP减少脚本体积

---

## 九、版本管理建议

### 使用Git管理项目
```bash
# 初始化仓库
git init

# 添加.gitignore（Unity专用）
# 忽略Library、Temp、Logs等目录

# 提交代码
git add .
git commit -m "初始版本"

# 推送到远程仓库
git remote add origin <仓库地址>
git push -u origin main
```

### 版本号规则
- 主版本号: 重大功能更新
- 次版本号: 功能添加
- 修订号: Bug修复

---

## 十、发布检查清单

### 发布前必检项目
- [ ] 游戏能正常启动和退出
- [ ] 主菜单所有按钮可点击
- [ ] 商店功能正常
- [ ] 抽卡功能正常
- [ ] 布阵界面可用
- [ ] 战斗过程无卡死
- [ ] 胜利/失败结算正常
- [ ] 音效和音乐正常播放
- [ ] 存档功能正常
- [ ] 设置功能正常

---

**文档版本**: 1.0
**更新日期**: 2026-06-27