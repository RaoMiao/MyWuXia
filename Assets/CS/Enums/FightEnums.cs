﻿using UnityEngine;
using System.Collections;
using System.ComponentModel;

namespace Game {
	/// <summary>
	/// 门派类型
	/// </summary>
	public enum OccupationType {
		/// <summary>
		/// 无门无派
		/// </summary>
		[Description("无门无派")]
		None,
		/// <summary>
		/// 丐帮
		/// </summary>
		[Description("丐帮")]
		GaiBang,
		/// <summary>
		/// 少林
		/// </summary>
		[Description("少林寺")]
		ShaoLin,
		/// <summary>
		/// 全真
		/// </summary>
		[Description("全真教")]
		QuanZhen,
		/// <summary>
		/// 逍遥
		/// </summary>
		[Description("逍遥派")]
		XiaoYao,
		/// <summary>
		/// 大理
		/// </summary>
		[Description("大理段氏")]
		DaLi,
		/// <summary>
		/// 岳家军
		/// </summary>
		[Description("岳家军")]
		YueJiaJun,
		/// <summary>
		/// 神兵营
		/// </summary>
		[Description("神兵营")]
		ShenBingYing,
		/// <summary>
		/// 唐门
		/// </summary>
		[Description("唐门")]
		TangMen,
		/// <summary>
		/// 峨眉
		/// </summary>
		[Description("峨眉")]
		EMei,
		/// <summary>
		/// 武当
		/// </summary>
		[Description("武当")]
		WuDang,
		/// <summary>
		/// 江南七怪
		/// </summary>
		[Description("江南七怪")]
		JiangNan7Guai,
		/// <summary>
		/// 金刚宗
		/// </summary>
		[Description("金刚宗")]
		JinGangZong,
		/// <summary>
		/// 桃花岛
		/// </summary>
		[Description("桃花岛")]
		TaoHuaDao,
		/// <summary>
		/// 古墓派
		/// </summary>
		[Description("古墓派")]
		GuMu,
		/// <summary>
		/// 白驼山
		/// </summary>
		[Description("白驼山")]
		BaiTuoShan
	}

	/// <summary>
	/// 伤势类型
	/// </summary>
	public enum InjuryType {
		/// <summary>
		/// 健康
		/// </summary>
		[Description("<color=\"#00FF00\">健康</color>")]
		None = 0,
		/// <summary>
		/// 白伤(全属性降低10%)
		/// </summary>
		[Description("<color=\"#AAAAAA\">白伤</color>")]
		White = 1,
		/// <summary>
		/// 黄伤(全属性降低20%)
		/// </summary>
		[Description("<color=\"#FFFF00\">黄伤</color>")]
		Yellow = 2,
		/// <summary>
		/// 紫伤(全属性降低40%)
		/// </summary>
		[Description("<color=\"#C709F7\">紫伤</color>")]
		Purple = 3,
		/// <summary>
		/// 红伤(全属性降低80%)
		/// </summary>
		[Description("<color=\"#FF0000\">红伤</color>")]
		Red = 4,
		/// <summary>
		/// 垂死(不能出战)
		/// </summary>
		[Description("<color=\"#BB4444\">垂死</color>")]
		Moribund = 5
	}

