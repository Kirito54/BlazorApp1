# BlazorApp1 Telegram Bot

This ASP.NET server hosts a Telegram bot that accepts Excel files.

## Running

1. Obtain a Telegram bot token from [@BotFather](https://t.me/BotFather).
2. Provide the token using either the `TELEGRAM_BOT_TOKEN` environment variable
   or by placing it in `appsettings.json` under `TelegramBot:Token`.
3. Build and run the server:

```bash
dotnet run --project BlazorApp1/BlazorApp1 -c Release
```

When you send an Excel file to the bot, it checks the file for personal data patterns and responds with a confirmation or an error message.

## Optimizations

The server uses response compression and ships only minified client assets to reduce bandwidth usage.

When you send an Excel file to the bot, it checks the file for personal data patterns and responds with a confirmation or an error message. The last valid file is stored on disk and loaded on server start so searches continue to work after a restart.

## Docker deployment with HTTPS

A `Dockerfile` and `docker-compose.yml` are provided to run the application with
automatic HTTPS certificates from Let's Encrypt. Set the `VIRTUAL_HOST`,
`LETSENCRYPT_HOST` and `LETSENCRYPT_EMAIL` environment variables in
`docker-compose.yml` to your domain and email before starting:

```bash
docker compose up -d
```

The `nginx-proxy` and `acme-companion` containers handle HTTPS termination and
certificate renewal. The application itself listens on port 80 inside the Docker
network.
