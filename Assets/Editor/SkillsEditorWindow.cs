﻿#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Game;
using UnityEngine.UI;
using System.Reflection;
using System.ComponentModel;
using System;

namespace GameEditor {
	public class SkillsEditorWindow : EditorWindow {

		static SkillsEditorWindow window = null;
		static GameObject showRolePrefab;
		static string laseSceneName;

		[MenuItem ("Editors/Skills Editor")]
		static void OpenWindow() {
			Open();
			InitParams();
		}

		// Use this for initialization
		void Start () {
			
		}

		// Update is called once per frame
		void Update () {

		}

		/// <summary>
		/// Open the specified pos.
		/// </summary>
		public static void Open() {
			float width = 1024;
			float height = Screen.currentResolution.height - 100;
			float x = Screen.currentResolution.width - width;
			float y = 25;
			Rect size = new Rect(x, y, width, height);
			if (window == null) {
				window = (SkillsEditorWindow)EditorWindow.GetWindowWithRect(typeof(SkillsEditorWindow), size, true, "技能编辑器");
			}
			window.Show();
			window.position = size;
			getData();
		}

		/// <summary>
		/// Hide this instance.
		/// </summary>
		public static void Hide() {
			if (window != null) {
				window.Close();
				window = null;
			}
		}

		static Dictionary<string, Texture> iconTextureMappings;
		static List<string> iconNames;
		static Dictionary<string, int> iconIdIndexs;
		static List<ResourceSrcData> icons;
		static List<SkillType> skillTypeEnums;
		static List<string> skillTypeStrs;
		static Dictionary<SkillType, int> skillTypeIndexMapping;
		static List<BuffType> buffTypeEnums;
		static List<string> buffTypeStrs;
		static Dictionary<BuffType, int> buffTypeIndexMapping;

		static void InitParams() { 
			//加载全部的icon对象
			iconTextureMappings = new Dictionary<string, Texture>();
			iconNames = new List<string>();
			iconIdIndexs = new Dictionary<string, int>();
			icons = new List<ResourceSrcData>();
			int index = 0;
			JObject obj = JsonManager.GetInstance().GetJson("Icons", false);
			ResourceSrcData iconData;
			GameObject iconPrefab;
			foreach(var item in obj) {
				if (item.Key != "0") {
					iconData = JsonManager.GetInstance().DeserializeObject<ResourceSrcData>(item.Value.ToString());
					iconPrefab = Statics.GetPrefabClone(JsonManager.GetInstance().GetMapping<ResourceSrcData>("Icons", iconData.Id).Src);
					iconTextureMappings.Add(iconData.Id, iconPrefab.GetComponent<Image>().sprite.texture);
					DestroyImmediate(iconPrefab);
					iconNames.Add(iconData.Name);
					iconIdIndexs.Add(iconData.Id, index);
					icons.Add(iconData);
					index++;
				}
			}

			FieldInfo fieldInfo;
			object[] attribArray;
			DescriptionAttribute attrib;
			//加载全部的SkillType枚举类型
			skillTypeEnums = new List<SkillType>();
			skillTypeStrs = new List<string>();
			skillTypeIndexMapping = new Dictionary<SkillType, int>();
			index = 0;
			foreach(SkillType type in Enum.GetValues(typeof(SkillType))) {
				skillTypeEnums.Add(type);
				fieldInfo = type.GetType().GetField(type.ToString());
				attribArray = fieldInfo.GetCustomAttributes(false);
				attrib = (DescriptionAttribute)attribArray[0];
				skillTypeStrs.Add(attrib.Description);
				skillTypeIndexMapping.Add(type, index);
				index++;
			}

			//加载全部的BuffType枚举类型
			buffTypeEnums = new List<BuffType>();
			buffTypeStrs = new List<string>();
			buffTypeIndexMapping = new Dictionary<BuffType, int>();
			index = 0;
			foreach(BuffType type in Enum.GetValues(typeof(BuffType))) {
				buffTypeEnums.Add(type);
				fieldInfo = type.GetType().GetField(type.ToString());
				attribArray = fieldInfo.GetCustomAttributes(false);
				attrib = (DescriptionAttribute)attribArray[0];
				buffTypeStrs.Add(attrib.Description);
				buffTypeIndexMapping.Add(type, index);
				index++;
			}
		}

