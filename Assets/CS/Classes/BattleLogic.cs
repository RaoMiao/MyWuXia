﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System;

namespace Game {
    public enum BattleProcessType {
        /// <summary>
        /// 普通行为
        /// </summary>
        [Description("普通行为")]
        Normal = 0,
        /// <summary>
        /// 攻击行为
        /// </summary>
        [Description("攻击行为")]
        Attack = 1,
        /// <summary>
        /// 对己方增益
        /// </summary>
        [Description("对己方增益")]
        Plus = 2,
        /// <summary>
        /// Buff/Debuff增减益行为
        /// </summary>
        [Description("Buff/Debuff增减益行为")]
        Increase = 3
    }
    /// <summary>
    /// 战斗过程类
    /// </summary>
    public class BattleProcess {
        /// <summary>
        /// 是否主队
        /// </summary>
        public bool IsTeam;
        /// <summary>
        /// 行为类型
        /// </summary>
        public BattleProcessType Type;
        /// <summary>
        /// 发招角色id
        /// </summary>
        public string RoleId;
        /// <summary>
        /// 易造成的伤害值
        /// </summary>
        public int HurtedHP;
        /// <summary>
        /// 是否闪避
        /// </summary>
        public bool IsMissed;
        /// <summary>
        /// 战报结果
        /// </summary>
        public string Result;
        public BattleProcess(bool isTeam, BattleProcessType type, string roleId, int hurtedHP, bool isMissed, string result) {
            IsTeam = isTeam;
            Type = type;
            RoleId = roleId;
            HurtedHP = hurtedHP;
            IsMissed = isMissed;
            Result = result;
        }
    }
    /// <summary>
    /// 战斗buff持续类
    /// </summary>
    public class BattleBuff {
        /// <summary>
        /// buff类型
        /// </summary>
        public BuffType Type;
        /// <summary>
        /// 持续时间
        /// </summary>
        public float Timeout;
        public BattleBuff(BuffType type, float timeout) {
            Type = type;
            Timeout = timeout;
        }
    }
    /// <summary>
    /// 战斗核心逻辑
    /// </summary>
    public class BattleLogic {
        static BattleLogic _instance = null;
        public static BattleLogic Instance {
            get { 
                if (_instance == null) {
                    _instance = new BattleLogic();
                }
                return _instance;
            }
        }

        /// <summary>
        /// 根据帧数换算成秒数
        /// </summary>
        /// <returns>The second.</returns>
        /// <param name="frame">Frame.</param>
        public static float GetSecond(long frame) {
            return (float)Statics.ClearError((double)frame * (double)Global.FrameCost, 10);
        }

        /// <summary>
        /// 是否自动战斗
        /// </summary>
        public bool AutoFight = false;

        public long Frame;
        bool paused = true;

        List<RoleData> teamsData;
        List<BuffData> teamBuffsData;
        RoleData teamRole;
        List<RoleData> enemysData;
        List<BuffData> enemyBuffsData;
        public RoleData CurrentEnemyRole;
        int currentEnemy;
        Queue<BattleProcess> battleProcessQueue;
        Queue<List<BattleBuff>> teamBuffsResultQueue; //主角buff总结果显示队列
        Queue<List<BattleBuff>> enemyBuffsResultQueue; //敌人buff总结果显示队列
        public BattleLogic() {
            battleProcessQueue = new Queue<BattleProcess>();
            teamBuffsResultQueue = new Queue<List<BattleBuff>>();
            enemyBuffsResultQueue = new Queue<List<BattleBuff>>();
        }

