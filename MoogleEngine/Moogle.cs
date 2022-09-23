namespace MoogleEngine;

public static class Moogle
{
    public static float[] TF_IDF_Query{get;private set;}
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
        //en caso de que la query contenga un operador que lo marque como true y cargue las variables de los operadores
        if(query.Contains('!'))
        {
            operador_1=true;
            Operadores.Operador1(query);
        }
        if(query.Contains('^'))
        {
            operador_2=true;
            Operadores.Operador2(query);
        }
        if(query.Contains('~'))
        {
            operador_3=true;
            Operadores.Operador3(query);
        }
        if(query.Contains('*'))
        {
            operador_4=true;
            Operadores.Operador4(query);
        }

        string[] palabras_query = query.ToLower().Split(new char[22] {' ','!','^','~','*',';','/','#','[',']','{','}','¡','¿','$',')','?',',',':','(','.','\n'}, StringSplitOptions.RemoveEmptyEntries);
        palabras_query = Words.CleanWords( palabras_query,false); 
        float[] IDF = (float[])Preprocesamiento.IDF_vector.Clone();
        string[] palabras = Preprocesamiento.words;
        Dictionary <string,int> dic = Preprocesamiento.Diccionario;
        TF_IDF_Query = new float[palabras.Length];
        float sqrt_query_score = 0;
        Document[] documentos = Preprocesamiento.Doc_.CloneDocs();

        for(int i=0;i<palabras_query.Length;i++)
        {   //itera x las palabras de la query y si una no existe la sustituye x la mas similar
            if(!palabras.Contains(palabras_query[i]))
            {
                palabras_query[i]=Sugerencia.BuscarPalabra(palabras_query[i]);
            }
            TF_IDF_Query[dic[palabras_query[i]]]+=1;//rellena el vector tf de la query x cada palabra
        }

        string[] palabras_query_sinrep= Words.DeleteDuplicate(palabras_query);//elimina las palabras repetidas de la query
        
        if(operador_4){

            float old_tfidf;
            int pos;
            //itera x las palabras afectadas x el operador *
            for(int i =0; i < words_operador_4.Length; i++)
            {   //modifica el peso del idf de la palabra afectada x el operador *
                pos=dic[words_operador_4[i].Item1];
                IDF[pos]=IDF[pos]*(2*words_operador_4[i].Item2);
                
                for(int j =0 ; j < documentos.Length; j++)
                {
                    if(documentos[j].Words_Text.Contains(words_operador_4[i].Item1))
                    {   //itera x los documentos para rectificar los tfidf y la sumatoria de los valores tfidf al cuadrado
                        old_tfidf= documentos[j].TF_IDF[pos];
                        documentos[j].TF_IDF[pos]=documentos[j].TF_IDF[pos] * (2*words_operador_4[i].Item2);
                        documentos[j].sqrt_doc_score += (float)Math.Pow(documentos[j].TF_IDF[pos],2) - (float)Math.Pow(old_tfidf,2);
                    }
                }
            }
            operador_4=false;
        }

        //calcula el vector tfidf de la query y la sumatoria de los tfIdf al cuadrado del vector de la query
        for(int i=0;i<palabras_query_sinrep.Length;i++)
        {
            TF_IDF_Query[dic[palabras_query_sinrep[i]]]=IDF[dic[palabras_query_sinrep[i]]]*TF_IDF_Query[dic[palabras_query_sinrep[i]]];
            sqrt_query_score+=(float)Math.Pow(TF_IDF_Query[dic[palabras_query_sinrep[i]]],2);
        }

        //organiza las palabras de la query sin repetir segun su peso tfidf
        palabras_query_sinrep=Ordenar.Organiza(palabras_query_sinrep,0,palabras_query_sinrep.Length-1);

        //calcula los score de los documentos
        for(int i=0;i < documentos.Length;i++)
        {   //formula de similitud de coseno
            documentos[i].Score = MultiplicarVectoresTfIdf(documentos[i].TF_IDF,TF_IDF_Query)/(float)Math.Sqrt(documentos[i].sqrt_doc_score * sqrt_query_score);
        }

        if(operador_3)
        {   //itera x las palabras q hay q hallarle la cercania
            for(int i = 0; i < words_operador_3.Count ;i++){

                Tuple<Document,int>[] ContDistance = new Tuple<Document,int>[documentos.Length];
                //itera x los documentos para hallar la menor distancia entre las palabras afectadas x el operador ~
                for(int j=0;j < documentos.Length;j++){

                    ContDistance[j]=new Tuple<Document, int>(documentos[j],Words.Words_Distance(documentos[j].Words_Text,words_operador_3[i]));

                }
                //ordena los documentos en forma ascendente 
                ContDistance=Ordenar.Ordenacion(ContDistance,0,ContDistance.Length-1);
                bool[] pos=new bool[5];
                //itera x los documentos y le suma el score en dependecia de la distancia entre las palabras afectadas x el operador
                for(int k=0;k<ContDistance.Length;k++){

                    if(ContDistance[k].Item2==-1)
                    {   //si tiene distancia d -1 es xq no contiene a las palabras a la vez
                        continue;
                    }
                    else if(!pos[0])
                    {   //posicion 1
                        ContDistance[k].Item1.Score+=(float)0.25;
                        pos[0]=true;
                    }
                    else if(!pos[1])
                    {   //posicion 2
                        ContDistance[k].Item1.Score+=(float)0.2;
                        pos[1]=true;
                    }
                    else if(!pos[2])
                    {   //posicion 3
                        ContDistance[k].Item1.Score+=(float)0.15;
                        pos[2]=true;
                    }
                    else if(!pos[3])
                    {   //posicion 4
                        ContDistance[k].Item1.Score+=(float)0.1;
                        pos[3]=true;
                    }
                    else
                    {   //a partir de la posicion 5
                        ContDistance[k].Item1.Score+=(float)0.05;
                    }
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
            documentos[i].Principal_Snippet=CrearSnippet(documentos[i].Snippet,palabras_query_sinrep);
            if(documentos[i].Principal_Snippet==""){
                documentos[i].Principal_Snippet=documentos[i].Text[0 .. 200];
            }

        }

        SearchItem[] items = new SearchItem[documentos.Length];
        int score0=-1;
        for(int i=0;i<documentos.Length;i++){
            if(documentos[i].Score==0 && score0==-1){
                score0=i;
            }
            items[i]= new SearchItem(documentos[i].Title,documentos[i].Principal_Snippet,documentos[i].Score);
        }

        return new SearchResult(items[0 .. score0], Sugerencia.ConstSuggestion(palabras_query));
    }
    //multiplica los vectore tfidf de los documentos con el de la query y devuelve la sumatoria de la multiplicacion
    public static float MultiplicarVectoresTfIdf(float[] doc,float[]que){
        float resultado=0;

        for(int i = 0; i < doc.Length; i++){
            resultado+=doc[i]*que[i];
        }

        return resultado;
    }
    //crea los snippet segun la palabra con mayor tfidf de la query
    public static string CrearSnippet(Snippet[] a,string[] que){

        string result="";

        for(int i=0;i<que.Length;i++)
        {   //itera x las palabras de mayor tfidf de la query y comprueba si tiene algun snippet esta palabra para ser devuelto
            for(int j =0;j<a.Length;j++)
            {
                if(a[j].Words_Snippet.Contains(que[i]))
                {
                    return a[j].Text_Snippet;
                }
            }
        }
        
        return result;
    }

}
