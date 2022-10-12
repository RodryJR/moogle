# **Moogle!** #

El proyecto se basa en el calculo por modelo vectorial por la formula de similitud de coseno y para implementar esta idea se modelo los documentos como objetos y realizaron todas las operaciones pertinentes para devolver los documentos con mayor score con respecto a la query y para eso contamos con una serie de metodos encargados de realizar esta misión.

## *class Preprocesamiento* ##
En esta clase como el nombre lo indica haremos todas las operaciones necesarias para crear nuestros documentos y acelerar el proceso de búsqueda, el método que se encarga de guiar e iniciar los documentos es *CargarArchivos()* en el que primeramente guardamos todas las direcciones de los archivos .txt guardandolas en la variable *string[] Direction* y llamamos a otros metodos como: 


```c#
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
```

**CargarTitulos()** : saca nombres de las direcciones de los documentos y los guarda en *string[] Titulos* .

```c#
public static void CargarTitulos()
{
    Titulos = new string[Direction.Length];

    for(int i =0; i < Direction.Length; i++)
    {
        Titulos[i] = Path.GetFileNameWithoutExtension(Direction[i]);
    }
}
```

**CargarDocumentos()** : con las direcciones guardadas ya podemos leer los archivos .txt correspondiente a cada documento y lo guardamos en la variable *string[] Documents* , además vamos guardando a su vez en la variable *stirng[][] words_document* los array de palabras de cada documento y vamos añadiendo a la vez todos estos arrays de palabras de cada documentos a una lista que al finalizar de iterar por todos los documentos eliminaremos las palabras duplicadas y guardaremos en la variable *string[] words* por lo que tendremos un array de todas las palabras de todos los documentos sin repetir.

```c#
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
```

**CargarDiccionario()** : creamos un diccionario en el cual a cada palabra le asignaremos un numero el que será su posición en el array de palabras de todos los documentos para a la hora de crear los vectores TF-IDF de cada documento ya ir directo a su posición correspondiente en el vector.

```c#
public static void CargarDiccionario()
{
    Diccionario = new Dictionary<string, int>();

    for(int i =0; i < words.Length;i++){
        Diccionario.Add(words[i],i);
    }
}
```

**CargarTF_IDF()** : rellenamos los vectores TF de todos los documentos recorriendo por todas las palabras de los documentos y como tenemos el diccionario de palabras nos indica en la posicion que debemos ir sumando el TF, a la par iremos llenando el vector IDF de la variables *float[] IDF_vector* donde cada posición se le hace corresponder a la posición de las palabras de todos los documentos y guardará de momento la cantidad de documento donde aparece la palabra, cuando termine de llenar los vectores TF de todos los documentos calcularemos el IDF correspondiendte a cada palabra con la formula Log10((cantidad total de documentos)/(cantidad de documentos donde aparece la palabra)) y nuevamente guardamos ese resultado en la variable *IDF_vector*.

```c#
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
```

**CrearVector()** : separa del array bidimensional *float[,] TF_IDF_vector* los vectores TF_IDF de cada document.

```c#
public static float[] CrearVector(int i){

    float[] result=new float[TF_IDF_vector.GetLength(1)];

    for(int j=0;j<TF_IDF_vector.GetLength(1);j++)
    {

        result[j]=TF_IDF_vector[i,j];

    }

    return result;
}
```

Después de recorrer todos estos métodos creamos los documentos con toda la informacion recopilada y el proyecto estaría listo para recibir la query.

## *Document* ##
Este objeto lo conforma:

*string Title* : Título del documento. 

*string Text* : Texto del documento.

*string[] Words_Text* : Palabras del texto del documento.

*string Snippet* : aquí se va a guardar el snippet que será devuelto.

*float[] TF_IDF* : El vector TF-IDF del documento.

*float score* : Score del documento.

```c#
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
```

## *Moogle.Query()* ##
En este método es donde calcularemos los score de los documentos, para ello recibimos la query y los primero que procedemos es a comprobar si contiene algún operador, en caso de tener, tenemos en la clase **Operadores** cuatro métodos para cada uno del los operadores que a modo de resumen verifica si el operador usado en la query es válido y devuelve las palabras que estan afectadas por dicho operador, ...

### *class Operadores* ###

