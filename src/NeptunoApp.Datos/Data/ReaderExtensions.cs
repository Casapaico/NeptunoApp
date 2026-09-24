using Microsoft.Data.SqlClient;

namespace NeptunoApp.Data;

/// <summary>Helpers para leer columnas que pueden ser NULL desde un SqlDataReader.</summary>
public static class ReaderExtensions
{
    public static string? GetStringOrNull(this SqlDataReader r, string col)
    {
        int i = r.GetOrdinal(col);
        return r.IsDBNull(i) ? null : r.GetValue(i).ToString();
    }

    public static int? GetIntOrNull(this SqlDataReader r, string col)
    {
        int i = r.GetOrdinal(col);
        return r.IsDBNull(i) ? null : Convert.ToInt32(r.GetValue(i));
    }

    public static DateTime? GetDateTimeOrNull(this SqlDataReader r, string col)
    {
        int i = r.GetOrdinal(col);
        return r.IsDBNull(i) ? null : Convert.ToDateTime(r.GetValue(i));
    }

    public static decimal GetDecimalSafe(this SqlDataReader r, string col)
    {
        int i = r.GetOrdinal(col);
        return r.IsDBNull(i) ? 0m : Convert.ToDecimal(r.GetValue(i));
    }

    public static short GetInt16Safe(this SqlDataReader r, string col)
    {
        int i = r.GetOrdinal(col);
        return r.IsDBNull(i) ? (short)0 : Convert.ToInt16(r.GetValue(i));
    }
}
