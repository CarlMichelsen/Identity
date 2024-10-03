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
dotnet ef migrations add InitialCreate --project App
```

### update database with latest migration
```bash
dotnet ef database update --project App
```