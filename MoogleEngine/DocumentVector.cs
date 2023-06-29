using System.Collections.Generic;
namespace MoogleEngine;

public class DocumentVector
{
    public string FileName { get; set; }
    public string FileText { get; private set; }
    public string[] Terms { get; set; }
    HashSet<string> words;
    public HashSet<string> Words { get { return this.words; } }
    Dictionary<string, double> docTermFrequency;
    public Dictionary<string, double> TFs { get { return docTermFrequency; } }
    double[] weights;
    public double Score { get; set; }
    double magnitude;
    HashSet<int> nonZeroIndexes;


    /// <summary>Class constructor. Represents a document text in a the form of a vector of its terms
    // and calculates the frequency of each term</summary>
    /// <param name="docText">string, text of the document</param>
    public DocumentVector(string docText)
    {
        this.FileText = docText;
        this.Terms = Tokenize(docText);
        var length = Terms.Length;
        this.words = new HashSet<string>(Terms);
        this.docTermFrequency = words.ToDictionary(term => term, term => 0.0);

        for (var i = 0; i < length; i++)
        {
            var term = Terms[i];
            docTermFrequency[term] += (double)1 / length;
        }
    }

    /// <summary>Initialize and fill the weights array calculating the weights of each term in the document
    /// according to TF-IDF factor </summary>
    /// <param name="corpusWord">Set of the all the words of the document collection</param>
    /// <param name="idfs">Dictionary containing th idf of each term</param>
    public void SetWeightsInCorpus(HashSet<string> corpusWords, Dictionary<string, double> idfs)
    {
        this.weights = new double[corpusWords.Count];
        // Remember non zero positions to optimize other processes
        this.nonZeroIndexes = new HashSet<int>();
        this.magnitude = 0.0;

        int i = 0;
        foreach (var term in corpusWords)
        {
            if (!this.words.Contains(term))
            {
                weights[i] = 0.0;
                i++;
            }
            else
            {
                double tf = this.docTermFrequency[term];
                double idf = idfs[term];
                weights[i] = tf * idf;
                // Remember the square of the weight to calculate the vector magnitude ahead
                magnitude += weights[i] * weights[i];
                nonZeroIndexes.Add(i);
                i++;
            }
        }
    }

    /// <summary>Normalize the weights vector dividing by its magnitude </summary>
    public void Normalize()
    {
        magnitude = Math.Sqrt(magnitude);
        foreach (var index in nonZeroIndexes)
        {
            weights[index] /= magnitude;
        }
    }

    /// <summary>Calculate the Cosine Similarity between two vectors</summary>
    /// <param name="v">Arbitrary Documents Vector</param>
    /// <param name="w">Arbitrary Documents Vector</param>
    public static double Similarity(DocumentVector v, DocumentVector w)
    {
        double product = 0.0;

        // Optimization : Go to the positions where both weights are different to zero
        var indexes = new HashSet<int>(v.nonZeroIndexes);
        indexes.IntersectWith(w.nonZeroIndexes);
        // If there isn't any return 0
        if (indexes.Count == 0) return 0.0;

        foreach (var index in indexes)
            product += v.weights[index] * w.weights[index];
        return product;
    }


    /// <summary>Split a text in its words</summary>
    /// <param name="text">String text</param>
    /// <returns> Return an array of words</returns>
    public static string[] Tokenize(string text)
    {
        char[] delim = { ' ', '\n', '.', '!', '?' };
        string[] tokens = text.ToLower().Split(delim, StringSplitOptions.RemoveEmptyEntries);

        for (int i = 0; i < tokens.Length; i++)
        {
            string input = tokens[i];
            string word = new string(input.SkipWhile(c => !char.IsLetterOrDigit(c)).TakeWhile(c => char.IsLetterOrDigit(c)).ToArray());
            tokens[i] = word;
        }
        return tokens;
    }
}