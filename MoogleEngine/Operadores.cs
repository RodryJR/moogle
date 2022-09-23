namespace MoogleEngine;

public static class Operadores
{
    public static void Operador1(string query)
    {
        //separa las palabras de la query y solo del deja el ! con las palabras
        string[] palabras_query = query.ToLower().Split(new char[21] {' ','^','~','*',';','/','#','[',']','{','}','¡','¿','$',')','?',',',':','(','.','\n'}, StringSplitOptions.RemoveEmptyEntries);
        palabras_query = Words.CleanWords(palabras_query,true);
        string[] word_operador_1=new string [palabras_query.Length];
        int count=0;

        for(int i =0;i < palabras_query.Length;i++){

            if(palabras_query[i][0]=='!'){
                //construye las palabras q no deben aparecer en los documentos quitando el caracter!
                palabras_query[i]=Words.ConstWord(palabras_query[i],false);

                if(palabras_query[i]=="")
                {
                    continue;
                }
                else
                {
                    word_operador_1[count]=palabras_query[i];
                    count++;
                }
            }

        }
        //defino las palabras q contiene el operador !
        if(count != 0)
        {
            Moogle.words_operador_1=word_operador_1[0 .. count];
        }
        else
        {
            Moogle.operador_1 = false;
        }

    }

    public static void Operador2(string query)
    {
        //separa las palabras de la query y solo del deja el ^ con las palabras
        string[] palabras_query = query.ToLower().Split(new char[21] {' ','!','~','*',';','/','#','[',']','{','}','¡','¿','$',')','?',',',':','(','.','\n'}, StringSplitOptions.RemoveEmptyEntries);
        string[] word_operador_2=new string [palabras_query.Length];
        int count=0;

        for(int i =0;i < palabras_query.Length;i++){

            if(palabras_query[i][0]=='^'){
                //construye las palabras q no deben aparecer en los documentos quitando el caracter ^
                palabras_query[i]=Words.ConstWord(palabras_query[i],false);

                if(palabras_query[i]=="")
                {
                    continue;
                }
                else
                {
                    //en caso de que la palabra no exista o este mal escrita la sustituyo x la mas similar
                    if(Preprocesamiento.words.Contains(palabras_query[i]))
                    {
                        word_operador_2[count]=palabras_query[i];
                        count++;
                    }
                    else
                    {
                        palabras_query[i]=Sugerencia.BuscarPalabra(palabras_query[i]);
                        word_operador_2[count]=palabras_query[i];
                        count++;
                    }
                }

            }

        }
        //defino las palabras q contiene el operador !
        if( count != 0 ){
            Moogle.words_operador_2 = word_operador_2 [ 0 .. count ];
        }
        else{
            Moogle.operador_2 = false;
        }
    }

    public static void Operador3(string query)
    {
        //separa la query donde haya el operdaor ~
        string[] Div_query = query.ToLower().Split(new char[1]{'~'},StringSplitOptions.RemoveEmptyEntries);
        int count;
       
        if(Div_query.Length == 1)
        {
            Moogle.operador_3=false;
        }
        else
        {
            Moogle.words_operador_3=new List<Tuple<string,string>>();
            //crea los array de palabras de las partes separadas de la query x el operador~
            string[][] wordsquerysplit=new string[Div_query.Length][];
            count=0;
            string[] aux;
            for(int i =0;i < Div_query.Length;i++)
            {
                aux=Div_query[i].ToLower().Split(new char[22] {' ',';','~','^','/','#','[',']','{','}','*','¡','!','¿','$',')','?',',',':','(','.','\n'}, StringSplitOptions.RemoveEmptyEntries);
                aux=Words.CleanWords(aux,false);
                
                if(aux.Length!=0)
                {
                    wordsquerysplit[count]=aux;
                    count++;
                }
            }

            // se asegura que las palabras existen y en caso de q no se sustituye x la mas similar
            for(int i = 1; i < count;i++)
            {
                if(!Preprocesamiento.words.Contains(wordsquerysplit[i-1][wordsquerysplit[i-1].Length-1]))
                {
                    wordsquerysplit[i-1][wordsquerysplit[i-1].Length-1]=Sugerencia.BuscarPalabra(wordsquerysplit[i-1][wordsquerysplit[i-1].Length-1]);
                }
                if(!Preprocesamiento.words.Contains(wordsquerysplit[i][0]))
                {
                    wordsquerysplit[i][0]=Sugerencia.BuscarPalabra(wordsquerysplit[i][0]);
                }
                //defino las palabras q modificadas x el operador ~
                Moogle.words_operador_3.Add(new Tuple<string,string>(wordsquerysplit[i-1][wordsquerysplit[i-1].Length-1],wordsquerysplit[i][0]));
            } 
        }
    }

    public static void Operador4(string query)
    {
        //separo las palabras de la query y solo dejo el operador * con las palabras
        string[] palabras_query = query.ToLower().Split(new char[21] {' ','!','^','~',';','/','#','[',']','{','}','¡','¿','$',')','?',',',':','(','.','\n'}, StringSplitOptions.RemoveEmptyEntries);
        Tuple<string,int>[] word_operador_4 = new Tuple<string,int>[palabras_query.Length];
        int count=0;
        int count_operador;

        for(int i = 0; i < palabras_query.Length ; i++){

            count_operador=0;

            if(palabras_query[i][0]=='*'){
                //itera x cada palabra y las q se encuentren con * en su primera posicion cuenta cuantos * tiene
                for(int j=0;j<palabras_query[i].Length;j++){

                    if(palabras_query[i][j]=='*'){
                        count_operador+=1;
                    }
                    else{
                        break;
                    }

                }
                //construyo la palabra sin los caracteres raros
                palabras_query[i]=Words.ConstWord(palabras_query[i],false);
                if(palabras_query[i]=="")
                {
                    continue;
                }
                else
                {
                    //en caso de q no exista la palabra la sustituyo x la mas similar
                    if(Preprocesamiento.words.Contains(palabras_query[i]))
                    {
                        word_operador_4[count] = new Tuple<string, int>(palabras_query[i],count_operador);
                        count++;
                    }
                    else
                    {
                        palabras_query[i]=Sugerencia.BuscarPalabra(palabras_query[i]);
                        word_operador_4[count] = new Tuple<string, int>(palabras_query[i],count_operador);
                        count++;
                    }
                }                   
            }

        }
        //defino las palabras q deben ser modificadas x el operador *
        if(count!=0){
            Moogle.words_operador_4 = word_operador_4[0 .. count];
        }
        else{
            Moogle.operador_4=false;
        }
    }
}
