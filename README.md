# Secrets

### Development
Make a folder called "secrets.json" in the Application project root.
Use the git-tracked "secrets-template.json" as a guide for the secrets.json file.

### Production
Make a docker readonly file-volume that maps next to the App.dll file in the docker container.

# Entity framework cheat sheet

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
dotnet ef migrations add IdentityInitialCreate --project App
```

### update database with latest migration
```bash
dotnet ef database update --project App
```

# Docker Database
create a folder on your host pc
"C:\Users\carll\Documents\postgres-volume"

```bash
docker run -p 5432:5432 --restart unless-stopped --name postgresdb -e "POSTGRES_USER=postgres" -e "POSTGRES_PASSWORD=developer-password" -e "POSTGRES_DB=devdb" -v C:\Users\carll\Documents\postgres-volume:/var/lib/postgresql/data -d postgres
```