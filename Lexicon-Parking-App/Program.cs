var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.MapPost("/start", (int accountID, string licensePlate) =>
{
    return "Begin a new period";
});

app.MapPost("/end", (int accountID, string licensePlate) =>
{
    return "End current period";
});

app.MapGet("/current", (int accountID) =>
{
    return "Get current period";
});

app.MapPost("/register", (string username, string firstname, string lastname, string licensePlate) =>
{
    return "Register user + car";
});

app.MapGet("/accountbalance", (int accountID) =>
{
    return "Get balance on account";
});

app.MapGet("/accountdetails", (int accountID) =>
{
    return "Get all user details";
});


app.Run();
