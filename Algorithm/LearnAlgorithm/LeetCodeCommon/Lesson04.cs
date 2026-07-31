using System;
using System.Collections.Generic;
using System.Text;

namespace LearnAlgorithm.LeetCodeCommon
{
    /// <summary>
    /// 递归与回溯
    /// </summary>
    public class Lesson04
    {

        /// <summary>
        /// 91 解码方法
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public int NumDecodings(string s)
        {
            char[] chars = s.ToCharArray();
            return decode(chars, chars.Length - 1); // length - 1 从最后一个字符不断的向前递归处理
        }

        private int decode(char[] chars, int index)
        {
            if (index <= 0) return 1; //从后往前处理到了第一个字符 只有一种解法

            int count = 0;
            char curr = chars[index];
            char prev = chars[index - 1];

            if (curr > '0')
            {
                count = decode(chars, index - 1);
            }

            if (prev == '1' || (prev == '2' && curr <= '6'))
            {
                count += decode(chars, index - 2);
            }
            return count;
        }


    }
}
