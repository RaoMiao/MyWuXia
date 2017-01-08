// ------------------------------------------------------------------------------
//  <autogenerated>
//      This code was generated by a tool.
//      Mono Runtime Version: 4.0.30319.1
// 
//      Changes to this file may cause incorrect behavior and will be lost if 
//      the code is regenerated.
//  </autogenerated>
// ------------------------------------------------------------------------------
using System;
using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using System.ComponentModel;

namespace Game
{
	public class Statics
	{
        
        /// <summary>
        /// 当前设备分辨率
        /// </summary>
        public static Vector2 Resolution = Vector2.zero;


		static Dictionary<string, Sprite> iconSpritesMapping;
		static Dictionary<string, Sprite> halfBodySpriteMapping;
		static Dictionary<string, Sprite> buffSpriteMapping;
		static Dictionary<string, Sprite> spritesMapping;
		static Dictionary<string, UnityEngine.Object> soundsMapping;
		static Dictionary<string, UnityEngine.Object> skillEffectsMapping;
		public static UnityEngine.Object GuoJingPrefab; //轻功登场人物模型预设
		static Dictionary<OccupationType, string> occupationNameMapping;
		static Dictionary<ResourceType, string> resourceNameMapping;
		static Dictionary<InjuryType, string> injuryNameMapping;
		static Dictionary<ItemType, string> itemTypeNameMapping;
		static string[] timeNames = new string[] { "午时", "未时", "申时", "酉时", "戌时", "亥时", "子时", "丑时", "寅时", "卯时", "辰时", "巳时" };
		static Dictionary<string, List<RateData>> meetEnemyRatesMapping;
        static Dictionary<string, Dictionary<string, string>> enumTypeDescMapping;
        static Dictionary<string, Material> materialMapping;
        /// <summary>
        /// 静态逻辑初始化
        /// </summary>
		public static void Init() {
			if (iconSpritesMapping == null) {
				iconSpritesMapping = new Dictionary<string, Sprite>();
				halfBodySpriteMapping = new Dictionary<string, Sprite>();
				buffSpriteMapping = new Dictionary<string, Sprite>();
				spritesMapping = new Dictionary<string, Sprite>();
				soundsMapping = new Dictionary<string, UnityEngine.Object>();
				skillEffectsMapping = new Dictionary<string, UnityEngine.Object>();
				GuoJingPrefab = GetPrefab("Prefabs/GuoJinSkill");
				
				occupationNameMapping = new Dictionary<OccupationType, string>();
				FieldInfo fieldInfo;
				object[] attribArray;
				DescriptionAttribute attrib;
				foreach(OccupationType type in Enum.GetValues(typeof(OccupationType))) {
					fieldInfo = type.GetType().GetField(type.ToString());
					attribArray = fieldInfo.GetCustomAttributes(false);
					attrib = (DescriptionAttribute)attribArray[0];
					occupationNameMapping.Add(type, attrib.Description);
				}
				resourceNameMapping = new Dictionary<ResourceType, string>();
				foreach(ResourceType type in Enum.GetValues(typeof(ResourceType))) {
					fieldInfo = type.GetType().GetField(type.ToString());
					attribArray = fieldInfo.GetCustomAttributes(false);
					attrib = (DescriptionAttribute)attribArray[0];
					resourceNameMapping.Add(type, attrib.Description);
				}
				injuryNameMapping = new Dictionary<InjuryType, string>();
				foreach(InjuryType type in Enum.GetValues(typeof(InjuryType))) {
					fieldInfo = type.GetType().GetField(type.ToString());
					attribArray = fieldInfo.GetCustomAttributes(false);
					attrib = (DescriptionAttribute)attribArray[0];
					injuryNameMapping.Add(type, attrib.Description);
				}
				itemTypeNameMapping = new Dictionary<ItemType, string>();
				foreach(ItemType type in Enum.GetValues(typeof(ItemType))) {
					fieldInfo = type.GetType().GetField(type.ToString());
					attribArray = fieldInfo.GetCustomAttributes(false);
					attrib = (DescriptionAttribute)attribArray[0];
					itemTypeNameMapping.Add(type, attrib.Description);
				}
				TextAsset asset = Resources.Load<TextAsset>("Data/Json/AreaMeetEnemys");
				meetEnemyRatesMapping = JsonManager.GetInstance().DeserializeObject<Dictionary<string, List<RateData>>>(asset.text);
				asset = null;

                enumTypeDescMapping = new Dictionary<string, Dictionary<string, string>>();

                materialMapping = new Dictionary<string, Material>();
                GetMaterial("UIDefaultGreyMaterialImage");

				AreaMain.Init();
				//初始化消息机制
				NotifyBase.Init();
				WorkshopModel.Init();
				//初始化本地数据库
				DbManager.Instance.CreateAllDbs();
			}
        }

