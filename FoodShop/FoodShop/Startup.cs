using FoodShop.CognitiveServices.Core;
using FoodShop.Domain;
using FoodShopBot;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Connector.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FoodShop
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient().AddControllers().AddNewtonsoftJson();
            
            services.AddTransient<INaturalLanguageUnderstandingService, NaturalLanguageUnderstandingService>();

            services.AddSingleton<IStorage, MemoryStorage>();
            services.AddSingleton<ConversationState>();

            services.AddBot<ChatBot>(options =>
           {
               options.CredentialProvider = new SimpleCredentialProvider(Configuration.GetSection("Bot:AppId")?.Value, Configuration.GetSection("Bot:AppPassword")?.Value);

               options.OnTurnError = async (context, exception) =>
               {
                   await context.SendActivityAsync("Error happens! Please try again");
               };
           });

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseDefaultFiles()
                .UseStaticFiles()
                .UseWebSockets()
                .UseRouting()
                .UseAuthorization()
                .UseEndpoints(endpoints =>
                {
                    endpoints.MapControllers();
                })
                .UseBotFramework();

        }
    }
}
