namespace NeptunoApp.Models;

/// <summary>Elemento para poblar ComboBox (Id + texto visible).</summary>
public class OpcionCombo
{
    public int Id { get; set; }
    public string Texto { get; set; } = string.Empty;

    public override string ToString() => Texto;
}