*Operador1()* : recibe la query separa en array de palabras sin los caracteres raros excepto los operadores, luego recorre por cada palabra buscando cual tiene en su inicio el '!', cuando la encuentra la guarda en un array de palabras y las guarda en la variable *string[] words_operador_1*.

*Operador2()* : realiza la misma operación que el anterior solo q con el caracter '^' como operador y guarda las palabras q contienen este operador en la variable *string[] words_operador_2*.

*Operador4()* : hace la misma operación que los 2 operadores anteriores para encontrar la palabra que contenga el operador '*' pero una vez que la encuentra comienza a iterar por la palabra para contar la cantidad de * que contiene la palabra y guardamos la palabra y la cantidad de veces que aparece el operador en una Tuple<string,int> y lo guardamos en la variable *Tuple<string,int>[] words_operador_4*.

*Operador3()* : recibimos la query le hacemos un .Split('~') y así tenemos pedazos de string en el cual la ultima palabra de un pedazo de string y la primera palabra del string que le sigue son las palabras q son afectadas por este operador, por tanto guardo esas 2 palabras en un Tuple<string,string> y las añado a una lista que es la variable *List<Tuple<string,string>> words_operador_3*.

... iteramos por las palabras de la query para ir rellenando el vector TF y de paso vamos verificando si las palabras de la query existen que en caso q no las sustituimos por una palabra similar con un método en la clase **Sugerencia** , ...

### *class Sugerencia* ###

*BuscarPalabra()* : dado una palabra que le pases te devuelve la palabra que menor número de cambios hay que hacerle para transformarla en una palabra valida auxiliándose en el metodo de *LevenshteinDistance()*. Primero verifica si ya la palabras que necesitamos obtener su palabra mas similar ya se hallo y si se encuentra guardada en *Dictionary<string,string>PalabrasSugeridas* y en caso de que no procede a buscar la palabra mas similar y la anade en este diccionario donde la key es la palabra que no se encuentra en los documentos y el value su palabra mas similar. Guarda en una lista las palabras de menor numero de cambios y con el metodo *PalabraSimilar()* devuelve la palabra mas similar.

```c#
public static string BuscarPalabra(string a)
{
    Moogle.sugerencia=true;

    if(PalabrasSugeridas.ContainsKey(a))
    {
        return PalabrasSugeridas[a];
    }
    List<string> palabra_similar= new List<string>();
    int menor_distancia = int.MaxValue;
    int auxiliar;

    for(int i=0;i < Preprocesamiento.words.Length;i++){
        auxiliar = LevenshteinDistance(a,Preprocesamiento.words[i]);
        if(auxiliar==menor_distancia){
            palabra_similar.Add(Preprocesamiento.words[i]);
        }
        if(auxiliar < menor_distancia){
            palabra_similar=new List<string>();
            palabra_similar.Add(Preprocesamiento.words[i]);
            menor_distancia = auxiliar;
        }

    }

    return PalabraSimilar(palabra_similar.ToArray(),a);
}
```

*LevenshteinDistance()* : dado 2 string te calcula el menor número de cambios que hay que hacer para llevar un string a otro.

```c#

public static int LevenshteinDistance(string a, string b)
{
    int costo=0;
    int k=a.Length;
    int l=b.Length;
    int [,] mat=new int[k+1,l+1];

    for(int i=0;i<=k;i++)
    {
        for(int j=0;j<=l;j++)
        {
            if(i==0)
            {
                mat[i,j]=j;
            }
            else if(j==0)
            {
                mat[i,j]=i;
            }
            else 
            {
                if(a[i-1]==b[j-1])
                {
                    costo=0;
                }else
                {
                    costo=1;
                }
                mat[i,j]=Math.Min(Math.Min(mat[i-1,j]+1,mat[i,j-1]+1),mat[i-1,j-1]+costo);
            }
        }
    }

    return mat[k,l];
}
```

*PalabraSimilar()*: dado un array de palabras y la palabra que no se encuentra en los documentos, devuelve la palabra q contenga el mayor prefijo comun entre la palabra de menor numero de cambios y la que no aparece en los documentos.

