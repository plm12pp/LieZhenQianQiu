# Unity Cloud Build 配置指南

本指南将帮助您在 Unity Cloud Build 上配置项目并生成 APK。

## 前提条件

1. **Unity 账号**: https://id.unity.com
2. **GitHub 账号**: 已连接到项目
3. **Unity Cloud 订阅**: 免费版即可（有限制）

---

## 步骤 1: 访问 Unity Cloud Build

1. 打开浏览器访问: **https://cloud.unity.com/**
2. 点击 **Sign In** 登录您的 Unity 账号
3. 授权 Unity 访问您的 GitHub 仓库

---

## 步骤 2: 创建 Organization（如果没有）

1. Unity Cloud 需要 Organization
2. 访问: https://cloud.unity.com/organizations
3. 点击 **Create Organization**
4. 填写组织名称（随意）
5. 选择 **Free** 计划

---

## 步骤 3: 创建 Cloud Build 项目

1. 访问: **https://cloud.unity.com/**
2. 点击 **New Project**
3. 填写项目信息:
   - **Project Name**: 列阵千秋
   - **Organization**: 选择您的组织
4. 点击 **CREATE PROJECT**

---

## 步骤 4: 连接 GitHub 仓库

1. 进入项目后，点击 **Settings** 标签
2. 找到 **Source Control** 部分
3. 点击 **Connect GitHub**
4. 选择仓库: `plm12pp/LieZhenQianQiu`
5. 选择分支: `main`

---

## 步骤 5: 配置 Android 构建

1. 进入 **Build** 标签
2. 点击 **Add Build Target**
3. 选择平台: **Android**
4. 配置构建设置:

```json
{
  "platform": "android",
  "branch": "main",
  "autoBuild": true,
  "buildTarget": {
    "platform": "Android",
    "architecture": "arm64-v8a",
    "buildType": "debug",
    "minSdkVersion": 24,
    "targetSdkVersion": 34
  }
}
```

---

## 步骤 6: 触发构建

1. 确保代码已推送到 GitHub
2. 点击 **Start Build** 或 **Build Now**
3. 等待构建完成（约5-15分钟）

---

## 步骤 7: 下载 APK

构建完成后:
1. 进入 **Builds** 列表
2. 找到成功的构建
3. 点击 **Artifacts**
4. 下载 `.apk` 文件

---

## 自动化构建

启用自动构建:
1. 在项目设置中开启 **Auto-build**
2. 每次推送到 main 分支自动触发构建

---

## 常见问题

### Q: 构建失败怎么办？
A: 查看构建日志，通常是缺少场景文件或材质引用错误

### Q: 免费版有构建配额吗？
A: 免费版每月有构建分钟数限制

### Q: 如何加速构建？
A: 购买 Unity Pro 订阅或优化项目大小

---

## 替代方案: Unity加速器

如果 Cloud Build 太慢，可以尝试:
1. **Unity Build Server**: https://unity.com/services/build
2. **GitHub Actions + Unity**: 使用CI/CD自动构建
3. **本地构建**: 在本地电脑安装Unity

---

**提示**: 如果您需要更简单的方案，可以考虑使用 GitHub Actions 来构建Unity项目。
