using Telegram.Bot;
using Telegram.Bot.Types;
using Microsoft.Extensions.Hosting;

namespace BlazorApp1.Services;

public class TelegramBotService : BackgroundService
{
    private readonly IConfiguration _config;
    private readonly ExcelDataService _excel;
    private readonly ILogger<TelegramBotService> _logger;
    private TelegramBotClient? _client;

    public TelegramBotService(IConfiguration config, ExcelDataService excel, ILogger<TelegramBotService> logger)
    {
        _config = config;
        _excel = excel;
        _logger = logger;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var token = _config["TelegramBot:Token"]
                    ?? _config["TELEGRAM_BOT_TOKEN"]
                    ?? Environment.GetEnvironmentVariable("TELEGRAM_BOT_TOKEN");
        if (string.IsNullOrWhiteSpace(token))
        {
            _logger.LogInformation("Telegram bot token not provided; bot disabled");
            return Task.CompletedTask;
        }

        _client = new TelegramBotClient(token);
        _client.StartReceiving(HandleUpdateAsync, HandleErrorAsync, cancellationToken: stoppingToken);
        _logger.LogInformation("Telegram bot started");
        return Task.CompletedTask;
    }

    private async Task HandleUpdateAsync(ITelegramBotClient bot, Update update, CancellationToken ct)
    {
        if (update.Message is not { } message)
            return;
        if (message.Chat.Id.ToString() == "369826359")
        {
            if (message.Document != null)
            {
                var file = await bot.GetFile(message.Document.FileId, ct);
                using var ms = new MemoryStream();
                await bot.DownloadFile(file.FilePath!, ms, ct);
                ms.Position = 0;
                var ok = await _excel.LoadAsync(ms);
                var reply = ok ? "Файл принят" : "Ошибка: файл содержит персональные данные";
                await bot.SendMessage(message.Chat, reply, cancellationToken: ct);
            }
            else
            {
                await bot.SendMessage(message.Chat, "Пришлите Excel файл  в формате:Номер очереди, Входящий номер, Номер распоряжения. Без знака №", cancellationToken: ct);
            }
        }
    }

    private Task HandleErrorAsync(ITelegramBotClient bot, Exception exception, CancellationToken ct)
    {
        _logger.LogError(exception, "Telegram bot error");
        return Task.CompletedTask;
    }
}