        /// <summary>
        /// 初始化战场
        /// </summary>
        /// <param name="teams">Teams.</param>
        /// <param name="enemys">Enemys.</param>
        public void Init(List<RoleData> teams, List<RoleData> enemys) {
            teamsData = teams;
            teamBuffsData = new List<BuffData>();
            enemysData = enemys;
            enemyBuffsData = new List<BuffData>();
            Frame = 0;
            //合并角色
            teamRole = new RoleData();
            teamRole.TeamName = "Team";
            teamRole.Name = "本方队伍";
            RoleData bindRole;
            for (int i = 0, len = teamsData.Count; i < len; i++) {
                bindRole = teamsData[i];
                bindRole.MakeJsonToModel();
                bindRole.TeamName = "Team";
                teamRole.MaxHP += bindRole.MaxHP;
                teamRole.HP += bindRole.HP;
                teamRole.MagicDefense += bindRole.MagicDefense;
                teamRole.PhysicsDefense += bindRole.PhysicsDefense;
                teamRole.Dodge += bindRole.Dodge;
                //初始化技能
                bindRole.GetCurrentBook().GetCurrentSkill().StartCD(Frame);
            }
            for (int i = 0, len = enemysData.Count; i < len; i++) {
                enemysData[i].MakeJsonToModel();
                enemysData[i].TeamName = "Enemy";
            }
            currentEnemy = 0;
            paused = false;
        }

        /// <summary>
        /// 判断是否胜利
        /// </summary>
        /// <returns><c>true</c> if this instance is window; otherwise, <c>false</c>.</returns>
        public bool IsWin() {
            int index = enemysData.FindIndex(item => item.HP > 0);
            return index < 0;
        }

        /// <summary>
        /// 判断是否失败
        /// </summary>
        /// <returns><c>true</c> if this instance is fail; otherwise, <c>false</c>.</returns>
        public bool IsFail() {
            return teamRole.HP <= 0;
        }

        /// <summary>
        /// 发招队列
        /// </summary>
        /// <param name="role">Role.</param>
        public void PushSkill(RoleData role) {
            if (IsFail() || IsWin() || !role.CanUseSkill) {
                return;
            }
            if (role.TeamName == "Team") {
                if (CurrentEnemyRole.HP > 0) {
                    role.GetCurrentBook().GetCurrentSkill().StartCD(Frame);
                    doSkill(role, CurrentEnemyRole);
                }
            } else {
                if (teamRole.HP > 0) {
                    role.GetCurrentBook().GetCurrentSkill().StartCD(Frame);
                    doSkill(role, teamRole);
                }
            }
        }

        /// <summary>
        /// 检测战斗指令
        /// </summary>
        /// <returns>The process.</returns>
        public BattleProcess PopProcess() {
            return battleProcessQueue.Count > 0 ? battleProcessQueue.Dequeue() : null;
        }

        /// <summary>
        /// 返回战斗指令长度
        /// </summary>
        /// <returns>The process count.</returns>
        public int GetProcessCount() {
            return battleProcessQueue.Count;
        }

        /// <summary>
        /// 检测主角buff/debuff追加结果
        /// </summary>
        /// <returns>The battle buff result.</returns>
        public List<BattleBuff> PopTeamBattleBuffResults() {
            return teamBuffsResultQueue.Count > 0 ? teamBuffsResultQueue.Dequeue() : null;
        }

        /// <summary>
        /// 检测敌人buff/debuff追加结果
        /// </summary>
        /// <returns>The battle buff result.</returns>
        public List<BattleBuff> PopEnemyBattleBuffResults() {
            return enemyBuffsResultQueue.Count > 0 ? enemyBuffsResultQueue.Dequeue() : null;
        }

        /// <summary>
        /// 生成本方buff追加返回结构
        /// </summary>
        void createTeamBattleBuffResult() {
            List<BattleBuff> buffResult = new List<BattleBuff>();
            BuffData buff;
            for (int i = 0, len = teamBuffsData.Count; i < len; i++) {
                buff = teamBuffsData[i];
                buffResult.Add(new BattleBuff(buff.Type, buff.Timeout));
            }
            teamBuffsResultQueue.Enqueue(buffResult);
        }

