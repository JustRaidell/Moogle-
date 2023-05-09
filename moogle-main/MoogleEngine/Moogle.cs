namespace MoogleEngine;

public class Moogle
{
    public static SearchResult Query(string query)
    {
        // Modifique este método para responder a la búsqueda
        query = Methods.Normalize(query);

        AuxItem[] non_normalized_files;
        non_normalized_files = Methods.ReadFiles();

        AuxItem[] files;
        files = Methods.ReadFiles();
        for (int i = 0; i < files.Length; i++)
        {
            files[i] = new AuxItem(files[i].Title, Methods.Normalize(files[i].Text));
        }

        Dictionary<string, int> universo = new Dictionary<string, int>();
        List<List<double>> vocabulario = new List<List<double>>();

        var aux = Methods.Initialize(0, universo, vocabulario, files);
        universo = aux.Item1;
        vocabulario = aux.Item2;

        double[] global_idf = Methods.IDF(vocabulario, universo, files);
        for (int i = 0; i < vocabulario.Count; i++)
        {
            vocabulario[i] = Methods.TF_IDF(files[i].Text, universo, vocabulario[i], global_idf);
        }

        List<double> vect_query = new List<double>();
        for (int i = 0; i < vocabulario[0].Count; i++)
        {
            vect_query.Add(0);
        }
        vect_query = (Methods.TF_IDF(query, universo, vect_query, global_idf));

        double[] score = new double[vocabulario.Count];
        for (int i = 0; i < vocabulario.Count; i++)
        {
            score[i] = Methods.CosSimilarity(vocabulario[i], vect_query);
        }

        SearchItem[] items = new SearchItem[files.Length];

        string[] splitted_query = query.Split();
        double[] query_max_relevance = new double[files.Length];
        string[] query_most_relevant_word = new string[files.Length];
        Array.Fill(query_max_relevance, 0);
        for (int i = 0; i < files.Length; i++)
        {
            query_most_relevant_word[i] = non_normalized_files[i].Text.Split()[0];
        }
        for (int i = 0; i < splitted_query.Length; i++)
        {
            for (int j = 0; j < files.Length; j++)
            {
                if (!universo.ContainsKey(splitted_query[i])) continue;
                if (query_max_relevance[j] < vocabulario[j][universo[splitted_query[i]]])
                {
                    query_most_relevant_word[j] = splitted_query[i];
                }
            }
        }

        string[] full_snippet = new string[items.Length];
        Array.Fill(full_snippet, "");
        for (int i = 0; i < items.Length; i++)
        {
            string[] pre_snippet = non_normalized_files[i].Text.Split(" , \n");
            for (int j = 0; j < pre_snippet.Length; j++)
            {
                int m = 0;
                int n = 0;
                if (pre_snippet[j] == query_most_relevant_word[i])
                {
                    if (j - 10 < 0)
                    {
                        m = 0;
                        n = j + 10;
                    }
                    else if (j + 10 >= pre_snippet.Length)
                    {
                        m = j - 10;
                        n = pre_snippet.Length - 1;
                    }
                    else
                    {
                        m = j - 10;
                        n = j + 10;
                    }
                }

                for (int k = m; k <= n; k++)
                {
                    full_snippet[i] = full_snippet[i] + pre_snippet[k];
                }
            }
        }

        /*
                bool[] scored = new bool [items.Length];
                Array.Fill (scored,false);
                for (int i = 0; i < items.Length; i++)
                {
                    double major_score = 0;
                    int pos = -1;
                    for (int j = 0; j < items.Length; j++)
                    {
                        if(major_score < score[j] && !scored[j]){
                            major_score = score[j];
                            pos = j;
                        }
                    }
        */
        //            if(score[pos] != 0)
        for (int i = 0; i < items.Length; i++)
        {
            //if(score[i] != 0)
            items[i] = new SearchItem(non_normalized_files[i].Title, full_snippet[i], ((float)score[i]));
        }

        //            scored[pos] = true;

        return new SearchResult(items, "¿No ecuentras lo que buscas? Revisa bien la escritura y ortografía");
    }

        /*       
                t = 0;
                for (int i = 0; i < valor.Length; i++)
                {
                    if (valor[i] == 0) continue;
                    else
                    {
                        items[t] = new SearchItem(non_normalized_files[i].Title, non_normalized_files[i].Text, ((float)valor[i]));
                        t++;
                    }
                }
        */

}