	/// <summary>
	/// buff类型(buff/debuff通用)
	/// </summary>
	public enum BuffType {
		/// <summary>
		/// 无
		/// </summary>
		[Description("无")]
		Normal = 0,
		/// <summary>
		/// 增减益气血
		/// </summary>
		[Description("增减益气血")]
		IncreaseHP = 1,
		/// <summary>
		/// 增减益气血上限
		/// </summary>
		[Description("增减益气血上限")]
		IncreaseMaxHP = 2,
		/// <summary>
		/// 增减益气血上限比例
		/// </summary>
		[Description("增减益气血上限比例")]
		IncreaseMaxHPRate = 3,
		/// <summary>
		/// 增减益外功点数
		/// </summary>
		[Description("增减益外功点数")]
		IncreasePhysicsAttack = 4,
		/// <summary>
		/// 增减益外功比例
		/// </summary>
		[Description("增减益外功比例")]
		IncreasePhysicsAttackRate = 5,
		/// <summary>
		/// 增减益外防点数
		/// </summary>
		[Description("增减益外防点数")]
		IncreasePhysicsDefense = 6,
		/// <summary>
		/// 增减益外防比例
		/// </summary>
		[Description("增减益外防比例")]
		IncreasePhysicsDefenseRate = 7,
		/// <summary>
		/// 增减益内功点数
		/// </summary>
		[Description("增减益内功点数")]
		IncreaseMagicAttack = 8,
		/// <summary>
		/// 增减益内功比例
		/// </summary>
		[Description("增减益内功比例")]
		IncreaseMagicAttackRate = 9,
		/// <summary>
		/// 增减益内防点数
		/// </summary>
		[Description("增减益内防点数")]
		IncreaseMagicDefense = 10,
		/// <summary>
		/// 增减益内防比例
		/// </summary>
		[Description("增减益内防比例")]
		IncreaseMagicDefenseRate = 11,
		/// <summary>
		/// 增减益固定伤害值
		/// </summary>
		[Description("增减益固定伤害值")]
		IncreaseFixedDamage = 12,
		/// <summary>
		/// 增减益伤害比例
		/// </summary>
		[Description("增减益伤害比例")]
		IncreaseDamageRate = 13,
		/// <summary>
		/// 增减益减伤比例
		/// </summary>
		[Description("增减益减伤比例")]
		IncreaseHurtCutRate = 14,
		/// <summary>
		/// 中毒(持续掉血10%)
		/// </summary>
		[Description("中毒(持续掉血10%)")]
		Drug = 15,
		/// <summary>
		/// 缴械(不能释放技能，可以切换门客，可以闪避)
		/// </summary>
		[Description("缴械(不能释放技能，可以切换门客，可以闪避)")]
		Disarm = 16,
		/// <summary>
		/// 眩晕(当靶子)
		/// </summary>
		[Description("眩晕(当靶子)")]
		Vertigo = 17,
		/// <summary>
		/// 定身(可以释放技能，不能切换门客)
		/// </summary>
		[Description("定身(可以释放技能，不能切换门客)")]
		CanNotMove = 18,
		/// <summary>
        /// 迟缓(百分比降低轻功)
		/// </summary>
		[Description("迟缓(百分比降低轻功)")]
		Slow = 19,
		/// <summary>
		/// 混乱(自己出招有50%概率攻击自己)
		/// </summary>
		[Description("混乱(自己出招有50%概率攻击自己)")]
		Chaos = 20,
		/// <summary>
        /// 疾走(百分比增加轻功)
		/// </summary>
        [Description("疾走(百分比增加轻功)")]
		Fast = 21,
		/// <summary>
		/// 中毒抵抗
		/// </summary>
		[Description("中毒抵抗")]
		DrugResistance = 22,
		/// <summary>
		/// 缴械抵抗
		/// </summary>
		[Description("缴械抵抗")]
		DisarmResistance = 23,
		/// <summary>
		/// 眩晕抵抗
		/// </summary>
		[Description("眩晕抵抗")]
		VertigoResistance = 24,
		/// <summary>
		/// 定身抵抗
		/// </summary>
		[Description("定身抵抗")]
		CanNotMoveResistance = 25,
		/// <summary>
		/// 迟缓抵抗
		/// </summary>
		[Description("迟缓抵抗")]
		SlowResistance = 26,
		/// <summary>
		/// 混乱抵抗
		/// </summary>
		[Description("混乱抵抗")]
		ChaosResistance = 27,
		/// <summary>
		/// 百分比反弹伤害
		/// </summary>
		[Description("百分比反弹伤害")]
		ReboundInjury = 28,
        /// <summary>
        /// 惊慌(不能吃药)
        /// </summary>
        [Description("惊慌(不能吃药)")]
        Alarmed = 29,
        /// <summary>
        /// 惊慌抵抗
        /// </summary>
        [Description("惊慌抵抗")]
        AlarmedResistance = 30,
        /// <summary>
        /// 一次性恢复气血上限百分比的气血
        /// </summary>
        [Description("一次性恢复气血上限百分比的气血")]
        AddRateMaxHP = 31,
        /// <summary>
        /// 清除所有的debuff
        /// </summary>
        [Description("清除所有的debuff")]
        ClearDebuffs = 32,
        /// <summary>
        /// 无敌(无视所有伤害包括debuff)
        /// </summary>
        [Description("无敌(无视所有伤害包括debuff)")]
        Invincible = 33,
        /// <summary>
        /// 必闪(100%闪避开一切招式)
        /// </summary>
        [Description("必闪(100%闪避开一切招式)")]
        MustMiss = 34,
        /// <summary>
        /// 吸血
        /// </summary>
        [Description("吸血")]
        SuckHP = 35,
        /// <summary>
        /// 无我(无视一切负面debuff)
        /// </summary>
        [Description("无我(无视一切负面debuff)")]
        ForgotMe = 36,
        /// <summary>
        /// 解毒(中毒时将损失的气血变为回复的气血)
        /// </summary>
        [Description("解毒(中毒时将损失的气血变为回复的气血)")]
        SolveDrug = 37,
        /// <summary>
        /// 致盲(武功cd时间增加n%)
        /// </summary>
        [Description("致盲(武功cd时间增加n%)")]
        Blindness = 38,
        /// <summary>
        /// 增加debuff时间
        /// </summary>
        [Description("增加debuff时间")]
        MakeDebuffStrong = 39,
        /// <summary>
        /// 武功CD时间瞬间清零
        /// </summary>
        [Description("武功CD时间瞬间清零")]
        CDTimeout = 40,
        /// <summary>
        /// 武功CD时间减少n%
        /// </summary>
        [Description("武功CD时间减少n%")]
        UpSpeedCDTime = 41
	}

