using System.Collections.Generic;
namespace MoogleEngine;
public class DocumentVector
    {
        public string FilePath{get; private set; }
        public string FileName { get; private set; }
        HashSet<string> docWords;
        public HashSet<string> DocWords { get { return this.docWords; } }
        Dictionary<string, double> docTermFrequency;
        double[] weights;
        public double Score { get; set; }
        double magnitude;
        HashSet<int> nonZeroIndexes;

        public DocumentVector(string filePath)
        {
            this.FileName = Path.GetFileName(filePath);
            this.FilePath = filePath;
            string document = File.ReadAllText(filePath);
            string[] terms = Tokenize(document);
            var length = terms.Length;
            this.docWords = new HashSet<string>(terms);
            this.docTermFrequency = docWords.ToDictionary(term => term, term => 0.0);
            this.magnitude = 0.0;

            for (var i = 0; i < length; i++)
            {
                var term = terms[i];
                docTermFrequency[term] += (double)1/length;
            }
        }

        // Alternative Constructor for queries
        public DocumentVector(string[] queryTerms)
        {
            var length = queryTerms.Length;
            this.docWords = new HashSet<string>(queryTerms);
            this.docTermFrequency = docWords.ToDictionary(term => term, term => 0.0);
            this.magnitude = 0.0; 
            for (int i = 0; i < length; i++)
            {
                string term = queryTerms[i];
                docTermFrequency[term]++;
            }
        }

        public void SetWeightsInCorpus(HashSet<string> corpusWords, Dictionary<string, double> vocabulary)
        {
            this.weights = new double[corpusWords.Count];
            this.nonZeroIndexes = new HashSet<int>();

            int i = 0;
            foreach (var term in corpusWords)
            {
                if (!this.docWords.Contains(term))
                {
                    weights[i] = 0.0;
                    i++;
                }
                else
                {
                    double tf = docTermFrequency[term];
                    double idf = vocabulary[term];
                    weights[i] = tf * idf;
                    magnitude += weights[i] * weights[i];
                    nonZeroIndexes.Add(i);
                    i++;
                }
            }
        }

        public void Normalize()
        {
            magnitude = Math.Sqrt(magnitude);
            foreach (var index in nonZeroIndexes)
            {
                weights[index] /= magnitude;
            }
        }

        public double InnerProduct(DocumentVector other)
        {
            double product = 0.0;
            var indexes = new HashSet<int>(this.nonZeroIndexes);
            indexes.IntersectWith(other.nonZeroIndexes);
            
            if(indexes.Count == 0) return 0.0;

            foreach (var index in indexes)
                product += this.weights[index] * other.weights[index];

            return product;
        }

        public static double Similarity(DocumentVector v1, DocumentVector v2)
        {
            return v1.InnerProduct(v2);
        }

        public static string[] Tokenize(string text)
        {
            string[] tokens = text.ToLower().Split(" ", StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < tokens.Length; i++)
            {
                string input = tokens[i];
                string word = new string(input.SkipWhile(c => !char.IsLetterOrDigit(c)).TakeWhile(c => char.IsLetterOrDigit(c)).ToArray());
                tokens[i] = word;
            }
            return tokens;
        }
}