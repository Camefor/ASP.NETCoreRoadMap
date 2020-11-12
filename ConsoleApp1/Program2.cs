using System;
using System.Diagnostics.Contracts;

namespace StringMatching {
    class Program {
        static void Main(string[] args) {
            char[] text1 = "BBC ABCDAB ABCDABCDABDE".ToCharArray();
            char[] pattern1 = "ABCDABD".ToCharArray();

            int firstShift1;
            bool isMatched1 = KmpStringMatcher.TryMatch1(text1, pattern1, out firstShift1);
            Contract.Assert(isMatched1);
            Contract.Assert(firstShift1 == 15);

            char[] text2 = "ABABDAAAACAAAABCABAB".ToCharArray();
            char[] pattern2 = "AAACAAAA".ToCharArray();

            int firstShift2;
            bool isMatched2 = KmpStringMatcher.TryMatch2(text2, pattern2, out firstShift2);
            Contract.Assert(isMatched2);
            Contract.Assert(firstShift2 == 6);

            char[] text3 = "ABAAACAAAAAACAAAABCABAAAACAAAAFDLAAACAAAAAACAAAA".ToCharArray();
            char[] pattern3 = "AAACAAAA".ToCharArray();

            int[] shiftIndexes3 = KmpStringMatcher.MatchAll1(text3, pattern3);
            Contract.Assert(shiftIndexes3.Length == 5);
            Contract.Assert(string.Join(",", shiftIndexes3) == "2,9,22,33,40");
            int[] shiftIndexes4 = KmpStringMatcher.MatchAll2(text3, pattern3);
            Contract.Assert(shiftIndexes4.Length == 5);
            Contract.Assert(string.Join(",", shiftIndexes4) == "2,9,22,33,40");

            Console.WriteLine("Well done!");
            Console.ReadKey();
        }
    }

    public class KmpStringMatcher {
        public static bool TryMatch1(char[] text, char[] pattern, out int firstShift) {
            // KMP needs a pattern preprocess to get the Partial Match Table
            int[] lps = PreprocessToComputeLongestProperPrefixSuffixArray(pattern);
            // pattern: ABCDABD
            // char:  | A | B | C | D | A | B | D |
            // index: | 0 | 1 | 2 | 3 | 4 | 5 | 6 |
            // lps:   | 0 | 0 | 0 | 0 | 1 | 2 | 0 |

            firstShift = -1;
            int n = text.Length;
            int m = pattern.Length;
            int s = 0, j = 0;

            while (s < n && j < m) {
                if (j == -1 || text[s] == pattern[j]) {
                    s++;
                    j++;
                } else {
                    // here is different with naive string matcher
                    if (j != 0)
                        j = lps[j - 1];
                    else
                        s++;
                }

                if (j == m) {
                    firstShift = s - j;
                    return true;
                }
            }

            return false;
        }

        static int[] PreprocessToComputeLongestProperPrefixSuffixArray(char[] pattern) {
            int m = pattern.Length;

            // hold the longest prefix suffix values for pattern
            int[] lps = new int[m];
            lps[0] = 0;

            // length of the previous longest prefix suffix
            int k = 0;
            int q = 1;
            while (q < m) {
                if (pattern[k] == pattern[q]) {
                    k++;
                    lps[q] = k;
                    q++;
                } else {
                    if (k != 0) {
                        k = lps[k - 1];
                    } else {
                        lps[q] = 0;
                        q++;
                    }
                }
            }

            return lps;
        }

        public static bool TryMatch2(char[] text, char[] pattern, out int firstShift) {
            // KMP needs a pattern preprocess
            int[] next = PreprocessToGetNextArray(pattern);
            // pattern: ABCDABD
            // char:  | A | B | C | D | A | B | D |
            // index: | 0 | 1 | 2 | 3 | 4 | 5 | 6 |
            // lps:   | 0 | 0 | 0 | 0 | 1 | 2 | 0 |
            // next:  |-1 | 0 | 0 | 0 | 0 | 1 | 2 | -> shift LPS 1 position to right

            firstShift = -1;
            int n = text.Length;
            int m = pattern.Length;
            int s = 0, j = 0;

            while (s < n && j < m) {
                if (j == -1 || text[s] == pattern[j]) {
                    s++;
                    j++;
                } else {
                    // here is different with naive string matcher
                    j = next[j];
                }

                if (j == m) {
                    firstShift = s - j;
                    return true;
                }
            }

            return false;
        }

        static int[] PreprocessToGetNextArray(char[] pattern) {
            int m = pattern.Length;
            int[] next = new int[m];
            next[0] = -1;

            int k = -1;
            int q = 0;
            while (q < m - 1) {
                if (k == -1 || pattern[k] == pattern[q]) {
                    k++;
                    q++;

                    //next[q] = k;       // does not optimize

                    if (pattern[k] != pattern[q])
                        next[q] = k;
                    else
                        next[q] = next[k]; // with optimization
                } else {
                    k = next[k];
                }
            }

            return next;
        }

        public static int[] MatchAll1(char[] text, char[] pattern) {
            // KMP needs a pattern preprocess
            int[] lps = PreprocessToComputeLongestProperPrefixSuffixArray(pattern);
            // pattern: ABCDABD
            // char:  | A | B | C | D | A | B | D |
            // index: | 0 | 1 | 2 | 3 | 4 | 5 | 6 |
            // lps:   | 0 | 0 | 0 | 0 | 1 | 2 | 0 |

            int n = text.Length;
            int m = pattern.Length;
            int s = 0, j = 0;
            int[] shiftIndexes = new int[n - m + 1];
            int c = 0;

            while (s < n && j < m) {
                if (j == -1 || text[s] == pattern[j]) {
                    s++;
                    j++;
                } else {
                    // here is different with naive string matcher
                    if (j != 0)
                        j = lps[j - 1];
                    else
                        s++;
                }

                if (j == m) {
                    shiftIndexes[c] = s - j;
                    c++;

                    j = lps[j - 1];
                }
            }

            int[] shifts = new int[c];
            for (int y = 0; y < c; y++) {
                shifts[y] = shiftIndexes[y];
            }

            return shifts;
        }

        public static int[] MatchAll2(char[] text, char[] pattern) {
            // KMP needs a pattern preprocess
            int[] next = PreprocessToGetNextArray(pattern);
            // pattern: ABCDABD
            // char:  | A | B | C | D | A | B | D |
            // index: | 0 | 1 | 2 | 3 | 4 | 5 | 6 |
            // lps:   | 0 | 0 | 0 | 0 | 1 | 2 | 0 |
            // next:  |-1 | 0 | 0 | 0 | 0 | 1 | 2 | -> shift LPS 1 position to right

            int n = text.Length;
            int m = pattern.Length;
            int s = 0, j = 0;
            int[] shiftIndexes = new int[n - m + 1];
            int c = 0;

            while (s < n && j < m) {
                if (j == -1 || text[s] == pattern[j]) {
                    s++;
                    j++;
                } else {
                    // here is different with naive string matcher
                    j = next[j];
                }

                if (j == m) {
                    shiftIndexes[c] = s - j;
                    c++;

                    j = next[j - 1];
                }
            }

            int[] shifts = new int[c];
            for (int y = 0; y < c; y++) {
                shifts[y] = shiftIndexes[y];
            }

            return shifts;
        }
    }
}