using FluentValidation;
using FluentValidation.AspNetCore;
using Hazparo.Application.User;
using Microsoft.Extensions.DependencyInjection;

namespace Hazparo.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static void AddApplication(this IServiceCollection services)
    {
        var applicationAssembly = typeof(ServiceCollectionExtensions).Assembly;

        services.AddMediatR(cfg=>
        {
            cfg.LicenseKey = "eyJhbGciOiJSUzI1NiIsImtpZCI6Ikx1Y2t5UGVubnlTb2Z0d2FyZUxpY2Vuc2VLZXkvYmJiMTNhY2I1OTkwNGQ4OWI0Y2IxYzg1ZjA4OGNjZjkiLCJ0eXAiOiJKV1QifQ.eyJpc3MiOiJodHRwczovL2x1Y2t5cGVubnlzb2Z0d2FyZS5jb20iLCJhdWQiOiJMdWNreVBlbm55U29mdHdhcmUiLCJleHAiOiIxODE2ODE5MjAwIiwiaWF0IjoiMTc4NTMzNzg4MyIsImFjY291bnRfaWQiOiIwMTlmYWU2ZDQzMzE3YjA0YjAxOGZmZTY0YmQwYWM4ZSIsImN1c3RvbWVyX2lkIjoiMDE5ZmFlNmQ0MzMxN2IwNGIwMThmZmU2NGJkMGFjOGUiLCJzdWJfaWQiOiItIiwiZWRpdGlvbiI6IjAiLCJ0eXBlIjoiMiJ9.DYWAljOmdNzHBEoS90WYh9kvRAjYrjh2yfgP33SQAFGs2TqYwU22Ya7ea1QAro8KYticm8Kp3HuItrx8r4a9LRY-x4Bh3UDkuNqLMG9FBJdw1HH0QKp3WFwvjkVcZ9c3gvyejDnHAK6eN4XFs6nC-8aevBYoCMcdj2FRz6L5vi-cF5_KSerhKWVox9d56Y97KyCSKg7goucQWOY_6s6UxRb9q0PMFRVYJeQVy8UBKwumiR6O9KOwvblL8PBsSF8P--xVHUIKDMk9EpP3u_Mlq26I0gfMIy1vu6UnUrAzT3voH5yZOJJhuA3yRGRlfUEC9Z0pwYFZLNF8hqmfkV0JXQ";
            cfg.RegisterServicesFromAssembly(applicationAssembly);
        });

        services.AddAutoMapper(cfg => { }, applicationAssembly);
        services.AddValidatorsFromAssembly(applicationAssembly)
                    .AddFluentValidationAutoValidation();

        services.AddScoped<IUserContext, UserContext>();
        services.AddHttpContextAccessor();
    }
}