using System.Collections.Generic;
namespace MoogleEngine;

public class DocumentVector
    {
        public string FileName { get; set; }
        public string FileText { get; private set; }
        public string[] Terms { get;}
        HashSet<string> words;
        public HashSet<string> Words { get { return this.words; } }
        Dictionary<string, double> docTermFrequency;
        public Dictionary<string, double> TFs { get { return docTermFrequency; } }
        double[] weights;
        public double Score { get; set; }
        double magnitude;
        HashSet<int> nonZeroIndexes;


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
            char[] delim = {' ', '\n', '.', '!', '?'};
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