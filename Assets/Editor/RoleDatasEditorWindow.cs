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
	public class RoleDatasEditorWindow : EditorWindow {

		static RoleDatasEditorWindow window = null;
		static GameObject showRolePrefab;
		static string laseSceneName;

		[MenuItem ("Editors/Role Datas Editor")]
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
				window = (RoleDatasEditorWindow)EditorWindow.GetWindowWithRect(typeof(RoleDatasEditorWindow), size, true, "武功招式编辑器");
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
		static Dictionary<string, Texture> halfBodyTextureMappings;
		static List<string> halfBodyNames;
		static Dictionary<string, int> halfBodyIdIndexs;
		static List<ResourceSrcData> halfBodys;
		static List<OccupationType> occupationTypeEnums;
		static List<string> occupationTypeStrs;
		static Dictionary<OccupationType, int> occupationTypeIndexMapping;
		static List<GenderType> genderTypeEnums;
		static List<string> genderTypeStrs;
		static Dictionary<GenderType, int> genderTypeIndexMapping;

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
					if (iconData.Name.IndexOf("头像-") < 0) {
						continue;
					}
					iconPrefab = Statics.GetPrefabClone(JsonManager.GetInstance().GetMapping<ResourceSrcData>("Icons", iconData.Id).Src);
					iconTextureMappings.Add(iconData.Id, iconPrefab.GetComponent<Image>().sprite.texture);
					DestroyImmediate(iconPrefab);
					iconNames.Add(iconData.Name);
					iconIdIndexs.Add(iconData.Id, index);
					icons.Add(iconData);
					index++;
				}
			}
			//获取全部的半身像对象
			halfBodyTextureMappings = new Dictionary<string, Texture>();
			halfBodyNames = new List<string>();
			halfBodyIdIndexs = new Dictionary<string, int>();
			halfBodys = new List<ResourceSrcData>();
			index = 0;
			obj = JsonManager.GetInstance().GetJson("HalfBodys", false);
			ResourceSrcData halfBodyData;
			GameObject halfBodyPrefab;
			foreach(var item in obj) {
				if (item.Key != "0") {
					halfBodyData = JsonManager.GetInstance().DeserializeObject<ResourceSrcData>(item.Value.ToString());
					halfBodyPrefab = Statics.GetPrefabClone(JsonManager.GetInstance().GetMapping<ResourceSrcData>("HalfBodys", halfBodyData.Id).Src);
					halfBodyTextureMappings.Add(halfBodyData.Id, halfBodyPrefab.GetComponent<Image>().sprite.texture);
					DestroyImmediate(halfBodyPrefab);
					halfBodyNames.Add(halfBodyData.Name);
					halfBodyIdIndexs.Add(halfBodyData.Id, index);
					halfBodys.Add(halfBodyData);
					index++;
				}
			}

			FieldInfo fieldInfo;
			object[] attribArray;
			DescriptionAttribute attrib;
			//加载全部的OccupationType枚举类型
			occupationTypeEnums = new List<OccupationType>();
			occupationTypeStrs = new List<string>();
			occupationTypeIndexMapping = new Dictionary<OccupationType, int>();
			index = 0;
			foreach(OccupationType type in Enum.GetValues(typeof(OccupationType))) {
				occupationTypeEnums.Add(type);
				fieldInfo = type.GetType().GetField(type.ToString());
				attribArray = fieldInfo.GetCustomAttributes(false);
				attrib = (DescriptionAttribute)attribArray[0];
				occupationTypeStrs.Add(attrib.Description);
				occupationTypeIndexMapping.Add(type, index);
				index++;
			}

			//加载全部的GenderType枚举类型
			genderTypeEnums = new List<GenderType>();
			genderTypeStrs = new List<string>();
			genderTypeIndexMapping = new Dictionary<GenderType, int>();
			index = 0;
			foreach(GenderType type in Enum.GetValues(typeof(GenderType))) {
				genderTypeEnums.Add(type);
				fieldInfo = type.GetType().GetField(type.ToString());
				attribArray = fieldInfo.GetCustomAttributes(false);
				attrib = (DescriptionAttribute)attribArray[0];
				genderTypeStrs.Add(attrib.Description);
				genderTypeIndexMapping.Add(type, index);
				index++;
			}
		}

		static Dictionary<string, RoleData> dataMapping;
		static void getData() {
			dataMapping = new Dictionary<string, RoleData>();
			JObject obj = JsonManager.GetInstance().GetJson("RoleDatas", false);
			foreach(var item in obj) {
				if (item.Key != "0") {
					dataMapping.Add(item.Value["Id"].ToString(), JsonManager.GetInstance().DeserializeObject<RoleData>(item.Value.ToString()));
				}
			}
			fetchData();
		}

		static List<RoleData> showListData;
		static List<string> listNames;
		static List<string> allDataNames;
		static List<RoleData> allRoleDatas;
		static string addedId = "";
		static void fetchData(string keyword = "") {
			showListData = new List<RoleData>();
			allDataNames = new List<string>(){ "无额外招式" };
			allRoleDatas = new List<RoleData>() { null };
			foreach(RoleData data in dataMapping.Values) {
				allDataNames.Add(data.Name);
				allRoleDatas.Add(data);
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
			foreach(RoleData data in dataMapping.Values) {
				if (index == 0) {
					index++;
					writeJson["0"] = JObject.Parse(JsonManager.GetInstance().SerializeObjectDealVector(data));
				}
				writeJson[data.Id] = JObject.Parse(JsonManager.GetInstance().SerializeObjectDealVector(data));
			}
			Base.CreateFile(Application.dataPath + "/Resources/Data/Json", "RoleDatas.json", JsonManager.GetInstance().SerializeObject(writeJson));
		}

		RoleData data;
		Vector2 scrollPosition;
		static int selGridInt = 0;
		int oldSelGridInt = -1;
		string searchKeyword = "";

		string showId = "";
		string roleName = "";
		int iconIndex = 0;
		int oldIconIndex = -1;
		Texture iconTexture = null;
		int genderTypeIndex = 0;
		int occupationTypeIndex = 0;
		int halfBodyIdIndex = 0;
		int oldHalfBodyIdIndex = -1;
		Texture halfBodyTexture = null;

		short toolState; //0正常 1增加 2删除
		string addId = "";
		string addRoleName = "";
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
					roleName = data.Name;
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
					occupationTypeIndex = occupationTypeIndexMapping[data.Occupation];
					genderTypeIndex = genderTypeIndexMapping[data.Gender];
					if (halfBodyIdIndexs.ContainsKey(data.HalfBodyId)) {
						halfBodyIdIndex = halfBodyIdIndexs[data.HalfBodyId];
					}
					else {
						halfBodyIdIndex = 0;
					}
					if (halfBodyTextureMappings.ContainsKey(data.HalfBodyId)) {
						halfBodyTexture = halfBodyTextureMappings[data.HalfBodyId];	
					}
					else {
						halfBodyTexture = null;
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
					GUI.Label(new Rect(205, 0, 40, 18), "姓名:");
					roleName = EditorGUI.TextField(new Rect(250, 0, 100, 18), roleName);
					GUI.Label(new Rect(355, 0, 40, 18), "性别:");
					genderTypeIndex = EditorGUI.Popup(new Rect(400, 0, 100, 18), genderTypeIndex, genderTypeStrs.ToArray());
					GUI.Label(new Rect(55, 20, 40, 18), "Icon:");
					iconIndex = EditorGUI.Popup(new Rect(100, 20, 100, 18), iconIndex, iconNames.ToArray());
					GUI.Label(new Rect(205, 20, 40, 18), "门派:");
					occupationTypeIndex = EditorGUI.Popup(new Rect(250, 20, 100, 18), occupationTypeIndex, occupationTypeStrs.ToArray());
					GUI.Label(new Rect(355, 20, 40, 18), "半身像:");
					halfBodyIdIndex = EditorGUI.Popup(new Rect(400, 20, 100, 18), halfBodyIdIndex, halfBodyNames.ToArray());
					if (halfBodyTexture != null) {
						GUI.DrawTexture(new Rect(505, 0, 325, 260), halfBodyTexture);
					}
					if (oldIconIndex != iconIndex) {
						oldIconIndex = iconIndex;
						iconTexture = iconTextureMappings[icons[iconIndex].Id];
					}
					if (GUI.Button(new Rect(0, 65, 80, 18), "修改基础属性")) {
						if (roleName == "") {
							this.ShowNotification(new GUIContent("招式名不能为空!"));
							return;
						}
						data.Name = roleName;
						data.IconId = icons[iconIndex].Id;
						data.Occupation = occupationTypeEnums[occupationTypeIndex];
						data.Gender = genderTypeEnums[genderTypeIndex];
						data.HalfBodyId = halfBodys[halfBodyIdIndex].Id;
						writeDataToJson();
						oldSelGridInt = -1;
						getData();
						fetchData(searchKeyword);
						this.ShowNotification(new GUIContent("修改成功"));
					}
					GUILayout.EndArea();
				}
				
			}

			GUILayout.BeginArea(new Rect(listStartX + 205, listStartY + 360, 300, 60));
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
				GUI.Label(new Rect(140, 0, 60, 18), "招式名:");
				addRoleName = EditorGUI.TextField(new Rect(205, 0, 100, 18), addRoleName);
				if (GUI.Button(new Rect(0, 20, 60, 18), "确定添加")) {
					toolState = 0;
					if (addId == "") {
						this.ShowNotification(new GUIContent("Id不能为空!"));
						return;
					}
					if (addRoleName == "") {
						this.ShowNotification(new GUIContent("角色姓名不能为空!"));
						return;
					}
					if (dataMapping.ContainsKey(addId)) {
						this.ShowNotification(new GUIContent("Id重复!"));
						return;
					}
					RoleData addRoleData = new RoleData();
					addRoleData.Id = addId;
					addRoleData.Name = addRoleName;	
					dataMapping.Add(addId, addRoleData);
					writeDataToJson();
					addedId = addId;
					getData();
					fetchData(searchKeyword);
					addId = "";
					addRoleName = "";
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