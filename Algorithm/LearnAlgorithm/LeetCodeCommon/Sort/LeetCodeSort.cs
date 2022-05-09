using System;
using System.Collections.Generic;
using System.Text;

namespace LearnAlgorithm.LeetCodeCommon.Sort
{
    /// <summary>
    /// 排序算法
    /// </summary>
    public class LeetCodeSort
    {

        /// <summary>
        /// 冒泡排序: 从左到右 从小到大排序
        /// </summary>
        /// <param name="nums"></param>
        public int[] BubbleSort(int[] nums)
        {
            //= { 1, 7, 3, 6, 5, 9 };
            List<string> record = new List<string>();//记录每次两两比较的数的索引
            for (int i = 0; i < nums.Length; i++)
            {
                record.Add($"======第{i + 1} 轮比较=========");
                for (int j = 0; j < nums.Length - 1 - i; j++)
                {
                    record.Add(j.ToString() + " 和 " + (j + 1).ToString());
                    if (nums[j] > nums[j + 1])
                    {
                        var tmp = nums[j + 1];
                        nums[j + 1] = nums[j];
                        nums[j] = tmp;
                    }
                }
            }
            foreach (var item in record)
            {
                Console.WriteLine(item);
            }
            return nums;
        }


    }
}
