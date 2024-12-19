# Установка ef tools
### Последняя
```dotnet tool install --global dotnet-ef```
### Конкретной версии
```dotnet tool install --global dotnet-ef --version 8.0.11```
# Добавление новой миграции
Запускать из директории конкретного проекта
```dotnet ef migrations add MigrationName --context TestDatabaseContext```
## Билд бандла
Запускать из директории решения
```dotnet ef migrations bundle --force -p .\EFDM.Sample.Npgs\EFDM.Sample.Npgs.csproj --output .\EFDM.Sample.Npgs\efbundle.exe```
```dotnet ef migrations bundle --force -p .\EFDM.Sample.Mssql\EFDM.Sample.Mssql.csproj --output .\EFDM.Sample.Mssql\efbundle.exe```
# Запуск бандла
В директории запуска бандла должен быть appsetting с ConnectionString, либо указывать в параметрах запуска
```.\efbundle.exe```
# Откат
### Удалить последнюю миграцию из проекта
```dotnet ef migrations remove --context TestDatabaseContext```
### Откатить БД к 0 миграции
```dotnet ef database update 0 --context TestDatabaseContext```
