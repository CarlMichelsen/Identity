## Testing
### Useful date strings
```bash
2025-10-18T15:00:00.00Z
```


## Development environment
Create this folder or modify the db docker run command:
```bash
C:\DockerVolumes\Database
```

Database container start command:
```bash
docker run -d --name postgres-db -e POSTGRES_USER=admin -e POSTGRES_PASSWORD=admin123 -e POSTGRES_DB=mydatabase -v C:/DockerVolumes/Database/postgres-data:/var/lib/postgresql/data -p 5432:5432 postgres:17
````

## Entity framework commands

### install entity framework cli tool
```bash
dotnet tool install --global dotnet-ef
```
or
```bash
dotnet tool update --global dotnet-ef
```

### create migration if there are changes to the database
```bash
dotnet ef migrations add InitialCreateIdentity --project ./App
```

### update database with latest migration
```bash
dotnet ef database update --project ./App
```
