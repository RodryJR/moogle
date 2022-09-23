namespace MoogleEngine;

public class Snippet
{
    public string Text_Snippet{get;private set;}
    public string[] Words_Snippet{get;private set;}
    public Snippet(string text,string[] word_snippet)
    {
        this.Text_Snippet=text;
        this.Words_Snippet=word_snippet;
    }

}
