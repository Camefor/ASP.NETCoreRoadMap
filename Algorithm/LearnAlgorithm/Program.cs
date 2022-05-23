using System;
using System.Collections.Generic;
using LearnAlgorithm.LeetCodeCommon;
using LearnAlgorithm.LeetCodeCommon.Sort;

namespace LearnAlgorithm
{
    class Program
    {

        static void Main(string[] args)
        {
            int[] nums = { 1, 7, 3, 6, 5, 9, 21, 2, 23, 11, 32, 45, 25 };

            var leetCodeSort = new Lesson03();
            var result_UserMergeSort = leetCodeSort.MergeSort(nums);


            //var result_UseBubbleSort = leetCodeSort.BubbleSort(nums);
            //var result_UserInsertSort = leetCodeSort.InsertSort(nums);

            //foreach (var item in result_UseBubbleSort) Console.Write(item + ",");
            //Console.WriteLine();
            //foreach (var item in result_UserInsertSort) Console.Write(item + ",");
            //Console.WriteLine();


            foreach (var item in result_UserMergeSort) Console.Write(item + ",");
            Console.WriteLine();
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();

        }


        private static int SearchInsert(int[] nums, int target)
        {
            int left = 0;
            int right = nums.Length - 1;
            while (left <= right)
            {
                int mid = (left + right) / 2;
                if (target == nums[mid]) return mid;
                if (target > nums[mid])
                {
                    left = mid + 1;
                }
                else
                {
                    right = mid - 1;
                }
            }

            return left;
        }


    }
}