using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace identity_server_oidc
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddIdentityServer()
            .AddInMemoryClients(Config.Clients)
            .AddInMemoryIdentityResources(Config.IdentityResources)
            // .AddInMemoryApiResources(Config.ApiResources)
            .AddInMemoryApiScopes(Config.ApiScopes)
            .AddTestUsers(Config.TestUsers)
            .AddDeveloperSigningCredential();

            //services.AddControllers();
            services.AddControllersWithViews();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "identityserver", Version = "v1" });
            });

            // services.AddAuthentication("Bearer")
            //     .AddJwtBearer("Bearer", options => {
            //         options.Authority = "https://localhost:5001";
            //         options.TokenValidationParameters = new TokenValidationParameters{
            //             ValidateAudience = false
            //         };
            //     });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "identityserver v1"));
            }
            
            app.UseStaticFiles();

            // set schema for https if use https
            // app.Use((context, next) =>
            // {
            //     context.Request.Scheme = "https";
            //     return next();
            // });

            app.UseRouting();

            app.UseIdentityServer();
            //app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                //endpoints.MapControllers();
                endpoints.MapDefaultControllerRoute();
            });
        }
    }
}
