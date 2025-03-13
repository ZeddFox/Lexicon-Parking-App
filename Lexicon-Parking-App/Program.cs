using Lexicon_Parking_App;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthorization();
builder.Services.AddSwaggerGen();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

Backend backend = new Backend();

app.MapPost("/start", (int accountID, string licenseplate) =>
{
    return Results.Json(backend.StartPeriod(accountID, licenseplate));
});

app.MapPost("/end", (int accountID, string licenseplate) =>
{
    return Results.Json(backend.EndPeriod(accountID, licenseplate));
});

app.MapGet("/current", (int accountID, string licenseplate) =>
{
    return Results.Json(backend.GetSession(accountID, licenseplate));
});

app.MapGet("/current", (string username, string password) =>
{
    return Results.Json(backend.Login(username, password));
});

app.MapPost("/register", (string username, string password, string firstname, string lastname, string licenseplate) =>
{
    return Results.Json(backend.RegisterNewUser(username, password, firstname, lastname, licenseplate));
});

app.MapGet("/accountbalance", (int accountID) =>
{
    return Results.Json(backend.AccountBalance(accountID));
});

app.MapGet("/accountdetails", (int accountID) =>
{
    return Results.Json(backend.AccountDetails(accountID));
});

app.Run();
