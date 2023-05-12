namespace MoogleEngine;
using System.IO;
using System.Text.RegularExpressions;

//Clase que contiene los metodos necesarios para que funcione Moogle.cs
static class Methods
{
    //Funcion que lee los documentos de una carpeta y llena un array de AuxItems con su respectivo Titulo y Contenido
    public static AuxItem[] ReadFiles()
    {
        string[] files = Directory.GetFiles("../Content", "*.txt*");
        AuxItem[] items = new AuxItem[Directory.GetFiles("../Content", "*.txt*", SearchOption.TopDirectoryOnly).Length];
        string name;

        int i = 0;
        foreach (var file in files)
        {
            if (!file.Contains(".txt")) continue;
            name = file.Substring(file.LastIndexOf('/') + 1);  //Comando para sacar el nombre del documento

            string text = File.ReadAllText($"{file}");

            items[i] = new AuxItem(name, text);
            i++;
        }
        return items;
    }

    //Funcion que trunca por un numero dado de lugares despues de la coma un double introducido
    private static double Truncate(this double value, int decimales)
    {
        double temp = Math.Pow(10, decimales);
        return (Math.Truncate(value * temp) / temp);
    }

    //Funcion que toma como parametro un texto y lo devuelve puramente en palabras, sin signos de puntuacion ni saltos de linea
    public static string Normalize(string text)
    {
        //El ultimo caracter \n es para quitar los saltos de linea
        text = Regex.Replace(text, "[!\"#$%&'()*+,-./:;<=>?@\\[\\]^_`{|}~¡¿\n]", string.Empty);
        text = text.ToLower();

        return text;
    }

    //Funcion que crea mi diccionario (universo de palabras) y mi matriz de TF*IDF
    public static Tuple<Dictionary<string, int>, List<List<double>>> Initialize(int cant_palabras, Dictionary<string, int> universo, List<List<double>> vocabulario, AuxItem[] items)
    {
        double[] init = new double[0];

        for (int i = 0; i < items.Length; i++)
        {
            vocabulario.Add(new List<double>());
            vocabulario[i] = init.ToList();

            string[] snippet_words_list = items[i].Text.Split(" ");
            for (int j = 0; j < snippet_words_list.Length; j++)
            {
                if (snippet_words_list[j] == String.Empty) continue;

                if (!universo.ContainsKey(snippet_words_list[j]))
                {
                    universo.Add(snippet_words_list[j], cant_palabras);
                    cant_palabras++;

                    for (int k = 0; k <= i; k++) vocabulario[k].Add(0);
                    init = new double[universo.Count];
                }
            }
        }

        return Tuple.Create(universo, vocabulario);
    }

    //Metodo que Calcula el TF*IDF de un vector. Esto es la relevancia de cada palabra que forman al vector
    public static List<double> TF_IDF(string Text, Dictionary<string, int> indexer, List<double> vector, double[] idf)
    {
        vector = TF(Text, indexer, vector);

        //Aqui multiplica el TF * el IDF previamente calculado
        for (int i = 0; i < vector.Count; i++)
        {
            vector[i] = vector[i] * idf[i];
            vector[i] = vector[i].Truncate(5);
        }
        return vector;
    }

    //Metodo privado que calcula solo el TF. Que cantidad de veces sale cada palabra en cada documento
    private static List<double> TF(string text, Dictionary<string, int> indexer, List<double> vector)
    {
        string[] word_list = text.Split(" ");
        for (int i = 0; i < word_list.Length; i++)
        {
            if (word_list[i] == String.Empty || !indexer.ContainsKey(word_list[i])) continue;
            vector[indexer[word_list[i]]]++;
        }
        return vector;
    }

    //Metodo que calcula el IDF de cada palabra. En cuantos documentos se repite cada palabra.
    public static double[] IDF(List<List<double>> vocabulario, Dictionary<string, int> universo, AuxItem[] items)
    {
        double[] doc_frec = new double[vocabulario[0].Count];
        Array.Fill(doc_frec, 0);

        foreach (KeyValuePair<string, int> key in universo)
        {
            for (int i = 0; i < vocabulario.Count; i++)
            {
                if (vocabulario[i][key.Value] != 0) doc_frec[key.Value]++;
            }
        }

        for (int i = 0; i < doc_frec.Length; i++)
        {
            if (doc_frec[i] != 0) doc_frec[i] = (Math.Log((items.Length) / (doc_frec[i]))) + 0.1;
            else doc_frec[i] = Math.Log((items.Length) / (doc_frec[i] + 1));
        }

        return doc_frec;
    }

    //Metodo que calcula la Similitud de Cosenos entre mi query y un documento. Devuelve que tan parecidos son estos documentos. Esta es la variable score
    public static double CosSimilarity(List<double> vector1, List<double> vector2)
    {
        double result = Vector_Multiply(vector1, vector2);
        result = result / Magnitude(vector1, vector2);

        return result;
    }

    //Este Metodo multiplica dos vectores
    private static double Vector_Multiply(List<double> vector1, List<double> vector2)
    {
        double result = 0;
        for (int i = 0; i < vector1.Count; i++)
        {
            result += vector1[i] * vector2[i];
        }
        return result;
    }

    //Metodo que calcula la multiplicacion de las magnitudes de dos vectores. Esto es la raiz cuadrada de la sumatoria de los cuadrados de sus terminos
    private static double Magnitude(List<double> vector1, List<double> vector2)
    {
        double a = 0;
        double b = 0;
        for (int i = 0; i < vector1.Count; i++)
        {
            a = a + Math.Pow(vector1[i], 2);
            b = b + Math.Pow(vector2[i], 2);
        }

        return (Math.Sqrt(a) * Math.Sqrt(b)) + 0.1;
    }

    /*public static double Helejnipe(string[] a, Dictionary<string, int> indexer, double[] idf, List<double> query)
    {
        List<string> oraciones;
        List<double> aux = new List<double>();
        

        oraciones = a.ToList();

        for (int i = 0; i < oraciones.Count; i++)
        {
            oraciones[i] = Normalize(oraciones[i]);
        }

        for (int i = 0; i < oraciones.Count; i++)
        {
            aux = TF_IDF(oraciones[i],indexer,aux,idf);
        }

        double res = CosSimilarity(aux,query);
        return res;
    }*/
}
