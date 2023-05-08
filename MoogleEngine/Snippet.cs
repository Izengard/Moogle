using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoogleEngine;
public class Snippet
{
    private string query;
    private string queryTerms;
    private string documentText;
    private string lowerText;
    private 
    private Dictionary<string, HashSet<int>> postingList;
    private HashSet<int> 

    public Snippet(string documentText, string query)
    {
        
        this.query = query.ToLower();
        this.documentText = documentText; 
        this.lowerText = documentText.ToLower();

        // Case 1: the document contains the query literally
        var position = lowerText.IndexOf(query);
        if (position != -1)
            return GetTextPieceAround(position);
        
        // Caso 2: El Texto no contiene la query literalmente
        // Extraer la parte del documento que contenga la mayor cantidad de palabras de la query posibles

        var delim = new char[] { ' ', '.', ',', ':', ';', '\'', '\"', '?' };
        this.queryTerms = this.query.Split(delim, StringSplitOptions.RemoveEmptyEntries);

        // Implementing posting list
        this.postingList = new Dictionary<string, HashSet<int>>();
        var allQueryIndexes = new HashSet<int>();
        foreach (var term in queryTerms)
        {
            if (!invertedIndex.ContainsKey(term))
            {
                var wordIndexes = new HashSet<int>();
                int index = 0;

                while (true)
                {
                    index = lowerText.IndexOf(term, index);
                    if (index == -1)
                        break;
                    wordIndexes.Add(index);
                    allQueryIndexes.Add(index);
                    index++;
                }
                invertedIndex[term] = wordIndexes;
            }
        }

        // Search best matching piece of documentText
        int bestMatchCount = 0, bestMatchIndex = 0;

        foreach (var key in invertedIndex)
        {
            var indexes = key.Value;
            var otherWordsIndexes = new HashSet<int>(allQueryIndexes);
            otherWordsIndexes.ExceptWith(indexes);
            int matchCount = 0;

            foreach (var index in indexes)
            {
                foreach (var otherWordIndex in otherWordsIndexes)
                    if (Math.Abs(otherWordIndex - index) <= 40)
                        matchCount++;

                if (matchCount > bestMatchCount)
                {
                    bestMatchCount = matchCount;
                    bestMatchIndex = index;
                }
            }
        }
        return GetTextPieceAround(documentText, bestMatchIndex);
    }

    static string ExtractFullSentence(int additionalRank = 0)
    {
        int start = documentText.ToLower().IndexOf(query);
        int end = documentText.IndexOf(".", start + additionalRank);
        var lastDot = documentText.LastIndexOf(".", start);
        start = (lastDot > 0) ? lastDot + 1 : 0; //si esta en la primera oracion, desde el inicio del documentTexto hasta el primer punto
        var result = documentText.Substring(start, end - start);
        return MyTrim(result);
    }

    static string GetTextPieceAround(int index)
    {
        int start = (index - 40 > 0) ? index - 40 : 0;
        int end = (index + 40 < documentText.Length) ? index + 40 : documentText.Length;
        string documentTextPiece = documentText.Substring(start, end - start);

        return ExtractFullSentence(documentTextPiece, documentText, 40);
    }

    static string MyTrim(string text)
    {
        var input = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        string trimmedText = String.Join(' ', input);
        return trimmedText;
    }
}