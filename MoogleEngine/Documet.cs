namespace MoogleEngine;

public class Document 
{
    
    public string Title{get;private set;}
    public string Text{get;private set;}
    public string[] Words_Text{get;private set;}
    public string Snippet{get;set;}
    public float[] TF_IDF{get;private set;}
    public float Score{get;set;}
                                                                                             
    public Document(string titulo, string text,string[] wordstext,string snippet,float[]tfidf,float score)
    {
        this.Title=titulo;
        this.Text=text;
        this.Words_Text = wordstext;
        this.Snippet=snippet;
        this.TF_IDF=tfidf;
        this.Score = score;
    }
}
