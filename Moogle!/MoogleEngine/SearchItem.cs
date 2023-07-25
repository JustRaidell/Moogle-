namespace MoogleEngine;

//Clase que guarda los items que se mostaran en pantalla cuando se realice la busqueda
public class SearchItem
{
    //Constructor.Toma Titulo del documento, Fragmento del documento que se asemeja a la query, y valor del documento respecto a la query
    public SearchItem(string title, string snippet, float score)
    {
        this.Title = title;
        this.Snippet = snippet;
        this.Score = score;
    }

    public string Title { get; private set; }

    public string Snippet { get; private set; }

    public float Score { get; private set; }
}
