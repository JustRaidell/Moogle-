namespace MoogleEngine;
using System.IO;
using System.Text.RegularExpressions;

static class Methods
{
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

    private static double Truncate(this double value, int decimales)
    {
        double temp = Math.Pow(10, decimales);
        return (Math.Truncate(value * temp) / temp);
    }

    public static string Normalize(string text)
    {
        //El ultimo caracter \n es para quitar los saltos de linea
        text = Regex.Replace(text, "[!\"#$%&'()*+,-./:;<=>?@\\[\\]^_`{|}~¡¿\n]", string.Empty);
        text = text.ToLower();

        return text;
    }

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

    public static List<double> TF_IDF(string Text, Dictionary<string, int> indexer, List<double> vector, double[] idf)
    {
        vector = TF(Text, indexer, vector);

        for (int i = 0; i < vector.Count; i++)
        {
            vector[i] = vector[i] * idf[i];
            vector[i] = vector[i].Truncate(5);
        }
        return vector;
    }

    private static List<double> TF(string text, Dictionary<string, int> indexer, List<double> vector)
    {
        string[] word_list = text.Split(" ");
        for (int i = 0; i < word_list.Length; i++)
        {
            if (word_list[i] == String.Empty && !indexer.ContainsKey(word_list[i])) continue;
            vector[indexer[word_list[i]]]++;
        }
        return vector;
    }

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

    public static double CosSimilarity(List<double> vector1, List<double> vector2)
    {
        double result = Vector_Multiply(vector1, vector2);
        result = result / Magnitude(vector1, vector2);

        return result;
    }

    private static double Vector_Multiply(List<double> vector1, List<double> vector2)
    {
        double result = 0;
        for (int i = 0; i < vector1.Count; i++)
        {
            result += vector1[i] * vector2[i];
        }
        return result;
    }

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
}
