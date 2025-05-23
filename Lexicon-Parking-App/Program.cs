using Lexicon_Parking_App;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<PersistanceContext>(options =>
    options.UseSqlite("DataSource=persistance.db"));

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

app.MapPost("/start-session", ([FromBody] int userID) =>
{
    try
    {
        Period? activatedPeriod = backend.StartPeriod(userID);

        if (activatedPeriod == null)
        {
            return Results.Conflict(new
            {
                message = backend.startPeriodMessage
            });
        }
        else
        {
            return Results.Ok(new
            {
                message = backend.startPeriodMessage,
                startTime = activatedPeriod.StartTime
            });
        }
    }
    catch
    {
        return Results.BadRequest(new
        {
            message = "userID was invalid"
        });
    }
});

app.MapPost("/end-session", ([FromBody] int userID) =>
{
    try
    {
        Period endedPeriod = backend.EndPeriod(userID);

        if (endedPeriod == null)
        {
            return Results.Conflict(new
            {
                message = backend.endPeriodMessage
            });
        }
        else
        {
            return Results.Ok(new
            {
                message = backend.endPeriodMessage,
            });
        }
    }
    catch
    {
        return Results.BadRequest(new
        {
            message = "userID was invalid"
        });
    }
});

app.MapGet("/current-session/{userID}", (int userID) =>
{
    Period? currentPeriod = backend.GetSession(userID);

    if(currentPeriod == null)
    {
        return Results.Ok(new
        {
            message = backend.currentPeriodMessage,
            startTime = "No active session",
            cost = "",
            isActive = false
        });
    }
    else
    {
        currentPeriod.PeriodCost = backend.CalculateCost(currentPeriod);

        return Results.Ok(new
        {
            message = backend.currentPeriodMessage,
            startTime = currentPeriod.StartTime,
            cost = currentPeriod.PeriodCost,
            isActive = true
        });
    }
});

app.MapGet("/previous-sessions/{userID}", (int userID) =>
{
    List<Period>? previousPeriods = backend.GetPreviousSessions(userID);

    return Results.Ok(new
    {
        message = $"The previous session for {userID}",
        previousSession = previousPeriods
    });
});

app.MapPost("/login", (LoginData loginData) =>
{
    User user = backend.Login(loginData.Username, loginData.Password);

    if (user != null)
    {
        return Results.Ok(new
        {
            message = backend.loginMessage,
            user = user
        });
    }
    else
    {
        return Results.BadRequest(new
        {
            message = backend.loginMessage
        });
    }
});

app.MapPost("/register-user", (User user) =>
{
    bool successful = backend.RegisterNewUser(user);

    if (successful)
    {
        return Results.Ok(new
        {
            message = backend.registerMessage
        });
    }
    else
    {
        return Results.Conflict(new
        {
            message = backend.registerMessage
        });
    }
});

app.MapGet("/user-balance/{userID}", (int userID) =>
{
    decimal? balance = backend.UserBalance(userID);

    if (balance == null)
    {
        return Results.Conflict(new
        {
            message = backend.userBalanceMessage
        });
    }
    else
    {
        return Results.Ok(new
        {
            message = backend.userBalanceMessage,
            totalBalance = balance
        });
    }
});

app.MapGet("/user-details", (int userID) =>
{
    User user = backend.UserDetails(userID);

    if (user != null)
    {
        return Results.Ok(new
        {
            message = "User details retrieved successfully",
            fullname = user.Firstname + " " + user.Lastname,
            licenseplate = user.Licenseplate,
            balance = user.Balance
        });
    }
    else
    {
        return Results.BadRequest(new
        {
            message = "User not found."
        });
    }
});



app.Run();