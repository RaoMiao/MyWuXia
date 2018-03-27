﻿using UnityEngine;
using System.Collections;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Game {
	public partial class NotifyTypes {
		/// <summary>
		/// 请求场景内的任务列表
		/// </summary>
		public static string GetTaskListDataInCityScene;
		/// <summary>
		/// 请求场景内的任务列表回调
		/// </summary>
		public static string GetTaskListDataInCitySceneEcho;
		/// <summary>
		/// 获取当前任务列表数据
		/// </summary>
		public static string GetTaskListData;
		/// <summary>
		/// 获取当前任务列表数据回调
		/// </summary>
		public static string GetTaskListDataEcho;
		/// <summary>
		/// 关闭任务列表界面
		/// </summary>
		public static string HideTaskListPanel;
		/// <summary>
		/// 打开任务跟踪按钮界面
		/// </summary>
		public static string ShowTaskBtnPanel;
		/// <summary>
		/// 关闭任务跟踪按钮界面
		/// </summary>
		public static string HideTaskBtnPanel;
		/// <summary>
		/// 从任务跟踪按钮界面关闭任务列表界面
		/// </summary>
		public static string MakeTaskListHide;
		/// <summary>
		/// 获取任务详细数据
		/// </summary>
		public static string GetTaslDetailInfoData;
		/// <summary>
		/// 再次加载任务详情数据
		/// </summary>
		public static string ReloadTaslDetailInfoData;
		/// <summary>
		/// 战斗胜利后通知该战斗任务步骤将战斗按钮禁用
		/// </summary>
		public static string MakeFightWinedBtnDisable;
		/// <summary>
		/// 打开任务详细信息界面
		/// </summary>
		public static string ShowTaskDetailInfoPanel;
		/// <summary>
		/// 关闭任务详细信息界面
		/// </summary>
		public static string HideTaskDetailInfoPanel;
		/// <summary>
		/// 检测任务对话状态
		/// </summary>
		public static string CheckTaskDialog;
		/// <summary>
		/// 检测任务对话状态回调
		/// </summary>
		public static string CheckTaskDialogEcho;
        /// <summary>
        /// 刷新任务相关界面
        /// </summary>
        public static string RefreshTaskInfos;
        /// <summary>
        /// 添加一个新任务
        /// </summary>
        public static string AddANewTask;
	}
	public partial class NotifyRegister {
		/// <summary>
		/// Tasks the notify init.
		/// </summary>
		public static void TaskNotifyInit() {
			Messenger.AddListener<string>(NotifyTypes.GetTaskListDataInCityScene, (cityId) => {
				if (CityScenePanelCtrl.Ctrl != null) {
					DbManager.Instance.GetTaskListDataInCityScene(cityId);
				}
			});
			
			Messenger.AddListener<List<TaskData>>(NotifyTypes.GetTaskListDataInCitySceneEcho, (list) => {
				CityScenePanelCtrl.ShowTask(list);
			});

			Messenger.AddListener(NotifyTypes.GetTaskListData, () => {
				DbManager.Instance.GetTaskListData();
			});

			Messenger.AddListener<List<TaskData>>(NotifyTypes.GetTaskListDataEcho, (list) => {
				TaskListPanelCtrl.Show(list);
			});

			Messenger.AddListener(NotifyTypes.HideTaskListPanel, () => {
				TaskListPanelCtrl.Hide();
			});

			Messenger.AddListener(NotifyTypes.ShowTaskBtnPanel, () => {
				TaskBtnPanelCtrl.Show();
			});

			Messenger.AddListener(NotifyTypes.HideTaskBtnPanel, () => {
				TaskBtnPanelCtrl.MakeMoveOut();
			});

			Messenger.AddListener(NotifyTypes.MakeTaskListHide, () => {
				TaskBtnPanelCtrl.MakeTaskListHide();
			});
			
			Messenger.AddListener<string>(NotifyTypes.GetTaslDetailInfoData, (taskId) => {
				DbManager.Instance.GetTaskDetailInfoData(taskId);
			});

			Messenger.AddListener(NotifyTypes.ReloadTaslDetailInfoData, () => {
				TaskDetailInfoPanelCtrl.Reload();
			});

			Messenger.AddListener<string>(NotifyTypes.MakeFightWinedBtnDisable, (fightId) => {
				TaskDetailInfoPanelCtrl.MakeFightWinedBtnDisable(fightId);
			});

			Messenger.AddListener<JArray>(NotifyTypes.ShowTaskDetailInfoPanel, (data) => {
				TaskDetailInfoPanelCtrl.Show(data);
				Messenger.Broadcast(NotifyTypes.MakeTaskListHide);
				//任务详情界面打开时需要关闭角色信息面板和任务列表入口按钮
				Messenger.Broadcast(NotifyTypes.HideRoleInfoPanel);
			});

			Messenger.AddListener(NotifyTypes.HideTaskDetailInfoPanel, () => {
				TaskDetailInfoPanelCtrl.Hide();
				//关闭任务对话详情界面时重新请求动态事件列表
				Messenger.Broadcast<string>(NotifyTypes.GetActiveEventsInArea, UserModel.CurrentUserData.CurrentAreaSceneName);
			});

			Messenger.AddListener<string, bool, bool>(NotifyTypes.CheckTaskDialog, (taskId, auto, selectedNo) => {
				DbManager.Instance.CheckTaskDialog(taskId, auto, selectedNo);
			});

			Messenger.AddListener<JArray>(NotifyTypes.CheckTaskDialogEcho, (data) => {
				TaskDetailInfoPanelCtrl.PopDialogToList(data);
			});

            Messenger.AddListener(NotifyTypes.RefreshTaskInfos, () => {
                //时辰更新时需要检测城镇场景中的任务列表是否有更新
                if (UserModel.CurrentUserData != null) {
                    if (CityScenePanelCtrl.MakeGetSceneData() != null) {
                        Messenger.Broadcast<string>(NotifyTypes.GetTaskListDataInCityScene, CityScenePanelCtrl.MakeGetSceneData().Id);
                    }
                    //当任务列表打开的时候每个时辰切换都要再刷新下任务列表，更新任务状态
                    if (TaskListPanelCtrl.Ctrl != null) {
                        Messenger.Broadcast(NotifyTypes.GetTaskListData);
                    }
                    if (TaskBtnPanelCtrl.Ctrl != null) {
                        Messenger.Broadcast(NotifyTypes.ShowTaskBtnPanel);
                    }

                }
            });

            Messenger.AddListener<string>(NotifyTypes.AddANewTask, (taskId) => {
                DbManager.Instance.AddNewTask(taskId);
                Messenger.Broadcast(NotifyTypes.RefreshTaskInfos);
            });
		}
	}
}