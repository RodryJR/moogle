namespace MoogleEngine;

public class Document 
{
    
    public string Title{get;private set;}
    public string Text{get;private set;}
    public string[] Words_Text{get;private set;}
    public string Principal_Snippet{get;set;}
    public Snippet[] Snippet{get;set;}
    public float[] TF_IDF{get;private set;}
    public float sqrt_doc_score{get;set;}
    public float Score{get;set;}
    
    public Document(string titulo, string text,string[] wordstext,string principal_snippet,Snippet[] snippet,float[]tfidf, float sqrtdocscore,float score)
    {
        this.Title=titulo;
        this.Text=text;
        this.Words_Text = wordstext;
        this.Principal_Snippet=principal_snippet;
        this.Snippet = snippet;
        this.TF_IDF=tfidf;
        this.sqrt_doc_score=sqrtdocscore;
        this.Score = score;
    }

    public Document CloneDoc()
    {

        return new Document (Title,Text,Words_Text,Principal_Snippet,Snippet,(float[])TF_IDF.Clone(),sqrt_doc_score,Score);

    }

}
public static class Ext
{
    public static Document[] CloneDocs(this Document[] documents)
    {
        var documentsCopy=new Document[documents.Length];

        for(int i = 0; i < documents.Length; i++ )
        {
            documentsCopy[i] = documents[i].CloneDoc();
        }

        return documentsCopy;
    }
}
