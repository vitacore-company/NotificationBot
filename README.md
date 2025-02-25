# NotaficationBot
## Что это и для чего?
Это телеграмм бот для уведомлений о событиях в Azure DevOps 2022
## Запуск
### Docker compose
1. Требуется настроить следующие переменные среды
- BotToken
- POSTGRES_DB
- POSTGRES_USER
- POSTGRES_PASSWORD
- SUBNET
- POSTGRES_PORTS
- POSTGRES_EXPOSE_PORTS
- ASPNETCORE_URLS
- PGADMIN_DEFAULT_EMAIL
- PGADMIN_DEFAULT_PASSWORD
- PGADMIN_LISTEN_PORT
- PGADMIN_PORTS

1.А Или отредактировать файл .env

2. Затем выполнить следующие команды
- ```docker compose build``` (Сборка)
- ```docker compose up``` (Запуск)

ГОТОВО 