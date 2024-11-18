namespace UShare
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();

            // Add CORS services
            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAnyOrigin", policy =>
                {
                    // Allow any origin to access the API
                    policy.AllowAnyOrigin()                 // Allows requests from any origin
                          .AllowAnyHeader()                 // Allows any headers
                          .AllowAnyMethod();                // Allows any HTTP methods (GET, POST, PUT, etc.)
                });
            });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Set Kestrel to listen on all interfaces (i.e., 0.0.0.0) and a specific port
            builder.WebHost.ConfigureKestrel(options =>
            {
                // Listen on all network interfaces, e.g., 0.0.0.0, and use port 5269
                options.Listen(System.Net.IPAddress.Any, 5269); // or your desired port
            });

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            // Enable serving static files from the wwwroot directory
            app.UseStaticFiles();

            // Configure CORS to allow any origin
            app.UseCors("AllowAnyOrigin");

            // Serve the index.html file at root ("/")
            //app.MapGet("/", () => Results.File());

            // Authorization middleware (optional)
            app.UseAuthorization();

            // Map controllers (API routes)
            app.MapControllers();

            // Run the application
            app.Run();
        }
    }
}