		/// <summary>
		/// 根据描述返回时间格式文本
		/// </summary>
		/// <returns>The time.</returns>
		/// <param name="second">Second.</param>
		/// <param name="seperator">Seperator.</param>
		public static string GetTime(int second,string seperator=":") {
			int h, m, s = second;
			string hstr, mstr, sstr;
			h = s < 3600 ? 0 : (int) (s/3600);
			hstr = h < 10 ? '0' + h.ToString() : h.ToString();
			m = (int) ((s%3600)/60);
			mstr = m < 10 ? '0' + m.ToString() : m.ToString();
			s = s%60;
			sstr = s < 10 ? '0' + s.ToString() : s.ToString();
			return hstr + seperator + mstr + seperator + sstr;
		}

		/// <summary>
		/// 格式类似 5天10小时30分
		/// </summary>
		/// <returns></returns>
		public static string GetShortTime(int second) {
			int d, h, m, s = 0;
			s = second;
			var miniute = 60;
			var hour = 3600;
			var day = 86400;
			d = s < day ? 0 : (int)(s / day);
			h = s < hour ? 0 : (int)((s%day)/hour);
			m = s < miniute ? 0 : (int)((s%hour)/ miniute);
			s = s%60;
			var str = String.Format("{0}天{1}小时{2}分", d, h, m);
			if (m == 0)
			{
				str = String.Format("{0}天{1}小时{2}分{3}秒", d, h, m, s);
			}
			return str;
		}

		/// <summary>
		/// 格式化数字(超出10000返回带W的文本)
		/// </summary>
		/// <param name="num"></param>
		/// <returns></returns>
		public static string GetNum(int num) {
			string result = "";
			if (num < 10000)
			{
				result = num.ToString();
			}
			else
			{
				result = (num/10000).ToString() + "W";
			}
			return result;
		}

		/// <summary>
		/// 根据秒数返回对应显示的字符串
		/// </summary>
		/// <param name="second"></param>
		/// <param name="format"></param>
		/// <returns></returns>
		public static string GetFullTime(int second, string format = "{0}:{1}:{2}", bool rightFormat = true) {
			int h, m, s = second;
			string hstr, mstr, sstr;
			string _format = format;
			h = s < 3600 ? 0 : (int)(s / 3600);
			//hstr = rightFormat && h < 10
			//    ? '0' + h.ToString()
			//    : (h >= 24 ? (h / 24).ToString() + "天 " + (h % 24).ToString() : h.ToString());
			int moH = h % 24;
			hstr = rightFormat && moH < 10
				? '0' + moH.ToString()
				: moH.ToString();
			int day = h / 24;
			if (day > 0) {
				hstr = day.ToString() + "天" + hstr;
			}
			m = (int) ((s%3600)/60);
			mstr = rightFormat && m < 10 ? '0' + m.ToString() : m.ToString();
			s = s%60;
			sstr = rightFormat && s < 10 ? '0' + s.ToString() : s.ToString();
			return String.Format(_format, hstr, mstr, sstr);
		}
		/// <summary>
		/// Gets the angle.
		/// </summary>
		/// <returns>The angle.</returns>
		/// <param name="x1">The first x value.</param>
		/// <param name="y1">The first y value.</param>
		/// <param name="x2">The second x value.</param>
		/// <param name="y2">The second y value.</param>
		public static float GetAngle(float x1, float y1, float x2, float y2) {
			float angle = Mathf.Atan2(x2 - x1, y2 - y1) / Mathf.PI * 180;
			return angle >= 0 ? angle : angle + 360;
		}
		/// <summary>
		/// Gets the radian.
		/// </summary>
		/// <returns>The radian.</returns>
		/// <param name="angle">Angle.</param>
		public static float GetRadian(float angle) {
			return Mathf.PI / 180 * angle;
		}
		/// <summary>
		/// Gets the line aim by points.
		/// </summary>
		/// <returns>The line aim by points.</returns>
		/// <param name="x1">The first x value.</param>
		/// <param name="y1">The first y value.</param>
		/// <param name="x2">The second x value.</param>
		/// <param name="y2">The second y value.</param>
		/// <param name="lineHeight">Line height.</param>
		public static Vector2 GetLineAimByPoints(float x1, float y1, float x2, float y2, float lineHeight) {
			return GetLineAimByAngle(GetAngle(x1, y1, x2, y2), lineHeight);
		}
		/// <summary>
		/// Gets the line aim by angle.
		/// </summary>
		/// <returns>The line aim by angle.</returns>
		/// <param name="angle">Angle.</param>
		/// <param name="lineHeight">Line height.</param>
		public static Vector2 GetLineAimByAngle(float angle, float lineHeight) {
			return GetLineAimByRadian(GetRadian(angle), lineHeight);
		}
		/// <summary>
		/// Gets the line aim by radian.
		/// </summary>
		/// <returns>The line aim by radian.</returns>
		/// <param name="radian">Radian.</param>
		/// <param name="lineHeight">Line height.</param>
		public static Vector2 GetLineAimByRadian(float radian, float lineHeight) {
			return new Vector2(lineHeight * Mathf.Sin(radian), lineHeight * Mathf.Cos(radian));
		}

