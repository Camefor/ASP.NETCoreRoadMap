using System;
using System.Collections.Generic;

namespace LearnAlgorithm {
    class Program {

        public int age { get; set; }

        static void Main (string[] args) {
            LearnAlgorithm.microsoft.General general = new microsoft.General ();
            int[] a = { 1, 3, 4, 2 };
            int[] b = { 5, 3, 4 };
            var res = general.Merge (a, b);
            foreach (var item in res) {
                System.Console.WriteLine (item);
            }
        }

        /// <summary>
        /// 搜索插入位置 
        /// </summary>
        /// <returns></returns>
        static int SearchInsert () {
            int[] nums = { 1, 3, 5, 6 };
            int target = 5;
            for (int i = 0; i < nums.Length; i++) {
                if (nums[i] == target) {
                    return i;
                }
                if (nums[i] > target) {
                    return i;
                }
            }
            return nums.Length;
        }

        /// <summary>
        /// 搜索插入位置 二分查找
        /// </summary>
        /// <param name="nums"></param>
        /// <param name="target"></param>
        /// <returns></returns>
        public int SearchInsert2 (int[] nums, int target) {
            int left = 0;
            int right = nums.Length - 1;
            int mid = 0;
            while (left <= right) {
                mid = (left + right) / 2;
                if (target == nums[mid]) return mid;
                if (target > nums[mid]) left = mid + 1;
                if (target < nums[mid]) right = mid - 1;
            }
            return left;
        }

        /// <summary>
        ///寻找数组的中心索引  前缀和
        /// </summary>
        /// <returns></returns>
        static int PivotIndex2 () {
            int[] nums = { 1, 7, 3, 6, 5, 6 };
            int sum = 0, leftsum = 0;
            foreach (var item in nums) sum += item; //数组和

            for (int i = 0; i < nums.Length; i++) {
                if (leftsum == sum - leftsum - nums[i]) return i;
                leftsum += nums[i];
            }
            return -1;
        }

        /// <summary>
        /// 寻找数组的中心索引:<br/> <see cref="https://leetcode-cn.com/problems/find-pivot-index/"/>
        /// 给定一个整数类型的数组 nums，请编写一个能够返回数组 “中心索引” 的方法。<br/>
        /// 我们是这样定义数组 中心索引 的：数组中心索引的左侧所有元素相加的和等于右侧所有元素相加的和。<br/>
        /// 如果数组不存在中心索引，那么我们应该返回 -1。如果数组有多个中心索引，那么我们应该返回最靠近左边的那一个。<br/>
        /// </summary>
        /// <returns></returns>
        static int PivotIndex () {
            int[] nums = { 1, 7, 3, 6, 5, 6 };
            if (nums.Length == 0) return -1;
            for (int i = 0; i < nums.Length; i++) {
                int a = 0, b = 0;
                for (int x = 0; x < i; x++) { //从左往右
                    a += nums[x]; //只计算第 i-1个元素的和 
                    //每次进入循环多计算一个元素
                }

                for (int y = nums.Length - 1; y > i; y--) { //从右往左
                    b += nums[y]; //计算数组的.Length -2 -i 个元素的和
                    // 每次进入循环少计算一次元素
                }

                if (a == b) {
                    return i;
                }
            }
            return -1;
        }

        static int FindRepeatNumber () {
            int[] nums = { 8, 3, 1, 6 };
            Dictionary<int, int> dic = new Dictionary<int, int> ();
            for (int i = 0; i < nums.Length; i++) {
                if (dic.ContainsKey (nums[i])) {
                    return nums[i];
                } else {
                    dic.Add (nums[i], nums[i]);
                }
            }
            return 0;
        }

        public static int[] BubbleSort (int[] array) {
            int length = array.Length;

            for (int i = 0; i < length; i++) {
                for (int j = i; j < length; j++) {
                    if (array[i] > array[j]) {
                        int temp = array[i];
                        array[i] = array[j];
                        array[j] = temp;
                    }
                }
            }
            return array;
        }

        public static int[] sort (int[] array) {
            int length = array.Length;
            if (length > 0) {
                for (int i = 1; i < length; i++) {
                    for (int j = 0; j < length - i; j++) {
                        if (array[j] > array[j + 1]) {
                            int temp = array[j];
                            array[j] = array[j + 1];
                            array[j + 1] = temp;
                        }
                    }
                }
            }
            return array;
        }

    }
}