using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoogleEngine;

public class Corpus
{
    string[] documents; 
    Dictionary<string, double> vocabulary;
    public Dictionary<string, double> Vocabulary { get{return vocabulary;}}
    DocumentVector[] vectorList; 
    HashSet<string> words;
    public HashSet<string> Words { get{ return words;}}
    
    
    public Corpus(string ContentPath)
    {
        System.Console.WriteLine("Setting Corpus");
        var timer = new Stopwatch(); timer.Start();

        this.documents = Directory.GetFiles(ContentPath, "*.txt");
        this.vocabulary = new Dictionary<string, double>();
        this.vectorList = new DocumentVector[documents.Length];
        this.words = new HashSet<string>();

        for (int i = 0; i < documents.Length; i++)
        {
            var document = documents[i];

            DocumentVector docVector = new DocumentVector(document);

            // Add all terms in the current doc to the vocabulary and determines how many documents contains each term
            foreach (var word in docVector.DocWords)
            {
                if (!this.words.Contains(word))
                {
                    this.words.Add(word);
                    vocabulary[word] = 0.0;
                }
                vocabulary[word]++;
            }

            vectorList[i] = docVector;
        }

        // Calculate IDF and assign it as the value of each terms in vocabulary
        foreach (var term in this.words)
        {
            var idf = Math.Log(documents.Length / vocabulary[term]);
            vocabulary[term] = idf;
        }

        // Set vectors' weights
        for (int i = 0; i < vectorList.Length; i++)
        {
            vectorList[i].SetWeightsInCorpus(words, vocabulary);
            vectorList[i].Normalize();
        }

        timer.Stop(); 
        var time = timer.ElapsedMilliseconds/1000;
        System.Console.WriteLine("Corpus  has been loaded in {0} seconds", time);
    }


    // Ranking Documents by its scores towards the query
    public void RankDocuments(DocumentVector query)
    {
        for (var i = 0; i < vectorList.Length; i++)
            vectorList[i].Score = DocumentVector.Similarity(vectorList[i], query);
    }
    
    
    public DocumentVector[] Ranking
    {
        get
        {
            Array.Sort(this.vectorList, (doc1, doc2) => doc2.Score.CompareTo(doc1.Score));
            DocumentVector[] searchRanking = vectorList.TakeWhile(doc => doc.Score > 0.0).ToArray();
            return searchRanking;
        }
    }

   
    
}