		/// <summary>
		/// Gets the circle point.
		/// </summary>
		/// <returns>The circle point.</returns>
		/// <param name="p">P.</param>
		/// <param name="r">The red component.</param>
		/// <param name="angle">Angle.</param>
		public static Vector2 GetCirclePoint(Vector2 p, float r, float angle) {
			return new Vector2(p.x + r * Mathf.Cos(angle * Mathf.PI / 180), p.y + r * Mathf.Sin(angle * Mathf.PI / 180));
		}

		/// <summary>
		/// 静态方法反射
		/// </summary>
		/// <param name="className"></param>
		/// <param name="methodName"></param>
		/// <param name="param"></param>
		public static bool CallStaticMethod(string className, string methodName, object[] param = null) {
			Type t = Type.GetType(className);
			if (t == null) {
				return false;
			}
			MethodInfo method = t.GetMethod(methodName);
			if (method != null) { 
				method.Invoke(null, param);
				return true;
			}
			return false;
		}
		
		/// <summary>
		/// 公共方法反射
		/// </summary>
		/// <param name="thisObj"></param>
		/// <param name="methodName"></param>
		/// <param name="param"></param>
		public static void CallPublicMethod(object thisObj, string methodName, object[] param) {
			Type t = thisObj.GetType();
			if (t == null) {
				return;
			}
			MethodInfo method = t.GetMethod(methodName);
			if (method != null) {
				method.Invoke(thisObj, param);
			}
		}

		/// <summary>
		/// 返回Resource路径下某个预设的克隆
		/// </summary>
		/// <param name="path">Resource路径</param>
		/// <returns>GameObject对象</returns>
		public static GameObject GetPrefabClone(string path) {
            UnityEngine.Object prefab = Statics.GetPrefab(path);
            return prefab != null ? MonoBehaviour.Instantiate(prefab) as GameObject : null;
		}

		/// <summary>
		/// Gets the prefab clone.
		/// </summary>
		/// <returns>The prefab clone.</returns>
		/// <param name="clone">Clone.</param>
		public static GameObject GetPrefabClone(UnityEngine.Object clone) {
			return MonoBehaviour.Instantiate(clone) as GameObject;
		}

		/// <summary>
		/// Gets the prefab.
		/// </summary>
		/// <returns>The prefab.</returns>
		/// <param name="path">Path.</param>
		public static UnityEngine.Object GetPrefab(string path) {
			return Resources.Load(path, typeof(GameObject));
		}