        /// <summary>
        /// 生成敌方debuff追加返回结构
        /// </summary>
        void createEnemyBattleBuffResult() {
            List<BattleBuff> deBuffResult = new List<BattleBuff>();
            BuffData buff;
            for (int i = 0, len = enemyBuffsData.Count; i < len; i++) {
                buff = enemyBuffsData[i];
                deBuffResult.Add(new BattleBuff(buff.Type, buff.Timeout));
            }
            enemyBuffsResultQueue.Enqueue(deBuffResult);
        }

//        /// <summary>
//        /// buff/debuff添加判定逻辑
//        /// </summary>
//        /// <param name="buffs">Buffs.</param>
//        /// <param name="toRole">To role.</param>
//        void dealBuff(List<BuffData> buffs, RoleData toRole) {
//            BuffData buff;
//            for (int i = 0, len = buffs.Count; i < len; i++) {
//                buff = buffs[i];
//                if (buff.IsTrigger()) {
//
//                }
//            }
//        }

        /// <summary>
        /// 主队监听器
        /// </summary>
        void teamsAction() {
            if (teamRole != null && teamRole.HP > 0) {
                //清空旧的buff和debuff
                teamRole.ClearPluses();
                for (int j = 0, len = teamsData.Count; j < len; j++) {
                    teamsData[j].ClearPluses();
                }
                BuffData curBuff;
                for (int i = teamBuffsData.Count - 1; i >= 0; i--) {
                    curBuff = teamBuffsData[i];
                    appendBuffParams(teamRole, curBuff);
                    for (int j = 0, len = teamsData.Count; j < len; j++) {
                        appendBuffParams(teamsData[j], curBuff);
                    }
                    if (curBuff.IsTimeout(Frame)) {
                        teamBuffsData.RemoveAt(i);
                    }
                }
            }
            //自动战斗检测
            if (AutoFight) {
                RoleData teamData;
                for (int i = 0, len = teamsData.Count; i < len; i++) {
                    teamData = teamsData[i];
                    if (teamData.HP > 0 && teamData.GetCurrentBook() != null && teamData.GetCurrentBook().GetCurrentSkill().IsCDTimeout(Frame)) {
                        PushSkill(teamData);
                    }
                }
            }
        }

        public void PopEnemy() {
            if (enemysData.Count > currentEnemy) {
                CurrentEnemyRole = enemysData[currentEnemy++];
                if (CurrentEnemyRole.GetCurrentBook() != null) {
                    CurrentEnemyRole.GetCurrentBook().GetCurrentSkill().StartCD(Frame);
                }
                battleProcessQueue.Enqueue(new BattleProcess(false, BattleProcessType.Normal, CurrentEnemyRole.Id, 0, false, string.Format("第{0}秒: {1}现身", GetSecond(Frame), CurrentEnemyRole.Name)));
                enemyBuffsData.Clear(); //清掉原有的debuff
                createEnemyBattleBuffResult();
            } else {
                CurrentEnemyRole = null;
            }
        }

        /// <summary>
        /// 敌人监听器
        /// </summary>
        void enemysAction() {
            if (CurrentEnemyRole == null || CurrentEnemyRole.HP <= 0) {
                PopEnemy();
                return;
            }
            if (CurrentEnemyRole != null && CurrentEnemyRole.HP > 0) {
                //清空旧的buff和debuff
                CurrentEnemyRole.ClearPluses();
                BuffData curBuff;
                for (int i = enemyBuffsData.Count - 1; i >= 0; i--) {
                    curBuff = enemyBuffsData[i];
                    appendBuffParams(CurrentEnemyRole, curBuff);
                    if (curBuff.IsTimeout(Frame)) {
                        enemyBuffsData.RemoveAt(i);
                    }
                }
                if (CurrentEnemyRole.GetCurrentBook() != null && CurrentEnemyRole.GetCurrentBook().GetCurrentSkill().IsCDTimeout(Frame)) {
                    PushSkill(CurrentEnemyRole);
                }
            }
        }

