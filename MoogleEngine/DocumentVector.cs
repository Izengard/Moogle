using System.Collections.Generic;
namespace MoogleEngine;


public class DocumentVector
    {
        public string FileName { get; set; }
        public string FileText { get; private set; }
        HashSet<string> docWords;
        public HashSet<string> DocWords { get { return this.docWords; } }
        Dictionary<string, double> docTermFrequency;
        double[] weights;
        public double Score { get; set; }
        double magnitude;
        HashSet<int> nonZeroIndexes;


        public DocumentVector(string docText)
        {
            this.FileText = docText;
            string[] terms = Tokenize(docText);
            var length = terms.Length;
            this.docWords = new HashSet<string>(terms);
            this.docTermFrequency = docWords.ToDictionary(term => term, term => 0.0);

            for (var i = 0; i < length; i++)
            {
                var term = terms[i];
                docTermFrequency[term] += (double)1/length;
            }
        }

        public void SetWeightsInCorpus(HashSet<string> corpusWords, Dictionary<string, double> idfs)
        {
            this.weights = new double[corpusWords.Count];
            this.nonZeroIndexes = new HashSet<int>();
            this.magnitude = 0.0;

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
                    double idf = idfs[term];
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

        public static double Similarity(DocumentVector v, DocumentVector w)
        {
            double product = 0.0;
            var indexes = new HashSet<int>(v.nonZeroIndexes);
            indexes.IntersectWith(w.nonZeroIndexes);
            
            if(indexes.Count == 0) return 0.0;

            foreach (var index in indexes)
                product += v.weights[index] * w.weights[index];
            return product;
        }

        public static string[] Tokenize(string text)
        {
            string[] tokens = text.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < tokens.Length; i++)
            {
                string input = tokens[i];
                string word = new string(input.SkipWhile(c => !char.IsLetterOrDigit(c)).TakeWhile(c => char.IsLetterOrDigit(c)).ToArray());
                tokens[i] = word;
            }
            return tokens;
        }
}