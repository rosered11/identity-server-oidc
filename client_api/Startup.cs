using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace client_api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "client_api", Version = "v1" });
            });

            services.AddAuthentication("Bearer")
                .AddJwtBearer("Bearer", options => {
                    options.Authority = "https://localhost:5001";
                    var logger = services.BuildServiceProvider().GetRequiredService<ILogger<JwtBearerHandler>>();
                    options.TokenValidationParameters = new TokenValidationParameters{
                        ValidateAudience = false
                    };
                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            logger.LogWarning(context.Exception, "Unauthorized request.");
                            return Task.CompletedTask;
                        },
                    };
                });

            services.AddAuthorization(options => {
                options.AddPolicy("ClientIdPolicy", policy => policy.RequireClaim("client_id", "defaultClient", "default2Client", "default3Client"));
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "client_api v1"));
            }

            //app.UseHttpsRedirection();
            // Set schema https for use https
            // app.Use((context, next) =>
            // {
            //     context.Request.Scheme = "https";
            //     return next();
            // });

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