        string getBuffDesc(BuffData buff, string head = "自身") {
//            string rateStr = buff.Rate >= 100 ? "" : "<color=\"#A64DFF\">" + buff.Rate + "%</color>概率";
            string rateStr = "";
            string firstEffectStr = buff.FirstEffect ? "" : "下招起";
            string roundRumberStr;
            string roundRumberStr2;
            if (!buff.FirstEffect && buff.RoundNumber <= 0) {
                roundRumberStr = "<color=\"#B20000\">无效</color>";
                roundRumberStr2 = "<color=\"#B20000\">无效</color>";
            } 
            else {
                roundRumberStr = buff.Timeout <= 0 ? "" : (buff.Timeout + "秒");
                roundRumberStr2 = buff.Timeout <= 0 ? "" : "持续" + (buff.Timeout + "秒");
            }
            switch(buff.Type) {
                case BuffType.CanNotMove:
                    return string.Format("{0}{1}{2}<color=\"#FF9326\">定身</color>{3}", rateStr, firstEffectStr, head, roundRumberStr);
                case BuffType.Chaos:
                    return string.Format("{0}{1}{2}<color=\"#FF9326\">混乱</color>{3}", rateStr, firstEffectStr, head, roundRumberStr);
                case BuffType.Disarm:
                    return string.Format("{0}{1}{2}<color=\"#FF9326\">缴械</color>{3}", rateStr, firstEffectStr, head, roundRumberStr);
                case BuffType.Drug:
                    return string.Format("{0}{1}{2}<color=\"#FF9326\">中毒</color>{3}", rateStr, firstEffectStr, head, roundRumberStr);
                case BuffType.Fast:
                    return string.Format("{0}{1}{2}触发<color=\"#FF9326\">疾走(加速{3}%)</color>持续{4}", rateStr, firstEffectStr, head, Mathf.Abs((int)(buff.Value * 100 + 0.5d)), roundRumberStr);
                case BuffType.Slow:
                    return string.Format("{0}{1}{2}<color=\"#FF9326\">迟缓(减速{3}%)</color>{4}", rateStr, firstEffectStr, head, Mathf.Abs((int)(buff.Value * 100 + 0.5d)), roundRumberStr);
                case BuffType.Vertigo:
                    return string.Format("{0}{1}{2}<color=\"#FF9326\">眩晕</color>{3}", rateStr, firstEffectStr, head, roundRumberStr);
                case BuffType.CanNotMoveResistance:
                    return string.Format("{0}{1}<color=\"#FF9326\">免疫定身</color>持续{2}", rateStr, head, roundRumberStr);
                case BuffType.ChaosResistance:
                    return string.Format("{0}{1}<color=\"#FF9326\">免疫混乱</color>持续{2}", rateStr, head, roundRumberStr);
                case BuffType.DisarmResistance:
                    return string.Format("{0}{1}<color=\"#FF9326\">免疫缴械</color>持续{2}", rateStr, head, roundRumberStr);
                case BuffType.DrugResistance:
                    return string.Format("{0}{1}<color=\"#FF9326\">免疫中毒</color>持续{2}", rateStr, head, roundRumberStr);
                case BuffType.SlowResistance:
                    return string.Format("{0}{1}<color=\"#FF9326\">免疫迟缓</color>持续{2}", rateStr, head, roundRumberStr);
                case BuffType.VertigoResistance:
                    return string.Format("{0}{1}<color=\"#FF9326\">免疫眩晕</color>持续{2}", rateStr, head, roundRumberStr);
                case BuffType.ReboundInjury:
                    return string.Format("{0}<color=\"#FF9326\">{3}获得反伤效果(将受到伤害的{2}％反弹给对方)</color>持续{1}", rateStr, roundRumberStr, (int)(buff.Value * 100 + 0.5d), head);
                case BuffType.IncreaseDamageRate:
                    return string.Format("{0}{1}{2}{3}", rateStr, firstEffectStr, head + "<color=\"#FF4DFF\">最终伤害</color>" + (buff.Value > 0 ? "+" : "-") + Mathf.Abs((int)(buff.Value * 100 + 0.5d)) + "%", roundRumberStr2);
                case BuffType.IncreaseFixedDamage:
                    return string.Format("{0}{1}{2}{3}", rateStr, firstEffectStr, head + "<color=\"#FF4DFF\">固定伤害</color>" + (buff.Value > 0 ? "+" : "-") + Mathf.Abs((int)buff.Value), roundRumberStr2);
                case BuffType.IncreaseHP:
                    return string.Format("{0}{1}{2}{3}", rateStr, firstEffectStr, head + "<color=\"#00FF00\">气血值</color>" + (buff.Value > 0 ? "+" : "-") + Mathf.Abs((int)buff.Value), roundRumberStr2);
                case BuffType.IncreaseHurtCutRate:
                    return string.Format("{0}{1}{2}{3}", rateStr, firstEffectStr, head + "<color=\"#FF4DFF\">所受伤害</color>" + (buff.Value > 0 ? "+" : "-") + Mathf.Abs((int)(buff.Value * 100 + 0.5d)) + "%", roundRumberStr2);
                case BuffType.IncreaseMagicAttack:
                    return string.Format("{0}{1}{2}{3}", rateStr, firstEffectStr, head + "<color=\"#2693FF\">内功点数</color>" + (buff.Value > 0 ? "+" : "-") + Mathf.Abs((int)buff.Value), roundRumberStr2);
                case BuffType.IncreaseMagicAttackRate:
                    return string.Format("{0}{1}{2}{3}", rateStr, firstEffectStr, head + "<color=\"#2693FF\">内功比例</color>" + (buff.Value > 0 ? "+" : "-") + Mathf.Abs((int)(buff.Value * 100 + 0.5d)) + "%", roundRumberStr2);
                case BuffType.IncreaseMagicDefense:
                    return string.Format("{0}{1}{2}{3}", rateStr, firstEffectStr, head + "<color=\"#73B9FF\">内防点数</color>" + (buff.Value > 0 ? "+" : "-") + Mathf.Abs((int)buff.Value), roundRumberStr2);
                case BuffType.IncreaseMagicDefenseRate:
                    return string.Format("{0}{1}{2}{3}", rateStr, firstEffectStr, head + "<color=\"#73B9FF\">内防比例</color>" + (buff.Value > 0 ? "+" : "-") + Mathf.Abs((int)(buff.Value * 100 + 0.5d)) + "%", roundRumberStr2);
                case BuffType.IncreaseMaxHP:
                    return string.Format("{0}{1}{2}{3}", rateStr, firstEffectStr, head + "<color=\"#00FF00\">气血值上限</color>" + (buff.Value > 0 ? "+" : "-") + Mathf.Abs((int)buff.Value), roundRumberStr2);
                case BuffType.IncreaseMaxHPRate:
                    return string.Format("{0}{1}{2}{3}", rateStr, firstEffectStr, head + "<color=\"#00FF00\">气血值上限</color>" + (buff.Value > 0 ? "+" : "-") + Mathf.Abs((int)(buff.Value * 100 + 0.5d)) + "%", roundRumberStr2);
                case BuffType.IncreasePhysicsAttack:
                    return string.Format("{0}{1}{2}{3}", rateStr, firstEffectStr, head + "<color=\"#FF0000\">外功点数</color>" + (buff.Value > 0 ? "+" : "-") + Mathf.Abs((int)buff.Value), roundRumberStr2);
                case BuffType.IncreasePhysicsAttackRate:
                    return string.Format("{0}{1}{2}{3}", rateStr, firstEffectStr, head + "<color=\"#FF0000\">外功比例</color>" + (buff.Value > 0 ? "+" : "-") + Mathf.Abs((int)(buff.Value * 100 + 0.5d)) + "%", roundRumberStr2);
                case BuffType.IncreasePhysicsDefense:
                    return string.Format("{0}{1}{2}{3}", rateStr, firstEffectStr, head + "<color=\"#FF7373\">外防点数</color>" + (buff.Value > 0 ? "+" : "-") + Mathf.Abs((int)buff.Value), roundRumberStr2);
                case BuffType.IncreasePhysicsDefenseRate:
                    return string.Format("{0}{1}{2}{3}", rateStr, firstEffectStr, head + "<color=\"#FF7373\">外防比例</color>" + (buff.Value > 0 ? "+" : "-") + Mathf.Abs((int)(buff.Value * 100 + 0.5d)) + "%", roundRumberStr2);
                case BuffType.Normal:
                    return "无";
                default:
                    return "";
            }
        }

