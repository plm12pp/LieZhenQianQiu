# 列阵千秋 (LieZhenQianQiu)

一款基于Unity的回合制策略游戏，以中国历史为背景，融合兵种克制、阵型系统和技能机制。

## 游戏特色

### 54个历史兵种
覆盖商、周、春秋、战国、秦、汉、三国、晋、南北朝、隋、唐、宋、金、元、明等朝代：

- **步兵类**: 刀兵、枪兵、盾兵、斧兵、青州兵、技击之士、秦锐士、陌刀队等
- **骑兵类**: 轻骑、重骑、铁浮屠、玄甲军、白马义从、西凉铁骑、蒙古骑射等
- **远程类**: 弓兵、弩兵、连弩车阵、秦弓手等
- **攻城类**: 投石车、虎蹲炮、神威炮阵、艨艟斗舰等
- **特殊类**: 火攻队、藤甲兵、象兵战阵、锦衣卫等

### 8种经典阵型
- 鹤翼阵、鱼鳞阵、雁行阵、方圆阵、方阵、长蛇阵、天地阵等

### 40+种技能效果
- 冲锋、溅射、燃烧、暴击、光环、破甲、骑射等

## 项目结构

```
列阵千秋/
├── Assets/
│   ├── Resources/
│   │   └── Data/
│   │       ├── Troops.json       # 兵种数据 (54个)
│   │       ├── Levels.json       # 关卡数据
│   │       ├── ShopPools.json    # 商店池数据
│   │       └── Achievements.json # 成就数据
│   └── Scripts/
│       ├── Battle/               # 战斗系统
│       │   ├── UnitController.cs
│       │   ├── BattleManager.cs
│       │   ├── FormationManager.cs
│       │   ├── BattleGrid.cs
│       │   ├── AIBattleController.cs
│       │   └── BattleEffectsManager.cs  # 粒子特效系统
│       ├── Core/                 # 核心管理器
│       │   ├── GameManager.cs
│       │   ├── CurrencyManager.cs
│       │   ├── SaveManager.cs
│       │   ├── ResourceManager.cs      # 资源管理
│       │   ├── SoundManager.cs         # 音效管理
│       │   └── BattleStatisticsManager.cs
│       ├── Data/                 # 数据结构
│       ├── UI/                   # 用户界面
│       │   ├── UIManager.cs
│       │   ├── TroopCardUI.cs
│       │   ├── FormationSelectUI.cs
│       │   ├── TroopDetailUI.cs
│       │   ├── BattleResultUI.cs
│       │   ├── GachaUI.cs
│       │   ├── AnimatedButton.cs
│       │   └── LoadingScreenUI.cs
│       └── Utilities/             # 工具类
│           ├── TroopIconGenerator.cs    # 兵种图标生成
│           ├── UITextureGenerator.cs    # UI纹理生成
│           └── BackgroundTextureGenerator.cs
├── BUILD_GUIDE.md               # 打包指导书
└── README.md                    # 项目说明
```

## 程序化美术资源

项目使用代码自动生成所有美术资源，无需外部图片：

### 兵种图标 (TroopIconGenerator.cs)
- 自动识别兵种类型生成对应图标
- 支持步兵、骑兵、弓兵、器械、水军等类型
- 每个兵种生成64/128/256三种尺寸

### UI纹理 (UITextureGenerator.cs)
- 渐变按钮背景
- 面板背景和对话框
- 进度条和货币图标
- 星形评级图标
- 战场网格和河流

### 粒子特效 (BattleEffectsManager.cs)
- 火焰、冰冻、雷电效果
- 暴击、冲锋、溅射特效
- 治疗和护盾光环
- 伤害数字飘字
- 屏幕震动效果

### 背景纹理 (BackgroundTextureGenerator.cs)
- 战场草地/土地背景
- 主菜单渐变背景
- 多种地形纹理（平原、沙漠、山地、雪地、森林）
- 地图瓦片

### 程序化音效 (SoundManager.cs)
- 点击、选中音效
- 攻击、命中音效
- 胜利、失败音效
- 金币、抽卡音效
- 程序化生成的8-bit风格音效

## 核心系统

### 战斗机制
- **兵种克制**: 骑兵克弓兵，枪兵克骑兵，弓兵克盾兵等
- **星级加成**: 1星至5星，属性倍率1.0x至1.72x
- **阵型加成**: 不同阵型提供不同兵种属性提升
- **技能系统**: 主动/被动技能，包含伤害加成、范围攻击、状态效果等

### 抽卡系统
- **普通招募**: 铜钱消耗，普通池
- **精锐招募**: 兵符消耗，稀有池
- **名将招募**: 玉令消耗，传说池
- **保底机制**: 10抽保底稀有，20抽保底传说

### 升星系统
- 消耗同名兵种或碎片升星
- 星级越高属性越强

## 技术栈

- **游戏引擎**: Unity 2021.3 LTS / Unity 2022.3 LTS
- **编程语言**: C#
- **数据格式**: JSON
- **渲染**: 2D系统
- **音效**: 程序化生成AudioClip

## 快速开始

### 1. 克隆项目
```bash
git clone https://github.com/plm12pp/LieZhenQianQiu.git
```

### 2. 打开项目
- 使用Unity Hub打开项目文件夹
- 等待资源导入完成

### 3. 运行游戏
- 打开主菜单场景
- 点击Play按钮运行

### 4. 打包发布
参考 [BUILD_GUIDE.md](BUILD_GUIDE.md) 进行打包

## 数据文件说明

### Troops.json
```json
{
  "id": "troop_spear",
  "name": "枪兵",
  "dynasty": "汉",
  "role": "反骑步兵",
  "rarity": "普通",
  "hp": 80,
  "atk": 24,
  "range": 1.5,
  "speed": 3.5,
  "armor": 9,
  "counterBonus": ["light_cavalry", "heavy_cavalry"],
  "counterMultiplier": 1.35,
  "skillEffect": "armor_pierce"
}
```

## 贡献指南

1. Fork本仓库
2. 创建特性分支 (`git checkout -b feature/AmazingFeature`)
3. 提交更改 (`git commit -m '添加某某功能'`)
4. 推送到分支 (`git push origin feature/AmazingFeature`)
5. 提交Pull Request

## 版本历史

- **v2.0** - 添加完整程序化美术资源
  - 兵种图标生成器
  - UI纹理生成器
  - 粒子特效系统
  - 背景纹理生成器
  - 程序化音效系统
  - 资源管理器统一管理

- **v1.0** - 初始版本，54个兵种，完整战斗系统
- 移除清朝兵种，新增技击之士、陌刀队、无当飞军

## 许可证

本项目仅供学习和研究使用。

---

**作者**: TRAE
**更新日期**: 2026-06-27