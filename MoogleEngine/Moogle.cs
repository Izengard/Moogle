using System.Diagnostics;
using System;
using System.Collections.Generic;
namespace MoogleEngine;
public static class Moogle
{
    static Corpus corpus  = new Corpus(Path.Combine("..", "Content"));
     
    public static SearchResult Query(string query)
    {
        System.Console.WriteLine("Processing Query");
        var timer = new Stopwatch(); timer.Start();
        
        QueryClass search = new QueryClass(query); 
        DocumentVector queryVector = new DocumentVector(search.QueryTerms); 
        queryVector.SetWeightsInCorpus(corpus.Words, corpus.Vocabulary);
        queryVector.Normalize();
        
        System.Console.WriteLine("Query Vector Set");
        
        corpus.RankDocuments(queryVector);
        
        timer.Stop(); var time = timer.ElapsedMilliseconds/1000;
        System.Console.WriteLine("Query processed in {0} seconds", time);
        
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
            var text = scoreBoard[i].FilePath;
            text = File.ReadAllText(text);
            var snippet =  new Snippet(text, query);
            
            items[i] = new SearchItem(title, snippet, score);
        }
        // SearchItem[] items = new SearchItem[3] {
        //     new SearchItem("Hello World", "Lorem ipsum dolor sit amet", 0.9f),
        //     new SearchItem("Hello World", "Lorem ipsum dolor sit amet", 0.5f),
        //     new SearchItem("Hello World", "Lorem ipsum dolor sit amet", 0.1f),
        // };
        return new SearchResult(items);
    }

        
}
