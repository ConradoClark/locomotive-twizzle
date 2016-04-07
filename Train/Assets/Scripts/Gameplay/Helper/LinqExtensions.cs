using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class LinqExtensions
{
    public static IEnumerable<IEnumerable<T>> Batch<T>(this IEnumerable<T> items,
                                                  int maxItems)
    {
        return items.Select((item, inx) => new { item, inx })
                    .GroupBy(x => x.inx / maxItems)
                    .Select(g => g.Select(x => x.item));
    }

    public static IEnumerable<IEnumerable<char>> HyphenateAllLines(this IEnumerable<char> chars, int charsPerLine)
    {
        return chars.Hyphenate(charsPerLine).ToArray().Batch(charsPerLine);
    }

    public static IEnumerable<char> Hyphenate(this IEnumerable<char> chars, int charsPerLine)
    {
        int currentCharCount = 1;
        int currentIndex = 0;
        var enumerator = chars.GetEnumerator();
        char previous = '\0';
        char[] copy = chars.ToArray();

        while (enumerator.MoveNext())
        {
            char current = enumerator.Current;
            if ((currentCharCount == 0 || currentCharCount % charsPerLine == 1) && current == ' ')
            {
                currentIndex++;
                Debug.Log(string.Format("Space at first char - current:'{0}' | currentCharCount:'{1}' | charsPerLine:'{2}'", current, currentCharCount, charsPerLine));
                continue;
            }

            if (currentCharCount % charsPerLine == charsPerLine - 1)
            {
                char next = copy.Skip(currentIndex + 1).FirstOrDefault();
                char next2 = copy.Skip(currentIndex + 2).FirstOrDefault();

                if (next == default(char))
                {
                    yield return current;
                    yield break;
                }

                if (next2 == default(char))
                {
                    yield return current;
                    yield return next;
                    yield break;
                }

                if (current != ' ' && previous != ' ' && next != ' ' && next2 != ' ')
                {
                    currentCharCount++;
                    Debug.Log(string.Format("Hyphenating - current:'{0}' | currentCharCount:'{1}' | charsPerLine:'{2}'", current.ToString() + next + next2, currentCharCount, charsPerLine));
                    yield return current;
                    if (charsPerLine > 1)
                    {
                        yield return '-';
                    }
                }
                else
                {
                    Debug.Log(string.Format("Not Hyphenating - current:'{0}' | currentCharCount:'{1}' | charsPerLine:'{2}'", current.ToString() + next + next2, currentCharCount, charsPerLine));
                    yield return current;
                }
            }
            else
            {
                Debug.Log(string.Format("Placing char - current:'{0}' | currentCharCount:'{1}' | charsPerLine:'{2}'", current, currentCharCount, charsPerLine));
                yield return current;
            }
            previous = current;
            currentCharCount++;
            currentIndex++;
        }
    }

    public static IEnumerable<IEnumerable<char>> GroupByWords(this IEnumerable<char> chars, int charsPerLine)
    {
        char[] copy = chars.ToArray();

        Queue<char> allChars = new Queue<char>(chars);

        while (allChars.Count > 0)
        {
            var firstSpaces = allChars.TakeWhile(c => c == ' ').ToList();
            var charsInLine = allChars.SkipWhile(c => c == ' ').Take(charsPerLine).ToList();
            var fittingChars = charsInLine.TakeWhile((c, i) => i < charsInLine.LastIndexOf(' ')).ToList();

            Debug.Log("charsInLine: " + new string(charsInLine.ToArray()));
            Debug.Log("fittingChars: " + new string(fittingChars.ToArray()));

            if (allChars.Count + fittingChars.Count <= charsPerLine)
            {
                for (int i=0;i< charsInLine.Count+1; i++)
                {
                    if (allChars.Count>0)
                    allChars.Dequeue();
                }
                yield return charsInLine;
                continue;
            }

            firstSpaces.ForEach(c => allChars.Dequeue());
            fittingChars.ForEach(c => allChars.Dequeue());
            if (fittingChars.Count == 0)
            {
                var charsInLineExceptLast = charsInLine.TakeWhile((c, i) => i < charsInLine.Count - 1).ToList();
                charsInLineExceptLast.ForEach(c => allChars.Dequeue());
                if (allChars.Count <= 1)
                {
                    yield return charsInLineExceptLast;
                    yield return allChars;
                    break;
                }
                if (allChars.Count > 0)
                {
                    yield return charsInLineExceptLast.Concat(new char[] { '-' });
                }
                continue;
            }
           
            yield return fittingChars;
        }
    }

    //public static IEnumerable<IEnumerable<char>> JustifyAllLines(this IEnumerable<char> chars, int charsPerLine)
    //{
    //    char[] copy = chars.ToArray();

    //    Queue<char> allChars = new Queue<char>(chars);

    //    while (allChars.Count > 0)
    //    {
    //        var firstSpaces = allChars.TakeWhile(c => c == ' ').ToList();
    //        var charsInLine = allChars.SkipWhile(c => c == ' ').Take(charsPerLine).ToList();
    //        var fittingChars = charsInLine.TakeWhile((c, i) => i < charsInLine.LastIndexOf(' ')).ToList();

    //        firstSpaces.ForEach(c => allChars.Dequeue());
    //        fittingChars.ForEach(c => allChars.Dequeue());

    //        int numberOfSpaces = fittingChars.Count(c => c == ' ');

    //        if (fittingChars.Count == 0)
    //        {
    //            if (allChars.Count <= 1) yield break;
    //            var charsInLineExceptLast = charsInLine.TakeWhile((c, i) => i < charsInLine.Count - 1).ToList();
    //            charsInLineExceptLast.ForEach(c => allChars.Dequeue());
    //            yield return charsInLineExceptLast.Concat(new char[] { '-' });
    //            continue;
    //        }

    //        if (fittingChars.Count == charsInLine.Count)
    //        {
    //            yield return charsInLine;
    //            continue;
    //        }

    //        if (numberOfSpaces > 0)
    //        {
    //            int spacesToFill = charsPerLine - fittingChars.Count;
    //            for (int i = 0; i < spacesToFill; i++)
    //            {
    //                fittingChars.Insert(fittingChars.FindNIndex((i+i) % numberOfSpaces, c => c == ' '), ' ');
    //                numberOfSpaces++;
    //            }

    //        }
    //        yield return fittingChars;
    //    }
    //}

    public static int FindNIndex<T>(this IEnumerable<T> source, int n, Predicate<T> match)
    {
        if (n < 0) return -1;
        if (match == null) throw new ArgumentNullException("match cannot be null.");

        int index = 0;
        var enumerator = source.GetEnumerator();
        while (enumerator.MoveNext())
        {
            var current = enumerator.Current;
            if (match(current))
            {
                n--;
                if (n < 0)
                {
                    return index;
                }
            }
            index++;
        }
        return -1;
    }
}
