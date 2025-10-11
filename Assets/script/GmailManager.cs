using System.Net;
using System.Net.Mail;
using UnityEngine;
using TMPro;

public class GmailManager : MonoBehaviour
{
    [Header("Configuración de Gmail")]
    public string fromEmail = "tucorreo@gmail.com";
    public string password = "tu_contraseña_de_aplicacion";

    [Header("Referencias UI")]
    public TMP_InputField inputMensaje;
    public TMP_InputField inputCorreo;
    public TMP_Text outputStatus;
    public TMP_Text outputCifrado;

    [Header("Script de Encriptación")]
    public Encriptacion encriptador;

    [ContextMenu("Enviar Mensaje")]
    public void EnviarMensaje()
    {
        string correoDestino = inputCorreo.text;
        string mensajeOriginal = inputMensaje.text;

        if (string.IsNullOrEmpty(correoDestino))
        {
            outputStatus.text = "❌ Ingresa un correo de destino.";
            Debug.LogError("Correo destino vacío.");
            return;
        }
        if (string.IsNullOrEmpty(mensajeOriginal))
        {
            outputStatus.text = "⚠️ Escribe un mensaje antes de enviarlo.";
            Debug.LogWarning("Mensaje vacío.");
            return;
        }

        string textoCifrado = encriptador.AplicarFiltros(mensajeOriginal);

        if (outputCifrado != null)
            outputCifrado.text = textoCifrado;

        SendEmail(correoDestino, "Mensaje Cifrado", textoCifrado);
    }

    private void SendEmail(string destinatario, string asunto, string cuerpo)
    {
        MailMessage mail = new MailMessage();
        mail.From = new MailAddress(fromEmail);
        mail.To.Add(destinatario);
        mail.Subject = asunto;
        mail.Body = cuerpo;

        SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
        smtpServer.Port = 587;
        smtpServer.Credentials = new NetworkCredential(fromEmail, password) as ICredentialsByHost;
        smtpServer.EnableSsl = true;

        try
        {
            smtpServer.Send(mail);
            Debug.Log("Correo enviado exitosamente.");
            if (outputStatus != null)
                outputStatus.text = "Correo enviado exitosamente.";
        }
        catch (System.Exception e)
        {
            Debug.LogError("❌ Error al enviar el correo: " + e.Message);
            if (outputStatus != null)
                outputStatus.text = "Error: " + e.Message;
        }
    }
}