# **Moogle!** #

El proyecto se basa en el calculo por modelo vectorial por la formula de similitud de coseno y para implementar esta idea se modelo los documentos como objetos y realizaron todas las operaciones pertinentes para devolver los documentos con mayor score con respecto a la query y para eso contamos con una serie de metodos encargados de realizar esta misión, pero primero empecemos desde el principio...

## *class Preprocesamiento* ##
En esta clase como el nombre lo indica haremos todas las operaciones necesarias para crear nuestros documentos y acelerar el proceso de búsqueda, el método que se encarga de guiar e iniciar los documentos es *CargarArchivos()* en el que primeramente guardamos todas las direcciones de los archivos .txt guardandolas en la variable *string[] Direction* y llamamos a otros metodos como: 

**CargarTitulos()** : separa los títulos de las direcciones de los documentos y los guarda en la variable *string[] Titulos* .

**CargarDocumentos()** : con las direcciones guardadas ya podemos leer los archivos .txt correspondiente a cada documento y lo guardamos en la variable *string[] Documents* , además vamos guardando a su vez en la variable *stirng[][] words_document* los array de palabras de cada documento y vamos añadiendo a la vez todos estos arrays de palabras de cada documentos a una lista que al finalizar de iterar x todos los documentos eliminaremos las palabras duplicadas y guardaremos en la variable *string[] words* por lo que tendremos un array de todas las palabras de todos los documentos sin repetir.

**CargarDiccionario()** : creamos un diccionario en el cual a cada palabra le asignaremos un numero el que será su posición en el array de palabras de todos los documentos para a la hora de crear los vectores TF-IDF de cada documento ya ir directo a la posición del vector.

**CargarTF_IDF()** : rellenamos los vectores TF de todos los documentos recorriendo todas las palabras de los documentos y como tenemos el diccionario de palabras nos indica en la posicion que debemos ir sumando el TF, a la par iremos llenando el vector IDF de la variables *float[] IDF_vector* donde cada posición se le hace corresponder a la posición de las palabras de todos los documentos y guardará de momento la cantidad de documento donde aparece la palabra, cuando termine de llenar los vectores TF de todos los documentos calcularemos el IDF correspondiendte a cada palabra con la formula Log10((cantidad total de documentos)/(cantidad de documentos donde aparece la palabra)) y nuevamente guardamos ese resultado en la variable *IDF_vector*.

**CargarSqrtScore()** : como ya tenemos los vectores TF de cada documento y el vector IDF de las palabras procedemos a calcular los vectores TF-IDF de cada documento y calculamos la variable *float[] Sqrt_Document_score* de cada documento que es la sumatoria de los TF-IDF de cada palabra al cuadrado que es necesario para la formula de similitud de coseno.

**CargarSnippet()** : recibe el texto de un documento, lo divide en strings de 201 caracteres y halla un array de palabras de ese pedazo de string con el metodo **CargarPalabrasSnippet()** para crear un array de tipo Snippet q contenga cada pedazo de texto y su array correspondiente de palabras.

**CrearVector()** : separa del array bidimensional *float[,] TF_IDF_vector* los vectores TF_IDF de cada document. 

Después de recorrer todos estos métodos creamos los documentos con toda la informacion recopilada y el proyecto estaría listo para recibir la query.

## *Document* ##
Este objeto lo conforma:

*string Title* : Título del documento. 

*string Text* : Texto del documento.

*string[] Words_Text* : Palabras del texto del documento.

*string PrincipalSnippet* : aquí se va a guardar el snippet que será devuelto.

*Snippet[] Snippet* : Todos los posibles snippet del documento.

*float[] TF_IDF* : El vector TF-IDF del documento.

*float sqrt_doc_score* : Variable necesaria para el cálculo del score.

*float score* : Score del documento.

Cuenta con una clase extensor donde definimos el clone de un array de documentos necesario cuando modifiquemos variables de un documento y no queremos que se modifiquen las variables precalculadas originalmente.

## *Moogle.Query()* ##
En este método es donde calcularemos los score de los documentos, para ello recibimos la query y los primero que procedemos es a comprobar si contiene algún operador, en caso de que si tenemos en la clase **Operadores** cuatro métodos para cada uno del los operadores que a modo de resumen verifica si el operador usado en la query es válido y devuelve las palabras que estan afectadas por dicho operador, ...

