using System;
using System.Collections.Generic;
using System.Text;

//https://www.youtube.com/watch?v=UOa3dQT3hOc&list=PLFOgdf0iZ6uskTJam3P2BZwpdAGGG0e24&index=4
namespace LearnAlgorithm.LeetCodeCommon
{


    /// <summary>
    /// 排序算法:
    /// </summary>
    public class Lesson03
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

        /// <summary>
        /// 插入排序算法
        /// </summary>
        /// <param name="nums"></param>
        /// <returns></returns>
        public int[] InsertSort(int[] nums)
        {
            int current = 0;
            for (int i = 1; i < nums.Length; i++) //int 默认等于1  默认数组的第一个元素已经排好序
            {
                int j = i;
                current = nums[i]; //  以便将来把它插入到合适的位置
                //内循环
                for (j = i - 1; j >= 0 && nums[j] > current; j--)
                {
                    nums[j + 1] = nums[j]; //如果当前j所指向的值比current大  就把该数往右移动一位
                }
                //内循环结束后 j + 1 所指向的位置  就是要把current 插入的位置
                nums[j + 1] = current;
            }

            return nums;
        }


        /// <summary>
        /// 归并排序算法
        /// </summary>
        /// <param name="nums"></param>
        /// <returns></returns>
        public int[] MergeSort(int[] A)
        {
            int lo = 0;
            int hi = A.Length - 1;

            int[] result = new int[hi];
            MergeSortHandler(A, lo, hi, ref result);
            return result;
        }
        void MergeSortHandler(int[] A, int lo, int hi, ref int[] result)
        {
            if (lo >= hi) return; //只剩下最后一个元素

            int mid = lo + (hi - lo) / 2; //分两半
            MergeSortHandler(A, lo, mid, ref result);
            MergeSortHandler(A, mid + 1, hi, ref result);
            result = merage(A, lo, mid, hi);
        }

        int[] merage(int[] nums, int lo, int mid, int hi)
        {
            int[] copy = nums.Clone() as int[];

            int k = lo, i = lo, j = mid + 1;

            while (k <= hi)
            {
                if (i > mid)
                {
                    nums[k++] = copy[j++];
                }
                else if (j > hi)
                {
                    nums[k++] = copy[i++];
                }
                else if (copy[j] < copy[i])
                {
                    nums[k++] = copy[j++];
                }
                else
                {
                    nums[k++] = copy[i++];
                }
            }

            return nums;
        }

        //快速排序算法：、、、
        //拓扑排序算法：、、、

    }
}
