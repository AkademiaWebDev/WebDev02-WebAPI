# Krok 1 - nowy projekt

 - Stwórz nowy katalog dla projektu 
 - Wejdź do katalogu 
 - Stwórz projekt WebAPI wpisz w konsole `dotnet new webapi`

 # Krok 2  - dodanie swaggera

 - Dodaj paczkę nugeta dla swaggera. Wpisz w konsole `dotnet add package Swashbuckle.AspNetCore --version 2.2.0`
 - Zarejestruj service swaggera. Dodaj w `Startup.cs` w **ConfigureServices**  
 ```
 services.AddSwaggerGen(c => c.SwaggerDoc("v1", new Info{ Title = "Warsaw Stops API", Version = "v1" }));
 ```  
 - Skonfiguruj swaggera. Dodaj w `Startup.cs` w **Configure**.    
 ```app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Warsaw Stops API"));
```

 - Zobacz rezultat: http://localhost:5000/swagger

 # Krok 3 konfiguracja Entity Framework oraz stworzenie bazy danych
  - Stwórz folder **Models** w głównym katalogu aplikacji
  - W folderze **Models** stówrz nową klase **Stop** w pliku `Stop.cs`  
  ```
    public class Stop
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public string Direction { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public double Lat { get; set; }
        public double Long { get; set; }
    }
```
 - Dodaje Entity Framework Core Sqlite do projektu. Wpisz w konsole: 
 ```
 dotnet add package Microsoft.EntityFrameworkCore.Sqlite --version 2.0.2
 ```
 - Wykonaj:
 ```
dotnet restore
 ```

 - Stwórz DbContext. Dodaj w głównym katalogu aplikacji plik `WarsawDbContext.cs` a w nim klasę **WarsawDbContext**, która dziedziczy po **DbContext**.

```
    public class WarsawDbContext : DbContext
    {
        public WarsawDbContext(DbContextOptions<WarsawDbContext> options) : base(options)
        {
        }

        public DbSet<Stop> Stops { get; set; }
    }
```
 - Dodaj ConnectionString dla Sqlite w `appsettings.json`
 ```
   "ConnectionStrings": {
    "WarsawDbConnection": "Data Source=WarsawDB.db"
  }
```
 - Zarejestruj WarsawDbContext w kontenerze DI oraz przekaz connections string. W pliku `Startup.cs` w **ConfigureServices** dodaj:
```
services.AddDbContext<WarsawDbContext>(options => options.UseSqlite(Configuration.GetConnectionString("WarsawDbConnection")));       
```
 - Dodaj CLI Entity Framework aby móc dodawać migracje. W pliku `WarsawCore.csproj` w sekcji "ItemGroup" dodaj:
 ```
 <DotNetCliToolReference Include="Microsoft.EntityFrameworkCore.Tools.DotNet" Version="2.0.0" />
 ```
 - Wykonaj:
 ```
dotnet restore
 ```
  - Dodaj migracje tworzącą tabelę Stops w bazie danych. Wykonaj:
  ```
dotnet ef migrations add "Create Stop table"
  ```
 - Stwórz bazę danych. Wykonaj: 
 ```
dotnet ef database update
 ```
- EF oraz baza są gotowe do uzycia