		static Dictionary<string, SkillData> dataMapping;
		static void getData() {
			dataMapping = new Dictionary<string, SkillData>();
			JObject obj = JsonManager.GetInstance().GetJson("Skills", false);
			foreach(var item in obj) {
				if (item.Key != "0") {
					dataMapping.Add(item.Value["Id"].ToString(), JsonManager.GetInstance().DeserializeObject<SkillData>(item.Value.ToString()));
				}
			}
			fetchData();
		}

		static List<SkillData> showListData;
		static List<string> listNames;
		static string addedId = "";
		static void fetchData(string keyword = "") {
			showListData = new List<SkillData>();
			foreach(SkillData data in dataMapping.Values) {
				if (keyword != "") {
					if (data.Name.IndexOf(keyword) < 0) {
						continue;
					}
				}
				showListData.Add(data);
			}

			listNames = new List<string>();
			showListData.Sort((a, b) => a.Id.CompareTo(b.Id));
			for(int i = 0; i < showListData.Count; i++) {
				listNames.Add(showListData[i].Name);
				if (addedId == showListData[i].Id) {
					selGridInt = i;
					addedId = "";
				}
			}
		}

		void writeDataToJson() {
			JObject writeJson = new JObject();
			int index = 0;
			foreach(SkillData data in dataMapping.Values) {
				if (index == 0) {
					index++;
					writeJson["0"] = JObject.Parse(JsonManager.GetInstance().SerializeObjectDealVector(data));
				}
				writeJson[data.Id] = JObject.Parse(JsonManager.GetInstance().SerializeObjectDealVector(data));
			}
			Base.CreateFile(Application.dataPath + "/Resources/Data/Json", "Skills.json", JsonManager.GetInstance().SerializeObject(writeJson));
		}

		SkillData data;
		Vector2 scrollPosition;
		static int selGridInt = 0;
		int oldSelGridInt = -1;
		string searchKeyword = "";

		string showId = "";
		string skillName = "";
		int iconIndex = 0;
		int oldIconIndex = -1;
		Texture iconTexture = null;
		int skillTypeIndex = 0;
		float rate = 0;

		int buffGridIndex = 0;
		List<int> theBuffTypeIndexs;
		List<float> theBuffRates;
		List<int> theBuffRoundNumbers;
		List<float> theBuffValues;
		List<bool> theBuffFirstEffects;

		List<int> theDeBuffTypeIndexs;
		List<float> theDeBuffRates;
		List<int> theDeBuffRoundNumbers;
		List<float> theDeBuffValues;
		List<bool> theDeBuffFirstEffects;

		int addBuffOrDeBuffTypeIndex = 0;
		float addBuffOrDeBuffRate = 100;
		int addBuffOrDeBuffRoundNumber = 0;
		float addBuffOrDeBuffValue = 1;
		bool addBuffOrDeBuffFirstEffect = true;

		short toolState; //0正常 1增加 2删除