		/// <summary>
		/// Gets the distance points.
		/// </summary>
		/// <returns>The distance points.</returns>
		/// <param name="from">From.</param>
		/// <param name="to">To.</param>
		/// <param name="distance">Distance.</param>
		public static List<Vector3> GetDistancePoints(Vector3 from, Vector3 to, float distance) {
			List<Vector3> points = new List<Vector3>();
			float num = Mathf.Ceil((Vector3.Distance(from, to) / distance) + 0.5f);
			float indexNum = 0;
			Vector3 cut = new Vector3((to.x - from.x) / num, from.y, (to.z - from.z) / num);
			while(indexNum++ < num - 1) {
				points.Add(new Vector3(from.x + cut.x * indexNum, cut.y, from.z + cut.z * indexNum));
			}
			points.Add(to);
			return points;
		}

		/// <summary>
		/// Determines if is pointer over U.
		/// </summary>
		/// <returns><c>true</c> if is pointer over U; otherwise, <c>false</c>.</returns>
		public static bool IsPointerOverUI() {
			return IPointerOverUI.Instance.IsPointerOverUIObject();
		}

		static float triangleArea(float v0x,float v0y,float v1x,float v1y,float v2x,float v2y) {
			return Mathf.Abs((v0x * v1y + v1x * v2y + v2x * v0y
			                  - v1x * v0y - v2x * v1y - v0x * v2y) / 2f);
		}
		/// <summary>
		/// Ises the IN triangle.
		/// </summary>
		/// <returns><c>true</c>, if IN triangle was ised, <c>false</c> otherwise.</returns>
		/// <param name="point">Point.</param>
		/// <param name="v0">V0.</param>
		/// <param name="v1">V1.</param>
		/// <param name="v2">V2.</param>
		public static bool IsInTriangle(Vector3 point,Vector3 v0,Vector3 v1,Vector3 v2) {
			float x = point.x;
			float y = point.z;
			
			float v0x = v0.x;
			float v0y = v0.z;
			
			float v1x = v1.x;
			float v1y = v1.z;
			
			float v2x = v2.x;
			float v2y = v2.z;
			
			float t = triangleArea(v0x,v0y,v1x,v1y,v2x,v2y);
			float a = triangleArea(v0x,v0y,v1x,v1y,x,y) + triangleArea(v0x,v0y,x,y,v2x,v2y) + triangleArea(x,y,v1x,v1y,v2x,v2y);
			
			if (Mathf.Abs(t - a) <= 0.01f) {
				return true;
			}
			else {
				return false;
			}
		}

		/// <summary>
		/// 多边形碰撞检测
		/// </summary>
		/// <returns><c>true</c>, if SA was polygoned, <c>false</c> otherwise.</returns>
		/// <param name="poly1">Poly1.</param>
		/// <param name="poly2">Poly2.</param>
		public static bool PolygonSAT(List<Vector3> poly1, List<Vector3> poly2) {
			bool allOutside;
			int alen = poly1.Count;
			int blen = poly2.Count;
			Vector3 pp = poly1[alen - 1];
			Vector3 qp;
			Vector3 vp;
			float nx;
			float nz;
			float ndotp;
			float det;
			for (int i = 0; i < alen; i++) {
				qp = poly1[i];
				//求法向量
				nx = qp.z - pp.z;
				nz = pp.x - qp.x;
				ndotp = nx * pp.x + nz * pp.z;
				allOutside = true;
				for (int j = 0; j < blen; j++) {
					vp = poly2[j];
					//判断一条边在共同面上的投影是否叠加来确定是否有一根线能切开两个多边形
					// det = N dot (V - P) = N dot V - N dot P
					det = nx * 	vp.x + nz * vp.z - ndotp;
					if (det < 0) { //叠加,不能切开
						allOutside = false;
						break;
					}
				}
				//如果有一个边和另一个多边形能够被一条直线切开的话就能判断两个都变形没有产生碰撞
				if (allOutside) {
					return false;
				}
				pp = qp; //继续检测下一个点
			}
			return true;
		}

