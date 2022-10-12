namespace MoogleEngine;

public static class Moogle
{
    public static float[] TF_IDF_Query{get;private set;}
    public static float Sqrt_Query_score{get;private set;}
    public static bool sugerencia{get; set;}
    public static bool operador_1{get; set;}
    public static bool operador_2{get; set;}
    public static bool operador_3{get;set;}
    public static bool operador_4{get; set;}
    public static string[] words_operador_1{get; set;}
    public static string[] words_operador_2{get;set;}
    public static List <Tuple<string,string>> words_operador_3{get; set;}
    public static Tuple<string,int>[] words_operador_4{get; set;}

    public static SearchResult Query(string query)
    {
        sugerencia=false;
        string[] palabras_query = Words.ArrayWordsWithOperator(query,true);

        //en caso de que la query contenga un operador que lo marque como true y cargue las variables de los operadores
        if(query.Contains('!'))
        {
            operador_1=true;
            Operadores.Operador1(palabras_query);
        }
        if(query.Contains('^'))
        {
            operador_2=true;
            Operadores.Operador2(palabras_query);
        }
        if(query.Contains('~'))
        {
            operador_3=true;
            Operadores.Operador3(query);
        }
        if(query.Contains('*'))
        {
            operador_4=true;
            Operadores.Operador4(palabras_query);
        }

        palabras_query = Words.ArrayWordsWithOperator(query,false);

        if(palabras_query.Length==0)
        {
            return new SearchResult(new SearchItem[0],"");
        } 

        float[] IDF = Preprocesamiento.IDF_vector;
        string[] palabras = Preprocesamiento.words;
        Dictionary <string,int> dic = Preprocesamiento.Diccionario;
        TF_IDF_Query = new float[palabras.Length];
        Sqrt_Query_score=0;
        Document[] documentos = Preprocesamiento.Doc_;

        for(int i=0;i<palabras_query.Length;i++)
        {   //itera x las palabras de la query y si una no existe la sustituye x la mas similar
            if(!palabras.Contains(palabras_query[i]))
            {
                palabras_query[i]=Sugerencia.BuscarPalabra(palabras_query[i]);
            }
            TF_IDF_Query[dic[palabras_query[i]]]+=1;//rellena el vector tf de la query x cada palabra
        }

        string[] palabras_query_sinrep= Words.DeleteDuplicate(palabras_query);//elimina las palabras repetidas de la query

        //calcula el vector tfidf de la query y la sumatoria de los tfIdf al cuadrado del vector de la query
        for(int i=0;i<palabras_query_sinrep.Length;i++)
        {
            TF_IDF_Query[dic[palabras_query_sinrep[i]]]=IDF[dic[palabras_query_sinrep[i]]]*TF_IDF_Query[dic[palabras_query_sinrep[i]]];
        }

        //organiza las palabras de la query sin repetir segun su peso tfidf
        palabras_query_sinrep=Ordenar.Organiza(palabras_query_sinrep,0,palabras_query_sinrep.Length-1);

        bool ModificarSqrt_TFIDF_query=true;//para que nada mas calcule la sumatoria del cuadrado de los tfidf de la query y 
        //q modifique una sola vez el vector de la query en caso del operador *

        //calcula los score de los documentos  
        for(int i=0;i < documentos.Length;i++)
        {   //formula de similitud de coseno
            documentos[i].Score = CalcularScore((float[])documentos[i].TF_IDF.Clone(),ModificarSqrt_TFIDF_query);
            ModificarSqrt_TFIDF_query=false;
        }
        operador_4=false;

        if(operador_3)
        {   //itera x las palabras q hay q hallarle la cercania
            for(int i = 0; i < words_operador_3.Count ;i++)
            {
                int distance;
                //itera x los documentos para hallar la menor distancia entre las palabras afectadas x el operador ~
                for(int j=0;j < documentos.Length;j++)
                {
                    distance=Words.Words_Distance(documentos[j].Words_Text,words_operador_3[i]);

                    if(distance==-1)
                    {
                        continue;
                    }
                    documentos[j].Score += (1/(float)distance);
                }
            }
            operador_3=false;
        }

        if(operador_1)
        {   //itera x las palabras q son modificadas x el operador !
            for(int i=0; i < words_operador_1.Length;i++)
            {   //itera x cada documento y el q contenga la palabra le asigna score 0
                for(int j=0;j<documentos.Length;j++)
                {
                    if( documentos[j].Words_Text.Contains( words_operador_1[i] ) )
                    {    
                        documentos[j].Score=0;
                    }
                }
            }
            operador_1=false;
        }

        if(operador_2)
        {   //itera x las palabras q son modificadas x el operador ^
            for(int i=0; i < words_operador_2.Length;i++)
            {   //itera x cada documento y el q no contenga la palabra le asigna score 0
                for(int j=0;j<documentos.Length;j++)
                {
                    if( !(documentos[j].Words_Text.Contains( words_operador_2[i])) )
                    {
                        documentos[j].Score=0;
                    }
                }
            }
            operador_2=false;
        }
        
        documentos = Ordenar.MergeSort(documentos,0,documentos.Length-1);
        //itera x los documentos ya ordenados descendientemente, suma 0.25 para los q devolvieron valores muy bajos y halla el snippet de cada documento
        for(int i=0;i < documentos.Length;i++)
        {
            if(documentos[i].Score==0){
                break;
            }
            else
            {
                documentos[i].Score += (float)0.25;
            }
            //hallando el snippet de cada documento
            documentos[i].Snippet=CrearSnippet(documentos[i].Text,palabras_query_sinrep);
        }

        SearchItem[] items = new SearchItem[documentos.Length];
        int score0 = documentos.Length;
        for(int i=0;i<documentos.Length;i++){
            if(documentos[i].Score == 0 ){
                score0=i;
                break;
            }
            items[i]= new SearchItem(documentos[i].Title,documentos[i].Snippet,documentos[i].Score);
        }

        return new SearchResult(items[0 .. score0], Sugerencia.ConstSuggestion(palabras_query));
    }
    //multiplica los vectore tfidf de los documentos con el de la query y devuelve la sumatoria de la multiplicacion
    public static float MultiplicarVectoresTfIdf(float[] doc,float[]que)
    {
        float resultado=0;

        for(int i = 0; i < doc.Length; i++)
        {
            resultado += doc[i]*que[i];
        }

        return resultado;
    }

