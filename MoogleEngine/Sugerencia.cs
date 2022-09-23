namespace MoogleEngine;

public static class Sugerencia
{
    //dado un string busca la palabra mas similar
    public static string BuscarPalabra(string a){
        
        string palabra_similar="";
        int menor_distancia = int.MaxValue;
        int auxiliar;

        for(int i=0;i < Preprocesamiento.words.Length;i++){
            auxiliar = LevenshteinDistance(a,Preprocesamiento.words[i]);
            if(auxiliar == 1){
                return Preprocesamiento.words[i];
            }
            if(auxiliar < menor_distancia){
                palabra_similar = Preprocesamiento.words[i];
                menor_distancia = auxiliar;
            }

        }

        return palabra_similar;
    }
    //calcula la distancia de levenshtein de dos palabras
    public static int LevenshteinDistance(string a, string b){
        int costo=0;
        int k=a.Length;
        int l=b.Length;
        int [,] mat=new int[k+1,l+1];

        for(int i=0;i<=k;i++){
            for(int j=0;j<=l;j++){
                if(i==0){
                    mat[i,j]=j;
                }
                else if(j==0){
                    mat[i,j]=i;
                }
                else {
                    if(a[i-1]==b[j-1]){
                        costo=0;
                    }else{
                        costo=1;
                    }
                    mat[i,j]=Math.Min(Math.Min(mat[i-1,j]+1,mat[i,j-1]+1),mat[i-1,j-1]+costo);
                }
            }
        }

        return mat[k,l];
    }
    //construye un string de la sugerencia
    public static string ConstSuggestion(string[] query){

        string result="";

        for(int i=0;i<query.Length;i++){
            result +=query[i];
            if(i != (query.Length-1)){
                result+=" ";
            }
        }
        return result;
    }
}