		/// <summary>
		/// 获取Icon图标的Sprite对象
		/// </summary>
		/// <returns>The icon sprite.</returns>
		/// <param name="iconId">Icon identifier.</param>
		public static Sprite GetIconSprite(string iconId) {
			if (!iconSpritesMapping.ContainsKey(iconId)) {
				string iconSrc = JsonManager.GetInstance().GetMapping<JObject>("Icons", iconId)["Src"].ToString();
				GameObject obj = Resources.Load<GameObject>(iconSrc);
				iconSpritesMapping.Add(iconId, obj.GetComponent<Image>().sprite);
				obj = null;
			}
			return iconSpritesMapping[iconId];
		}

		/// <summary>
		/// 获取半身像的Sprite对象
		/// </summary>
		/// <returns>The half body sprite.</returns>
		/// <param name="halfBodyId">Half body identifier.</param>
		public static Sprite GetHalfBodySprite(string halfBodyId) {
			if (!halfBodySpriteMapping.ContainsKey(halfBodyId)) {
				string src = JsonManager.GetInstance().GetMapping<JObject>("HalfBodys", halfBodyId)["Src"].ToString();
				GameObject obj = Resources.Load<GameObject>(src);
				halfBodySpriteMapping.Add(halfBodyId, obj.GetComponent<Image>().sprite);
				obj = null;
			}
			return halfBodySpriteMapping[halfBodyId];
		}

		/// <summary>
		/// 获取buff图标的Sprite对象
		/// </summary>
		/// <returns>The buff sprite.</returns>
		/// <param name="buffTypeId">Buff type identifier.</param>
		public static Sprite GetBuffSprite(string buffTypeId) {
			if (!buffSpriteMapping.ContainsKey(buffTypeId)) {
				string src = JsonManager.GetInstance().GetMapping<JObject>("Buffs", buffTypeId)["Src"].ToString();
				GameObject obj = Resources.Load<GameObject>(src);
				buffSpriteMapping.Add(buffTypeId, obj.GetComponent<Image>().sprite);
				obj = null;
			}
			return buffSpriteMapping[buffTypeId];
		}

		/// <summary>
		/// 获取通用图标的Sprite对象
		/// </summary>
		/// <returns>The sprite.</returns>
		/// <param name="name">Name.</param>
		public static Sprite GetSprite(string name) {
			if (!spritesMapping.ContainsKey(name)) {
				string src = "Prefabs/UI/Sprites/" + name;
				GameObject obj = Resources.Load<GameObject>(src);
				spritesMapping.Add(name, obj.GetComponent<Image>().sprite);
				obj = null;
			}
			return spritesMapping[name];
		}

		/// <summary>
		/// 2D矩形碰撞检测
		/// </summary>
		/// <returns><c>true</c>, if d was collision2ed, <c>false</c> otherwise.</returns>
		/// <param name="x1">The first x value.</param>
		/// <param name="y1">The first y value.</param>
		/// <param name="w1">W1.</param>
		/// <param name="h1">H1.</param>
		/// <param name="x2">The second x value.</param>
		/// <param name="y2">The second y value.</param>
		/// <param name="w2">W2.</param>
		/// <param name="h2">H2.</param>
		public static bool Collision2D (float x1, float y1, float w1, float h1, float x2, float y2, float w2, float h2) {
			//_that.canvas.fillStyle('#FF0000').fillRect(x1, y1, w1, h1).fillStyle('#0000FF').fillRect(x2, y2, w2, h2);
			if(Mathf.Abs((x1 + (w1 * 0.5f)) - (x2 + (w2 * 0.5f))) < ((w1 + w2) * 0.5f) && Mathf.Abs((y1 + (h1 * 0.5f)) - (y2 + (h2 * 0.5f))) < ((h1 + h2) * 0.5f)) {
				return true;
			}
			return false;
		}

		static UnityEngine.Object popMsgObj = null;

		/// <summary>
		/// 创建飘字预设
		/// </summary>
		/// <param name="position">Position.</param>
		/// <param name="msg">Message.</param>
		/// <param name="color">Color.</param>
		/// <param name="fontSize">Font size.</param>
		/// <param name="strength">Strength.</param>
		public static void CreatePopMsg(Vector3 position, string msg, Color color, int fontSize = 50, float strength = 1) {
			if (popMsgObj == null) {
				popMsgObj = GetPrefab("Prefabs/UI/Comm/PopMsg");
			}
			GameObject popMsgPrefab = GetPrefabClone(popMsgObj);
			popMsgPrefab.transform.SetParent(UIModel.FontCanvas.transform);
			RectTransform rectTrans = popMsgPrefab.GetComponent<RectTransform>();
			popMsgPrefab.transform.position = position;
			rectTrans.localScale = Vector3.one;
			PopMsg popMsg = popMsgPrefab.GetComponent<PopMsg>();
			popMsg.Msg = msg;
			popMsg.Color = color;
			popMsg.FontSize = fontSize;
			popMsg.Strength = strength;
		}

