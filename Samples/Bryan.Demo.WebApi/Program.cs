var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;
config.AddConfigurationFolder();
//config.AddApolloCenter();
//config.AddNacos();
config.AddSnowFlakeId();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddException();
//builder.Services.AddMongoDb(config);
//builder.Services.AddElastic(config);
builder.Services.AddSwagger(config);
builder.Services.AddSpeechRecognition(config);
//builder.Services.AddHangfire(config);
//builder.Services.AddCaching(config);

var app = builder.Build();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
