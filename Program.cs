//using Hangfire;
//using EmailBackgroundJobDemo.Services;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Hosting;
//using Microsoft.OpenApi.Models;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddControllers();

//// Add Swagger generation
//builder.Services.AddSwaggerGen(c =>
//{
//    c.SwaggerDoc("v1", new OpenApiInfo { Title = "EmailBackgroundJobDemo API", Version = "v1" });
//});

//builder.Services.AddHangfire(config =>
//    config.UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection"))
//);
//builder.Services.AddHangfireServer();

//// Register EmailService and EmailBackgroundJob
//builder.Services.AddScoped<IEmailService, EmailService>();
//builder.Services.AddScoped<EmailBackgroundJob>();

//var app = builder.Build();

//// Trigger email in background job when the app starts
//// Trigger email in background job when the app starts
//app.Lifetime.ApplicationStarted.Register(() =>
//{
//    // Create a scope to resolve scoped services
//    using (var scope = app.Services.CreateScope())
//    {
//        var backgroundJobClient = scope.ServiceProvider.GetRequiredService<IBackgroundJobClient>();
//        var emailBackgroundJob = scope.ServiceProvider.GetRequiredService<EmailBackgroundJob>();

//        var emailRequest = new EmailRequest
//        {
//            To = "itidolmail@gmail.com",
//            Subject = "Welcome Email",
//            Body = "<h1>Welcome to Our Service!</h1>"
//        };

//        backgroundJobClient.Schedule(() => emailBackgroundJob.SendEmailJob(emailRequest.To, emailRequest.Subject, emailRequest.Body), TimeSpan.FromMinutes(1));

//        Console.WriteLine("Email job has been scheduled to send after 1 minute.");
//    }
//});

//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();  
//    app.UseSwaggerUI(c =>
//    {
//        c.SwaggerEndpoint("/swagger/v1/swagger.json", "EmailBackgroundJobDemo API v1");
//    });
//}

//app.UseAuthorization();

//app.UseHangfireDashboard("/hangfire");

//app.MapControllers();

//app.Run();


using Hangfire;
using EmailBackgroundJobDemo.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Add Swagger generation
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "EmailBackgroundJobDemo API", Version = "v1" });
});

builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(builder.Configuration.GetConnectionString("HangfireConnection"))
);
builder.Services.AddHangfireServer();

// Register EmailService and EmailBackgroundJob
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<EmailBackgroundJob>();

var app = builder.Build();

// Trigger email in background job when the app starts
app.Lifetime.ApplicationStarted.Register(() =>
{
    // Create a scope to resolve scoped services
    using (var scope = app.Services.CreateScope())
    {
        var backgroundJobClient = scope.ServiceProvider.GetRequiredService<IBackgroundJobClient>();
        var emailBackgroundJob = scope.ServiceProvider.GetRequiredService<EmailBackgroundJob>();

        // Define the email request
        var emailRequest = new EmailRequest
        {
            To = "itidolmail@gmail.com",
            Subject = "Welcome Email",
            Body = "<h1>Welcome to Our Service!</h1>"
        };
        ////Send Mail in 30 Sec after Execution
        //backgroundJobClient.Schedule(() => emailBackgroundJob.SendEmailJob(emailRequest.To, emailRequest.Subject, emailRequest.Body), TimeSpan.FromSeconds(30));

        //Send Mail At every 1 Min
        RecurringJob.AddOrUpdate(
            () => emailBackgroundJob.SendEmailJob(emailRequest.To, emailRequest.Subject, emailRequest.Body),
            Cron.MinuteInterval(1) 
        );

        ////Send Mail At Specific Time
        //DateTime specificTime = DateTime.Today.AddHours(14).AddMinutes(30); // For 2:30 PM today
        //backgroundJobClient.Schedule(() => emailBackgroundJob.SendEmailJob(emailRequest.To, emailRequest.Subject, emailRequest.Body), specificTime);


        Console.WriteLine("Email job is now scheduled to send every 1 minute.");
    }
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "EmailBackgroundJobDemo API v1");
    });
}

app.UseAuthorization();

app.UseHangfireDashboard("/hangfire");

app.MapControllers();

app.Run();
