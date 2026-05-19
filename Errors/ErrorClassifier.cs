namespace VidDrop.Errors;

public static class ErrorClassifier
{
    public static DownloadException Classify(string ytDlpOutput)
    {
        if (ytDlpOutput.Contains("Unsupported URL") || ytDlpOutput.Contains("is not a valid URL"))
            return new DownloadException(ErrorCategory.UnsupportedUrl,
                "URL não suportada. Verifique o link e tente novamente.");

        if (ytDlpOutput.Contains("login required") || ytDlpOutput.Contains("Login Required") ||
            ytDlpOutput.Contains("Please log in") || ytDlpOutput.Contains("authentication required"))
            return new DownloadException(ErrorCategory.DownloadFailed,
                "Este conteúdo requer login. Conteúdo privado não é suportado.");

        if (ytDlpOutput.Contains("age") && (ytDlpOutput.Contains("restricted") || ytDlpOutput.Contains("verification")))
            return new DownloadException(ErrorCategory.DownloadFailed,
                "Vídeo com restrição de idade — não é possível baixar sem login.");

        if (ytDlpOutput.Contains("geographic") || ytDlpOutput.Contains("not available in your country") ||
            ytDlpOutput.Contains("blocked"))
            return new DownloadException(ErrorCategory.DownloadFailed,
                "Vídeo não disponível na sua região.");

        if (ytDlpOutput.Contains("Video unavailable") || ytDlpOutput.Contains("Private video") ||
            ytDlpOutput.Contains("has been removed") || ytDlpOutput.Contains("no longer available"))
            return new DownloadException(ErrorCategory.DownloadFailed,
                "Vídeo indisponível ou privado.");

        if (ytDlpOutput.Contains("Unable to download") || ytDlpOutput.Contains("Connection") ||
            ytDlpOutput.Contains("HTTP Error") || ytDlpOutput.Contains("Network"))
            return new DownloadException(ErrorCategory.NetworkError,
                "Erro de conexão. Verifique sua internet e tente novamente.");

        return new DownloadException(ErrorCategory.Unknown,
            "Não foi possível baixar. Tente novamente.");
    }
}
