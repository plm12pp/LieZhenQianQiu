# Unity Cloud Build - 完整操作指南

由于GitHub Actions的Unity镜像有限制，建议使用 **Unity Cloud Build** 官方服务。

---

## 方法一：Unity Cloud Build 网页操作

### 步骤1: 准备Unity Cloud账号
1. 访问: **https://cloud.unity.com/**
2. 使用Unity账号登录
3. 如果没有Organization，创建一个:
   - 点击右上角头像 → **Manage Organizations**
   - 点击 **Create Organization**
   - 填写名称（随意）

### 步骤2: 创建项目
1. 点击 **Projects** → **New Project**
2. 填写:
   - **Project Name**: `LieZhenQianQiu`
   - **Organization**: 选择您的组织
3. 点击 **CREATE**

### 步骤3: 连接GitHub
1. 进入项目 → **Settings** 标签
2. 滚动到 **Source Control**
3. 点击 **Connect GitHub**
4. 授权访问 `plm12pp/LieZhenQianQiu` 仓库
5. 选择 `main` 分支

### 步骤4: 添加Android构建目标
1. 进入 **Builds** 标签
2. 点击 **Add Build Target**
3. 选择平台: **Android**
4. 配置:
   - **Target**: Android
   - **Build Type**: Development (测试用)
   - **Auto-build**: 开启（可选）

### 步骤5: 触发构建
1. 点击 **START BUILD**
2. 等待构建完成（约10-20分钟）
3. 构建成功后点击 **Download Build**

---

## 方法二：使用 Unity Build Server (推荐用于生产环境)

如果需要更快的构建，可以订阅Unity Build Server:
- 访问: https://unity.com/services/build
- 按月计费，有更多构建配额

---

## 方法三：本地快速打包（最简单）

### 安装Unity (约30分钟)

1. **下载Unity Hub**
   - https://unity.com/download
   - 安装并打开

2. **安装Unity编辑器**
   - Unity Hub → **Installs** → **Add** → **Official releases**
   - 选择 **Unity 2021.3 LTS** 或 **Unity 2022.3 LTS**
   - ✅ 勾选 **Android Build Support**

3. **打开项目**
   ```bash
   git clone https://github.com/plm12pp/LieZhenQianQiu.git
   ```
   - Unity Hub → **Projects** → **Open** → 选择项目文件夹
   - 等待资源导入完成

4. **打包APK**
   - **File** → **Build Settings**
   - 选择 **Android** → 点击 **Switch Platform**
   - 点击 **Build**
   - 选择保存位置 → 等待完成

---

## 快速获取帮助

如果您在配置过程中遇到问题:
1. 查看构建日志具体错误信息
2. 访问Unity Answers: https://answers.unity.com/
3. 查看官方文档: https://docs.unity3d.com/

---

## APK下载后安装

1. 将APK文件传输到手机
2. 打开手机设置 → **安全** → 允许**未知来源**
3. 点击APK文件安装

**注意**: 首次安装可能需要卸载旧版本
