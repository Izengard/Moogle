using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoogleEngine;
public static class Snippet
{
    static string documentText;
    static string lowerText;
    static string lowerQuery;

    public static string GetSnippet(DocumentVector queryVector, DocumentVector docVector, 
                                                                HashSet<string> vocabulary)
    {

        lowerQuery = queryVector.FileText.ToLower();
        documentText = docVector.FileText;
        lowerText = documentText.ToLower();

        // Case 1: the document contains the query literally
        var position = lowerText.IndexOf(lowerQuery);
        if (position != -1)
            return GetTextPieceAround(position);


        // Case 2: The text does not contains the query literally

        // 2.1) Determine query relevant words towards the document
        var queryRelevantWords = new HashSet<string>(queryVector.DocWords);
        queryRelevantWords.IntersectWith(docVector.DocWords);
        queryRelevantWords.IntersectWith(vocabulary);
        var allQueryIndexes = new HashSet<int>();

        // 2.2) Determine all the indexes at which query relevant words appears in the text
        foreach (var word in queryRelevantWords)
        {
                int index = 0;
                while (true)
                {
                    index = lowerText.IndexOf(word, index);
                    if (index == -1)
                        break;
                    allQueryIndexes.Add(index);
                    index++;
                }
        }

        // 2.3) Search best matching piece of document text considering the best piece  
        // the one containing more relevant words of the query 

        int bestMatchCount = 0, bestMatchIndex = 0;

        foreach (var index in allQueryIndexes)
        {
            int matchCount = 0;
            foreach (var otherIndex in allQueryIndexes)
                if (Math.Abs(otherIndex - index) <= 50)
                    matchCount++;

            if (matchCount > bestMatchCount)
            {
                bestMatchCount = matchCount;
                bestMatchIndex = index;
            }
        }
        var snippet = GetTextPieceAround(bestMatchIndex);

        return snippet;
    }


    static string GetTextPieceAround(int index)
    {
        int start = (index - 40 > 0) ? index - 40 : 0;
        int end = (index + 40 < documentText.Length) ? index + 40 : documentText.Length;
        string documentTextPiece = lowerText.Substring(start, end - start);

        return ExtractFullSentence(documentTextPiece);
    }

    static string ExtractFullSentence(string str)
    {
        int start = lowerText.IndexOf(str);
        int end = documentText.IndexOf(".", start + 40);
        end = (end != -1) ? end : documentText.Length;
        var lastDot = documentText.LastIndexOf(".", start);
        start = (lastDot > 0) ? lastDot + 1 : 0;
        var result = documentText.Substring(start, end - start);
        return MyTrim(result);
    }

    static string MyTrim(string text)
    {
        var input = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        string trimmedText = String.Join(' ', input);
        return trimmedText;
    }
}