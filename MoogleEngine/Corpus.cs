using System.IO;
using System.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MoogleEngine;

namespace MoogleEngine;

public class Corpus
{
    string[] documents;
    Dictionary<string, double> idfs;
    public Dictionary<string, double> IDFs { get { return idfs; } }
    DocumentVector[] vectorList;
    public DocumentVector[] VectorList { get { return vectorList; } }
    HashSet<string> vocabulary;
    public HashSet<string> Vocabulary { get { return vocabulary; } }
    public HashSet<string> stopWords { get;}


    #region Constructor
    // Class constructor. Load and process all the .txt files in the given path
    public Corpus(string contentPath)
    {
        System.Console.WriteLine("Setting Corpus");
        var timer = new Stopwatch(); timer.Start();

        this.documents = Directory.GetFiles(contentPath, "*.txt");
        this.idfs = new Dictionary<string, double>();
        this.vectorList = new DocumentVector[documents.Length];
        this.vocabulary = new HashSet<string>();
        this.stopWords = new HashSet<string>();
        

        for (int i = 0; i < documents.Length; i++)
        {
            var document = documents[i];
            var docText = File.ReadAllText(document);
            DocumentVector docVector = new DocumentVector(docText);
            docVector.FileName = Path.GetFileName(document);


            // Add all words in the current document to the vocabulary  
            // and count how many documents contains each word
            foreach (var word in docVector.Words)
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

        // Calculate IDFs and remove stop words
        var stopWordThreshold = Math.Log((double)100 / 85);
        foreach (var term in this.vocabulary)
        {
            var idf = Math.Log(documents.Length / idfs[term]);
            if (idf > stopWordThreshold)
                idfs[term] = idf;
            else
            {
                stopWords.Add(term);
                idfs.Remove(term);
                vocabulary.Remove(term);
            }
        }

        // Set vectors' weights
        for (int i = 0; i < vectorList.Length; i++)
        {
            vectorList[i].SetWeightsInCorpus(vocabulary, idfs);
            vectorList[i].Normalize();
        }

        timer.Stop();
        var time = timer.ElapsedMilliseconds / 1000;
        System.Console.WriteLine("Corpus  has been loaded in {0} seconds", time);
    }
    #endregion


    #region Ranking Methods
    
    /// <summary>Asign a value to all DocumentVectors's scores given by cosine similarity
    ///between the docuemnt and the query</summary>
    /// <param name="query">Query typed by the user</param>
    public void RankDocumentsBySimilarity(DocumentVector query)
    {
        for (var i = 0; i < this.vectorList.Length; i++)
            vectorList[i].Score = DocumentVector.Similarity(vectorList[i], query);
    }

    /// <summary>Modifies the documents' scores by factor given by the search operators</summary>
    public void RankDocumentsWithOperators()
    {
        for (var i = 0; i < this.vectorList.Length; i++)
        {
            var doc = vectorList[i];
            var scoreModifier = SearchOperators.ApplySearchOperators(doc);
            doc.Score *= scoreModifier;
        }
    }

    // Returns an arry of Document Vectors sorted by score
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
#endregion