    public static float CalcularScore(float[] tf_idf_document,bool ModificarSqrt_TFIDF_query)
    {
        float sqrt_document = 0;

        if(operador_4)
        {
            int pos;
            for(int i=0;i < words_operador_4.Length;i++)
            {
                pos=Preprocesamiento.Diccionario[words_operador_4[i].Item1];
                tf_idf_document[pos] = tf_idf_document[pos] *((float)(2 * words_operador_4[i].Item2 ));
                if(!ModificarSqrt_TFIDF_query) { continue; }
                TF_IDF_Query[pos] = TF_IDF_Query[pos] * ((float)(2 * words_operador_4[i].Item2 ));
            }
        }
        for(int i =0; i < tf_idf_document.Length;i++)
        {
            if(tf_idf_document[i]!=0){ sqrt_document+=(float)Math.Pow(tf_idf_document[i],2); }
            if(!ModificarSqrt_TFIDF_query) { continue; }
            if(TF_IDF_Query[i]!=0){ Sqrt_Query_score+=(float)Math.Pow(TF_IDF_Query[i],2); }
        }
        return (MultiplicarVectoresTfIdf(tf_idf_document,TF_IDF_Query)/(float)Math.Sqrt((float)(sqrt_document*Sqrt_Query_score)));
    }
    //crea los snippet segun la palabra con mayor tfidf de la query
    public static string CrearSnippet(string text,string[] que){
        int pos;
        string result="";
        string textaux=text.ToLower();

        for(int i=0;i<que.Length;i++)
        {   //itera x las palabras de mayor tfidf de la query y comprueba si tiene algun snippet esta palabra para ser devuelto
            pos=textaux.IndexOf(que[i]);
            if(pos!=(-1))
            {
                if((pos-100>=0)&&(pos+100<text.Length))
                {
                    return text[(pos-100) .. (pos+100)];
                }
                else if(pos-100>=0)
                {
                    return text[(pos-100) .. (text.Length)];
                }
                else if(pos+100<=text.Length){
                    return text[0 .. (pos+100)];
                }
                else{
                    return text;
                }
            }
        }
        return result;   
    }

}
