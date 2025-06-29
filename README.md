# BlazorApp1 Telegram Bot

This ASP.NET server hosts a Telegram bot that accepts Excel files.

## Running

1. Obtain a Telegram bot token from [@BotFather](https://t.me/BotFather).
2. Set the environment variable `TELEGRAM_BOT_TOKEN` with the token value.
3. Build and run the server:

```bash
dotnet run --project BlazorApp1/BlazorApp1 -c Release
```

When you send an Excel file to the bot, it checks the file for personal data patterns and responds with a confirmation or an error message.
