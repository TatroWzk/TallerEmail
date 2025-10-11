using System.Collections.Generic;
using System.Text;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Encriptacion : MonoBehaviour
{
    [Header("UI")]
    public TMP_InputField inputField;
    public TMP_Text outputText;
    public Button convertButton;
    public Button copyButton; // opcional: copia al portapapeles

    [Header("Mapas de emojis")]
    public Dictionary<char, string> asciiEmojis = new Dictionary<char, string>()
    {
        {'A', ":D"}, {'B', ":)"},
        {'C', ":P"}, {'D', ":("}, {'E', ";)"}, {'F', "xD"},
        {'G', "^-^"}, {'H', ":3"}, {'I', ":O"}, {'J', "O:)"}, {'K', ">:)"},
        {'L', ":'D"}, {'M', ":|"}, {'N', "UwU"}, {'O', "OwO"},
        {'P', ":]"}, {'Q', ">:("}, {'R', ":<"}, {'S', ":>"},
        {'T', "--"}, {'U', "^^"}, {'V', "o_o"}, {'W', ":v"},
        {'X', "x_x"}, {'Y', ":*"}, {'Z', "<3"},
        {'0', "(°o°)"}, {'1', "(•‿•)"}, {'2', "(¬‿¬)"}, {'3', "(☉‿☉)"},
        {'4', "(ಠ_ಠ)"}, {'5', "(ʘ‿ʘ)"}, {'6', "(◕‿◕)"}, {'7', "(>‿<)"},
        {'8', "(ʕ•ᴥ•ʔ)"}, {'9', "(✿◠‿◠)"}
    };

    [Header("Filtros (configura desde el Inspector)")]
    [Tooltip("Quitar tildes/diacríticos (á -> a)")]
    public bool removeAccents = true;
    [Tooltip("Forzar mayúsculas antes de convertir (A, B, C...)")]
    public bool forceUppercase = true;
    [Tooltip("Permitir dígitos (0-9) en la conversión")]
    public bool allowDigits = true;
    [Tooltip("Eliminar puntuación / símbolos (ej: @, #, $, %, &, etc.)")]
    public bool removePunctuation = true;
    [Tooltip("Colapsar múltiples espacios en uno solo")]
    public bool collapseMultipleSpaces = true;
    [Tooltip("Reemplazo para espacios (por defecto: \"/\")")]
    public string spaceToken = "/";

    void Start()
    {
        if (convertButton != null)
            convertButton.onClick.AddListener(ConvertirAEmojis);

    }

    public void ConvertirAEmojis()
    {
        string raw = inputField != null ? inputField.text : "";
        string textoFiltrado = FiltrarTexto(raw);
        string resultado = ConvertirTextoConMapa(textoFiltrado, asciiEmojis);
        if (outputText != null) outputText.text = resultado.Trim();
    }

    public string AplicarFiltros(string textoOriginal)
    {
        string filtrado = FiltrarTexto(textoOriginal);
        string resultado = ConvertirTextoConMapa(filtrado, asciiEmojis);
        return resultado;
    }

    private string ConvertirTextoConMapa(string texto, Dictionary<char, string> mapa)
    {
        StringBuilder sb = new StringBuilder();

        foreach (char c in texto)
        {
            if (mapa.ContainsKey(c))
                sb.Append(mapa[c]).Append(' ');
            else if (char.IsWhiteSpace(c))
                sb.Append(spaceToken).Append(' ');
            else
                sb.Append("# ").Append(' '); // caracter no mapeado
        }

        return sb.ToString().Trim();
    }

    private string FiltrarTexto(string texto)
    {
        if (string.IsNullOrEmpty(texto)) return "";

        if (removeAccents)
            texto = RemoveDiacritics(texto);

        if (forceUppercase)
            texto = texto.ToUpperInvariant();

        StringBuilder sb = new StringBuilder(texto.Length);
        foreach (char ch in texto)
        {
            if (char.IsWhiteSpace(ch))
            {
                sb.Append(' ');
                continue;
            }

            if (char.IsLetter(ch))
            {
                sb.Append(ch);
                continue;
            }

            if (char.IsDigit(ch))
            {
                if (allowDigits)
                    sb.Append(ch);
                else
                    continue;
            }

            if (!removePunctuation)
                sb.Append(ch);
        }

        string result = sb.ToString();

        if (collapseMultipleSpaces)
        {
            string[] parts = result.Split(new char[] { ' ' }, System.StringSplitOptions.RemoveEmptyEntries);
            result = string.Join(" ", parts);
        }

        return result;
    }

    private string RemoveDiacritics(string text)
    {
        if (string.IsNullOrEmpty(text)) return text;
        string normalized = text.Normalize(NormalizationForm.FormD);
        StringBuilder sb = new StringBuilder();
        foreach (char c in normalized)
        {
            var uc = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
            if (uc != UnicodeCategory.NonSpacingMark)
                sb.Append(c);
        }
        return sb.ToString().Normalize(NormalizationForm.FormC);
    }

}