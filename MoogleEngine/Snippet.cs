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


    // In DEVELOPMENT

    // static string MostRelevantPhrase(DocumentVector docVector, DocumentVector query)
    // {
    //     char[] delim = { '.', ';', '!', '?' };
    //     string[] sentences = docVector.FileText.Split(delim, StringSplitOptions.RemoveEmptyEntries);
    //     var relevantSentences = new List<DocumentVector>();
    //     Dictionary<string, double> idfs = Moogle.corpus.IDFs;
    //     var snippet = "";

    //     for (var i = 0; i < sentences.Length; i++)
    //     {
    //         var sentence = sentences[i];

    //         if (!IsRelevant(sentence, query))
    //             continue;

    //         var sentenceVector = new DocumentVector(sentence);
    //         relevantSentences.Add(sentenceVector);
    //     }

    //     for (var i = 0; i < relevantSentences.Count; i++)
    //     {
    //         var relevantSentence = relevantSentences[i];
    //         foreach (var word in commonWords)
    //         {
    //             var tf = relevantSentence.TFs[word];
    //             var idf = idfs[word];
    //             relevantSentence.Score += tf * idf;
    //         }
    //     }
    //     var bestMatchingIndexes = BestTripleIndex(relevantSentences.ToArray());
    //     foreach (var i in bestMatchingIndexes)
    //     {
    //         snippet += relevantSentences[i].FileText;
    //     }
    //     return MyTrim(snippet);
    // }
    // private static bool IsRelevant(string sentence, DocumentVector query)
    // {
    //     var sentenceWords = DocumentVector.Tokenize(sentence);
    //     var intersection = commonWords.Intersect(sentenceWords).ToHashSet();
    //     if (intersection.Count == 0)
    //         return false;
    //     return true;
    // }

    // static int[] BestTripleIndex(DocumentVector[] rankedSentences)
    // {
    //     var scores = new double[rankedSentences.Length];
    //     for (int i = 0; i < rankedSentences.Length; i++)
    //     {
    //         scores[i] = rankedSentences[i].Score;
    //     }

    //     double bestSum = 0;
    //     int bestIndex = 0;
    //     for (int i = 1; i < scores.Length - 1; i++)
    //     {
    //         double sum = scores[i - 1] + scores[i] + scores[i + 1];
    //         if (sum > bestSum)
    //         {
    //             bestSum = sum;
    //             bestIndex = i;
    //         }
    //     }
    //     var result = new int[] { bestIndex - 1, bestIndex, bestIndex + 1 };
    //     return result;
    // }
    