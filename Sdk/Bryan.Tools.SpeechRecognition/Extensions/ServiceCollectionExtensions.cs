using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tools.SpeechRecognition;

public static class ServiceCollectionExtensions
{
    public static void AddSpeechRecognition(this IServiceCollection services, string sectionName = SpeechRecognitionOptions.SectionName, Action<SpeechRecognitionOptions> configure = null)
    {
        using ServiceProvider provider = services.BuildServiceProvider();
        IConfigurationSection section = (provider.GetRequiredService<IConfiguration>() ?? throw new ArgumentNullException("IConfiguration")).GetSection(sectionName);
        if (!section.Exists())
        {
            throw new Exception("Config file not exist '" + sectionName + "' section.");
        }
        SpeechRecognitionOptions option = section.Get<SpeechRecognitionOptions>();
        if (option == null)
        {
            throw new Exception($"Get SpeechRecognition option from config file failed.");
        }

        services.AddOptions<SpeechRecognitionOptions>()
            .Bind(section)
            .ValidateDataAnnotations();

        services.PostConfigure<SpeechRecognitionOptions>(x =>
        {
            configure?.Invoke(x);
        });

        services.AddTransient<ISpeechRecognition, SpeechRecognitionService>();
    }
}
