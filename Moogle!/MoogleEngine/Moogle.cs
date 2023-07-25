namespace MoogleEngine;

public class Moogle
{

    public static SearchResult Query(string query)
    {
        // Modifique este método para responder a la búsqueda

        //Con la query ya introducida, normalizo la query
        query = Methods.Normalize(query);

        //Array de AuxItems, más detalles en la definicion de la clase en su correspondiente en el .cs
        //Creo un array de los Titulos y los textos de los documentos tal y como estan en la carpeta.  
        AuxItem[] non_normalized_files;
        non_normalized_files = Methods.ReadFiles();

        //Este otro Array guarda los Titulos y textos de los documentos ya normalizados 
        AuxItem[] files;
        files = Methods.ReadFiles();
        for (int i = 0; i < files.Length; i++)
        {
            files[i] = new AuxItem(files[i].Title, Methods.Normalize(files[i].Text));
        }

        //Hace falta guardar el universo de palabras en esta clase?
        //Diccionario universo, le asigna a cada palabra un numero entero
        Dictionary<string, int> universo = new Dictionary<string, int>();
        //Matriz de documentos;palabras.Guarda en cada posicion la relevancia de la palabra en el documento 
        List<List<double>> vocabulario = new List<List<double>>();

        //Lleno mi diccionario y mi matriz
        var aux = Methods.Initialize(universo, vocabulario, files);
        universo = aux.Item1;
        vocabulario = aux.Item2;

        //Llamo al metodo que calcula el IDF. Su valor se guarda en la clase Methods
        Methods.IDF(vocabulario, files);

        //Con esto voy llenando la matriz con los TF*IDF de cada palabra en cada documento
        for (int i = 0; i < vocabulario.Count; i++)
        {
            vocabulario[i] = Methods.TF_IDF(files[i].Text, vocabulario[i]);
        }

        //Esta lista guarda el equivalente a la query en mi universo de palabras. O sea guarda el TF*IDF de la query  
        List<double> vect_query = new List<double>();
        for (int i = 0; i < vocabulario[0].Count; i++)
        {
            vect_query.Add(0);
        }
        vect_query = (Methods.TF_IDF(query, vect_query));

        //Este array score guarda la similitud de cada documento con la query. Mientras mayor score, mas parecido el documento
        double[] score = new double[vocabulario.Count];
        for (int i = 0; i < vocabulario.Count; i++)
        {
            score[i] = Methods.CosSimilarity(vocabulario[i], vect_query);
        }

        //Lista de los scores de los documentos y sus posiciones, ordenados de mayor a menor por score
        List<Tuple<double, int>> docs_order = new List<Tuple<double, int>>();
        for (int i = 0; i < files.Length; i++)
        {
            docs_order.Add(Tuple.Create(score[i], i));
        }
        docs_order.Sort((x, y) => y.Item1.CompareTo(x.Item1));

        //Array de los Snippet de cada documento
        string[] snippet = new string[files.Length];

        for (int i = 0; i < snippet.Length; i++)
        {
            snippet[i] = Methods.Helejnipe(non_normalized_files[i].Text, query);
        }

        //Lista de los documentos que no tienen cero de score
        List<SearchItem> non_zero_list = new List<SearchItem>();
        for (int i = 0; i < files.Length; i++)
        {
            if (score[docs_order[i].Item2] != 0) non_zero_list.Add(new SearchItem(non_normalized_files[docs_order[i].Item2].Title, snippet[docs_order[i].Item2], ((float)score[docs_order[i].Item2])));
        }
        SearchItem[] items = non_zero_list.ToArray();

        return new SearchResult(items, "¿No encuentras lo que buscas? Revisa bien la escritura y ortografía");
    }
}

