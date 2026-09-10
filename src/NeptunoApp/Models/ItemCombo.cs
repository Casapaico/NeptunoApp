namespace NeptunoApp.Models
{
    /// <summary>Elemento generico para poblar ComboBox (Id + texto visible).</summary>
    public class ItemCombo
    {
        public int Id { get; set; }
        public string Texto { get; set; }

        public override string ToString() => Texto;
    }
}
