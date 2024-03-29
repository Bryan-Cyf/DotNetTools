var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;
config.AddConfigurationFolder();
//config.AddApolloCenter();
//config.AddNacos();
config.AddSnowFlakeId();

builder.Services.AddServiceAccessor();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddException();
//builder.Services.AddMongoDb();
//builder.Services.AddElastic();
builder.Services.AddSwagger();
builder.Services.AddSpeechRecognition();
//builder.Services.AddHangfire(config);
//builder.Services.AddCaching();

var app = builder.Build();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
