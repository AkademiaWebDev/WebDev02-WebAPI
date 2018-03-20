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