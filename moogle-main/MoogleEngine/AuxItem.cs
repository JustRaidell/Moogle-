namespace MoogleEngine;

public class AuxItem
{
    public AuxItem(string title, string text)
    {
        this.Title = title;
        this.Text = text;
    }

    public string Title { get; private set; }

    public string Text { get; private set; }
}