### *class Operadores* ###

*Operador1()* : recibe la query y la separa en array de palabras eliminando todos los caracteres raros excepto el de '!', luego recorre por cada palabra buscando cual tiene en su inicio el '!', cuando la encuentra la guarda en un array de palabras y las guarda en la variable *string[] words_operador_1*.

*Operador2()* : realiza la misma operación que el anterior solo q con el caracter '^' como operador y guarda las palabras q contienen este operador en la variable *string[] words_operador_2*.

*Operador4()* : hace la misma operación que los 2 operadores anteriores para encontrar la palabra que contenga el operador '*' pero una vez que la encuentra comienza a iterar por la palabra para contra la cantidad de * que contiene la palabra y guardamos la palabra y la cantidad de veces que aparece el operador en una Tuple<string,int> y lo guardamos en la variable *Tuple<string,int>[] words_operador_4*.

*Operador3()* : recibimos la query le hacemos un .Split('~') y así tenemos pedazos de string en el cual la ultima palabra de un pedazo de string y la primera palabra del string que le sigue son las palabras q son afectadas por este operador, por tanto guardo esas 2 palabras en un Tuple<string,string> y las añado a una lista que es la variable *List<Tuple<string,string>> words_operador_3*.

... iteramos por las palabras de la query para ir rellenando el vector TF y de paso vamos verificando si las palabras de la query existen que en caso q no las sustituimos por una palabra similar con un método en la clase **Sugerencia** , ...

### *class Sugerencia* ###

*BuscarPalabra()* : dado una palabra que le pases te devuelve la palabra que menor número de cambios hay que hacerle para transformarla en una palabra valida auxiliándose en el metodo de *LevenshteinDistance()*.

*LevenshteinDistance()* : dado 2 string te calcula el menor número de cambios que hay que hacer para llevar un string a otro.

*ConstSuggestion()* : dado un array de palabras de la query te construye la sugerencia.

... creamos un array de palabras de la query sin repetir con un metodo de la clase **Words**, asi podremos manejar más fácil el completamiento de los vectores tfidf de la query y el cálculo de los score de los documento...

### *class Words* ###

*CleanWords()* : dado un array de palabras te devuelve un array de palabras pero con las palabras sin caracteres raros auxiliándose del método *ConstWord()*.

*ConstWord()* : dado un string construye una palabra obviando los caracteres raros.

*DeleteDuplicates()* : dado un array de string elimina los string repetidos.

*Words_Distance()* : dado un array de palabras y un Tuple de palabras devuelve la menor distancia que hay entre las palabras de la Tuple.

... , si el operador 4 es válido entonces pasamos a modificar el idf de la palabra que afecta este operador y hacemos los respectivos cambios en los tfidf de los documentos ect.., una vez ya tenemos el vector TF de la query y el vector IDF procedemos a calcular el vector TF-IDF de la query y el sqrt_query_score, ya con todo listo calculamos el Score de los documentos por la formula de similitud de coseno que es la sumatoria de las multiplicaciones de los vectores tfidf del documento con la query de las posiciones i ésima dividido por la raiz cuadrada de la multiplicacion de los sqrt_doc_score y sqrt_query_score, obteniendo asi los Score de cada documento. Ahora comprobamos si el operador 3 es válido, en caso de que si recorremos por cada documento y con el metodo de *Words.WordsDistance()* calculamos las distancias minima de las palabras en los documentos y lo metemos en un array de tuple de documentos con la distancia en el q ordenamos de forma ascendente y las primeras 4 posiciones recibiran un score escalonadamente mayor a los demas.
Comprobamos si los operadores 1 y 2 estan activados en caso de que si recorremos los documentos y los documentos que tengan las palabras afectadas por el operador 1 se le asignara valor de score 0 y los documentos que no tengan las palabras afectadas por el operador 2 se le asignara score 0 tambien.
Una vez ya resuelto los score de todos los documentos procedemos a hallar los Snippet.

*CrearSnippet()* : dado el array de posibles snippet de un documento y las palabras de la query organizadas por su nivel de importancia, iteramos por las palabras de la query de mayor importancia y retornamos el snippet que contenga la palabra de la query con mayor valor.