	public enum SkillType {
		/// <summary>
		/// 对己方增益
		/// </summary>
		[Description("对己方增益")]
		Plus,
		/// <summary>
		/// 外功伤害
		/// </summary>
		[Description("外功伤害")]
		PhysicsAttack,
		/// <summary>
		/// 内功伤害
		/// </summary>
		[Description("内功伤害")]
		MagicAttack,
		/// <summary>
		/// 固定伤害
		/// </summary>
		[Description("固定伤害")]
		FixedDamage
	}

	public enum FightType {
		/// <summary>
		/// 普通战斗(普通流程出发的战斗,如野外踩地雷或野外手动触发的怪物战斗等)
		/// </summary>
		[Description("普通战斗(普通流程出发的战斗,如野外踩地雷或野外手动触发的怪物战斗等)")]
		Normal,
		/// <summary>
		/// 任务战斗(任务的其中一环战斗)
		/// </summary>
		[Description("任务战斗(任务的其中一环战斗)")]
		Task,
		/// <summary>
		/// 城镇场景战斗(永久存在于场景中的战斗)
		/// </summary>
		[Description("城镇场景战斗")]
		Scene
	}

    /// <summary>
    /// 诀要类型
    /// </summary>
    public enum SecretType {
        /// <summary>
        /// 无
        /// </summary>
        [Description("无")]
        None = 0,
        /// <summary>
        /// 增减益气血上限
        /// </summary>
        [Description("增减益气血上限")]
        IncreaseMaxHP = 1,
        /// <summary>
        /// 增减益气血上限比例
        /// </summary>
        [Description("增减益气血上限比例")]
        IncreaseMaxHPRate = 2,
        /// <summary>
        /// 增减益外功点数
        /// </summary>
        [Description("增减益外功点数")]
        IncreasePhysicsAttack = 3,
        /// <summary>
        /// 增减益外功比例
        /// </summary>
        [Description("增减益外功比例")]
        IncreasePhysicsAttackRate = 4,
        /// <summary>
        /// 增减益外防点数
        /// </summary>
        [Description("增减益外防点数")]
        IncreasePhysicsDefense = 5,
        /// <summary>
        /// 增减益外防比例
        /// </summary>
        [Description("增减益外防比例")]
        IncreasePhysicsDefenseRate = 6,
        /// <summary>
        /// 增减益内功点数
        /// </summary>
        [Description("增减益内功点数")]
        IncreaseMagicAttack = 7,
        /// <summary>
        /// 增减益内功比例
        /// </summary>
        [Description("增减益内功比例")]
        IncreaseMagicAttackRate = 8,
        /// <summary>
        /// 增减益内防点数
        /// </summary>
        [Description("增减益内防点数")]
        IncreaseMagicDefense = 9,
        /// <summary>
        /// 增减益内防比例
        /// </summary>
        [Description("增减益内防比例")]
        IncreaseMagicDefenseRate = 10,
        /// <summary>
        /// 增减益固定伤害值
        /// </summary>
        [Description("增减益固定伤害值")]
        IncreaseFixedDamage = 11,
        /// <summary>
        /// 增减益伤害比例
        /// </summary>
        [Description("增减益伤害比例")]
        IncreaseDamageRate = 12,
        /// <summary>
        /// 增减益减伤比例
        /// </summary>
        [Description("增减益减伤比例")]
        IncreaseHurtCutRate = 13,
        /// <summary>
        /// 增减益轻功点数
        /// </summary>
        [Description("增减益轻功点数")]
        IncreaseDodge = 14,
        /// <summary>
        /// 中毒抵抗
        /// </summary>
        [Description("中毒抵抗")]
        DrugResistance = 15,
        /// <summary>
        /// 缴械抵抗
        /// </summary>
        [Description("缴械抵抗")]
        DisarmResistance = 16,
        /// <summary>
        /// 眩晕抵抗
        /// </summary>
        [Description("眩晕抵抗")]
        VertigoResistance = 17,
        /// <summary>
        /// 定身抵抗
        /// </summary>
        [Description("定身抵抗")]
        CanNotMoveResistance = 18,
        /// <summary>
        /// 迟缓抵抗
        /// </summary>
        [Description("迟缓抵抗")]
        SlowResistance = 19,
        /// <summary>
        /// 混乱抵抗
        /// </summary>
        [Description("混乱抵抗")]
        ChaosResistance = 20,
        /// <summary>
        /// 惊慌抵抗
        /// </summary>
        [Description("惊慌抵抗")]
        AlarmedResistance = 21,
        /// <summary>
        /// 减少武功招式CD时间
        /// </summary>
        [Description("减少武功招式CD时间")]
        CutCD = 22,
        /// <summary>
        /// 抵御n次阵亡效果
        /// </summary>
        [Description("抵御n次阵亡效果")]
        Immortal = 23,
        /// <summary>
        /// n%概率秒杀敌方(对Boss无效)
        /// </summary>
        [Description("n%概率秒杀敌方(对Boss无效)")]
        Killed = 24,
        /// <summary>
        /// 增加掉落概率n%
        /// </summary>
        [Description("增加掉落概率n%")]
        MakeAFortune = 25,
        /// <summary>
        /// 增加气血恢复点数
        /// </summary>
        [Description("增加气血恢复点数")]
        PlusIncreaseHP = 26
    }

