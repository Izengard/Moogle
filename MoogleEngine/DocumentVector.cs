namespace MoogleEngine;
public class DocumentVector
{
    string filePath;
    public string FileName { get; private set; }
    Dictionary<string, int> docTermFrequency;
    double[] weights; // Document's terms weight using tf-idf

    public DocumentVector(string filePath)
    {
        this.FileName = Path.GetFileName(filePath);
        this.filePath = filePath;
        this.docTermFrequency = new Dictionary<string, int>();
        // char[] delim = { ' ', '.', ',', '?', '!', ':', ';', '\'', '/', '\"', '\n', '(', ')', '<', '>', '-', '_', '*', '#', '\r' };
        var delim = new char[] { ' ' };
        string document = File.ReadAllText(filePath);
        string[] terms = document.ToLower().Split(delim, StringSplitOptions.RemoveEmptyEntries);

        foreach (string term in terms)
        {
            if (!docTermFrequency.ContainsKey(term))
                docTermFrequency[term] = 0;
            docTermFrequency[term]++;
        }
    }

    // Alternative Constructor for queries
    public DocumentVector(string[] query)
    {
        this.docTermFrequency = new Dictionary<string, int>();

        foreach (string term in query)
        {
            if (!docTermFrequency.ContainsKey(term))
                docTermFrequency[term] = 0;
            docTermFrequency[term]++;
        }

    }

    public int Count { get { return docTermFrequency.Count; } }

    public System.Collections.Generic.Dictionary<string, int>.KeyCollection Keys
    {
        get { return docTermFrequency.Keys; }
    }

    public double Magnitude
    {
        get
        {
            double magnitude = 0;
            foreach (var weight in weights)
            {
                if (weight != 0)
                    magnitude += weight * weight;
            }
            return Math.Sqrt(magnitude);
        }
    }
    public double Score { get; set; }

    public void SetWeightsInCorpus(Dictionary<string, double> vocabulary)
    {
        this.weights = new double[vocabulary.Count];
        int i = 0;

        foreach (var term in vocabulary.Keys)
        {
            if (!docTermFrequency.ContainsKey(term))
            {
                weights[i] = 0;
                i++;
            }
            else
            {
                double tf = docTermFrequency[term] / docTermFrequency.Count;
                double idf = vocabulary[term];
                weights[i] = tf * idf;
                i++;
            }
        }
    }


    public void Normalize()
    {
        for (int i = 0; i < weights.Length; i++)
            weights[i] /= Magnitude;
    }

    public double InnerProduct(DocumentVector other)
    {
        double product = 0;

        for (int i = 0; i < other.weights.Length; i++)
        {
            if (this.weights[i] != 0 && other.weights[i] != 0)
                product += this.weights[i] * other.weights[i];
        }

        return product;
    }

    public static double operator *(DocumentVector v1, DocumentVector v2)
    {
        return v1.InnerProduct(v2);
    }


    public static double Similarity(DocumentVector v1, DocumentVector v2)
    {
        return v1 * v2;
    }
}