        void dealHP(RoleData role, int increaseHP) {
            role.HP += increaseHP;
            if (role.HP <= 0) {
                if (role.TeamName == "Team") {
                    battleProcessQueue.Enqueue(new BattleProcess(true, BattleProcessType.Normal, role.Id, 0, false, string.Format("第{0}秒: 技不如人, 全体侠客集体阵亡", GetSecond(Frame))));
                } else {
                    battleProcessQueue.Enqueue(new BattleProcess(false, BattleProcessType.Normal, role.Id, 0, false, string.Format("第{0}秒: {1}被击毙", GetSecond(Frame), role.Name)));
                }
            }
        }

        /// <summary>
        /// 处理buff和debuff的属性叠加
        /// </summary>
        /// <param name="teamName">Team name.</param>
        /// <param name="buff">Buff.</param>
        void appendBuffParams(RoleData role, BuffData buff) {
            switch (buff.Type) {
                case BuffType.Slow: //迟缓
                    role.AttackSpeedPlus -= role.AttackSpeed * buff.Value;
                    break;
                case BuffType.Fast: //疾走
                    role.AttackSpeedPlus += role.AttackSpeed * buff.Value;
                    break;
                case BuffType.Drug: //中毒
                    if (buff.IsSkipTimeout(Frame)) {
                        int cutHP = -(int)((float)role.HP * 0.1f);
                        battleProcessQueue.Enqueue(new BattleProcess(role.TeamName == "Team", BattleProcessType.Increase, role.Id, 0, false, string.Format("第{0}秒: {1}中毒, 损耗{2}点气血", GetSecond(Frame), role.Name, cutHP)));
                        dealHP(role, cutHP);
                    }
                    break;
                case BuffType.CanNotMove: //定身
                    role.CanChangeRole = false;
                    role.CanMiss = false;
                    break;
                case BuffType.Chaos: //混乱
                    role.CanNotMakeMistake = false;
                    break;
                case BuffType.Disarm: //缴械
                    role.CanUseSkill = false;
                    if (role.GetCurrentBook() != null) {
                        role.GetCurrentBook().GetCurrentSkill().ExtendOneFrameCD();
                    }
                    break;
                case BuffType.Vertigo: //眩晕
                    role.CanUseSkill = false;
                    role.CanChangeRole = false;
                    role.CanUseTool = false;
                    role.CanMiss = false;
                    if (role.GetCurrentBook() != null) {
                        role.GetCurrentBook().GetCurrentSkill().ExtendOneFrameCD();
                    }
                    break;
                case BuffType.IncreaseDamageRate: //增减益伤害比例
                    role.DamageRatePlus += (int)((float)role.DamageRate * buff.Value);
                    break;
                case BuffType.IncreaseFixedDamage: //增减益固定伤害
                    role.FixedDamagePlus += (int)buff.Value;
                    break;
                case BuffType.IncreaseHP: //增减益气血
                    if (buff.IsSkipTimeout(Frame)) {
                        int addHP = (int)buff.Value;
                        battleProcessQueue.Enqueue(new BattleProcess(role.TeamName == "Team", BattleProcessType.Increase, role.Id, 0, false, string.Format("第{0}秒: {1}{2}{3}点气血", GetSecond(Frame), role.Name, addHP > 0 ? "恢复" : "损耗", addHP)));
                        dealHP(role, addHP);
                    }
                    break;
                case BuffType.IncreaseMaxHP: //增减益气血上限
                    role.MaxHPPlus += (int)buff.Value;
                    break;
                case BuffType.IncreaseMaxHPRate: //增减益气血上限比例
                    role.MaxHPPlus += (int)((float)role.MaxHP * buff.Value);
                    break;
                case BuffType.IncreaseHurtCutRate: //增减减伤比例
                    role.HurtCutRatePlus += buff.Value;
                    break;
                case BuffType.IncreaseMagicAttack: //增减益内功点数
                    role.MagicAttackPlus += buff.Value;
                    break;
                case BuffType.IncreaseMagicAttackRate: //增减益内功比例
                    role.MagicAttackPlus += (role.MagicAttack * buff.Value);
                    break;
                case BuffType.IncreaseMagicDefense: //增减益内防点数
                    role.MagicDefensePlus += buff.Value;
                    break;
                case BuffType.IncreaseMagicDefenseRate: //增减益内防比例
                    role.MagicDefensePlus += (role.MagicDefense * buff.Value);
                    break;
                case BuffType.IncreasePhysicsAttack: //增减益外功点数
                    role.PhysicsAttackPlus += buff.Value;
                    break;
                case BuffType.IncreasePhysicsAttackRate: //增减益外功比例
                    role.PhysicsAttackPlus += (role.PhysicsAttack * buff.Value);
                    break;
                case BuffType.IncreasePhysicsDefense: //增减益外防点数
                    role.PhysicsDefensePlus += buff.Value;
                    break;
                case BuffType.IncreasePhysicsDefenseRate: //增减益外防比例
                    role.PhysicsDefensePlus += (role.PhysicsDefense * buff.Value);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// 发招
        /// </summary>
        /// <param name="roleRole">Role role.</param>
        /// <param name="toRole">To role.</param>
        void doSkill(RoleData fromRole, RoleData toRole) {
            BattleProcessType processType = BattleProcessType.Attack;
            int hurtedHP = 0;
            bool isMissed = false;
            string result = "";
            BookData currentBook = fromRole.GetCurrentBook();
            SkillData currentSkill = currentBook.GetCurrentSkill();

            if (currentSkill.Type == SkillType.Plus) {
                processType = BattleProcessType.Plus;
                result = string.Format("第{0}秒: {1}施展{2}", BattleLogic.GetSecond(Frame), fromRole.Name, currentBook.Name);
                //己方增益技能的技能施展通知要先于攻击类型技能,这样才能表现出先释放技能再回血的效果
                battleProcessQueue.Enqueue(new BattleProcess(fromRole.TeamName == "Team", processType, fromRole.Id, hurtedHP, isMissed, result)); //添加到战斗过程队列
            }

            //处理buff/debuff
            string buffDesc = "";
            List<BattleBuff> addBuffs = null;
            List<BattleBuff> addDeBuffs = null;
            if (!isMissed) {
                addBuffs = new List<BattleBuff>();
                addDeBuffs = new List<BattleBuff>();
                buffDesc = ", ";
                bool hasNewBuff = false;
                BuffData buff;
                List<BattleBuff> buffResult = new List<BattleBuff>();
                for(int i = 0, len = currentSkill.BuffDatas.Count; i < len; i++) {
                    buff = currentSkill.BuffDatas[i].GetClone(Frame);
                    if (buff.IsTrigger() && teamBuffsData.FindIndex(item => item.Type == buff.Type) < 0) {
                        if (buff.FirstEffect) {
                            appendBuffParams(teamRole, buff);
                        } else {
                            buff.IsSkipTimeout(Frame + 1); //不是立即执行的buff强制是间隔计时器启动
                        }
                        teamBuffsData.Add(buff);
                        addBuffs.Add(new BattleBuff(buff.Type, buff.RoundNumber));
//                        buffDesc += " " + getBuffDesc(buff, "自身") + ",";
                        hasNewBuff = true;
                        if (buff.Timeout > 0) {
                            buffResult.Add(new BattleBuff(buff.Type, buff.Timeout));
                        }
                    }
                }
                List<BattleBuff> deBuffResult = new List<BattleBuff>();
                for(int i = 0, len = currentSkill.DeBuffDatas.Count; i < len; i++) {
                    buff = currentSkill.DeBuffDatas[i].GetClone(Frame);
                    if (buff.IsTrigger() && enemyBuffsData.FindIndex(item => item.Type == buff.Type) < 0) {
                        if (buff.FirstEffect) {
                            appendBuffParams(CurrentEnemyRole, buff);
                        } else {
                            buff.IsSkipTimeout(Frame + 1); //不是立即执行的debuff强制是间隔计时器启动
                        }
                        enemyBuffsData.Add(buff);
                        addDeBuffs.Add(new BattleBuff(buff.Type, buff.RoundNumber));
                        buffDesc += " " + getBuffDesc(buff, "致敌") + ",";
                        hasNewBuff = true;
                        if (buff.Timeout > 0) {
                            deBuffResult.Add(new BattleBuff(buff.Type, buff.Timeout));
                        }
                    }
                }
                if (buffDesc.Length > 1) {
                    buffDesc = buffDesc.Remove(buffDesc.Length - 1, 1);
                }
                if (addBuffs.Count > 0) {
                    //                    createTeamBattleBuffResult(); 
                    teamBuffsResultQueue.Enqueue(buffResult); //通知本方buff变更
                }
                if (addDeBuffs.Count > 0) {
                    //                    createEnemyBattleBuffResult(); 
                    enemyBuffsResultQueue.Enqueue(deBuffResult); //通知地方debuff变更
                }
            }

            //处理攻击伤害
            switch (currentSkill.Type) {
                case SkillType.FixedDamage:
                    hurtedHP = -fromRole.FixedDamage;
                    result = string.Format("第{0}秒: {1}施展{2}, 造成{3}点固定伤害", BattleLogic.GetSecond(Frame), fromRole.Name, currentBook.Name, hurtedHP);
                    break;
                case SkillType.MagicAttack:
                    if (!toRole.CanMiss || fromRole.IsHited(toRole)) {
                        hurtedHP = -fromRole.GetMagicDamage(toRole);
                        result = string.Format("第{0}秒: {1}施展{2}, 造成{3}点内功伤害", BattleLogic.GetSecond(Frame), fromRole.Name, currentBook.Name, hurtedHP);
                    } else {
                        isMissed = true;
                        result = string.Format("第{0}秒: {1}施展{2}, {3}", BattleLogic.GetSecond(Frame), fromRole.Name, currentBook.Name, "被对手闪躲");
                    }
                    break;
                case SkillType.PhysicsAttack:
                    if (!toRole.CanMiss || fromRole.IsHited(toRole)) {
                        hurtedHP = -fromRole.GetPhysicsDamage(toRole);
                        result = string.Format("第{0}秒: {1}施展{2}, 造成{3}点外功伤害", BattleLogic.GetSecond(Frame), fromRole.Name, currentBook.Name, hurtedHP);
                    } else {
                        isMissed = true;
                        result = string.Format("第{0}秒: {1}施展{2}, {3}", BattleLogic.GetSecond(Frame), fromRole.Name, currentBook.Name, "被对手闪躲");
                    }
                    break;
                default:
                    
                    break;
            }
            //攻击类型技能通知要放到最后面,这样才能保证基础属性增益或者建议的基础上计算伤害
            if (currentSkill.Type != SkillType.Plus) {
                result = string.Format("{0}{1}", result, buffDesc);
                battleProcessQueue.Enqueue(new BattleProcess(fromRole.TeamName == "Team", processType, fromRole.Id, hurtedHP, isMissed, result)); //添加到战斗过程队列
                //处理扣血
                if (hurtedHP < 0) {
                    dealHP(toRole, hurtedHP);
                }
            }
        }

        /// <summary>
        /// 逻辑监听器
        /// </summary>
        public void Action() {
            if (paused) {
                return;
            }
            if (IsFail()) {
                paused = true;
                return;
            }
            if (IsWin()) {
                paused = true;
                return;
            }
            //战斗超过3分钟强制主角输
            if (GetSecond(Frame) >= 180) {
                teamRole.HP = 0;
                battleProcessQueue.Enqueue(new BattleProcess(false, BattleProcessType.Normal, CurrentEnemyRole.Id, 0, false, string.Format("第{0}秒: 时间结束", GetSecond(Frame))));
                return;
            }
            //核心逻辑
            teamsAction();
            enemysAction();
            Frame++;
        }
    }   
    
}