    /// <summary>
    /// 兵器buff类型
    /// </summary>
    public enum WeaponBuffType {
        /// <summary>
        /// 无
        /// </summary>
        [Description("无")]
        None = 0,
        /// <summary>
        /// 气血每降低x%基础外功增加y%
        /// </summary>
        [Description("啸天狂龙0")]
        PAUpWhenHPDown = 1,
        /// <summary>
        /// 生命低于30%时附加反伤x%伤害效果
        /// </summary>
        [Description("啸天狂龙1")]
        ReboundInjuryWhenHPDown = 2,
        /// <summary>
        /// 每次攻击x%概率基础外功增加100%，最高叠加至500%
        /// </summary>
        [Description("神威")]
        PAMultipleIncrease = 3,
        /// <summary>
        /// x%概率触发无敌气墙，持续y秒，cd20秒
        /// </summary>
        [Description("伏虎")]
        InvincibleWall = 4,
        /// <summary>
        /// 敌人闪避后增加基础内功x%，最高叠加至y%，命中敌人后内功增益消失
        /// </summary>
        [Description("承影")]
        MAMultipleIncreaseWhenBeMissed = 5,
        /// <summary>
        /// x%概率触发攻击吸收气墙，气墙回血一次后消失，cd30秒
        /// </summary>
        [Description("清音")]
        AttackAbsorption = 6,
        /// <summary>
        /// 对处于免疫攻击状态下的敌人造成大幅伤害（基础内功提高600%）
        /// </summary>
        [Description("天谴")]
        BreachAttack = 7
    }
}