		string addId = "";
		string addSkillName = "";
		//绘制窗口时调用
	    void OnGUI () {
			data = null;

			GUILayout.BeginArea(new Rect(5, 5, 200, 20));
			GUI.Label(new Rect(0, 0, 50, 18), "搜索名称:");
			searchKeyword = GUI.TextField(new Rect(55, 0, 100, 18), searchKeyword);
			if (GUI.Button(new Rect(160, 0, 30, 18), "搜索")) {
				selGridInt = 0;
				fetchData(searchKeyword);
			}
			GUILayout.EndArea();

			float listStartX = 5;
			float listStartY = 25;
			float scrollHeight = Screen.currentResolution.height - 110;
			if (listNames != null && listNames.Count > 0) {


				float contextHeight = listNames.Count * 21;
				//开始滚动视图  
				scrollPosition = GUI.BeginScrollView(new Rect(listStartX, listStartY, 200, scrollHeight), scrollPosition, new Rect(5, 5, 190, contextHeight), false, scrollHeight < contextHeight);

				selGridInt = GUILayout.SelectionGrid(selGridInt, listNames.ToArray(), 1, GUILayout.Width(190));
				selGridInt = selGridInt >= listNames.Count ? listNames.Count - 1 : selGridInt;
				data = showListData[selGridInt];
				if (selGridInt != oldSelGridInt) {
					oldSelGridInt = selGridInt;
					showId = data.Id;
					skillName = data.Name;
					if (iconIdIndexs.ContainsKey(data.IconId)) {
						iconIndex = iconIdIndexs[data.IconId];
					}
					else {
						iconIndex = 0;
					}
					if (iconTextureMappings.ContainsKey(data.IconId)) {
						iconTexture = iconTextureMappings[data.IconId];	
					}
					else {
						iconTexture = null;
					}	
					skillTypeIndex = skillTypeIndexMapping[data.Type];
					rate = data.Rate;

					theBuffTypeIndexs = new List<int>();
					theBuffRates = new List<float>();
					theBuffRoundNumbers = new List<int>();
					theBuffValues = new List<float>();
					theBuffFirstEffects = new List<bool>();
					foreach(BuffData buff in data.BuffDatas) {
						theBuffTypeIndexs.Add(buffTypeIndexMapping[buff.Type]);
						theBuffRates.Add(buff.Rate);
						theBuffRoundNumbers.Add(buff.RoundNumber);
						theBuffValues.Add(buff.Value);
						theBuffFirstEffects.Add(buff.FirstEffect);
					}

					theDeBuffTypeIndexs = new List<int>();
					theDeBuffRates = new List<float>();
					theDeBuffRoundNumbers = new List<int>();
					theDeBuffValues = new List<float>();
					theDeBuffFirstEffects = new List<bool>();
					foreach(BuffData deBuff in data.DeBuffDatas) {
						theDeBuffTypeIndexs.Add(buffTypeIndexMapping[deBuff.Type]);
						theDeBuffRates.Add(deBuff.Rate);
						theDeBuffRoundNumbers.Add(deBuff.RoundNumber);
						theDeBuffValues.Add(deBuff.Value);
						theDeBuffFirstEffects.Add(deBuff.FirstEffect);
					}
				}
				//结束滚动视图  
				GUI.EndScrollView();

				if (data != null) {
					GUILayout.BeginArea(new Rect(listStartX + 205, listStartY, 800, 250));
					if (iconTexture != null) {
						GUI.DrawTexture(new Rect(0, 0, 50, 50), iconTexture);
					}
					showId = data.Id;
					GUI.Label(new Rect(55, 0, 40, 18), "Id:");
					showId = EditorGUI.TextField(new Rect(100, 0, 100, 18), showId);
					GUI.Label(new Rect(205, 0, 40, 18), "名称:");
					skillName = EditorGUI.TextField(new Rect(250, 0, 100, 18), skillName);
					GUI.Label(new Rect(55, 20, 40, 18), "Icon:");
					iconIndex = EditorGUI.Popup(new Rect(100, 20, 100, 18), iconIndex, iconNames.ToArray());
					GUI.Label(new Rect(205, 20, 40, 18), "类型:");
					skillTypeIndex = EditorGUI.Popup(new Rect(250, 20, 100, 18), skillTypeIndex, skillTypeStrs.ToArray());
					GUI.Label(new Rect(55, 40, 40, 18), "概率:");
					rate = EditorGUI.Slider(new Rect(100, 40, 180, 18), rate, 0, 100);
					if (oldIconIndex != iconIndex) {
						oldIconIndex = iconIndex;
						iconTexture = iconTextureMappings[icons[iconIndex].Id];
					}
					if (GUI.Button(new Rect(0, 65, 80, 18), "修改基础属性")) {
						if (skillName == "") {
							this.ShowNotification(new GUIContent("技能名不能为空!"));
							return;
						}
						data.Name = skillName;
						data.IconId = icons[iconIndex].Id;
						data.Type = skillTypeEnums[skillTypeIndex];
						data.Rate = rate;
						writeDataToJson();
						oldSelGridInt = -1;
						getData();
						fetchData(searchKeyword);
						this.ShowNotification(new GUIContent("修改成功"));
					}
					buffGridIndex = GUI.SelectionGrid(new Rect(0, 90, 80, 50), buffGridIndex, new string[2]{ "Buff", "DeBuff" }, 1);
					GUI.Label(new Rect(85, 90, 40, 18), "类型:");
					addBuffOrDeBuffTypeIndex = EditorGUI.Popup(new Rect(130, 90, 100, 18), addBuffOrDeBuffTypeIndex, buffTypeStrs.ToArray());
					GUI.Label(new Rect(235, 90, 40, 18), "概率:");
					addBuffOrDeBuffRate = Mathf.Clamp(float.Parse(EditorGUI.TextField(new Rect(280, 90, 40, 18), addBuffOrDeBuffRate.ToString())), 0, 100);
					GUI.Label(new Rect(325, 90, 40, 18), "回合:");
					addBuffOrDeBuffRoundNumber = Mathf.Clamp(int.Parse(EditorGUI.TextField(new Rect(370, 90, 40, 18), addBuffOrDeBuffRoundNumber.ToString())), 0, 10);
					GUI.Label(new Rect(415, 90, 40, 18), "数值:");
					addBuffOrDeBuffValue = Mathf.Clamp(float.Parse(EditorGUI.TextField(new Rect(460, 90, 80, 18), addBuffOrDeBuffValue.ToString())), 0, getBuffValueRangeTop(buffTypeEnums[addBuffOrDeBuffTypeIndex]));
					GUI.Label(new Rect(545, 90, 70, 18), "首回合生效:");
					addBuffOrDeBuffFirstEffect = EditorGUI.Toggle(new Rect(620, 90, 30, 18), addBuffOrDeBuffFirstEffect);
					if (GUI.Button(new Rect(655, 90, 40, 18), "+")) {
						List<BuffData> buffs = buffGridIndex == 0 ? data.BuffDatas : data.DeBuffDatas;
						if (buffs.Count < 5) {
							BuffData buff = buffs.Find((item) => { return item.Type == buffTypeEnums[addBuffOrDeBuffTypeIndex]; });
							if (buff == null) {
								BuffData newBuff = new BuffData();
								newBuff.Type = buffTypeEnums[addBuffOrDeBuffTypeIndex];
								newBuff.Rate = addBuffOrDeBuffRate;
								newBuff.RoundNumber = addBuffOrDeBuffRoundNumber;
								newBuff.Value = addBuffOrDeBuffValue;
								newBuff.FirstEffect = addBuffOrDeBuffFirstEffect;
								buffs.Add(newBuff);
								writeDataToJson();
								oldSelGridInt = -1;
								getData();
								fetchData(searchKeyword);
								addBuffOrDeBuffTypeIndex = 0;
								addBuffOrDeBuffRate = 100;
								addBuffOrDeBuffRoundNumber = 0;
								addBuffOrDeBuffValue = 1;
								addBuffOrDeBuffFirstEffect = true;
								this.ShowNotification(new GUIContent("添加成功"));
							}
							else {
								this.ShowNotification(new GUIContent("Buff或DeBuff类型已存在, 不能添加!"));
							}
						}
						else {
							this.ShowNotification(new GUIContent("Buff或DeBuff已到上限,不能添加!"));
						}
					}
					float buffsStartY = 110;
					if (buffGridIndex == 0) {
						for (int i = 0; i < theBuffTypeIndexs.Count; i++) {
							GUI.Label(new Rect(85, buffsStartY + i * 20, 40, 18), "类型:");
							theBuffTypeIndexs[i] = EditorGUI.Popup(new Rect(130, buffsStartY + i * 20, 100, 18), theBuffTypeIndexs[i], buffTypeStrs.ToArray());
							GUI.Label(new Rect(235, buffsStartY + i * 20, 40, 18), "概率:");
							theBuffRates[i] = Mathf.Clamp(float.Parse(EditorGUI.TextField(new Rect(280, buffsStartY + i * 20, 40, 18), theBuffRates[i].ToString())), 0, 100);
							GUI.Label(new Rect(325, buffsStartY + i * 20, 40, 18), "回合:");
							theBuffRoundNumbers[i] = Mathf.Clamp(int.Parse(EditorGUI.TextField(new Rect(370, buffsStartY + i * 20, 40, 18), theBuffRoundNumbers[i].ToString())), 0, 10);
							GUI.Label(new Rect(415, buffsStartY + i * 20, 40, 18), "数值:");
							theBuffValues[i] = Mathf.Clamp(float.Parse(EditorGUI.TextField(new Rect(460, buffsStartY + i * 20, 80, 18), theBuffValues[i].ToString())), 0, getBuffValueRangeTop(buffTypeEnums[theBuffTypeIndexs[i]]));
							GUI.Label(new Rect(545, buffsStartY + i * 20, 70, 18), "首回合生效:");
							theBuffFirstEffects[i] = EditorGUI.Toggle(new Rect(620, buffsStartY + i * 20, 30, 18), theBuffFirstEffects[i]);
							if (GUI.Button(new Rect(655, buffsStartY + i * 20, 40, 18), "-")) {
								if (data.BuffDatas.Count > i) {
									data.BuffDatas.RemoveAt(i);
									writeDataToJson();
									oldSelGridInt = -1;
									getData();
									fetchData(searchKeyword);
									this.ShowNotification(new GUIContent("删除成功"));
								}
							}
						}
					}
					else {
						for (int i = 0; i < theDeBuffTypeIndexs.Count; i++) {
							GUI.Label(new Rect(85, buffsStartY + i * 20, 40, 18), "类型:");
							theDeBuffTypeIndexs[i] = EditorGUI.Popup(new Rect(130, buffsStartY + i * 20, 100, 18), theDeBuffTypeIndexs[i], buffTypeStrs.ToArray());
							GUI.Label(new Rect(235, buffsStartY + i * 20, 40, 18), "概率:");
							theDeBuffRates[i] = Mathf.Clamp(float.Parse(EditorGUI.TextField(new Rect(280, buffsStartY + i * 20, 40, 18), theDeBuffRates[i].ToString())), 0, 100);
							GUI.Label(new Rect(325, buffsStartY + i * 20, 40, 18), "回合:");
							theDeBuffRoundNumbers[i] = Mathf.Clamp(int.Parse(EditorGUI.TextField(new Rect(370, buffsStartY + i * 20, 40, 18), theDeBuffRoundNumbers[i].ToString())), 0, 10);
							GUI.Label(new Rect(415, buffsStartY + i * 20, 40, 18), "数值:");
							theDeBuffValues[i] = Mathf.Clamp(float.Parse(EditorGUI.TextField(new Rect(460, buffsStartY + i * 20, 80, 18), theDeBuffValues[i].ToString())), 0, getBuffValueRangeTop(buffTypeEnums[theBuffTypeIndexs[i]]));
							GUI.Label(new Rect(545, buffsStartY + i * 20, 70, 18), "首回合生效:");
							theDeBuffFirstEffects[i] = EditorGUI.Toggle(new Rect(620, buffsStartY + i * 20, 30, 18), theDeBuffFirstEffects[i]);
							if (GUI.Button(new Rect(655, buffsStartY + i * 20, 40, 18), "-")) {
								if (data.DeBuffDatas.Count > i) {
									data.DeBuffDatas.RemoveAt(i);
									writeDataToJson();
									oldSelGridInt = -1;
									getData();
									fetchData(searchKeyword);
									this.ShowNotification(new GUIContent("删除成功"));
								}
							}
						}
					}
					GUILayout.EndArea();
				}
			}

			GUILayout.BeginArea(new Rect(listStartX + 205, listStartY + 255, 300, 60));
			switch (toolState) {
			case 0:
				if (GUI.Button(new Rect(0, 0, 80, 18), "添加")) {
					toolState = 1;
				}
				if (GUI.Button(new Rect(85, 0, 80, 18), "删除")) {
					toolState = 2;
				}
				break;

			case 1:
				GUI.Label(new Rect(0, 0, 30, 18), "Id:");
				addId = EditorGUI.TextField(new Rect(35, 0, 100, 18), addId);
				GUI.Label(new Rect(140, 0, 60, 18), "技能名:");
				addSkillName = EditorGUI.TextField(new Rect(205, 0, 100, 18), addSkillName);
				if (GUI.Button(new Rect(0, 20, 60, 18), "确定添加")) {
					toolState = 0;
					if (addId == "") {
						this.ShowNotification(new GUIContent("Id不能为空!"));
						return;
					}
					if (addSkillName == "") {
						this.ShowNotification(new GUIContent("技能名不能为空!"));
						return;
					}
					if (dataMapping.ContainsKey(addId)) {
						this.ShowNotification(new GUIContent("Id重复!"));
						return;
					}
					SkillData addSkillData = new SkillData();
					addSkillData.Id = addId;
					addSkillData.Name = addSkillName;
					dataMapping.Add(addId, addSkillData);
					writeDataToJson();
					addedId = addId;
					getData();
					fetchData(searchKeyword);
					addId = "";
					addSkillName = "";
					this.ShowNotification(new GUIContent("添加成功"));
				}
				if (GUI.Button(new Rect(65, 20, 60, 18), "取消")) {
					toolState = 0;
				}
				break;

			case 2:
				if (GUI.Button(new Rect(0, 0, 60, 18), "确定删除")) {
					toolState = 0;
					if (!dataMapping.ContainsKey(data.Id)) {
						this.ShowNotification(new GUIContent("待删除的数据不存在!"));
						return;
					}
					dataMapping.Remove(data.Id);
					writeDataToJson();
					selGridInt = 0;
					oldSelGridInt = -1;
					getData();
					fetchData(searchKeyword);
					this.ShowNotification(new GUIContent("删除成功"));
				}
				if (GUI.Button(new Rect(65, 0, 60, 18), "取消")) {
					toolState = 0;
				}
				break;
			default:
				break;
			}
			GUILayout.EndArea();
	    }

