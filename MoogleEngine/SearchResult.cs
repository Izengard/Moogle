using System.Globalization;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Linq;
namespace MoogleEngine;

public class SearchResult
{
    private SearchItem[] items;

    public SearchResult(SearchItem[] items, string suggestion="")
    {
        if (items == null) {
            throw new ArgumentNullException("items");
        }

        this.items = items;
        this.Suggestion = suggestion;
    }

    public SearchResult() : this(new SearchItem[0]) {}

    public string Suggestion { get; set; }

    public IEnumerable<SearchItem> Items() => this.items;

    public int Count { get { return this.items.Length; } }

    public static string SearchSuggestion(string query)
    {
        var terms = DocumentVector.Tokenize(query);
        for (int i = 0; i < terms.Length; i++)
        {
        var term = terms[i];
        if (!Moogle.corpus.Vocabulary.Contains(term) && !Moogle.corpus.stopWords.Contains(term))  
            term = MostSimilarWordInCorpus(term);
        }
        var result = string.Join(" ", terms);
        
        return result;
    }
    private static string MostSimilarWordInCorpus(string term)
    {
        int bestDistance = int.MaxValue;
        string result = "";
        foreach (var word in Moogle.corpus.Vocabulary)
        {
            int distance = EditDistance(term, word, term.Length, word.Length);
            if (distance < bestDistance && distance < 4 )
            {
                bestDistance = distance;
                result = word;
            }
        }

        result = (result != "") ? result : term;
        return result;
    }

    private static int EditDistance(string s1, string s2, int m ,int n)
    {
        int[,] dp = new int[m + 1, n +1];

        for(int i = 0; i <= m; i++)
        {
            for (int j = 0; j <= n; j++)
            {
                if(i == 0)
                    dp[i, j] = j;
                else if(j == 0)
                    dp[i, j] = i;
                else if ( s1[i - 1] == s2[j - 1])
                    dp[i, j] = dp[i - 1, j -1];
                else
                    dp[i , j] = 1 + min(dp[i,j-1], dp[i-1,j], dp[i-1, j-1]);
            }
        }
        return dp[m,n];
    }
    private static int min(int x, int y, int z)
    {
        if ( x <= y && x <= z) return x;
        if (y <= x && y <= z) return y;
        return z;
    }
    
}
