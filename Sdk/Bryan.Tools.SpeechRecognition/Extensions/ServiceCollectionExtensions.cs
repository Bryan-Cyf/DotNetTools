using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Tools.SpeechRecognition;

public static class ServiceCollectionExtensions
{
    public static void AddSpeechRecognition(this IServiceCollection services, IConfiguration configuration, Action<SpeechRecognitionOptions> configure = null)
    {
        services.AddOptions<SpeechRecognitionOptions>()
            .Bind(configuration.GetSection(SpeechRecognitionOptions.SectionName))
            .ValidateDataAnnotations();

        services.PostConfigure<SpeechRecognitionOptions>(x =>
        {
            configure?.Invoke(x);
        });

        services.AddTransient<ISpeechRecognition, SpeechRecognitionService>();
    }
}
