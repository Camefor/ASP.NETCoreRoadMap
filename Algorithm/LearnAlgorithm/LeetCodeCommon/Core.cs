using System;
using System.Collections.Generic;
using System.Linq;
namespace LearnAlgorithm.LeetCodeCommon
{
    public class Core
    {

        /// <summary>
        /// 搜索插入位置 
        /// </summary>
        /// <returns></returns>
        static int SearchInsert()
        {
            int[] nums = { 1, 3, 5, 6 };
            int target = 5;
            for (int i = 0; i < nums.Length; i++)
            {
                if (nums[i] == target)
                {
                    return i;
                }
                if (nums[i] > target)
                {
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
        public int SearchInsert2(int[] nums, int target)
        {
            int left = 0;
            int right = nums.Length - 1;
            int mid = 0;
            while (left <= right)
            {
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
        static int PivotIndex2()
        {
            int[] nums = { 1, 7, 3, 6, 5, 6 };
            int sum = 0, leftsum = 0;
            foreach (var item in nums) sum += item; //数组和

            for (int i = 0; i < nums.Length; i++)
            {
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
        static int PivotIndex()
        {
            int[] nums = { 1, 7, 3, 6, 5, 6 };
            if (nums.Length == 0) return -1;
            for (int i = 0; i < nums.Length; i++)
            {
                int a = 0, b = 0;
                for (int x = 0; x < i; x++)
                { //从左往右
                    a += nums[x]; //只计算第 i-1个元素的和 
                    //每次进入循环多计算一个元素
                }

                for (int y = nums.Length - 1; y > i; y--)
                { //从右往左
                    b += nums[y]; //计算数组的.Length -2 -i 个元素的和
                    // 每次进入循环少计算一次元素
                }

                if (a == b)
                {
                    return i;
                }
            }
            return -1;
        }

        static int FindRepeatNumber()
        {
            int[] nums = { 8, 3, 1, 6 };
            Dictionary<int, int> dic = new Dictionary<int, int>();
            for (int i = 0; i < nums.Length; i++)
            {
                if (dic.ContainsKey(nums[i]))
                {
                    return nums[i];
                }
                else
                {
                    dic.Add(nums[i], nums[i]);
                }
            }
            return 0;
        }

        public static int[] BubbleSort(int[] array)
        {
            int length = array.Length;

            for (int i = 0; i < length; i++)
            {
                for (int j = i; j < length; j++)
                {
                    if (array[i] > array[j])
                    {
                        int temp = array[i];
                        array[i] = array[j];
                        array[j] = temp;
                    }
                }
            }
            return array;
        }

        public static int[] sort(int[] array)
        {
            int length = array.Length;
            if (length > 0)
            {
                for (int i = 1; i < length; i++)
                {
                    for (int j = 0; j < length - i; j++)
                    {
                        if (array[j] > array[j + 1])
                        {
                            int temp = array[j];
                            array[j] = array[j + 1];
                            array[j + 1] = temp;
                        }
                    }
                }
            }
            return array;
        }




        public int[] Bubble(int[] arr)
        {
            int length = arr.Length;
            int temp = 0;
            for (int i = 1; i < length; i++)
            { //变量 i 决定开始第几轮排序
                for (int j = 0; j < length - 1; j++)
                {
                    if (arr[j] > arr[j + 1])
                    { //相邻的元素 两两比较 ，根据大小来交换元素的位置
                        temp = arr[j];
                        arr[j] = arr[j + 1];
                        arr[j + 1] = temp;
                    }
                }
            }
            return arr;
        }

        /// <summary>
        /// 判断回文
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public bool CheckPalinDrome(string str)
        {
            char[] charCompare = str.ToCharArray();
            Stack<char> s = new Stack<char>();
            Queue<char> q = new Queue<char>();
            bool justfy = true;

            for (var i = 0; i < str.Length; i++)
            {
                s.Push(charCompare[i]); //栈 先进后出 后进先出 
                q.Enqueue(charCompare[i]); //队列  先进先出
            }

            for (var j = 0; j < str.Length; j++)
            {
                // justfy = s.Pop () != q.Dequeue ();
                if (s.Pop() != q.Dequeue())
                {
                    justfy = false;
                }
            }

            return justfy;
        }

        public bool CheckPalinDrome2(string str)
        {
            bool justfy = true;
            char[] charCompare = str.ToCharArray();
            char[] SArr = new char[charCompare.Length];
            char[] QArr = new char[charCompare.Length];

            int addC = 0, minC = str.Length - 1;
            foreach (var item in charCompare)
            {
                //str = "smadamq";//madam1
                //qmadams
                SArr[addC] = item;
                addC++;

                //smadamq
                QArr[minC] = item;
                minC--;
            }

            for (var k = 0; k < str.Length; k++)
            {
                if (SArr[k] != QArr[k])
                {
                    justfy = false;
                }
            }

            return justfy;
        }

        /// <summary>
        /// 两个无序数组的合并
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public List<int> Merge(int[] a, int[] b)
        {
            List<int> c = new List<int>();
            int i = 0, j = 0;
            if (a == null || a.Length == 0) return b.ToList<int>();
            if (b == null || b.Length == 0) return a.ToList<int>();

            Array.Sort(a);
            Array.Sort(b);

            if (a[0] > b[0])
            {
                c.Add(b[0]);
                j++;
            }
            else
            {
                c.Add(a[0]);
                i++;
            }
            while (i < a.Length && j < b.Length)
            {
                if (a[i] < b[j])
                {
                    c.Add(a[i]);
                    i++;
                }
                else
                {
                    c.Add(b[j]);
                    j++;
                }
            }

            while (i < a.Length)
            {
                c.Add(a[i]);
                i++;
            }

            while (j < b.Length)
            {
                c.Add(b[j]);
                j++;
            }

            return c;
        }
    }
}
