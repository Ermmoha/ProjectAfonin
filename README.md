# ProjectAfonin – Обувной магазин

## Что внутри
- `db/schema.sql` – схема БД (3НФ, PK/FK, справочники)
- `db/seed.sql` – данные для загрузки
- `db/ERD.pdf` – ER-диаграмма
- `tools/generate_seed.ps1` – генерация `seed.sql` из `import/*.xlsx`
- `tools/generate_erd.ps1` – генерация ERD PDF
- `ShoeStore/` – приложение Windows Forms

## Создание БД
Пример для SQL Server (LocalDB):
1. Создайте базу `ShoeStore`.
2. Выполните `db/schema.sql`.
3. Выполните `db/seed.sql`.
Или выполните автоматизированный скрипт:
`powershell -ExecutionPolicy Bypass -File tools\\setup-db.ps1`

## Настройка приложения
- Подключение настраивается в `ShoeStore/Db.cs` (строка `ConnectionString`).
- Иконка и изображения лежат в `ShoeStore/Assets`.

## Логины
Пароли и логины импортируются из `import/user_import.xlsx` в таблицу `Users`.

## Примечание по NuGet
Проект использует пакет `Microsoft.Data.SqlClient`. Для сборки нужен `dotnet restore` при наличии доступа к NuGet.
