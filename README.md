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

# Krok 4 dodanie repozytorium

W tym kroku wprowadzimy, juz trochę logiki do naszej aplikacji. Z frontendem ustaliśmy jak będzie działać aplikacja.
Strona główna to tabela wszystkich przystanków z paginacją oraz wyszukiwarką. Wyszukiwarka wyszukuje po nazwie przystanku. Wynik wyszukiwania zawęża tabelę poniżej. W tabelki nie są widoczne wszystkie dane. Tylko, te które pozwalają zidentyfikać przystanek. Reszta danych dostępna jest w widoku szczegółowym.

Oprócz tego użytkownik powinień mieć możliwość dodawania, usuwania oraz modyfikowania przystanku.

Dzięki tym wymaganiom możemy teraz zaprojektować repozytorium, czyli warstwę danych.

 - Stwórz folder **Repositories** w głównym katalogu projektu. A w nim interfejs `IStopsRepository` oraz klasę `StopsRepository`

```
    public interface IStopsRepository
    {
        (IEnumerable<Stop>, int) Get(string search, int skip);
        Stop Get(int Id);
        Stop Create(Stop stop);
        Stop Update(Stop stop);
        void Delete(int id);        
    }
```
```
    public class StopsRepository : IStopsRepository
    {
        private readonly WarsawDbContext _context;
        public StopsRepository(WarsawDbContext context)
        {
            _context = context;

        }

        public void Delete(int id)
        {
            Stop stopEntity = _context.Stops.Find(id);
            _context.Stops.Remove(stopEntity);
            _context.SaveChanges();
        }

        public Stop Create(Stop stop)
        {
            _context.Stops.Add(stop);
            _context.SaveChanges();
            return stop;
        }

        public Stop Update(Stop stop)
        {
            _context.Stops.Attach(stop);
            _context.Entry(stop).State = EntityState.Modified;
            _context.SaveChanges();
            return stop;
        }

        public (IEnumerable<Stop>, int) Get(string search, int skip)
        {
            var stopsFilteredByName = search != null ? _context.Stops
                .Where(x => x.Name.ToLower()
                .Contains(search)) : _context.Stops;
             
            var stopsCount = stopsFilteredByName.Count();

            var paginatedStop = stopsFilteredByName
                .OrderBy(x => x.Id)
                .Skip(skip)
                .Take(20);

            return (paginatedStop, stopsCount);
        }

        public Stop Get(int id){
            return _context.Stops.Find(id);
        }
    }
```
 - Zrejestruj Repozytorium jako serwis. Dodaj w `Startup.cs` w **ConfigureServices**

 ```
services.AddTransient<IStopsRepository, StopsRepository>();  
 ```

# Krok 5 dodanie kontrolera WebApi
## Dodanie modeli domenowych.
 -  W katalogu **Models** Utwórz plik `CreateStopRequest.cs` a w nim klase.
 ```
    public class CreateStopRequest
    {
        public string Name { get; set; }
        public string Number { get; set; }
        public string Direction { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public double Lat { get; set; }
        public double Long { get; set; }

        public Stop GetStop()
        {
            var stop = new Stop
            {
                Name = this.Name,
                Number = this.Number,
                Direction = this.Direction,
                Street = this.Street,
                City = this.City,
                Lat = this.Lat,
                Long = this.Long
            };

            return stop;
        }
    }
 ```

  - Uwórz plik `GetStopRequest.cs` w **Models** a w nim klase
  ```
    public class GetStopRequest{
        public int? Page { get; set; } = 1;
        public string Search { get; set; }
    }
  ```

  - Utwórz plik `SearchResult.cs` w **Models** a w nim klasy
  ```
    public class SearchResult
    {
        public IEnumerable<StopResult> Items { get; set; }
        public PageInfo PageInfo { get; set; }
    }

    public class PageInfo
    {
        public int CurrentPage { get; set; }

        public int MaxPage { get; set; }
    }

    public class StopResult
    {
        public StopResult(Stop stop)
        {
            Id = stop.Id;
            Name = stop.Name;
            Number = stop.Number;
            Street = stop.Street;
            City = stop.City;
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public string Number { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        
    }   
  ```

  - Utwórz plik `StopsController.cs` w **Controllers** a w nim:
  ```
    public class StopsController : Controller
    {
        private readonly IStopsRepository repository;
        private int itemPerPage = 10;
        public StopsController(IStopsRepository repository)
        {
            this.repository = repository;

        }

        [HttpGet("{id}")]
        // GET api/stops/{id}
        public IActionResult Get(int id)
        {
            return Ok(repository.Get(id));
        }

        //GET api/stops/?search={string}&page={int}
        [HttpGet]
        public IActionResult Get([FromQuery]GetStopRequest request)
        {
            var (stops, count) = repository
                    .Get(request.Search, (request.Page.Value - 1) * itemPerPage);
            var result = new SearchResult
            {
                PageInfo = new PageInfo
                {
                    CurrentPage = request.Page.Value,
                    MaxPage = count % itemPerPage == 0 ? count / itemPerPage : count / itemPerPage + 1
                },
                Items = stops.Select(x => new StopResult(x))
            };
            return Ok(result);
        }

        // DELETE api/stops/{id}
        [HttpDelete]
        public IActionResult Delete(int id)
        {
            repository.Delete(id);
            return Ok();
        }

        //POST api/stops
        [HttpPost]
        public IActionResult Post([FromBody]CreateStopRequest createStop)
        {
            return Ok(repository.Create(createStop.GetStop()));
        }

        //POST api/stops
        [HttpPut]
        public IActionResult Put([FromBody]Stop stop)
        {
            return Ok(repository.Update(stop));
        }
    }
  ```
 - Usuń `ValuesController.cs`

 # Krok 6 dokeryzacja projektu - dla chętnych
  - Zainstaluj dockera https://docs.docker.com/docker-for-windows/install/
  - Dodaj plik w głównym katalogu projektu `Dockerfile`:
  ```
FROM microsoft/aspnetcore-build:2.0 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out
COPY WarsawDB.db ./out/

# Build runtime image
FROM microsoft/aspnetcore:2.0
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "WarsawCore.dll"]
  ```

  - Dodaj plik w głównym katalogu projektu `.dockerignore`:
  ```
bin\
obj\
  ```
 - Wykonaj aby zbudować obraz konternera
 ```
 docket build -t warsawapp .
 ```

 - Wykonaj aby stworzyć i uruchomić kontener
 ```
docker run -d -p 8080:80 --name MojaNazwaKontenera warsawapp
 ```

- Kontener został stworzony. Twoja aplikacja działa teraz na porcie 8080 w kontenerze dockerowym.

### Baza wiedzy
 - [Wyjaśnienie czym jest kontener i Docker](https://www.ratioweb.pl/pl/blog/toolkit/przygody-z-dockerem-1-instalacja-i-podstawy)  
 - [Konfiguracja kontenera z MySql](http://sebcza.pl/przydatne-narzedzia/mysql-w-kontenerze-i-zdalny-dostep-czyli-docker-w-akcji/)  
 - [Codzienna praca z kontenerami](https://www.ratioweb.pl/pl/blog/web-development-toolkit/przygody-z-dockerem-2-praca-z-kontenerami)
  - [Dokeryzacja ASP .NET Core + Sql Server on Linux](https://docs.docker.com/compose/aspnet-mssql-compose/)