		static UnityEngine.Object diaogMsgObj = null;

		/// <summary>
		/// 创建文字对话旗袍
		/// </summary>
		/// <param name="position">Position.</param>
		/// <param name="msg">Message.</param>
		/// <param name="color">Color.</param>
		/// <param name="fontSize">Font size.</param>
		public static void CreateDialogMsgPop(Vector3 position, string msg, Color color, int fontSize = 22) {
			if (diaogMsgObj == null) {
				diaogMsgObj = GetPrefab("Prefabs/UI/Comm/DialogMsgPop");
			}
			if (UIModel.DialogMsgPopScript != null) {
				UIModel.DialogMsgPopScript.Disposed();
				MonoBehaviour.Destroy(UIModel.DialogMsgPopScript.gameObject);
				UIModel.DialogMsgPopScript = null;
			}
			UIModel.DialogMsgPopScript = GetPrefabClone(diaogMsgObj).GetComponent<DialogMsgPop>();
			UIModel.DialogMsgPopScript.transform.SetParent(UIModel.UICanvas.transform);
			UIModel.DialogMsgPopScript.SetData(position, msg, color, fontSize);
		}

		/// <summary>
		/// 过滤掉html标签
		/// </summary>
		/// <returns>The html.</returns>
		/// <param name="strHtml">String html.</param>
		public static string StripHtml(string strHtml) {
			Regex objRegExp = new Regex("<(.|\n)+?>");
			string strOutput = objRegExp.Replace(strHtml, "");
			strOutput = strOutput.Replace("<", "&lt;");
			strOutput = strOutput.Replace(">", "&gt;");
			return strOutput;
		}

		/// <summary>  
		/// 将c# DateTime时间格式转换为Unix时间戳格式  
		/// </summary>  
		/// <param name="time">时间</param>  
		/// <returns>long</returns>  
		public static long ConvertDateTimeToInt(System.DateTime time) {
			System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0));
			long t = (time.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位      
			return t;
		}
		/// <summary>        
		/// 时间戳转为C#格式时间        
		/// </summary>        
		/// <param name=”timeStamp”></param>        
		/// <returns></returns>        
		public static DateTime ConvertStringToDateTime(string timeStamp) {
			DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
			long lTime = long.Parse(timeStamp + "0000");
			TimeSpan toNow = new TimeSpan(lTime);
			return dtStart.Add(toNow);
		}

		/// <summary>
		/// 获取当前时间戳
		/// </summary>
		/// <returns></returns>
		public static long GetNowTimeStamp() {
			return ConvertDateTimeToInt(DateTime.Now);
		}

		/// <summary>
		/// 获取声音的克隆对象
		/// </summary>
		/// <returns>The sound prefab clone.</returns>
		/// <param name="soundId">Sound identifier.</param>
		public static GameObject GetSoundPrefabClone(string soundId) {
			if (!soundsMapping.ContainsKey(soundId)) {
				soundsMapping.Add(soundId, GetPrefab(JsonManager.GetInstance().GetMapping<SoundData>("Sounds", soundId).Src));
			}
			return GetPrefabClone(soundsMapping[soundId]);
		}

		/// <summary>
		/// 获取技能粒子特效的克隆对象
		/// </summary>
		/// <returns>The skill effect prefab clone.</returns>
		/// <param name="effectSrc">Effect source.</param>
		public static GameObject GetSkillEffectPrefabClone(string effectSrc) {
			if (!skillEffectsMapping.ContainsKey(effectSrc)) {
				UnityEngine.Object obj = GetPrefab(effectSrc);
				if (obj == null) {
					return null;
				}
				skillEffectsMapping.Add(effectSrc, obj);
			}
			return GetPrefabClone(skillEffectsMapping[effectSrc]);
		}

