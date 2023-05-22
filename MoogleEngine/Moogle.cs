using System.Diagnostics;
using System;
using System.Collections.Generic;
using MoogleEngine;
namespace MoogleEngine;
public static class Moogle
{
    public static string contentPath = Path.Combine("..", "Content");

    public static Corpus corpus;
     
    public static SearchResult Query(string query)
    {
        System.Console.WriteLine("Searching...");
        var timer = new Stopwatch(); timer.Start();
        
        DocumentVector queryVector = new DocumentVector(query); 
        queryVector.SetWeightsInCorpus(corpus.Vocabulary, corpus.IDFs);
        queryVector.Normalize();
        
        if(SearchOperators.QueryContainsOperators(query)){
            SearchOperators.SetMarkers(query);
            corpus.RankDocumentsWithOperators(queryVector);
        }
        else
            corpus.RankDocumentsByQuery(queryVector);
        
        var scoreBoard = corpus.Ranking;
        if (scoreBoard.Length == 0)
        {
            SearchItem[] result = new SearchItem[1] {new SearchItem("No results found","We are sorry, try again",0)};
            return new SearchResult(result);
        }

        SearchItem[] items = new SearchItem[scoreBoard.Length];
        
        for (var i = 0; i < items.Length; i++)
        {
            var docVector = scoreBoard[i];
            
            var title = docVector.FileName;
            var score = docVector.Score;
            var snippet = Snippet.GetSnippet(queryVector,docVector);
            
            items[i] = new SearchItem(title, snippet, score);
        }
        
        timer.Stop(); 
        var time = timer.ElapsedMilliseconds/1000;
        System.Console.WriteLine("Search completed in {0} seconds", time);
        PrintScores(items);
        return new SearchResult(items);
    }

        
    public static void SetCorpus()
    {
        corpus = new Corpus(contentPath);
    }

    private static void PrintScores(SearchItem[] items)
    {
        foreach (var item in items)
            System.Console.WriteLine($"{item.Title} : {item.Score*100}");
    }
}
