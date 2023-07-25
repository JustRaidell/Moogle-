namespace MoogleEngine;

//Clase que guarda El Titulo y el contenido de un documento, sí, es una copia de la clase SearchItem
public class AuxItem
{
    //Constructor de la clase
    public AuxItem(string title, string text)
    {
        this.Title = title;
        this.Text = text;
    }

    public string Title { get; private set; }

    public string Text { get; private set; }
}
