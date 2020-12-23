using System;
using System.Collections.Generic;
using System.Linq;

namespace LearnAlgorithm.microsoft {
    public class General {

        public int[] Bubble (int[] arr) {
            int length = arr.Length;
            int temp = 0;
            for (int i = 1; i < length; i++) { //变量 i 决定开始第几轮排序
                for (int j = 0; j < length - 1; j++) {
                    if (arr[j] > arr[j + 1]) { //相邻的元素 两两比较 ，根据大小来交换元素的位置
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
        public bool CheckPalinDrome (string str) {
            char[] charCompare = str.ToCharArray ();
            Stack<char> s = new Stack<char> ();
            Queue<char> q = new Queue<char> ();
            bool justfy = true;

            for (var i = 0; i < str.Length; i++) {
                s.Push (charCompare[i]); //栈 先进后出 后进先出 
                q.Enqueue (charCompare[i]); //队列  先进先出
            }

            for (var j = 0; j < str.Length; j++) {
                // justfy = s.Pop () != q.Dequeue ();
                if (s.Pop () != q.Dequeue ()) {
                    justfy = false;
                }
            }

            return justfy;
        }

        public bool CheckPalinDrome2 (string str) {
            bool justfy = true;
            char[] charCompare = str.ToCharArray ();
            char[] SArr = new char[charCompare.Length];
            char[] QArr = new char[charCompare.Length];

            int addC = 0, minC = str.Length - 1;
            foreach (var item in charCompare) {
                //str = "smadamq";//madam1
                //qmadams
                SArr[addC] = item;
                addC++;

                //smadamq
                QArr[minC] = item;
                minC--;
            }

            for (var k = 0; k < str.Length; k++) {
                if (SArr[k] != QArr[k]) {
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
        public List<int> Merge (int[] a, int[] b) {
            List<int> c = new List<int> ();
            int i = 0, j = 0;
            if (a == null || a.Length == 0) return b.ToList<int> ();
            if (b == null || b.Length == 0) return a.ToList<int> ();

            Array.Sort (a);
            Array.Sort (b);

            if (a[0] > b[0]) {
                c.Add (b[0]);
                j++;
            } else {
                c.Add (a[0]);
                i++;
            }
            while (i < a.Length && j < b.Length) {
                if (a[i] < b[j]) {
                    c.Add (a[i]);
                    i++;
                } else {
                    c.Add (b[j]);
                    j++;
                }
            }

            while (i < a.Length) {
                c.Add (a[i]);
                i++;
            }
            
            while (j < b.Length) {
                c.Add (b[j]);
                j++;
            }

            return c;
        }

    }

}