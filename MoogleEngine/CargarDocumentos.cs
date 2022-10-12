namespace MoogleEngine;

public static class Preprocesamiento
{
    public static string[] Direction{get;private set;}
    public static string[] Titulos{get;private set;}
    public static string[] Documents{get; private set;}
    public static string[][] words_document{get; private set;}
    public static string[] words{get;private set;}
    public static Dictionary<string,int> Diccionario{get;private set;}
    public static float[,] TF_IDF_vector{get;private set;}
    public static float[] IDF_vector{get;private set;}
    public static Document[] Doc_{get;private set;}

    public static void CargarArchivos()
    {
        Direction = Directory.GetFiles(@"../Content","*.txt");
        CargarTitulos();
        CargarDocumentos();
        CargarDiccionario();
        CargarTF_IDF();
        Sugerencia.PalabrasSugeridas=new Dictionary<string, string>();

        //creando todos los documentos
        Doc_ = new Document[Direction.Length];
        for(int i=0;i<Direction.Length;i++)
        {                                                                                                      
            Doc_[i] = new Document(Titulos[i],Documents[i],words_document[i][0 .. (words_document[i].Length)],"",CrearVector(i),0);
        }
    }
    //itera por el array de direcciones las divide y se queda con el titulo de cada documento
    public static void CargarTitulos()
    {
        Titulos = new string[Direction.Length];

        for(int i =0; i < Direction.Length; i++)
        {
            Titulos[i] = Path.GetFileNameWithoutExtension(Direction[i]);
        }
    }
    //itera por las direcciones y guarda los textos de los documentos, las palabras de cada documento por separado y las 
    //palabras de todos los documentos en general
    public static void CargarDocumentos()
    {
        Documents = new string[Direction.Length];
        words_document= new string[Documents.Length][];
        List <string> palabras = new List <string>();
            
        for(int i =0;i < Direction.Length;i++){
                
            StreamReader fileReader = new StreamReader(@Direction[i]);
            Documents[i]=fileReader.ReadToEnd();
            words_document[i]=Words.CleanWords(Documents[i].ToLower().Split(new char[22] {' ',';','^','/','#','[',']','~','{','}','*','¡','!','¿','$',')','?',',',':','(','.','\n'}, StringSplitOptions.RemoveEmptyEntries),false);
            palabras.AddRange(words_document[i]);
        }

        //eliminando las palabras repetidas para quedarnos con el array de palabras de todos los documentos en general
        palabras.Sort();
        HashSet <string> DeleteDuplicates = new HashSet <string>(palabras);
        words=DeleteDuplicates.ToArray();   
    }
    //crear un diccionario donde a cada palabra de los documentos en general se le asigna una posicion
    public static void CargarDiccionario()
    {
        Diccionario = new Dictionary<string, int>();

        for(int i =0; i < words.Length;i++){
            Diccionario.Add(words[i],i);
        }
    }
    //calculando los TF de los documentos
    public static void CargarTF_IDF()
    {
        TF_IDF_vector = new float[Direction.Length,words.Length];
        IDF_vector = new float [words.Length];
        bool[] map = new bool [words.Length];
        string pal_dic;

        for(int i = 0; i < Direction.Length;i++)
        {
            map = new bool [words.Length];
                
            for(int j = 0; j< words_document[i].Length;j++){
                
                pal_dic = words_document[i][j];
                TF_IDF_vector[i,Diccionario[pal_dic]]+=1;
                    
                if(!map[Diccionario[pal_dic]]){
                        
                    IDF_vector[Diccionario[pal_dic]]+=1;
                    map[Diccionario[pal_dic]]=true;
                }
            }
        }
        //calculando el vector IDF   
        for(int i = 0; i < IDF_vector.Length;i++)
        {
            IDF_vector[i]=(float)Math.Log10(Direction.Length/IDF_vector[i]);
        }
    }

    //separa el vector tf-idf del documento de la matriz TF-IDF
    public static float[] CrearVector(int i){

        float[] result=new float[TF_IDF_vector.GetLength(1)];

        for(int j=0;j<TF_IDF_vector.GetLength(1);j++)
        {

            result[j]=TF_IDF_vector[i,j];

        }

        return result;
    }
}