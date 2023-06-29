using System.Diagnostics;
using System;
using System.Collections.Generic;
using MoogleEngine;

namespace MoogleEngine;

public static class Moogle
{
    public static Corpus corpus;

    /// <summary>Main method. This method is called when the user presss the search button in the web 
    /// app. It process the query, score the documents vector by the Cosine Similarity to the query,
    /// creates and returns a the search result </summary>
    /// <param name="query">String. Query typed by the user</param>
    public static SearchResult Query(string query)
    {
        
        System.Console.WriteLine("Searching...");
        var timer = new Stopwatch(); timer.Start();

        //var suggestion = SearchResult.SearchSuggestion(query);
        DocumentVector queryVector = new DocumentVector(query);
        queryVector.SetWeightsInCorpus(corpus.Vocabulary, corpus.IDFs);
        queryVector.Normalize();
        corpus.RankDocumentsBySimilarity(queryVector);

        if (SearchOperators.QueryContainsOperators(query))
        {
            SearchOperators.SetMarkers(query);
            corpus.RankDocumentsWithOperators();
        }

        var scoreBoard = corpus.Ranking;
        if (scoreBoard.Length == 0)
        {
            SearchItem[] result = new SearchItem[1] { new SearchItem("No results found", "We are sorry, try again", 0) };
            return new SearchResult(result);
        }

        SearchItem[] items = new SearchItem[scoreBoard.Length];

        for (var i = 0; i < items.Length; i++)
        {
            var docVector = scoreBoard[i];

            var title = docVector.FileName;
            var score = docVector.Score;
            var snippet = Snippet.GetSnippet(queryVector, docVector);

            items[i] = new SearchItem(title, snippet, score);
        }

        timer.Stop();
        var time = timer.ElapsedMilliseconds / 1000;
        System.Console.WriteLine("Search completed in {0} seconds", time);
        var searchResult = new SearchResult(items);
        // if (suggestion != query)
        //     searchResult.Suggestion = suggestion;
        return searchResult;
    }

    /// <summary>Instantiate the Corpus class creating a corpus from the collection of documents</summary>
    public static void SetCorpus()
    {
        corpus = new Corpus(Path.Combine("..", "Content"));
    }

    
}
