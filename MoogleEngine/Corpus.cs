using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoogleEngine;

public class Corpus
{
    string[] documents; 
    Dictionary<string, double> idfs;
    public Dictionary<string, double> IDFs { get{return idfs;}}
    DocumentVector[] vectorList; 
    HashSet<string> vocabulary;
    public HashSet<string> Vocabulary { get{ return vocabulary;}}
    
    
    public Corpus(string contentPath)
    {
        System.Console.WriteLine("Setting Corpus");
        var timer = new Stopwatch(); timer.Start();
        
        this.documents = Directory.GetFiles(contentPath, "*.txt");
        this.idfs = new Dictionary<string, double>();
        this.vectorList = new DocumentVector[documents.Length];
        this.vocabulary = new HashSet<string>();

        for (int i = 0; i < documents.Length; i++)
        {
            var document = documents[i];
            var docText = File.ReadAllText(document);
            DocumentVector docVector = new DocumentVector(docText);
            docVector.FileName = Path.GetFileName(document);
            

            // Add all words in the current document to the vocabulary  
            // and count how many documents contains each word
            foreach (var word in docVector.DocWords)
            {
                if (!this.vocabulary.Contains(word))
                {
                    this.vocabulary.Add(word);
                    this.idfs[word] = 0;
                }
                idfs[word]++;
            }

            vectorList[i] = docVector;
        }

        // Calculate IDF and assign it as the value of each term in idfs
        var stopWordThreshold = Math.Log(100/95);
        foreach (var term in this.vocabulary)
        {
            var idf = Math.Log(documents.Length / idfs[term]);
            idfs[term] = (idf > stopWordThreshold) ? idf : 0;
        }

        // Set vectors' weights
        for (int i = 0; i < vectorList.Length; i++)
        {
            vectorList[i].SetWeightsInCorpus(vocabulary, idfs);
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
            // Apply Search Operators
            Array.Sort(this.vectorList, (doc1, doc2) => doc2.Score.CompareTo(doc1.Score));
            DocumentVector[] searchRanking = vectorList.TakeWhile(doc => doc.Score > 0.001).ToArray();
            return searchRanking;
        }
    }

   
    
}
