namespace MoogleEngine;
//metodos de ordenacion por MergeSort
public static class Ordenar
{
    //ordena los documentos x el peso de score
    public static Document[] MergeSort(Document[] a,int start,int end){

        if(start == end){
            return a [start .. (end+1)];
        }

        Document[]b = MergeSort(a,start,(start+end)/2);
        Document[]c = MergeSort(a,((start+end)/2)+1,end);
        Document[]d=MezclaDescendiente(b,c);
        return d;

    }
    public static Document[] MezclaDescendiente(Document[] a, Document[] b){

        Document[] c = new Document[a.Length + b.Length];
        int i = 0;
        int j = 0;

        for(int k = 0; k < c.Length; k++){

            if((i < a.Length) && ( j < b.Length) && (a[i].Score >= b[j].Score)){
                c[k] = a[i];
                i++;
            }
            else if (j < b.Length){
                c[k] = b[j];
                j++;
            }
            else{
                c[k] = a[i];
                i++;
            }
        }
        return c;
    }
    //ordena las palabras de la query x el peso del TFIDF en la query
    public static string[] Organiza(string[] query,int start,int end){

        if(start == end){
            return query [start .. (end+1)];
        }

        string[]b = Organiza(query,start,(start+end)/2);
        string[]c = Organiza(query,((start+end)/2)+1,end);
        string[]d=MezclaDescendienteQuery(b,c);
        return d;

    }
    public static string[] MezclaDescendienteQuery(string[] a, string[] b){

        string[] c = new string[a.Length + b.Length];
        int i = 0;
        int j = 0;

        for(int k = 0; k < c.Length; k++){

            if((i < a.Length) && ( j < b.Length) && (Moogle.TF_IDF_Query[Preprocesamiento.Diccionario[a[i]]] >= Moogle.TF_IDF_Query[Preprocesamiento.Diccionario[b[j]]])){
                c[k] = a[i];
                i++;
            }
            else if (j < b.Length){
                c[k] = b[j];
                j++;
            }
            else{
                c[k] = a[i];
                i++;
            }
        }
        return c;       
    }
}
