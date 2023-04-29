namespace MoogleEngine;


public static class Moogle
{
    public static SearchResult Query(string query)
    {
        string contentPath = Path.Combine("..", "Content");
        Corpus corpus = new Corpus(contentPath);

        string[] queryTerms = query.ToLower().Split();
        DocumentVector queryVector = new DocumentVector(queryTerms);

        var list = corpus.SortByScore();

        var items = new SearchItem[list.Count];
        for (int i = 0; i < list.Count; i++)
        {
            items[i] = new SearchItem( list[i].FileName, "", 0);
        }







        // SearchItem[] items = new SearchItem[3] {
        //     new SearchItem("Hello World", "Lorem ipsum dolor sit amet", 0.9f),
        //     new SearchItem("Hello World", "Lorem ipsum dolor sit amet", 0.5f),
        //     new SearchItem("Hello World", "Lorem ipsum dolor sit amet", 0.1f),
        // };


        return new SearchResult(items, query);
    }
}
