﻿using UnityEngine;
using System.Collections;

namespace Game {
	public partial class NotifyTypes {
		/// <summary>
		/// 初始化UI组件的根
		/// </summary>
		public static string InitUISystem;
		/// <summary>
		/// 清空所有文字UI
		/// </summary>
		public static string ClearAllFonts;
	}
	public partial class NotifyRegister {
		/// <summary>
		/// Scenes the notify init.
		/// </summary>
		public static void UINotifyInit() {
			//订阅 初始化UI组件的根 消息
			Messenger.AddListener(NotifyTypes.InitUISystem, () => {
				if (UIModel.UICanvas == null) {
					UIModel.UICanvas = GameObject.Find("UICanvas");
				}
				if (UIModel.FrameCanvas == null) {
					UIModel.FrameCanvas = GameObject.Find("FrameCanvas");
				}
				if (UIModel.FontCanvas == null) {
					UIModel.FontCanvas = GameObject.Find("FontCanvas");
				}
			});
			Messenger.Broadcast(NotifyTypes.InitUISystem);

			//订阅 清空所有文字UI 消息
			Messenger.AddListener(NotifyTypes.ClearAllFonts, () => {
				foreach (Transform child in UIModel.FontCanvas.transform) {
					MonoBehaviour.Destroy(child.gameObject);
				}
			});

		}
	}
}