		/// <summary>
		/// 根据buff类型获取数值上限
		/// </summary>
		/// <returns>The buff value range top.</returns>
		/// <param name="buffType">Buff type.</param>
		float getBuffValueRangeTop(BuffType buffType) {
			switch (buffType) {
			case BuffType.Fast:	
			case BuffType.Slow:
			case BuffType.IncreaseHurtCutRate:
			case BuffType.IncreaseMagicAttackRate:
			case BuffType.IncreaseMagicDefenseRate:
			case BuffType.IncreaseMaxHPRate:
			case BuffType.IncreasePhysicsAttackRate:
			case BuffType.IncreasePhysicsDefenseRate:		
				return 1;
			default:
				return 100000;
			}
		}

		/// <summary>
		/// 当窗口获得焦点时调用一次
		/// </summary>
		void OnFocus() {

		}

		/// <summary>
		/// 当窗口丢失焦点时调用一次
		/// </summary>
		void OnLostFocus() {

		}

		/// <summary>
		/// 当Hierarchy视图中的任何对象发生改变时调用一次
		/// </summary>
		void OnHierarchyChange() {

		}

		/// <summary>
		/// 当Project视图中的资源发生改变时调用一次
		/// </summary>
		void OnProjectChange() {

		}

		/// <summary>
		/// 这里开启窗口的重绘，不然窗口信息不会刷新
		/// </summary>
		void OnInspectorUpdate() {
			this.Repaint();
		}

		/// <summary>
		/// 当窗口出去开启状态，并且在Hierarchy视图中选择某游戏对象时调用
		/// </summary>
		void OnSelectionChange() {
//			foreach(Transform t in Selection.transforms) {
//				//有可能是多选，这里开启一个循环打印选中游戏对象的名称
//				Debug.Log("OnSelectionChange" + t.name);
//			}
		}

		/// <summary>
		/// 当窗口关闭时调用
		/// </summary>
		void OnDestroy() {
			
		}
	}
}
#endif