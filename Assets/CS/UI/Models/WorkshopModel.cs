﻿using System;
using System.Collections.Generic;

namespace Game {
	/// <summary>
	/// 工坊相关静态数据
	/// </summary>
	public class WorkshopModel {
		/// <summary>
		/// 原材料关系数据
		/// </summary>
		public static List<ResourceRelationshipData> Relationships = null;
		/// <summary>
		/// 初始化资源生产关系
		/// </summary>
		public static void Init() {
			if (Relationships == null) {
				Relationships = new List<ResourceRelationshipData>();
                //牛家村
				Relationships.Add(new ResourceRelationshipData(ResourceType.Wheat, "00001", new List<ResourceData>(), 1, 1, 0.6f, -1));
				Relationships.Add(new ResourceRelationshipData(ResourceType.Food, "00001", new List<ResourceData>() { new ResourceData(ResourceType.Wheat, 2) }, 1, 2, 1.1f, -1));
                Relationships.Add(new ResourceRelationshipData(ResourceType.Cloth, "00001", new List<ResourceData>() { new ResourceData(ResourceType.Food, 1) }, 3, 0.7f, 0.4f, 1));
                Relationships.Add(new ResourceRelationshipData(ResourceType.Wood, "00001", new List<ResourceData>() { new ResourceData(ResourceType.Food, 1) }, 3, 0.7f, 0.4f, 1));
                Relationships.Add(new ResourceRelationshipData(ResourceType.Iron, "00001", new List<ResourceData>() { new ResourceData(ResourceType.Food, 2) }, 1, 4, 2.2f, 3));
                Relationships.Add(new ResourceRelationshipData(ResourceType.Silver, "00001", new List<ResourceData>() { new ResourceData(ResourceType.SilverOre, 1) }, 11, 1.8f, 1, -1));
                //临安集市
                Relationships.Add(new ResourceRelationshipData(ResourceType.SilverOre, "0002", new List<ResourceData>() { new ResourceData(ResourceType.Food, 10) }, 1, 20, 11, 12));
                //观前街
                Relationships.Add(new ResourceRelationshipData(ResourceType.Steel, "1001", new List<ResourceData>() { new ResourceData(ResourceType.Iron, 4) }, 1, 16, 8.8f, 15));
                //归云庄
                Relationships.Add(new ResourceRelationshipData(ResourceType.Tungsten, "2002", new List<ResourceData>() { new ResourceData(ResourceType.Silver, 4) }, 1, 7.3f, 4, 6));
                //参合堂
                Relationships.Add(new ResourceRelationshipData(ResourceType.Jade, "3002", new List<ResourceData>() { new ResourceData(ResourceType.Silver, 5) }, 1, 9.1f, 5, -1));
                //琅嬛玉洞
                Relationships.Add(new ResourceRelationshipData(ResourceType.RefinedSteel, "4002", new List<ResourceData>() { new ResourceData(ResourceType.Steel, 10) }, 1, 160, 88, 150));
                //巨阙阁
                Relationships.Add(new ResourceRelationshipData(ResourceType.RedSteel, "5002", new List<ResourceData>() { new ResourceData(ResourceType.Tungsten, 3), new ResourceData(ResourceType.Cloth, 1) }, 1, 22.5f, 12.4f, 20));
                //金国王府
                Relationships.Add(new ResourceRelationshipData(ResourceType.DarksteelIngot, "6002", new List<ResourceData>() { new ResourceData(ResourceType.Silver, 310) }, 1, 6200, 3410, 4500));
                //襄阳城楼
                Relationships.Add(new ResourceRelationshipData(ResourceType.TungstenSteel, "7002", new List<ResourceData>() { new ResourceData(ResourceType.Tungsten, 100) }, 1, 727.3f, 400, 600));
                //丐帮分舵
                Relationships.Add(new ResourceRelationshipData(ResourceType.Zingana, "8001", new List<ResourceData>() { new ResourceData(ResourceType.Wood, 900), new ResourceData(ResourceType.TungstenSteel, 1) }, 1, 1327.3f, 730, 1500));
			}
		}
	}
}

