using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MoogleEngine;

namespace MoogleEngine;
public static class Snippet
{
    static string documentText;
    static string lowerText;

    public static string GetSnippet(DocumentVector queryVector, DocumentVector docVector)
    {
        documentText = docVector.FileText;
        lowerText = documentText.ToLower();

        var bestWord = MostRelevantWord(queryVector, docVector);
        var bestIndex = GetIndexOf(bestWord);
        return GetTextPieceAround(bestIndex);
    }

    static string MostRelevantWord(DocumentVector queryVector, DocumentVector docVector)
    {
        var queryRelevantWords = new HashSet<string>(queryVector.Words);
        queryRelevantWords.IntersectWith(docVector.Words);
        queryRelevantWords.IntersectWith(Moogle.corpus.Vocabulary); // To filter stopWords 

        string bestWord = "";
        double bestWeight = double.MinValue;
        foreach (var word in queryRelevantWords)
        {
            var tf = docVector.TFs[word];
            var idf = Moogle.corpus.IDFs[word];
            var wordWeight = tf * idf;

            if (wordWeight > bestWeight)
            {
                bestWeight = wordWeight;
                bestWord = word;
            }
        }
        return bestWord;
    }

    public static int GetIndexOf(string word)
    {
        return GetIndexOf(word, 0);

        static int GetIndexOf(string word, int start)
        {
            start = lowerText.IndexOf(word, start);
            if (start == -1) return -1;

            int end = start + word.Length;
            while (start > 0 && char.IsLetterOrDigit(lowerText[start - 1]))
                start--;
            while (end < lowerText.Length && char.IsLetterOrDigit(lowerText[end]))
                end++;
            string fullWord = lowerText.Substring(start, end - start);
            if (fullWord == word)
                return start;
            return GetIndexOf(word, end);
        }
    }

    static string GetTextPieceAround(int index)
    {
        int start = (index - 40 > 0) ? index - 40 : 0;
        int end = (index + 40 < documentText.Length) ? index + 40 : documentText.Length;
        end = SentenceEnding(end);
        start = SentenceBeginning(start);
        var result = documentText.Substring(start, end - start);
        return MyTrim(result);
    }

    static int SentenceEnding(int start)
    {
        var endMarks = new HashSet<char>() { '.', '?', ';', '!' };
        for (int i = start; i < lowerText.Length; i++)
        {
            if (endMarks.Contains(lowerText[i]))
                return i;
        }
        return lowerText.Length;
    }

    static int SentenceBeginning(int start)
    {
        var endMarks = new HashSet<char>() { '.', '?', ';', '!' };
        for (int i = start; i > 0; i--)
        {
            if (endMarks.Contains(lowerText[i]))
                return i + 1;
        }
        return 0;
    }

    static string MyTrim(string text)
    {
        var input = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        string trimmedText = String.Join(' ', input);
        return trimmedText;
    }
    
}