		/// <summary>
		/// 获取门派名称
		/// </summary>
		/// <returns>The occupation name.</returns>
		/// <param name="type">Type.</param>
		public static string GetOccupationName(OccupationType type) {
			if (occupationNameMapping.ContainsKey(type)) {
				return occupationNameMapping[type];
			}
			return "";
		}

        /// <summary>
        /// 获取门派弟子称谓
        /// </summary>
        /// <returns>The occupation desc.</returns>
        /// <param name="type">Type.</param>
        public static string GetOccupationDesc(OccupationType type) {
            string occStr = GetOccupationName(type);
            switch (type) {
                case OccupationType.YueJiaJun:
                case OccupationType.ShenBingYing:
                    return occStr + "将士";
                default:
                    return occStr + "弟子";
            }
        }

		/// <summary>
		/// 生产资源名称
		/// </summary>
		/// <returns>The resource name.</returns>
		/// <param name="type">Type.</param>
		public static string GetResourceName(ResourceType type) {
			if (resourceNameMapping.ContainsKey(type)) {
				return resourceNameMapping[type];
			}
			return "";
		}

		/// <summary>
		/// 伤势类型
		/// </summary>
		/// <returns>The injury name.</returns>
		/// <param name="type">Type.</param>
		public static string GetInjuryName(InjuryType type) {
			if (injuryNameMapping.ContainsKey(type)) {
				return injuryNameMapping[type];
			}
			return "";
		}

		/// <summary>
		/// 获取物品类型名称
		/// </summary>
		/// <returns>The item type name.</returns>
		/// <param name="type">Type.</param>
		public static string GetItemTypeName(ItemType type) {
			if (itemTypeNameMapping.ContainsKey(type)) {
				return itemTypeNameMapping[type];
			}
			return "";
		}

		/// <summary>
		/// 获取区域大地图的随机遇敌概率
		/// </summary>
		/// <returns>The meet enemy rates.</returns>
		/// <param name="areaId">Area identifier.</param>
		public static List<RateData> GetMeetEnemyRates(string areaId) {
			if (meetEnemyRatesMapping.ContainsKey(areaId)) {
				return meetEnemyRatesMapping[areaId];
			}
			return meetEnemyRatesMapping["0"];
		}

		/// <summary>
		/// 获取伤势颜色
		/// </summary>
		/// <returns>The injury color.</returns>
		/// <param name="type">Type.</param>
		public static Color GetInjuryColor(InjuryType type) {
			switch(type) {
			case InjuryType.None:
			default:
				return new Color(0, 1, 0, 1);
			case InjuryType.White:
				return new Color(1, 1, 1, 1);
			case InjuryType.Yellow:
				return new Color(1, 1, 0, 1);
			case InjuryType.Purple:
				return new Color(1, 0, 1, 1);
			case InjuryType.Red:
				return new Color(1, 0, 0, 1);
			case InjuryType.Moribund:
				return new Color(0.5f, 0, 0, 1);
			}
		}

		/// <summary>
		/// 获取性别称谓
		/// </summary>
		/// <returns>The gender desc.</returns>
		/// <param name="gender">Gender.</param>
		public static string GetGenderDesc(GenderType gender) {
			if (gender == GenderType.Male) {
				return "少侠";
			}
			else {
				return "女侠";
			}
		}

		/// <summary>
		/// 获取生产资源Icon Sprite
		/// </summary>
		/// <returns>The resource sprite.</returns>
		/// <param name="type">Type.</param>
		public static Sprite GetResourceSprite(ResourceType type) {
			return GetIconSprite((600000 + (int)type).ToString());
		}

		/// <summary>
		/// 获取时辰
		/// </summary>
		/// <returns>The time name.</returns>
		/// <param name="index">Index.</param>
		public static string GetTimeName(int index) {
			if (timeNames.Length > index) {
				return timeNames[index];
			}
			return "";
		}

		/// <summary>
		/// 获取时辰名列表
		/// </summary>
		/// <returns>The time names.</returns>
		public static string[] GetTimeNames() {
			return timeNames;
		}

