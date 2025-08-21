# AuthenticationSandbox

Пример ASP.NET Core API с аутентификацией через middleware и хранением пользователей в MongoDB. Поддерживает роль администратора, ограничение доступа к эндпоинтам и однократную инициализацию первого админа.

## Требования
- .NET SDK 9.0
- Запущенный MongoDB по адресу `mongodb://localhost:27017`
- API слушает `http://localhost:8080` (см. `AuthenticationSandbox/Properties/launchSettings.json`)

## Конфигурация
Настройки MongoDB находятся в `AuthenticationSandbox/appsettings.json`:
```json
{
  "Mongo": {
    "ConnectionString": "mongodb://localhost:27017",
    "Database": "AuthenticationSandbox"
  }
}
```
При необходимости измените строку подключения и/или имя базы.

## Запуск
Из корня репозитория выполните:
```bash
# восстановить зависимости
 dotnet restore

# запустить приложение
 dotnet run --project AuthenticationSandbox/AuthenticationSandbox.csproj
```
Приложение будет доступно на `http://localhost:8080`.

### Первый запуск (инициализация админа)
Если в базе ещё нет администраторов, при старте автоматически создаётся пользователь `admin`. В консоли появится его API-ключ, например:
```
admin has been created successfully, admin api key: <API_KEY>
```
Сохраните этот ключ — он нужен для админских запросов.

## Эндпоинты
Базовый URL: `http://localhost:8080`

Публичные (без `X-Api-Key`):
- `GET /api` — статус
- `GET /api/ping` — pong
- `POST /api/user` — регистрация пользователя, тело: `{ "login": "user123" }`
  - Ответ: `{ "apiKey": "..." }`
  - Логин должен соответствовать `^[a-zA-Z0-9_]{3,16}$`

Защищённые (требуется заголовок `X-Api-Key`):
- `GET /api/protected` — статус (любой авторизованный пользователь)
- `GET /api/protected/ping` — pong (любой авторизованный пользователь)
- `GET /api/user/all` — список всех пользователей (только админ)
- `POST /api/user/admin` — назначить/снять админа (только админ)
  - Тело: `{ "login": "user123", "isAdmin": true }`
  - Нельзя снять флаг у последнего админа (вернётся 409)

Коды ошибок: 400 (некорректный заголовок ключа), 401 (пользователь не найден по ключу), 403 (нет прав), 404 (пользователь не найден), 409 (конфликт — дублирование логина или попытка снять последнего админа).

## Тестирование в Postman
1. Запустите приложение и сохраните `<API_KEY_ADMIN>` из консоли при первом старте.
2. Зарегистрируйте пользователя:
```http
POST http://localhost:8080/api/user
Content-Type: application/json

{ "login": "user1" }
```
Сохраните `apiKey` из ответа.

3. Получите данные по ключу пользователя:
```http
GET http://localhost:8080/api/user
X-Api-Key: <API_KEY_USER1>
```

4. Попробуйте получить список всех пользователей обычным ключом (ожидаемо 403):
```http
GET http://localhost:8080/api/user/all
X-Api-Key: <API_KEY_USER1>
```

5. Получите список всех пользователей админским ключом (ожидаемо 200):
```http
GET http://localhost:8080/api/user/all
X-Api-Key: <API_KEY_ADMIN>
```

6. Назначьте пользователя админом (только админ):
```http
POST http://localhost:8080/api/user/admin
Content-Type: application/json
X-Api-Key: <API_KEY_ADMIN>

{ "login": "user1", "isAdmin": true }
```

7. Снимите флаг админа (разрешено, только если останется другой админ):
```http
POST http://localhost:8080/api/user/admin
Content-Type: application/json
X-Api-Key: <API_KEY_ADMIN>

{ "login": "user1", "isAdmin": false }
```
Если пытаетесь снять флаг у единственного админа, будет 409 Conflict.

8. Проверка защищённых пингов:
```http
GET http://localhost:8080/api/protected
X-Api-Key: <API_KEY_USER1>
```
```http
GET http://localhost:8080/api/protected/ping
X-Api-Key: <API_KEY_USER1>
```

## Примеры cURL
```bash
# регистрация
curl -s -X POST http://localhost:8080/api/user \
  -H "Content-Type: application/json" \
  -d '{"login":"user1"}'

# получить свои данные
curl -s http://localhost:8080/api/user -H "X-Api-Key: <API_KEY_USER1>"

# список всех пользователей (админ)
curl -s http://localhost:8080/api/user/all -H "X-Api-Key: <API_KEY_ADMIN>"

# назначить админа (админ)
curl -s -X POST http://localhost:8080/api/user/admin \
  -H "Content-Type: application/json" \
  -H "X-Api-Key: <API_KEY_ADMIN>" \
  -d '{"login":"user1","isAdmin":true}'
```

## Траблшутинг
- Убедитесь, что MongoDB запущен и доступен по `mongodb://localhost:27017`.
- При ошибке подключения проверьте `appsettings.json` и права к базе.
- Если порт 8080 занят, измените `applicationUrl` в `AuthenticationSandbox/Properties/launchSettings.json`.
