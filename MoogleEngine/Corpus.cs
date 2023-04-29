using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoogleEngine;

public class Corpus
{
    string[] documents; // List all documents in Content
    Dictionary<string, double> vocabulary; // Contains all terms in Corpus adn its idf
    List<DocumentVector> termsMatrix; // List  of document vectors
    List<DocumentVector> searchRanking;

    public Corpus(string ContentPath)
    {
        this.documents = Directory.GetFiles(ContentPath, "*.txt");
        this.vocabulary = new Dictionary<string, double>();
        this.termsMatrix = new List<DocumentVector>();

        foreach (var document in documents)
        {

            DocumentVector docVector = new DocumentVector(document);

            // Add all terms in the Corpus to the vocabulary and determines how many documents contains each term
            foreach (var term in docVector.Keys)
            {
                if (!vocabulary.ContainsKey(term))
                    vocabulary[term] = 0;
                vocabulary[term]++;
            }

            termsMatrix.Add(docVector);
        }

        // Calculate IDF and assign it as the value of each terms in vocabulary
        foreach (var term in vocabulary.Keys)
            vocabulary[term] = Math.Log(documents.Length / vocabulary[term]);

        // Set vectors' weights
        foreach (var vector in termsMatrix)
        {
            vector.SetWeightsInCorpus(vocabulary);
            vector.Normalize();
        }

    }

    // Ranking Documents by its scores to a query
    public void RankDocuments(DocumentVector query)
    {
        foreach (var vector in termsMatrix)
            vector.Score = DocumentVector.Similarity(vector, query);
    }
    public List<DocumentVector> SortByScore()
    {
        var searchRanking = this.termsMatrix.ToList();

        Console.WriteLine(this.termsMatrix.Count);

        searchRanking.Sort((item1, item2) => item1.Score.CompareTo(item2.Score));

        return searchRanking.Take(10).ToList();
    }



}
