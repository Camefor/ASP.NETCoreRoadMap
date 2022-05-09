using System;
using System.Collections.Generic;
using LearnAlgorithm.LeetCodeCommon;
namespace LearnAlgorithm
{
    class Program
    {

        static void Main(string[] args)
        {
            Core general = new Core();
            int[] nums = { 1, 7, 3, 6, 5, 9 };
            int target = 9;
            Console.WriteLine(string.Join(" , ", nums));
            Console.WriteLine(target);
            var result = general.SearchInsert2(nums, target);
            Console.WriteLine(result);
            var result2 = SearchInsert(nums, target);
            Console.WriteLine(result2);
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