```c#
public static string PalabraSimilar(string[] palabras,string a)
{
    if(palabras.Length==1){
        return palabras[0];
    }
    string result="";
    int cantletrascomun=0;
    int aux;
    for(int i=0;i<palabras.Length;i++)
    {
        aux=0;
        for(int j=0;(j<a.Length && j<palabras[i].Length);j++)
        {
            if(a[j]==palabras[i][j])
            {
                aux++;
            }
            else
            {
                break;
            }
        }
        if(aux>cantletrascomun)
        {
            cantletrascomun=aux;
            result= palabras[i];
        }
    }
    if(result=="")
    {
        return palabras[0];
    }
    PalabrasSugeridas.Add(a,result);
    return result;
}
```

*ConstSuggestion()* : dado un array de palabras de la query te construye la sugerencia y en caso de que la variable *Moogle.ssugerencia* sea *false* significa que no hay ninguna palabra mal escrita en la query por lo que te devolvera string vacio y en la pagina no aparecera ninguna sugerencia.

```c#
public static string ConstSuggestion(string[] query)
{
    string result="";
        
    if(Moogle.sugerencia)
    {
        for(int i=0;i<query.Length;i++)
        {
            result += query[i];
            if(i != (query.Length-1))
            {
                result+=" ";
            }
        }
    }

    return result;
}
```


... creamos un array de palabras de la query sin repetir con un metodo de la clase **Words**, asi podremos manejar más fácil el completamiento de los vectores tfidf de la query y el cálculo de los score de los documento...

### *class Words* ###

*ArrayWordsWithOperator()*: dado la query y un valor bool que representara si quiere q sea devuelto el array de palabras con los operadores o sin ellos, para esto utilizamos *.Split()*

*CleanWords()* : dado un array de palabras te devuelve un array de palabras pero con las palabras sin caracteres raros auxiliándose del método *ConstWord()*.

*ConstWord()* : dado un string construye una palabra obviando los caracteres raros.

*DeleteDuplicates()* : dado un array de string elimina los string repetidos.

*Words_Distance()* : dado un array de palabras y un Tuple de palabras devuelve la menor distancia que hay entre las palabras de la Tuple.

... , procedemos a calcular los score de cada documento con el metodo *CalcularScore()* en el cual le pasamos el vector tf-idf del documento y un valor bool que para el primer calculo estara en *true* para calcular la sumatoria de los valores del vector tf-idf de la query y modificar (en caso de que este activado el operador 4) el vector de la query una sola vez ya q seria innecesario realizar estas operaciones por cada documento ya que son las mismas siempre, calcula la sumatoria de los valores del vector tf-idf del documento al cuadrado y en caso de que el operador 4 este activado modificara el valor en los vectores tf-idf del documento y de la query en la posicion correspondiente a las palabras afectadas por dicho operador, multiplicando el valor del vector tf-idf por (2*cantidad de veces que aparecer el operador en la palabra). 
```c#
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
```

Ahora comprobamos si el operador 3 es válido, en caso de que si recorremos por cada documento y con el metodo de *Words.WordsDistance()* calculamos las distancias minima de las palabras en los documentos y anadimos el 1/(distancia de las palabras) al score del documento.
Comprobamos si los operadores 1 y 2 estan activados en caso de que si recorremos los documentos y los documentos que tengan las palabras afectadas por el operador 1 se le asignara valor de score 0 y los documentos que no tengan las palabras afectadas por el operador 2 se le asignara score 0 tambien.
Una vez ya resuelto los score de todos los documentos procedemos a ordenar los documentos y hallar los Snippet de los que no tengan score 0.

*CrearSnippet()* : dado el texto de un documento y las palabras de la query organizadas por su nivel de importancia, primeramente guardamos el texto en una variable auxiliar y le hacemos *.ToLower()* para que lleve todas las palabras a miniscula y asi poder encontrar las palabras de la query aunque esten en mayuscula, iteramos por el array de palabras de la query organizado por su peso y hacemos un IndexOf() con dicha palabra de la query que en caso de que devuelva -1 significa q no se encuentra por lo q seguimos iterando por el array de la query y en caso de que devuelva un valor vamos a retornar en un rango de 200 caracteres (100 a la derecha y 100 a la izquierda, tomando todas las precauciones por si nos salimos del rango del texto) el snippet del texto que contenga la palabra con mayor peso de la query.

```c#
public static string CrearSnippet(string text,string[] que)
{
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
```


