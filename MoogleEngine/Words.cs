namespace MoogleEngine;

public static class Words
{
    //elimina los caracteres raros de un array de palabras y devuelve las palabras sin caracteres raros
    public static string[] CleanWords(string[] words, bool operador){

        string [] result = new string[words.Length];
        int cont = 0;
        string aux;

        for(int i = 0;i < words.Length;i++){

            aux = ConstWord(words[i], operador);

            if(aux==""){
                continue;
            }

            result[cont]=aux;
            cont++;

        }

        return result[0 .. cont];
    }
    //construye una palabras solo con los caracteres que nos interesa
    public static string ConstWord(string a, bool operador){

        string result ="";

        if(operador)
        {

            for(int i=0;i<a.Length;i++){
                if((a[i]>='a' && a[i] <= 'z')||(a[i]>='0'&& a[i]<='9') ||a[i]=='*'||a[i]=='^'||a[i]=='~'||a[i]=='!'|| a[i]==241 || a[i]==225 || a[i]==233 || a[i]==237 || a[i]==243 || a[i]==250 ){
                    result += a[i];
                }
            }

        }
        else
        {
            for(int i=0;i<a.Length;i++){
                if((a[i]>='a' && a[i] <= 'z')||(a[i]>='0'&& a[i]<='9') || a[i]==241 || a[i]==225 || a[i]==233 || a[i]==237 || a[i]==243 || a[i]==250 ){
                    result += a[i];
                }
            }
        }
        return result;
    }
    //calcula la distancia minima entre dos palabras en un documento
    public static int Words_Distance(string[] a,Tuple<string,string> b)
    {

        int item1=-1;
        int item2=-1;
        int result=int.MaxValue;

        if(!a.Contains(b.Item1) || !a.Contains(b.Item2)){
            return -1;
        }

        for(int i=0;i<a.Length;i++)
        {
            if(a[i]==b.Item1){

                item1=i;

                if((item1-item2<result) && item2!=-1 ){
                    result=item1-item2;
                }
            }
            else if(a[i]==b.Item2){

                item2=i;

                if((item2-item1<result) && item1 !=-1){
                    result=item2-item1;
                }
            } 
        }
        return result;
    }
    //elimina palabras repetidas de un array
    public static string[] DeleteDuplicate(string[] wordsrep)
    {
        List<string> palabras=new List<string>();
        palabras.AddRange(wordsrep);
        HashSet <string> DeleteDuplicates = new HashSet <string>(palabras);
        string[] result=DeleteDuplicates.ToArray();

        return result;
    }
}