		/// <summary>
		/// 返回品质对应的色值
		/// </summary>
		/// <returns>The quality color string.</returns>
		/// <param name="type">Type.</param>
		public static string GetQualityColorString(QualityType type) {
			switch(type) {
			case QualityType.White:
			default:
				return "#AAAAAA";
			case QualityType.Green:
				return "#52CC33";
			case QualityType.Blue:
				return "#1A94E6";
			case QualityType.Purple:
				return "#BB44BB";
			case QualityType.Gold:
				return "#DDDD22";
			case QualityType.Orange:
				return "#FF9900";
			case QualityType.Red:
				return "#EE1111";
			}
		}

		/// <summary>
		/// 将数据库中的物品类型转换成资源类型
		/// </summary>
		/// <returns>The item type to resourct type.</returns>
		/// <param name="type">Type.</param>
		public static ResourceType ChangeItemTypeToResourctType(ItemType type) {
			int _type = Mathf.Clamp((int)type, (int)ItemType.Wheat, (int)ItemType.DarksteelIngot) - 10;
			return (ResourceType)_type;
		}



		/// <summary>
		/// 根据角色模型层名称设置对象的层(只包括第一层的子对象)
		/// </summary>
		/// <param name="trans">Trans.</param>
		/// <param name="name">Name.</param>
		public static void ChangeLayers(Transform trans, string name) {
			ChangeLayers(trans, LayerMask.NameToLayer(name));
		}

		/// <summary>
		/// 根据角色模型层索引设置对象的层
		/// </summary>
		/// <param name="trans">Trans.</param>
		/// <param name="layer">Layer.</param>
		public static void ChangeLayers(Transform trans, int layer) {
			if (trans == null) {
				return;
			}
			trans.gameObject.layer = layer;
			foreach (Transform child in trans) {
				ChangeLayers(child, layer);
			}
		}

		/// <summary>
		/// 返回性别对应颜色
		/// </summary>
		/// <returns>The gender color.</returns>
		/// <param name="type">Type.</param>
		public static string GetGenderColor(GenderType type) {
			return type == GenderType.Male ? "#4DD0FB" : "#FF67C8";
		}

        /// <summary>
        /// 返回枚举值的描述
        /// </summary>
        /// <returns>The enmu desc.</returns>
        /// <param name="enmuType">Enmu type.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static string GetEnmuDesc<T>(T enmuType) {
            string enumTypeName = typeof(T).Name;
            if (!enumTypeDescMapping.ContainsKey(enumTypeName)) {
                Dictionary<string, string> mapping = new Dictionary<string, string>();
                FieldInfo fieldInfo;
                object[] attribArray;
                DescriptionAttribute attrib;
                foreach(T type in Enum.GetValues(typeof(T))) {
                    fieldInfo = type.GetType().GetField(type.ToString());
                    attribArray = fieldInfo.GetCustomAttributes(false);
                    for (int i = 0; i < attribArray.Length; i++) {
                        if (attribArray[i].GetType() == typeof(DescriptionAttribute)) {
                            attrib = (DescriptionAttribute)attribArray[i];
                            mapping.Add(type.ToString(), attrib.Description);
                            break;
                        }

                    }
                }
                enumTypeDescMapping.Add(enumTypeName, mapping);
            }
            if (enumTypeDescMapping[enumTypeName].ContainsKey(enmuType.ToString())) {
                return enumTypeDescMapping[enumTypeName][enmuType.ToString()];
            }
            return null;
        }

        /// <summary>
        /// 消除误差
        /// </summary>
        /// <returns>The error.</returns>
        /// <param name="value">Value.</param>
        /// <param name="rate">Rate.</param>
        public static double ClearError(double value, double rate = 1000d) {
            return ((long)(value * rate)) / rate;
        }

        /// <summary>
        /// 动态获取材质
        /// </summary>
        /// <param name="iconId"></param>
        /// <returns></returns>
        public static Material GetMaterial(string name) {
            if (!materialMapping.ContainsKey(name)) {
                GameObject obj = Resources.Load<GameObject>("Prefabs/Material/" + name);
                if (obj != null) {
                    materialMapping.Add(name, obj.GetComponent<Image>().material);
                }
                else {
                    name = "UIDefaultGreyMaterialImage";
                }
                obj = null;
            }
            return materialMapping[name];
        }
	}
}

