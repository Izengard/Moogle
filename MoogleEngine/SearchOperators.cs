using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MoogleEngine;
namespace MoogleEngine;

public static class SearchOperators
{
    static HashSet<char> operators = new HashSet<char>("~!^*");
    static HashSet<(string word, int count)> importanceMarkers;
    static HashSet<string> existenceMarkers;
    static HashSet<string> nonExistenceMarkers;
    static HashSet<(string, string)> distanceMarkers;
    public static bool[] operationsSwitch;
    // Position:    Operation:
    // 0            NonExistence
    // 1            Existence
    // 2            Importance
    // 3            Distance
   

    /// <summary>Determina si la qery contiene operadores de busqueda. Crea un conjunto con los caracteres de la query
    ///y lo interseca con el conjunto de los caracteres de los operadores </summary>
    /// <param name="query">Consulta introducida por el usuario </param>
    public static bool QueryContainsOperators(string query)
    {
        var queryChars = new HashSet<char>(query);
        queryChars.IntersectWith(operators);
        if (queryChars.Count == 0)
            return false;
        return true;
    }

    /// <summary>Determina las palabras operandos, las que se denominaron markers
    /// crea un array de las palabras de la query y analiza el primer caraceter de cada 
    /// una de ellas usando Switch</summary>
    /// <param name="query">Consulta introducida por el usuario </param>
    public static void SetMarkers(string query)
    {
        var queryTerms = query.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
        importanceMarkers = new HashSet<(string word, int count)> ();
        existenceMarkers = new HashSet<string>();   
        nonExistenceMarkers = new HashSet<string>();
        distanceMarkers = new HashSet<(string, string)>();

        for (var i = 0; i < queryTerms.Length; i++)
        {
            var term = queryTerms[i];
            var firstChar = term[0];
            switch (firstChar)
            {
                case '!':
                    nonExistenceMarkers = new HashSet<string>();
                    nonExistenceMarkers.Add(TakeWord(term));
                    operationsSwitch[0] = true;
                    break;
                case '^':
                    existenceMarkers = new HashSet<string>();
                    existenceMarkers.Add(TakeWord(term));
                    operationsSwitch[1] = true;
                    break;
                case '*':
                    importanceMarkers = new HashSet<(string word, int count)>();
                    var word = TakeWord(term);
                    var count = 1;
                    for (var j = 1; j < term.Length; j++)
                    {
                        if (term[j] != '*') break;
                        count++;
                    }
                    (string, int) pair = (word, count);
                    importanceMarkers.Add(pair);
                    operationsSwitch[2] = true;

                    break;
                case '~':
                    distanceMarkers = new HashSet<(string, string)>();
                    var marker1 = TakeWord(queryTerms[i - 1]);
                    var marker2 = TakeWord(term);
                    (string, string) marker = (marker1, marker2);
                    distanceMarkers.Add(marker);
                    operationsSwitch[3] = true;
                    break;  

                default:
                    continue;
            }
        }
    }
    // Depura la palabra de otros caracteres no alfanumericos como los operadores
    private static string TakeWord(string input)
    {
        string word = new string(input.SkipWhile(c => !char.IsLetterOrDigit(c)).TakeWhile(c => char.IsLetterOrDigit(c)).ToArray());
        return word;
    }

    /// <summary>Calcula el scoreModifier aplicando los operadores de busqueda que se determinaron en SetMarkers </summary>
    /// <param name="doc">Vector del documento a operar</param>
    public static float ApplySearchOperators(DocumentVector doc)
    {
        var scoreModifier = 0.0f;
        if (operationsSwitch[0])
            scoreModifier += ApplyNonExistenceOp(doc);
        if (operationsSwitch[1])
            scoreModifier += ApplyExistenceOp(doc);
        if (operationsSwitch[2])
            scoreModifier += ApplyImportanceOp(doc);
        if (operationsSwitch[3]) 
            scoreModifier += ApplyDistanceOp(doc);
        return 1.0f + scoreModifier;
    }

    private static float ApplyNonExistenceOp(DocumentVector doc)
    {
        var scoreModifier = 0.0f;
        foreach (var marker in nonExistenceMarkers)
        {
            if (doc.Words.Contains(marker))
                scoreModifier -= 1.0f;
        }
        return scoreModifier;
    }

    private static float ApplyExistenceOp(DocumentVector doc)
    {
        var scoreModifier = 0.0f;
        foreach (var marker in existenceMarkers)
        {
            if (!doc.Words.Contains(marker))
                scoreModifier -= 1.0f;
        }
        return scoreModifier;
    }

    private static float ApplyImportanceOp(DocumentVector doc)
    {
        var scoreModifier = 0.0f;

        foreach (var marker in importanceMarkers)
        {
            if (doc.Words.Contains(marker.word))
                scoreModifier += 0.35f * marker.count;
        }
        return scoreModifier;
    }

    private static float ApplyDistanceOp(DocumentVector doc)
    {
        float scoreModifier = 0.0f;
        foreach (var pair in distanceMarkers)
        {
            if (!doc.Words.Contains(pair.Item1) || !doc.Words.Contains(pair.Item1) || pair.Item1.Equals(pair.Item2))
            {
                scoreModifier += 0.0f;
                break;
            }
            string[] docWords = doc.Terms;
            int distance = ShortestDistance(docWords, pair);
            scoreModifier += 1.0f - (float)distance / docWords.Length;
        }
        return scoreModifier;

    }
    /// <summary>Determina la menor distancia entre dos palabras dadas en un documento</summary>
    /// <param name="docWords">Array de las palabras del documento</param>
    /// <param name="pair">Tupla que contiene las dos palabras entre las cuales se determina la distancia</param>
    private static int ShortestDistance(string[] docWords, (string, string) pair)
    {
        string word1 = pair.Item1;
        string word2 = pair.Item2;
        int i1 = -1, i2 = -1;
        int shortest = int.MaxValue;
        int distance = 0;

        for (var i = 0; i < docWords.Length; i++)
        {
            if (docWords[i].Equals(word1))
                i1 = i;
            if (docWords[i].Equals(word2))
                i2 = i;
            if (i1 != -1 && i2 != -1)
            {
                distance = Math.Abs(i1 - i2);
                shortest = Math.Min(shortest, distance);
            }
        }

        return shortest;
    }

}
