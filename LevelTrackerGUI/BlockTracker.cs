using System;
using System.Collections.Generic;
using System.Linq;
using Assets.Scripts.LevelGeneration;
using UnityEngine;

namespace LevelTracker
{
	public static class BlockTracker
	{
		public static void Load()
		{
			BlockTracker.List.Clear();
			MineableBlock[] array = UnityEngine.Object.FindObjectsOfType<MineableBlock>().ToArray<MineableBlock>();
			MineableBlock[] array2 = array.Where((MineableBlock x) => x.materialType != (ECurrency)(-1)).ToArray<MineableBlock>();
			MineableBlock[] array3 = array2;
			for (int i = 0; i < array3.Length; i++)
			{
				MineableBlock mineableBlock = array3[i];
				string blockName = mineableBlock.ExGetMaterialName();
				BlockTracker.BlockData blockData = BlockTracker.List.FirstOrDefault((BlockTracker.BlockData x) => x.Name == blockName);
				bool flag = blockData == null;
				if (flag)
				{
					blockData = new BlockTracker.BlockData(blockName)
					{
						Type = mineableBlock.materialType
					};
					BlockTracker.List.Add(blockData);
				}
				bool flag2 = mineableBlock.materialCount == 0;
				if (!flag2)
				{
					blockData.RateList.Add(mineableBlock.materialCount);
					blockData.Blocks.Add(mineableBlock);
				}
			}
			BlockTracker.ListBlocks();
		}

		public static void Unload()
		{
			BlockTracker.List.Clear();
		}

		public static void ListBlocks()
		{
			Debug.LogWarning("[Level Tracker] Listing blocks");
			Debug.LogWarning("[Level Tracker] ========================================");
			int num = BlockTracker.List.Max((BlockTracker.BlockData x) => x.Name.Length);
			foreach (BlockTracker.BlockData blockData in BlockTracker.List.OrderByDescending((BlockTracker.BlockData x) => x.Type))
			{
				bool flag = blockData.TotalBlocks == 0;
				if (!flag)
				{
					string text = string.Format("[Level Tracker] {0} | x{1:00} | Total: {2:00}", blockData.Name.Extend(num), blockData.TotalBlocks, blockData.TotalCurrency);
					bool flag2 = (int)blockData.Type <= 3;
					if (flag2)
					{
						Debug.Log(text);
					}
					else
					{
						Debug.LogWarning(text);
					}
				}
			}
			Debug.LogWarning("[Level Tracker] ========================================");
		}

		public static List<BlockTracker.BlockData> List = new List<BlockTracker.BlockData>();

		public class BlockData
		{
			public BlockData(string name)
			{
				this.Name = name;
			}

			public int TotalBlocks
			{
				get
				{
					return this.Blocks.Count((MineableBlock x) => x.ExIsValid());
				}
			}

			public int TotalCurrency
			{
				get
				{
					return this.Blocks.Where((MineableBlock x) => x.ExIsValid()).Sum((MineableBlock x) => x.materialCount);
				}
			}

			public double RateAverage
			{
				get
				{
					return this.RateList.Average();
				}
			}

			public string Name;

			public ECurrency Type;

			public List<MineableBlock> Blocks = new List<MineableBlock>();

			public List<int> RateList = new List<int>();
		}
	}
}
