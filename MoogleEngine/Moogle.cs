using System.Diagnostics;
using System;
using System.Collections.Generic;
using MoogleEngine.SearchOperators;
namespace MoogleEngine;
public static class Moogle
{
    static Corpus corpus;
     
    public static SearchResult Query(string query)
    {
        System.Console.WriteLine("Searching...");
        var timer = new Stopwatch(); timer.Start();
        
        DocumentVector queryVector = new DocumentVector(query); 
        queryVector.SetWeightsInCorpus(corpus.Vocabulary, corpus.IDFs);
        queryVector.Normalize();
        
        System.Console.WriteLine("Query Vector Set");
        
        corpus.RankDocuments(queryVector);
        
        
        var scoreBoard = corpus.Ranking;
        if (scoreBoard.Length == 0)
        {
            SearchItem[] result = new SearchItem[1] {
            new SearchItem("No results found","We are sorry, try again",0)
            };
            return new SearchResult(result);
        }

        SearchItem[] items = new SearchItem[scoreBoard.Length];
        
        for (var i = 0; i < items.Length; i++)
        {
            var title = scoreBoard[i].FileName;
            var score = scoreBoard[i].Score;
            
            // Snippet
            var docVector = scoreBoard[i];
            var snippet = Snippet.GetSnippet(queryVector,docVector, corpus.IDFs);
            
            items[i] = new SearchItem(title, snippet, score);
        }
        
        return new SearchResult(items);
        timer.Stop(); var time = timer.ElapsedMilliseconds/1000;
        System.Console.WriteLine("Search completed in {0} seconds", time);
    }

        
    public static void SetCorpus()
    {
        corpus = new Corpus(Path.Combine("..", "Content"));
    }
}
