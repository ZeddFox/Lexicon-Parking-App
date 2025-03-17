using Lexicon_Parking_App;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddAuthorization();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
app.UseHttpsRedirection();
app.UseAuthorization();

Backend backend = new Backend();

app.MapPost("/start-session", (int accountID) =>
{
    return Results.Json(backend.StartPeriod(accountID));
});

app.MapPost("/end-session", (int accountID) =>
{
    return Results.Json(backend.EndPeriod(accountID));
});

app.MapGet("/current-session", (int accountID) =>
{
    return Results.Json(backend.GetSession(accountID));
});

app.MapGet("/previous-sessions/{userID}", (int userID) =>
{
    return Results.Json(backend.GetPreviousSessions(userID));
});

app.MapPost("/login", (string username, string password) =>
{
    return Results.Json(backend.Login(username, password));
});

app.MapPost("/register", (User user) =>
{
    return Results.Json(backend.RegisterNewUser(user));
});

app.MapGet("/user-balance", (int accountID) =>
{
    return Results.Json(backend.AccountBalance(accountID));
});

app.MapGet("/user-details", (int accountID) =>
{
    var toReturn = backend.AccountDetails(accountID);

    if (toReturn != null)
    {
        return Results.Json(toReturn);
    }
    else
    {
        return Results.Json("Account not found.");
    }
});

app.Run();

backend.SaveAccountXML(backend.programPath + backend.accountsFilename);
backend.SavePeriodsXML(backend.programPath + backend.periodsFilename);